using MinerApp;
class MinerMain
{
    static void Main(string[] args)
    {
        string musicPath = "/home/alan/Downloads";
        Miner miner = new Miner(musicPath);
        if (miner.Mine(musicPath))
        {
            Console.WriteLine("Mining complete");
        }
        miner.SaveMetadata();
        miner.GetDataBase().Disconnect();
    }
}