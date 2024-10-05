namespace ControllerApp
{
    using MinerApp;
    using DataBaseApp;
    public class Controller
    {
        private string _currentPath;
        private string _configFilePath = "config.txt";
        private bool _isMining;
        private int _progress;
        private Miner _miner;
        private DataBase _database = DataBase.Instance();

        // Constructor
        public Controller()
        {
            _currentPath = LoadPathFromConfig();
            _miner = new Miner();
            _isMining = false;
            _progress = 0;
        }

        private string LoadPathFromConfig()
        {
            if (File.Exists(_configFilePath))
            {
                string pathFromFile = File.ReadAllText(_configFilePath).Trim();
                if(Directory.Exists(pathFromFile)) return pathFromFile;
                else return "Invalid path";
            }
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            if(string.IsNullOrWhiteSpace(defaultPath))
                defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Music");
            if(!Directory.Exists(defaultPath)) 
                Directory.CreateDirectory(defaultPath);
            File.WriteAllText(_configFilePath, defaultPath);
            return defaultPath;
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
        
        public string GetCurrentPath() => _currentPath;
        public Miner GetMiner() => _miner;
        public DataBase GetDataBase() => _database;

        public void StartMining()
        {
            if(!_isMining)
            {
                _isMining = true;
                _miner.Mine(_currentPath);
                _miner.SaveMetadata();
                _progress = 100;
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
            List<Rola> rolas_in_path = _miner.GetRolas();
            List<string> rolasInfo = new List<string>();
            foreach (Rola rola in rolas_in_path)
            {
                string performerName = GetPerformerName(rola.GetIdPerformer());
                string albumName = GetAlbumName(rola.GetIdAlbum());
                string rolaInfo = $"Title: {rola.GetTitle()}, Performer: {GetPerformerName(rola.GetIdPerformer())}, " +
                                  $"Album: {GetAlbumName(rola.GetIdAlbum())}, Year: {rola.GetYear()}, " +
                                  $"Track: {rola.GetTrack()}, Genre: {rola.GetGenre()}";
                rolasInfo.Add(rolaInfo);
            }
            return rolasInfo;
        }

        public void editRolaDetails(Rola rola)
        {
            _database.UpdateRola(rola);
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
            Performer? performer = _miner.GetPerformers().Find(p => p.GetIdPerformer() == performerId);
            return performer != null ? performer.GetName() : "Unknown Performer";
        }

        private string GetAlbumName(int albumId)
        {
            Album? album = _miner.GetAlbums().Find(a => a.GetIdAlbum() == albumId);
            return album != null ? album.GetName() : "Unknown Album";
        }

        
        public void showAlbumDetails()
        {

        }

        public void editAlbumDetails(Album album)
        {
            _database.UpdateAlbum(album);
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