namespace ControllerApp
{
    using MinerApp;
    using DataBaseApp;
    public class Controller
    {
        private string _currentPath;
        private string _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "MusicLibraryEditorConfg", "config.txt");
        private Miner _miner;
        private DataBase _database = DataBase.Instance();

        // Constructor
        public Controller()
        {
            _currentPath = LoadPathFromConfig();
            _miner = new Miner();
            CheckForDeletedFiles();
        }

        // read the path config
        private string LoadPathFromConfig()
        {
            string configDirectory = Path.GetDirectoryName(_configFilePath);
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
                Console.WriteLine($"Directory '{configDirectory}' created.");
            }
            if (File.Exists(_configFilePath))
            {
                string pathFromFile = File.ReadAllText(_configFilePath).Trim();
                if(Directory.Exists(pathFromFile)) return pathFromFile;
            }
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            if(string.IsNullOrWhiteSpace(defaultPath))
                defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Music");
            if(!Directory.Exists(defaultPath)) 
                Directory.CreateDirectory(defaultPath);
            File.WriteAllText(_configFilePath, defaultPath);
            return defaultPath;
        }

        // getters
        public string GetCurrentPath() => _currentPath;
        public Miner GetMiner() => _miner;
        public DataBase GetDataBase() => _database;
        public List<string> GetLog() => _miner.GetLog();
        public int GetTotalMp3FilesInPath() => _miner.GetTotalMp3FilesCount(_currentPath);
        public List<Rola> GetAllRolasInDB() => _database.GetAllRolas();

        public void SetProcessedFilesNumber(int count) => _miner.SetProcessedFilesCount(count);

        // set the current path
        public bool SetCurrentPath(string current_path)
        {
            if (!Directory.Exists(current_path))
            {
                Console.WriteLine("Path does not exists");
                return false;
            }
            _currentPath =  current_path;
            File.WriteAllText(_configFilePath, _currentPath);
            return true;
        }

        //check ir a file was deleted and update the database
        public void CheckForDeletedFiles()
        {
            List<Rola> allRolas = _database.GetAllRolas();
            foreach (Rola rola in allRolas)
            {
                if (!File.Exists(rola.GetPath()))
                {
                    Console.WriteLine($"File '{rola.GetPath()}' Not founded, removing from database");
                    _database.DeleteRola(rola.GetIdRola());
                }
            }
        }

        // start mining method
        public void StartMining(Action<int> onFileProcessed)
        {
            _miner.GetLog().Clear();
            SetProcessedFilesNumber(0);
            _miner.Mine(_currentPath, onFileProcessed);
            Console.WriteLine("Mining finished.");
        }

        //show rolas in path, maybe erase it or change name
        public List<string> GetRolasInfoInPath()
        {
            List<Rola> rolas_in_path = _database.GetAllRolas();
            List<string> rolasInfo = new List<string>();
            foreach (Rola rola in rolas_in_path)
            {
                string performerName = GetPerformerName(rola.GetIdPerformer());
                string albumNameame = GetAlbumName(rola.GetIdAlbum());
                string rolaInfo = $"{rola.GetTitle()}, - Performer: {GetPerformerName(rola.GetIdPerformer())}, " +
                                  $" - Album: {GetAlbumName(rola.GetIdAlbum())}, Year: {rola.GetYear()}, " +
                                  $" - Track: {rola.GetTrack()} -  Genre: {rola.GetGenre()}";
                rolasInfo.Add(rolaInfo);
            }
            return rolasInfo;
        }

        public List<string> GetRolasOptions(string title)
        {
            List<Rola> matchedRolas = _database.GetAllRolas().Where(r => r.GetTitle() == title).ToList();
            return matchedRolas.Select(r => r.GetPath()).ToList();
        }
    
        public List<string> GetRolaDetails(string title, string path)
        {
            Rola rola = _database.GetRolaByTitleAndPath(title, path);
            List<string> rolaDetails = new List<string>
            {
                rola.GetTitle(),
                rola.GetGenre(),
                rola.GetTrack().ToString(),
                GetPerformerName(rola.GetIdPerformer()),
                rola.GetYear().ToString(),
                GetAlbumName(rola.GetIdAlbum())
            };

            return rolaDetails;
        }

        public void UpdateRolaDetails(string title, string path, string newTitle, string newGenre, string newTrack, string performerName, string year, string albumName)
        {
            Rola? rolaToEdit = _database.GetRolaByTitleAndPath(title, path);
            if (rolaToEdit == null) return;
            
            List<Performer> performers = _database.GetAllPerformers();
            Performer rolaPerformer = performers.Find(p => p.GetIdPerformer() == rolaToEdit.GetIdPerformer());
            rolaPerformer.SetName(performerName);
            _database.UpdatePerformer(rolaPerformer);

            List<Album> albums = _database.GetAllAlbums();
            Album rolaAlbum = albums.Find(a => a.GetIdAlbum() == rolaToEdit.GetIdAlbum());
            rolaAlbum.SetName(albumName);
            _database.UpdateAlbum(rolaAlbum);

            if (!string.IsNullOrEmpty(newTitle)) rolaToEdit.SetTitle(newTitle);
            if (!string.IsNullOrEmpty(newGenre)) rolaToEdit.SetGenre(newGenre);
            if (!string.IsNullOrEmpty(newTrack)) rolaToEdit.SetTrack(int.Parse(newTrack));
            if (!string.IsNullOrEmpty(year)) rolaToEdit.SetYear(int.Parse(year));

            _database.UpdateRola(rolaToEdit);

            UpdateMp3Metadata(rolaToEdit);
        }

        private void UpdateMp3Metadata(Rola rola)
        {
            var file = TagLib.File.Create(rola.GetPath());
            file.Tag.Title = rola.GetTitle();
            file.Tag.Performers = new[] { GetPerformerName(rola.GetIdPerformer()) };
            file.Tag.Album = GetAlbumName(rola.GetIdAlbum());
            file.Tag.Year = (uint)rola.GetYear();
            file.Tag.Track = (uint)rola.GetTrack();
            file.Tag.Genres = new[] { rola.GetGenre() };
            file.Save();
            Console.WriteLine("MP3 metadata updated.");
        }

        public string ShowRolaDetails(Rola rola)
        {
            string performerName = GetPerformerName(rola.GetIdPerformer());
            string albumNameame = GetAlbumName(rola.GetIdAlbum());
            
            return $"Title: {rola.GetTitle()}\n" +
                $"Performer: {performerName}\n" +
                $"Album: {albumNameame}\n" +
                $"Track: {rola.GetTrack()}\n" +
                $"Year: {rola.GetYear()}\n" +
                $"Genre: {rola.GetGenre()}";
        }

        //aux method for Update metadata
        private string GetPerformerName(int performerId)
        {
            List<Performer> performers = _database.GetAllPerformers();
            Performer? performer = performers.Find(p => p.GetIdPerformer() == performerId);
            return performer != null ? performer.GetName() : "Unknown Performer";
        }

        //aux method for Update metadata
        private string GetAlbumName(int albumId)
        {
            List<Album> albums = _database.GetAllAlbums();
            Album? album = albums.Find(a => a.GetIdAlbum() == albumId);
            return album != null ? album.GetName() : "Unknown Album";
        }


        public List<string> GetAlbumsOptions(string name)
        {
            List<Album> matchedAlbums = _database.GetAllAlbums().Where(r => r.GetName() == name).ToList();
            return matchedAlbums.Select(r => r.GetPath()).ToList();
        }

        public void UpdateAlbumDetails(string oldName, string newName, string path, string year)
        {
            Album? albumToEdit = _database.GetAlbumByNameAndPath(oldName, path);
            if (albumToEdit == null)
            {
                Console.WriteLine("Album not found.");
                return;
            }
            if (!string.IsNullOrEmpty(newName)) 
            {
                Console.WriteLine($"Updating album name to: {newName}");
                albumToEdit.SetName(newName);
            }

            if (!string.IsNullOrEmpty(year)) 
            {
                Console.WriteLine($"Updating album year to: {year}");
                albumToEdit.SetYear(int.Parse(year));
            }
            bool isUpdated = _database.UpdateAlbum(albumToEdit);
            if (isUpdated) Console.WriteLine("Album details successfully updated.");
            else
            {
                Console.WriteLine("Failed to update album details.");
                return;
            }
            UpdateRolasMetadataForAlbum(albumToEdit);
        }

        private void UpdateRolasMetadataForAlbum(Album album)
        {
            List<Rola> rolas = _database.GetAllRolas().Where(r => r.GetIdAlbum() == album.GetIdAlbum()).ToList();
            if (rolas.Count == 0)
            {
                Console.WriteLine("No songs associated with this album.");
                return;
            }
            foreach (var rola in rolas)
            {
                Console.WriteLine($"Updating metadata for song: {rola.GetTitle()}");
                UpdateMp3Metadata(rola);
            }
        }
                
        public List<string> GetAlbumDetails(string name, string path)
        {
            Album album = _database.GetAlbumByNameAndPath(name, path);
            List<string> albumDetails = new List<string>
            {
                album.GetName(),
                album.GetYear().ToString()
            };
            return albumDetails;
        }

        public void showPerformerDetails()
        {

        }

        public void definePerformer()
        {

        }

        public void addPersonToGroup()
        {

        }
        //this method is for the compiler
        public void search()
        {

        }
    }
}