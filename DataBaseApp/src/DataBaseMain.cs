using DataBaseApp;
public class DataBaseMain{
    static void Main(string[] args)
    {
        DataBase db = DataBase.Instance;
        Console.WriteLine("Data base ready to use");
        //add rola example
        Rola rola = new Rola(1, 1, 1, "/pathExample/ExampleSong.mp3", " Example1", 1, 2024, "example");
        bool isInserted = db.InsertRola(rola);
        if (isInserted)
        {
            rola.ShowInfo();
        }
        //add performer example
        Performer p = new Performer(3, "M2222", 2);
        bool isInsertedP = db.InsertPerformer(p);
        if (isInsertedP)
        {
            p.ShowInfo();
        }
        //get performer example
        Performer? retrievedPerformer = db.GetPerformerById(100);
        if (retrievedPerformer != null)
        {
            Console.WriteLine("Performer founded: ");
            retrievedPerformer.ShowInfo();
        }
        else
        {
            Console.WriteLine("Performer does not exists");
        }
        db.Disconnect();
    }
}