namespace ApplicationApp
{
    using MinerApp;
    using DataBaseApp;
    public class Application
    {
        private string _currentPath; //maybe here put the default path
        private bool _isMining;
        private int progress;
        public Miner miner;
        private DataBase database;

        // Constructor
        public Application()
        {
            miner = new Miner("");
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

        public void ShowRolasInPath()
        {
            List<Rola> rolas_in_path = miner.GetRolas();
            foreach(Rola rola in rolas_in_path)
            {
                Console.WriteLine(rola.GetTitle());
            }
        }
    }
}