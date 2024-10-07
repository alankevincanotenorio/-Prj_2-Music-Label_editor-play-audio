namespace MinerApp
{
    using TagLib;
    using DataBaseApp;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    public class Miner
    {
        private DataBase _database = DataBase.Instance();
        public List<string> _log = new List<string>();
        private int processedFilesCount = 0;

        // getters
        public List<string> GetLog() => _log;
        public int GetProcessedFilesCount() => processedFilesCount;

        //setter
        public void SetProcessedFilesCount(int count) => processedFilesCount = count;

        //browse directories and add the rola in rolas mining metadata
        public bool Mine(string path, Action<int> onFileProcessed)
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
                    if (HasReadAccess(file, false))
                    {
                        Rola? rola = GetMetadata(file);
                        if (rola != null)
                        {
                            Rola? existingRola = _database.GetRolaByTitleAndPath(rola.GetTitle(), rola.GetPath());
                            if (existingRola == null)
                            {
                                _database.InsertRola(rola);
                                _log.Add($"Rola '{rola.GetTitle()}' added with ID: {rola.GetIdRola()}");
                            }
                            else Console.WriteLine($"Rola '{rola.GetTitle()}' Already exists");
                        }
                        processedFilesCount++;
                        onFileProcessed(processedFilesCount);
                    }
                    else _log.Add($"Inaccessible file '{file}': Permission denied.");
                }
            }
            var subDirectories = Directory.GetDirectories(path);
            foreach (var directory in subDirectories)
            {
                if (HasReadAccess(directory, true)) Mine(directory, onFileProcessed);
                else _log.Add($"Inaccessible subdirectory '{directory}': Permission denied.");
            }
            return true;
        }

        // checks if a file or directory has read access
        public bool HasReadAccess(string path, bool isDirectory)
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
            if (performer != null) return performer.GetIdPerformer();
            else
            {
                performer = new Performer(performer_name);
                _database.InsertPerformer(performer);
                _log.Add($"Performer '{performer.GetName()}' added with ID: {performer.GetIdPerformer()}");
                return performer.GetIdPerformer();
            }
        }

        // insert album
        public int InsertAlbumIfNotExists(string album_name, string album_path, int year)
        {
            Album? album = _database.GetAlbumByName(album_name);
            if (album != null) return album.GetIdAlbum();
            else
            {
                album = new Album(album_path, album_name, year);
                _database.InsertAlbum(album);
                _log.Add($"Album '{album.GetName()}' added with ID: {album.GetIdAlbum()}");
                return album.GetIdAlbum();
            }
        }

        public int GetTotalMp3FilesCount(string path)
        {
            int totalMp3FilesCount = 0;
            if (!HasReadAccess(path, true))
            {
                Console.WriteLine($"Inaccessible directory: '{path}'");
                return totalMp3FilesCount;
            }
            var mp3Files = Directory.GetFiles(path, "*.mp3", SearchOption.TopDirectoryOnly);
            foreach (var file in mp3Files)
            {
                if (HasReadAccess(file, false)) totalMp3FilesCount++;
                else Console.WriteLine($"Inaccessible file: {file}");
            }
            var subDirectories = Directory.GetDirectories(path);
            foreach (var directory in subDirectories)
            {
                if (HasReadAccess(directory, true)) totalMp3FilesCount += GetTotalMp3FilesCount(directory);
                else Console.WriteLine($"Inaccessible directory: {directory}");
            }
            Console.WriteLine($"Total accessible MP3 files in '{path}' and subdirectories: {totalMp3FilesCount}");
            return totalMp3FilesCount;
        }
    }
}