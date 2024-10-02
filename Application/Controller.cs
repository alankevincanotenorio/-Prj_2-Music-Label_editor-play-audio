namespace ControllerApp
{
    using MinerApp;
    using DataBaseApp;
    public class Controller
    {
        private string _configFilePath = "config.txt";
        private string _currentPath;
        private bool _isMining;
        private int progress;
        private Miner miner;
        private DataBase database = DataBase.Instance();

        // Constructor
        public Controller()
        {
            _currentPath = LoadPathFromConfig();
            if(!Directory.Exists(_currentPath))
            {
                Directory.CreateDirectory(_currentPath);
            }
            miner = new Miner();
            _isMining = false;
            progress = 0;
        }

        private string LoadPathFromConfig()
        {
            if (File.Exists(_configFilePath))
            {
                return File.ReadAllText(_configFilePath).Trim();
            }
            else
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string defaultPath = Path.Combine(userProfile, "Downloads"); //maybe change to a convencial path
                File.WriteAllText(_configFilePath, defaultPath);
                return defaultPath;
            }
        }

        public void SetCurrentPath(string current_path)
        {
            if (!Directory.Exists(current_path))
            {
                Console.WriteLine("Path does not exists");
                return;
            }
            _currentPath =  current_path;
            
            File.WriteAllText(_configFilePath, _currentPath);
        }
        
        public string GetCurrentPath()
        {
            return _currentPath;
        }

        public Miner GetMiner()
        {
            return miner;
        }

        public DataBase GetDataBase()
        {
            return database;
        }

        public void StartMining()
        {
            if(!_isMining)
            {
                _isMining = true;
                miner.Mine(_currentPath);
                miner.SaveMetadata();
                progress = 100;
                Console.WriteLine("Mining finished.");
            }
            else
            {
                Console.WriteLine("You can't mine because you already mining");
            }
            _isMining = false;
        }

        public List<string> GetRolasInfoInPath()
        {
            List<Rola> rolas_in_path = miner.GetRolas();
            List<string> rolasInfo = new List<string>();
            foreach (Rola rola in rolas_in_path)
            {
                string performerName = GetPerformerName(rola.GetIdPerformer());
                string albumName = GetAlbumName(rola.GetIdAlbum());
                string rolaInfo = $"Title: {rola.GetTitle()}, Performer: {performerName}, Album: {albumName}, Year: {rola.GetYear()}, Track: {rola.GetTrack()}, Genre: {rola.GetGenre()}";
                rolasInfo.Add(rolaInfo);
            }
            return rolasInfo;
        }

        public void editRolaDetails(Rola rola)
        {
            database.UpdateRola(rola);
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

        private string GetPerformerName(int performerId)
        {
            Performer? performer = miner.GetPerformers().Find(p => p.GetIdPerformer() == performerId);
            return performer != null ? performer.GetName() : "Unknown Performer";
        }

        private string GetAlbumName(int albumId)
        {
            Album? album = miner.GetAlbums().Find(a => a.GetIdAlbum() == albumId);
            return album != null ? album.GetName() : "Unknown Album";
        }

        
        public void showAlbumDetails()
        {

        }

        public void editAlbumDetails(Album album)
        {
            database.UpdateAlbum(album);
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