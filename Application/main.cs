using ApplicationApp;
class main
{
    static void Main(string[] args)
    {
        Controller Application = new Controller();
        Application.SetCurrentPath("/home/alan/Downloads");
        Console.WriteLine(Application.miner.GetPath());
        Application.StartMining();
        Application.ShowRolasInPath();
    }
}