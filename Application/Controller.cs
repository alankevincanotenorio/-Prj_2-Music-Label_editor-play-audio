namespace ApplicationApp
{
    using MinerApp;
    using DataBaseApp;
    public class Controller
    {
        private string _currentPath = "/home/alan/Downloads"; //maybe here put the default path
        private bool _isMining;
        private int progress;
        public Miner miner;
        public DataBase database = DataBase.Instance();

        // Constructor
        public Controller()
        {
            miner = new Miner("/home/alan/Downloads");
            _isMining = false;
            progress = 0;
        }

        public void StartMining()
        {
            if(!_isMining)
            {
                _isMining = true;
                miner.Mine(_currentPath);
                miner.SaveMetadata();
                progress = 100;
            }
            else
            {
                Console.WriteLine("You can't mine because you already mining");
            }
            _isMining = false;
        }

        public void SetCurrentPath(string current_path)
        {
            _currentPath =  current_path;
            miner = new Miner(_currentPath);
        }

        public string GetCurrentPath()
        {
            return _currentPath;
        }

        public List<string> ShowRolasInPath()
        {
            List<Rola> rolas_in_path = miner.GetRolas();
            List<string> rolasInfo = new List<string>();

            foreach (Rola rola in rolas_in_path)
            {
                Performer? performer = miner.GetPerformers().Find(p => p.GetIdPerformer() == rola.GetIdPerformer());
                Album? album = miner.GetAlbums().Find(a => a.GetIdAlbum() == rola.GetIdAlbum());
                string performerName = performer != null ? performer.GetName() : "Unknown Performer";
                string albumName = album != null ? album.GetName() : "Unknown Album";
                string rolaInfo = $"Title: {rola.GetTitle()}, Performer: {performerName}, Album: {albumName}, Year: {rola.GetYear()}, Track: {rola.GetTrack()}, Genre: {rola.GetGenre()}";
                rolasInfo.Add(rolaInfo);
            }
            
            return rolasInfo;
        }


        public void showRolaDetails()
        {

        }

        public void editRolaDetails()
        {

        }

        
        public void showAlbumDetails()
        {

        }

        public void editAlbumDetails()
        {

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