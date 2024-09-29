using ApplicationApp;
class main
{
    static void Main(string[] args)
    {
        Application Application = new Application();
        Application.SetCurrentPath("/home/alan/Downloads");
        Console.WriteLine(Application.miner.GetPath());
        Application.StartMining();
        Application.ShowRolasInPath();
    }
}