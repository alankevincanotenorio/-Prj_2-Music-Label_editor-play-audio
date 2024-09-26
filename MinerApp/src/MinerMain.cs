using MinerApp;
class MinerMain
{
    static void Main(string[] args)
    {
        string musicPath = "/home/alan/Downloads";
        Miner miner = new Miner(musicPath);

        if (miner.BrowsePaths(musicPath))
        {
            Console.WriteLine("Mining complete");
        }

        foreach (var foundRola in miner._rolas)
        {
            foundRola.ShowInfo();
            miner.db.InsertRola(foundRola);
        }

        miner.db.Disconnect();
    }
}
