namespace MinerApp
{
    using TagLib;
    using DataBaseApp;
    using System.Collections;
    using System.Collections.Generic;
    public class Miner
    {
        private List<Rola> _rolas = new List<Rola>();
        private List<Performer> _performers = new List<Performer>();
        private List<Album> _albums = new List<Album>();
        private DataBase _database = DataBase.Instance();

        //browse directories and add the rola in rolas mining metadata
        public bool Mine(string path)
        {
            try
            {
                var mp3Files = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
                foreach (var file in mp3Files)
                {
                    bool IsValidFile = Path.GetExtension(file).Equals(".mp3", StringComparison.OrdinalIgnoreCase);
                    if (IsValidFile)
                    {
                        Rola? rola = GetMetadata(file);
                        if (rola != null)
                        {
                            if (!_rolas.Exists(r => r.GetPath() == rola.GetPath())) //check this condition
                            {
                                _rolas.Add(rola);
                            }
                            else
                            {
                                Console.WriteLine($"Rola '{rola.GetTitle()}' ya existe en la lista de rolas.");
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mining error: " + ex.Message);
                return false;
            }
        }

        //save metadata 
        public void SaveMetadata()
        {
            foreach (Rola rola in _rolas)
            {
                _database.InsertRola(rola);                
            }
        }

        //Get metadata
        private Rola? GetMetadata(string rola_str)
        {
            Rola? rola = null;
            try
            {
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
                uint totalTracks = file.Tag.TrackCount;
                string trackInfo = totalTracks > 0 ? $"{track}/{totalTracks}" : $"{track}";
                int performerId = InsertPerformerIfNotExists(performer);
                int albumId = InsertAlbumIfNotExists(album, rola_str, (int)year);
                rola = new Rola(performerId, albumId, rola_str, title, (int)track, (int)year, genre);
                return rola;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error mining metadata: " + ex.Message);
            }
            return rola;
        }

        // insert performer
        public int InsertPerformerIfNotExists(string performer_name)
        {
            Performer? performer = _database.GetPerformerByName(performer_name);
            if (performer != null)
            { 
                _performers.Add(performer);  //check this cause is for re-open the app
                return performer.GetIdPerformer();
            }
            else
            {
                performer = new Performer(performer_name);
                _database.InsertPerformer(performer);
                _performers.Add(performer);
                return performer.GetIdPerformer();
            }
        }

        // insert album but this method also may admit same albums but with diferent id
        public int InsertAlbumIfNotExists(string album_name, string album_path, int year)
        {
            Album? album = _database.GetAlbumByName(album_name);
            if (album != null)
            {
                _albums.Add(album); //check this cause is for re-open the app
                return album.GetIdAlbum();
            }
            else
            {
                album = new Album(album_path, album_name, year);
                _database.InsertAlbum(album);   
                _albums.Add(album); //
                return album.GetIdAlbum();
            }
        }


        // SETTERS & GETTERS

        public List<Rola> GetRolas()
        {
            return _rolas;
        }
            
        public List<Performer> GetPerformers()
        {
            return _performers;
        }

        public List<Album> GetAlbums()
        {
            return _albums;
        }

        public DataBase GetDataBase()
        {
            return _database;
        }
    }
}