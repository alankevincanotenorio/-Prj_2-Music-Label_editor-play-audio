using DataBaseApp;
public class DataBaseMain{
    static void Main(string[] args)
    {
        DataBase db = DataBase.Instance();
        Rola rola = db.GetRolaByTitleAndPath("Unknown", "/home/alan/Downloads/Camilo Sesto/Otro/Triste Final - Camilo Sesto.mp3");
        Performer p =  db.GetPerformerByName("Camilo Sesto");
         if (p == null)
        {
            Console.WriteLine("Performer not found. Exiting.");
            return;
        }
        rola.SetTitle("New Title");
        rola.SetGenre("New Genre");
        rola.SetIdPerformer(p.GetIdPerformer());
        rola.SetTrack(2);
        bool isUpdated = db.UpdateRola(rola);
        if (isUpdated)
        {
        Console.WriteLine("Rola updated successfully.");
        }
        else
        {
        Console.WriteLine("Failed to update the rola.");
        }
        db.Disconnect();
    }
}