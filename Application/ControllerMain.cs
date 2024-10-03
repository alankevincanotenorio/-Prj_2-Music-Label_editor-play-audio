using ControllerApp;
class ControllerMain
{
    static void Main(string[] args)
    {
        Controller app = new Controller();
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("=== Music Library Manager ===");
            Console.WriteLine($"Current path: {app.GetCurrentPath()}");
            Console.WriteLine("1. Modify Path");
            Console.WriteLine("2. Start Mining");
            Console.WriteLine("3. Show Mined Songs");
            Console.WriteLine("4. Edit Song Details");
            Console.WriteLine("5. Edit Album Details");
            Console.WriteLine("6. Define Performer as Person or Group");
            Console.WriteLine("7. Exit");
            Console.Write("Choose an option (1-7): ");
            string? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.Write("Enter new path: ");
                    string? newPath = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        app.SetCurrentPath(newPath);
                        Console.WriteLine($"Path updated to: {app.GetCurrentPath()}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid path.");
                    }
                    break;
                case "2":
                    app.StartMining();
                    break;
                case "3":
                    List<string> songs = app.GetRolasInfoInPath();
                    Console.WriteLine("Mined Songs:");
                    if (songs.Count == 0)
                    {
                        Console.WriteLine("No songs mined yet.");
                    }
                    else
                    {
                        foreach (string songInfo in songs)
                        {
                            Console.WriteLine(songInfo);
                        }
                    }
                    break;
                case "4":
                    Console.Write("Enter song title to edit: ");
                    string? rolaTitle = Console.ReadLine();
                    List<Rola> rolas = app.GetMiner().GetRolas();                    
                    Rola? rola = rolas.Find(r => r.GetTitle() == rolaTitle);
                    Rola? rolaToEdit = app.GetDataBase().GetRolaByTitleAndPath(rolaTitle, rola.GetPath());
                    if (rolaToEdit != null)
                    {
                        Console.WriteLine("Editing Song Details:");
                        Console.Write("New Title: ");
                        string? newTitle = Console.ReadLine();

                        Console.Write("New Genre: ");
                        string? newGenre = Console.ReadLine();

                        Console.Write("New Track Number: ");
                        string? newTrack = Console.ReadLine();

                        Console.Write("New Performer Name: ");
                        string? performerName = Console.ReadLine();
                        Performer? newPerformer = app.GetDataBase().GetPerformerByName(performerName);
                        if(newPerformer == null) 
                        {
                            int idp = app.GetMiner().InsertPerformerIfNotExists(performerName);
                            rolaToEdit.SetIdPerformer(idp);
                        }
                        Console.Write("New year: ");
                        string? year = Console.ReadLine();

                        Console.Write("New Album Name: ");
                        string? albumN = Console.ReadLine();
                        Album? album = app.GetDataBase().GetAlbumByName(albumN);
                        if(album == null) 
                        {
                            int idA = app.GetMiner().InsertAlbumIfNotExists(albumN, rolaToEdit.GetPath(), rolaToEdit.GetYear());
                            rolaToEdit.SetIdAlbum(idA);
                        }
                        if (!string.IsNullOrEmpty(newTitle)) rolaToEdit.SetTitle(newTitle);
                        if (!string.IsNullOrEmpty(newGenre)) rolaToEdit.SetGenre(newGenre);
                        if (!string.IsNullOrEmpty(newTrack)) rolaToEdit.SetTrack(int.Parse(newTrack));
                        if (newPerformer != null) rolaToEdit.SetIdPerformer(newPerformer.GetIdPerformer());
                        if (!string.IsNullOrEmpty(year)) rolaToEdit.SetYear(int.Parse(year));
                        if (album != null)
                        {
                            rolaToEdit.SetIdAlbum(album.GetIdAlbum());
                        } 
                        app.editRolaDetails(rolaToEdit);
                        Console.WriteLine("Song details updated.");
                    }
                    else
                    {
                        Console.WriteLine("Song not found.");
                    }
                    break;
                case "5":
                    Console.Write("Enter album name to edit: ");
                    string? albumName = Console.ReadLine();
                    Album? albumToEdit = app.GetDataBase().GetAlbumByName(albumName);
                    if (albumToEdit != null)
                    {
                        Console.WriteLine("Editing Album Details:");
                        Console.Write("New Album Name: ");
                        string? newAlbumName = Console.ReadLine();
                        Console.Write("New Year: ");
                        string? newYear = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newAlbumName)) albumToEdit.SetName(newAlbumName);
                        if (!string.IsNullOrEmpty(newYear)) albumToEdit.SetYear(int.Parse(newYear));
                        app.editAlbumDetails(albumToEdit);
                        Console.WriteLine("Album details updated.");
                    }
                    else
                    {
                        Console.WriteLine("Album not found.");
                    }
                    break;
                case "6":
                    Console.Write("Enter the performer name to edit: ");
                    string? performer = Console.ReadLine();
                    Performer? performerToEdit = app.GetDataBase().GetPerformerByName(performer);
                    if(performerToEdit != null)
                    {
                        Console.Write("Enter 0 if is a person, 1 if is a group: ");
                        string? response = Console.ReadLine();
                        switch(response)
                        {
                            case "0":

                                break;
                            case "1":
                                Console.Write("Name: ");
                                string? groupName = Console.ReadLine();

                                Console.Write("Start date: ");
                                string? startDate = Console.ReadLine();

                                Console.Write("end date: ");
                                string? endDate = Console.ReadLine();
                                Group group = new Group(groupName, startDate, endDate);
                                bool isAdded = app.GetDataBase().InsertGroup(group);
                                performerToEdit.SetIdType(PerformerType.Group);
                                app.GetDataBase().UpdatePerformer(performerToEdit);
                                if(isAdded) Console.WriteLine("Performer defined as group");
                                else Console.WriteLine("Error");
                                break;
                            default:
                                Console.WriteLine("Invalid type");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Performer not found.");
                    }
                    break;
                case "7":
                    exit = true;
                    Console.WriteLine("Exiting the program...");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}
