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
                    else Console.WriteLine("Invalid path.");
                    break;
                case "2":
                    Console.WriteLine("Starting mining...");
                    int totalFiles = app.GetTotalMp3FilesInPath();
                    if(totalFiles == 0) Console.WriteLine("Mining process 100%");
                    Console.WriteLine($"Total MP3 Files: {totalFiles}");
                    app.StartMining((processedFiles) =>
                    {
                        float progress = (float)processedFiles / totalFiles;
                        Console.WriteLine($"Mining Progress: {processedFiles}/{totalFiles} ({(int)(progress * 100)}%)");
                    });
                    Console.WriteLine("Mining finished.");
                    break;
                case "3":
                    List<string> songs = app.GetRolasInfoInPath();
                    Console.WriteLine("Mined Songs:");
                    if (songs.Count == 0) Console.WriteLine("No songs mined yet.");
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
                    List<string> rolasOptions = app.GetRolasOptions(rolaTitle);
                    if (rolasOptions.Count == 0) Console.WriteLine("Song not found.");
                    else if (rolasOptions.Count == 1) EditRolaDetails(rolaTitle, rolasOptions.First());
                    else
                    {
                        Console.WriteLine("Multiple songs found with the same title:");
                        for (int i = 0; i < rolasOptions.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. Path: {rolasOptions[i]}");
                        }
                        Console.Write("Select the song number to edit: ");
                        if (int.TryParse(Console.ReadLine(), out int selectedOption) && selectedOption >= 1 && selectedOption <= rolasOptions.Count)
                        {
                            EditRolaDetails(rolaTitle, rolasOptions[selectedOption - 1]);
                        }
                        else Console.WriteLine("Invalid selection.");
                    }
                    void EditRolaDetails(string title, string path)
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
                        Console.Write("New Year: ");
                        string? year = Console.ReadLine();
                        Console.Write("New Album Name: ");
                        string? albumN = Console.ReadLine();

                        app.UpdateRolaDetails(
                            title,
                            path,
                            newTitle ?? string.Empty,
                            newGenre ?? string.Empty,
                            newTrack ?? string.Empty,
                            performerName ?? string.Empty,
                            year ?? string.Empty,
                            albumN ?? string.Empty
                        );
                        Console.WriteLine("Song details updated.");
                    }
                    break;
                case "5":
                    Console.Write("Enter album name to edit: ");
                    string? albumName = Console.ReadLine();
                    List<string> albumsOptions = app.GetAlbumsOptions(albumName);
                    if (albumsOptions.Count == 0) Console.WriteLine("Album not found.");
                    else if (albumsOptions.Count == 1) editAlbumDetails(albumName, albumsOptions.First());
                    else
                    {
                        Console.WriteLine("Multiple albums found with the same Name:");
                        for (int i = 0; i < albumsOptions.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. Path: {albumsOptions[i]}");
                        }
                        Console.Write("Select the album number to edit: ");
                        if (int.TryParse(Console.ReadLine(), out int selectedOption) && selectedOption >= 1 && selectedOption <= albumsOptions.Count)
                        {
                            editAlbumDetails(albumName, albumsOptions[selectedOption - 1]);
                        }
                        else Console.WriteLine("Invalid selection.");
                    }
                    void editAlbumDetails(string name, string path)
                    {
                        Console.WriteLine("Editing Album Details:");
                        Console.Write("New name: ");
                        string? newName = Console.ReadLine();
                        Console.Write("New Year: ");
                        string? year = Console.ReadLine();

                        app.UpdateAlbumDetails(
                            name,
                            newName ?? string.Empty,
                            path,
                            year ?? string.Empty
                        );
                        Console.WriteLine("Album details updated.");
                    }
                    break;
                case "6":
                    Console.Write("Enter the performer name to edit: ");
                    string? performer = Console.ReadLine();
                    Performer? performerToEdit = app.GetDataBase().GetPerformerByName(performer);

                    if (performerToEdit != null)
                    {
                        Console.Write("Enter 0 if is a person, 1 if is a group: ");
                        string? response = Console.ReadLine();
                        bool isAdded = false;

                        switch (response)
                        {
                            case "0":
                                Console.Write("Stage name: ");
                                string? stageName = Console.ReadLine();
                                Console.Write("Real name: ");
                                string? realName = Console.ReadLine();
                                Console.Write("Birth date: ");
                                string? birthDate = Console.ReadLine();
                                Console.Write("Death date: ");
                                string? deathDate = Console.ReadLine();

                                Person person = new Person(stageName, realName, birthDate, deathDate);
                                isAdded = app.GetDataBase().InsertPerson(person);
                                performerToEdit.SetIdType(PerformerType.Person);
                                app.GetDataBase().UpdatePerformer(performerToEdit);

                                if (isAdded) Console.WriteLine("Performer defined as person.");
                                else Console.WriteLine("Error defining performer.");
                                break;

                            case "1":
                                Console.Write("Group name: ");
                                string? groupName = Console.ReadLine();
                                Console.Write("Start date: ");
                                string? startDate = Console.ReadLine();
                                Console.Write("End date: ");
                                string? endDate = Console.ReadLine();

                                Group group = new Group(groupName, startDate, endDate);
                                isAdded = app.GetDataBase().InsertGroup(group);
                                performerToEdit.SetIdType(PerformerType.Group);
                                app.GetDataBase().UpdatePerformer(performerToEdit);

                                if (isAdded) Console.WriteLine("Performer defined as group.");
                                else Console.WriteLine("Error defining performer.");
                                break;

                            default:
                                Console.WriteLine("Invalid option.");
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
