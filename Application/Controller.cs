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
        private DataBase database;

        // Constructor
        public Controller()
        {
            miner = new Miner("/home/alan/Downloads");
            database = DataBase.Instance();
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
            List<string> n = new List<string>();
            foreach(Rola rola in rolas_in_path)
            {
                n.Add(rola.GetTitle());
            }
            return n;
        }
    }
}