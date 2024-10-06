namespace MinerApp
{
    using TagLib;
    using DataBaseApp;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    public class Miner
    {
        private List<Rola> _rolas = new List<Rola>();
        private List<Performer> _performers = new List<Performer>();
        private List<Album> _albums = new List<Album>();
        private DataBase _database = DataBase.Instance();
        public List<string> _log { get; private set; } = new List<string>();

        // getters
        public List<Rola> GetRolas() => _rolas;
        public List<Performer> GetPerformers() => _performers;
        public List<Album> GetAlbums() => _albums;
        public List<string> GetErrors() => _log;
        public DataBase GetDataBase() => _database;

        //browse directories and add the rola in rolas mining metadata
        public bool Mine(string path)
        {
            if (!HasReadAccess(path, true)) 
            {
                _log.Add($"Inaccessible directory: '{path}': Permission denied");
                return false;
            }
            var mp3Files = Directory.GetFiles(path, "*.mp3", SearchOption.TopDirectoryOnly);   
            foreach (var file in mp3Files)
            {
                bool IsValidFile = Path.GetExtension(file).Equals(".mp3", StringComparison.OrdinalIgnoreCase);
                if (IsValidFile)
                {
                    if(HasReadAccess(file, false))
                    {
                        Rola? rola = GetMetadata(file);
                        if (rola != null)
                        {
                            if (!_rolas.Exists(r => r.GetPath() == rola.GetPath()))
                            {
                                _rolas.Add(rola);
                                _database.InsertRola(rola);
                                _log.Add($"Rola '{rola.GetTitle()}' added with ID: {rola.GetIdRola()}");
                            }
                            else Console.WriteLine($"Rola '{rola.GetTitle()}' Already exists");
                        }
                    } 
                    else _log.Add($"Inaccessible file '{file}': Permission denied.");
                }
            }
            var subDirectories = Directory.GetDirectories(path);
            foreach (var directory in subDirectories)
            {
                if (HasReadAccess(directory, true)) Mine(directory);
                else _log.Add($"Inaccessible subdirectory '{directory}': Permission denied.");
            }
            return true;
        }

        // checks if a file or directory has read access
        private bool HasReadAccess(string path, bool isDirectory)
        {
            try
            {
                if(isDirectory) Directory.GetFiles(path);
                else using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read)) { }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        //Get metadata
        private Rola? GetMetadata(string rola_str)
        {
            Rola? rola = null;
            var file = TagLib.File.Create(rola_str);
            //TPE1
            string performer = file.Tag.FirstPerformer ?? "Unknown";
            //TIT2
            string title = file.Tag.Title ?? "Unknown";
            //TALB
            string album = file.Tag.Album ?? "Unknown";
            //TDRC
            uint year = file.Tag.Year != 0 ? file.Tag.Year : (uint)System.IO.File.GetCreationTime(rola_str).Year;
            //TCON
            string genre = file.Tag.FirstGenre ?? "Unknown";
            //TRCK
            uint track = file.Tag.Track != 0 ? file.Tag.Track : 0;

            int performerId = InsertPerformerIfNotExists(performer);

            string albumPath = Directory.GetParent(rola_str)?.FullName ?? "Unknown";
            int albumId = InsertAlbumIfNotExists(album, albumPath, (int)year);

            rola = new Rola(performerId, albumId, rola_str, title, (int)track, (int)year, genre);
            return rola;
        }

        // insert performer
        public int InsertPerformerIfNotExists(string performer_name)
        {
            Performer? performer = _database.GetPerformerByName(performer_name);
            if (performer != null)
            { 
                _performers.Add(performer);
                return performer.GetIdPerformer();
            }
            else
            {
                performer = new Performer(performer_name);
                _database.InsertPerformer(performer);
                _log.Add($"Performer '{performer.GetName()}' added with ID: {performer.GetIdPerformer()}");
                _performers.Add(performer);
                return performer.GetIdPerformer();
            }
        }

        // insert album
        public int InsertAlbumIfNotExists(string album_name, string album_path, int year)
        {
            Album? album = _database.GetAlbumByName(album_name);
            if (album != null)
            {
                _albums.Add(album);
                return album.GetIdAlbum();
            }
            else
            {
                album = new Album(album_path, album_name, year);
                _database.InsertAlbum(album);
                _log.Add($"Album '{album.GetName()}' added with ID: {album.GetIdAlbum()}");
                _albums.Add(album);
                return album.GetIdAlbum();
            }
        }
    }
}