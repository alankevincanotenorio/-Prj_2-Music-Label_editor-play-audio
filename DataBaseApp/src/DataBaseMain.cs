using DataBaseApp;
public class DataBaseMain{
    static void Main(string[] args)
    {
        DataBase db = DataBase.Instance();
        Console.WriteLine("Data base ready to use");
        //add performer example
        Performer example1 = new Performer("Example1");
        bool isInserted = db.InsertPerformer(example1);
        if (isInserted)
        {
            example1.ShowInfo();
        }
        //get performer example
        Performer? retrievedPerformer = db.GetPerformerByName("Example1");
        if (retrievedPerformer != null)
        {
            Console.WriteLine("Performer founded: ");
            //retrievedPerformer.ShowInfo();
        }
        else
        {
            Console.WriteLine("Performer does not exists");
        }
        Performer example2 = new Performer("Example4");
        bool isInserted2 = db.InsertPerformer(example2);
        if (isInserted2)
        {
            example2.ShowInfo();
        }
        //get performer example
        Performer? retrievedPerformer2 = db.GetPerformerByName("Example3");
        if (retrievedPerformer2 != null)
        {
            Console.WriteLine("Performer founded: ");
            //retrievedPerformer2.ShowInfo();
        }
        else
        {
            Console.WriteLine("Performer does not exists");
        }
        Rola rola = new Rola(example2.GetIdPerformer(), 1, "path/to/rola.mp3", "Song ", 1, 2022, "Rock");
        db.InsertRola(rola);
        db.Disconnect();
    }
}