#nullable disable
namespace ControllerApp
{
    using MinerApp;
    using DataBaseApp;
    using CompilerApp;
    public class Controller
    {
        private string _currentPath;
        private string _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "MusicLibraryEditorConfg", "config.txt");
        private Miner _miner;
        private DataBase _database = DataBase.Instance();
        private Compiler _compiler;

        // Constructor
        public Controller()
        {
            _currentPath = LoadPathFromConfig();
            _miner = new Miner();
            _compiler = new Compiler();
            CheckForDeletedFiles();
        }

        // read the config file from path config
        private string LoadPathFromConfig()
        {
            string configDirectory = Path.GetDirectoryName(_configFilePath);
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
                Console.WriteLine($"Directory '{configDirectory}' created.");
            }
            if (File.Exists(_configFilePath))
            {
                string pathFromFile = File.ReadAllText(_configFilePath).Trim();
                if(Directory.Exists(pathFromFile)) 
                    return pathFromFile;
            }
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            if(string.IsNullOrWhiteSpace(defaultPath))
                defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Music");
            if(!Directory.Exists(defaultPath)) 
                Directory.CreateDirectory(defaultPath);

            File.WriteAllText(_configFilePath, defaultPath);
            return defaultPath;
        }

        // getters
        public string GetCurrentPath() => _currentPath;
        public List<string> GetLog() => _miner.GetLog();
        public int GetTotalMp3FilesInPath() => _miner.GetTotalMp3FilesCount(_currentPath);
        public List<Rola> GetAllRolasInDB() => _database.GetAllRolas();
        public float GetProgress(float processedFiles, int totalFiles) => processedFiles / totalFiles;

        // set the number of files processed
        public void SetProcessedFilesNumber(int count) => _miner.SetProcessedFilesCount(count);

        // checks if the Rolas table is empty
        public bool AreRolasInDatabase() => _database.IsRolasTableEmpty();

        //check ir a file was deleted and update the database
        public void CheckForDeletedFiles()
        {
            List<Rola> allRolas = _database.GetAllRolas();
            foreach (Rola rola in allRolas)
            {
                if (!File.Exists(rola.GetPath()))
                {
                    Console.WriteLine($"File '{rola.GetPath()}' Not founded, removing from database");
                    _database.DeleteRola(rola.GetIdRola());
                }
            }
        }
       
        // set the current path
        public bool SetCurrentPath(string current_path)
        {
            if (!Directory.Exists(current_path))
            {
                Console.WriteLine("Path does not exists");
                return false;
            }
            _currentPath =  current_path;
            File.WriteAllText(_configFilePath, _currentPath);
            return true;
        }

        // start mining method
        public void StartMining(Action<int> onFileProcessed)
        {
            _miner.GetLog().Clear();
            SetProcessedFilesNumber(0);
            _miner.Mine(_currentPath, onFileProcessed);
            Console.WriteLine("Mining finished.");
        }

        // get performer name by id
        public string GetPerformerName(int performerId)
        {
            List<Performer> performers = _database.GetAllPerformers();
            Performer performer = performers.Find(p => p.GetIdPerformer() == performerId);
            return performer != null ? performer.GetName() : "Unknown Performer";
        }

        // get album name by id
        private string GetAlbumName(int albumId)
        {
            List<Album> albums = _database.GetAllAlbums();
            Album album = albums.Find(a => a.GetIdAlbum() == albumId);
            return album != null ? album.GetName() : "Unknown Album";
        }

        //show rolas in path, maybe erase it or change name
        public List<string> GetRolasInfoInPath()
        {
            List<Rola> rolas_in_path = _database.GetAllRolas();
            List<string> rolasInfo = new List<string>();
            foreach (Rola rola in rolas_in_path)
            {
                string performerName = GetPerformerName(rola.GetIdPerformer());
                string albumNameame = GetAlbumName(rola.GetIdAlbum());
                string rolaInfo = $"{rola.GetTitle()}, - Performer: {GetPerformerName(rola.GetIdPerformer())}, " +
                                  $" - Album: {GetAlbumName(rola.GetIdAlbum())}, Year: {rola.GetYear()}, " +
                                  $" - Track: {rola.GetTrack()} -  Genre: {rola.GetGenre()}";
                rolasInfo.Add(rolaInfo);
            }
            return rolasInfo;
        }
    
        // get rola details with title and path
        public List<string> GetRolaDetails(string title, string path)
        {
            Rola rola = _database.GetRolaByTitleAndPath(title, path);
            List<string> rolaDetails = new List<string>
            {
                rola.GetTitle(),
                rola.GetGenre(),
                rola.GetTrack().ToString(),
                GetPerformerName(rola.GetIdPerformer()),
                rola.GetYear().ToString(),
                GetAlbumName(rola.GetIdAlbum())
            };

            return rolaDetails;
        }

        // update rola  details in database
        public bool UpdateRolaDetails(string title, string path, string newTitle, string newGenre, string newTrack, string performerName, string year, string albumName)
        {
            Rola rolaToEdit = _database.GetRolaByTitleAndPath(title, path);
            if (rolaToEdit == null)
            {
                Console.WriteLine("Rola not found.");
                return false;
            }

            if (!string.IsNullOrEmpty(newTrack))
            {
                if (!int.TryParse(newTrack, out int trackNumber))
                {
                    Console.WriteLine("Track number must be an integer.");
                    return false;
                }
                rolaToEdit.SetTrack(trackNumber);
            }

            if (!string.IsNullOrEmpty(year))
            {
                if (!int.TryParse(year, out int yearNumber))
                {
                    Console.WriteLine("Year must be an integer.");
                    return false;
                }
                rolaToEdit.SetYear(yearNumber);
            }
            if (!string.IsNullOrEmpty(newTitle)) 
                rolaToEdit.SetTitle(newTitle);
            if (!string.IsNullOrEmpty(newGenre)) 
                rolaToEdit.SetGenre(newGenre);
            if (!string.IsNullOrEmpty(performerName)) 
                UpdatePerformer(rolaToEdit, performerName);

            if (!string.IsNullOrEmpty(albumName)) 
                UpdateAlbum(rolaToEdit, albumName);

            _database.UpdateRola(rolaToEdit);

            UpdateMp3Metadata(rolaToEdit);
            return true;
        }

        // update performer from rolaToEdit
        private void UpdatePerformer(Rola rolaToEdit, string performerName)
        {
            List<Performer> performers = _database.GetAllPerformers();
            Performer existingPerformer = performers.Find(p => p.GetName() == performerName);
            if (existingPerformer == null)
            {
                List<Rola> rolas = _database.GetAllRolas();
                List<Rola> rolasWithSamePerformer = rolas.Where(r => r.GetIdPerformer() == rolaToEdit.GetIdPerformer()).ToList();
                if(rolasWithSamePerformer.Count == 1 && rolasWithSamePerformer[0].GetIdRola() == rolaToEdit.GetIdRola())
                {
                    Performer p = performers.Find(p => p.GetIdPerformer() == rolaToEdit.GetIdPerformer());
                    string oldName = p.GetName();
                    p.SetName(performerName);
                    _database.UpdatePerformer(p);
                    if(p.GetIdType() == 0)
                    {
                        Person personToUpdate = _database.GetAllPersons().Find(person => person.GetStageName() == oldName);
                        personToUpdate.SetStageName(performerName);
                        _database.UpdatePerson(personToUpdate);
                        Console.WriteLine("Person performer updated successfully.");
                    }
                    if(p.GetIdType() == 1)
                    {
                        Group groupToUpdate = _database.GetAllGroups().Find(group => group.GetName() == oldName);
                        groupToUpdate.SetName(performerName);
                        _database.UpdateGroup(groupToUpdate);
                        Console.WriteLine("Group performer updated successfully.");
                    }
                    return;
                }
                Performer newPerformer = new Performer(performerName);
                _database.InsertPerformer(newPerformer);
                rolaToEdit.SetIdPerformer(newPerformer.GetIdPerformer());
            }
            else 
                rolaToEdit.SetIdPerformer(existingPerformer.GetIdPerformer());
        }

        // update album from rolaToEdit
        private void UpdateAlbum(Rola rolaToEdit, string albumName)
        {
            List<Album> albums = _database.GetAllAlbums();
            Album rolaAlbum = albums.Find(a => a.GetIdAlbum() == rolaToEdit.GetIdAlbum());
            Album existingAlbum = albums.Find(a => a.GetName() == albumName && a.GetPath() == rolaAlbum.GetPath());
            if (existingAlbum == null)
            {
                List<Rola> rolas = _database.GetAllRolas();
                List<Rola> rolasWithSameAlbumAndPath = rolas.Where(r => r.GetIdAlbum() == rolaToEdit.GetIdAlbum()).ToList();
                if (rolasWithSameAlbumAndPath.Count == 1 && rolasWithSameAlbumAndPath[0].GetIdRola() == rolaToEdit.GetIdRola())
                {
                    rolaAlbum.SetName(albumName);
                    _database.UpdateAlbum(rolaAlbum);
                    Console.WriteLine($"Album updated to {albumName}.");
                    return;
                }
                Album newAlbum = new Album(rolaAlbum.GetPath(), albumName, rolaAlbum.GetYear());
                _database.InsertAlbum(newAlbum);
                rolaToEdit.SetIdAlbum(newAlbum.GetIdAlbum());
                Console.WriteLine($"New album {albumName} created and assigned to the song.");
            }
            else 
                rolaToEdit.SetIdAlbum(existingAlbum.GetIdAlbum());
        }

        // update rola metadata
        private void UpdateMp3Metadata(Rola rola)
        {
            var file = TagLib.File.Create(rola.GetPath());
            file.Tag.Title = rola.GetTitle();
            file.Tag.Performers = new[] { GetPerformerName(rola.GetIdPerformer()) };
            file.Tag.Album = GetAlbumName(rola.GetIdAlbum());
            file.Tag.Year = (uint)rola.GetYear();
            file.Tag.Track = (uint)rola.GetTrack();
            file.Tag.Genres = new[] { rola.GetGenre() };
            file.Save();
            Console.WriteLine("MP3 metadata updated.");
        }

        // displayes the rola details
        public string ShowRolaDetails(List<string> rolaDetails, string rolaPath)
        {
            string rolaInfo = $"Title: {rolaDetails[0]}\n" +
                            $"Genre: {rolaDetails[1]}\n" +
                            $"Track: {rolaDetails[2]}\n" +
                            $"Performer: {rolaDetails[3]}\n" +
                            $"Year: {rolaDetails[4]}\n" +
                            $"Album: {rolaDetails[5]}\n" +
                            $"Path: {rolaPath}\n";;
            return rolaInfo;
        }

        //get all the rolas that mathched with the title
        public List<string> GetRolasOptions(string title)
        {
            List<Rola> matchedRolas = _database.GetAllRolas().Where(r => r.GetTitle() == title).ToList();
            return matchedRolas.Select(r => r.GetPath()).ToList();
        }

        //get the album cover
        public Gdk.Pixbuf GetAlbumCover(string rolaPath) => _miner.GetAlbumCover(rolaPath);

        //Get the rolas info, includethe cover
        public List<(string rolaInfo, Gdk.Pixbuf albumCover)> GetRolasInfoWithCovers()
        {
            List<Rola> rolasInPath = _database.GetAllRolas();
            List<(string rolaInfo, Gdk.Pixbuf albumCover)> rolasInfo = new List<(string rolaInfo, Gdk.Pixbuf albumCover)>();
            foreach (Rola rola in rolasInPath)
            {
                string rolaInfo = $"{rola.GetTitle()} \nPerformer: {GetPerformerName(rola.GetIdPerformer())}, " +
                                $"\nAlbum: {GetAlbumName(rola.GetIdAlbum())} \nYear: {rola.GetYear()}, " +
                                $"\nTrack: {rola.GetTrack()} \nGenre: {rola.GetGenre()} \nPath: {rola.GetPath()}";
                Gdk.Pixbuf albumCover = GetAlbumCover(rola.GetPath());
                rolasInfo.Add((rolaInfo, albumCover));
            }
            return rolasInfo;
        }

        // get albums that mathched with name
        public List<string> GetAlbumsOptions(string name)
        {
            List<Album> matchedAlbums = _database.GetAllAlbums().Where(r => r.GetName() == name).ToList();
            return matchedAlbums.Select(r => r.GetPath()).ToList();
        }

        // update album details in database
        public bool UpdateAlbumDetails(string oldName, string newName, string path, string year)
        {
            Album albumToEdit = _database.GetAlbumByNameAndPath(oldName, path);
            if (albumToEdit == null)
            {
                Console.WriteLine("Album not found.");
                return false;
            }
            if (!string.IsNullOrEmpty(newName)) 
            {
                Console.WriteLine($"Updating album name to: {newName}");
                albumToEdit.SetName(newName);
            }

            if (!string.IsNullOrEmpty(year)) 
            {
                if (!int.TryParse(year, out int yearNumber)) 
                {
                    Console.WriteLine("Year must be an integer.");
                    return false;
                }
                Console.WriteLine($"Updating album year to: {year}");
                albumToEdit.SetYear(yearNumber);
            }
            bool isUpdated = _database.UpdateAlbum(albumToEdit);
            if (isUpdated) 
                Console.WriteLine("Album details successfully updated.");
            else
            {
                Console.WriteLine("Failed to update album details.");
                return false;
            }
            UpdateRolasMetadataForAlbum(albumToEdit);
            return true;
        }

        // update the rola album metadata 
        private void UpdateRolasMetadataForAlbum(Album album)
        {
            List<Rola> rolas = _database.GetAllRolas().Where(r => r.GetIdAlbum() == album.GetIdAlbum()).ToList();
            if (rolas.Count == 0)
            {
                Console.WriteLine("No songs associated with this album.");
                return;
            }
            foreach (var rola in rolas)
            {
                Console.WriteLine($"Updating metadata for song: {rola.GetTitle()}");
                UpdateMp3Metadata(rola);
            }
        }
        
        //get all albums details hat mathches with albumName 
        public List<string> GetAlbumDetailsWithOptions(string albumName)
        {
            List<Album> matchedAlbums = _database.GetAllAlbums().Where(r => r.GetName() == albumName).ToList();

            if (matchedAlbums.Count == 0) return new List<string>();

            List<string> albumInfo = new List<string>();
            foreach (var album in matchedAlbums)
            {
                albumInfo.Add($"Name: {album.GetName()} \nYear: {album.GetYear()} \nPath: {album.GetPath()}");
            }
            return albumInfo;
        }

        // get album details with name and path
        public List<string> GetAlbumDetails(string name, string path)
        {
            Album album = _database.GetAlbumByNameAndPath(name, path);
            List<string> albumDetails = new List<string>
            {
                album.GetName(),
                album.GetYear().ToString()
            };
            return albumDetails;
        }

        // get performer details
        public List<string> GetPerformerDetails(string performerName)
        {
            List<string> performerDetails = new List<string>();
            Performer performer = _database.GetPerformerByName(performerName);
            if (performer == null) return performerDetails;
            if (performer.GetIdType() == 0)
            {
                Person person = _database.GetAllPersons().Find(p => p.GetStageName() == performerName);
                if (person != null)
                {
                    performerDetails.Add(person.GetStageName());
                    performerDetails.Add(person.GetRealName());
                    performerDetails.Add(person.GetBirthDate());
                    performerDetails.Add(person.GetDeathDate());
                }
            }
            else if (performer.GetIdType() == 1)
            {
                Group group = _database.GetAllGroups().Find(g => g.GetName() == performerName);
                if (group != null)
                {
                    performerDetails.Add(group.GetName());
                    performerDetails.Add(group.GetStartDate());
                    performerDetails.Add(group.GetEndDate());
                }
            }
            return performerDetails;
        }

        //check if a performer is defined or not and return the performer type
        public string CheckPerformer(string performerName, string typeToDefineAs)
        {
            if (string.IsNullOrEmpty(performerName) || !ExistsPerformer(performerName)) 
                return "NotFound";
            if (IsDefined(performerName))
            {
                int performerType = GetTypePerformer(performerName);
                if ((performerType == 0 && typeToDefineAs == "Group")) return "Person";   
                if (performerType == 1 && typeToDefineAs == "Person") return "Group";
                return "Redefine";
            }
            return "NotDefined";
        }

        //checks if a performer exists in database
        public bool ExistsPerformer(string performerName)
        {
            Performer performer = _database.GetPerformerByName(performerName);
            return performer != null ? true : false;
        }

        //check if a performer is defined
        public bool IsDefined(string performerName)
        {   
            Performer performer = _database.GetPerformerByName(performerName);
            return performer.GetIdType() == 2 ? false : true;
        }

        //get the performer type
        public int GetTypePerformer(string performerName)
        {
            Performer performer = _database.GetPerformerByName(performerName);
            return performer.GetIdType();
        }

        //define a performer as person
        public void DefinePerformerAsPerson(string performerName, string stage_name, string real_name, string birth_date, string death_date)
        {
            Performer performer = _database.GetPerformerByName(performerName);
            List<Person> allPersons = _database.GetAllPersons();
            Person existingPerson = allPersons.Find(p => p.GetStageName() == stage_name);
            if (existingPerson != null)
            {
                existingPerson.SetRealName(real_name);
                existingPerson.SetBirthDate(birth_date);
                existingPerson.SetDeathDate(death_date);
                _database.UpdatePerson(existingPerson);
            }
            else
            {
                Person newPerson = new Person(stage_name, real_name, birth_date, death_date);
                _database.InsertPerson(newPerson);
            }
            performer.SetIdType(PerformerType.Person);
            _database.UpdatePerformer(performer);
        }

        //define a performer as group
        public void DefinePerformerAsGroup(string performerName, string name, string start_date, string end_date)
        {
            Performer performer = _database.GetPerformerByName(performerName);
            List<Group> allGroups = _database.GetAllGroups();
            Group existingGroup = allGroups.Find(p => p.GetName() == name);
            if(existingGroup != null)
            {
                existingGroup.SetName(name);
                existingGroup.SetStartDate(start_date);
                existingGroup.SetEndDate(end_date);
                _database.UpdateGroup(existingGroup);
            }
            else
            {
                Group group = new Group(name, start_date, end_date);
                _database.InsertGroup(group);
            }
            performer.SetIdType(PerformerType.Group);
            _database.UpdatePerformer(performer);
        }
        
        //add a person in a group
        public void AddPersonToGroup(string personName, string groupName)
        {
            Person person = _database.GetAllPersons().Find(p => p.GetStageName() == personName);
            Group group = _database.GetAllGroups().Find(g => g.GetName() == groupName);
            bool isAdded = _database.AddInGroup(person, group);
            if (isAdded)
                 Console.WriteLine("Person added to the group successfully.");
            else 
                Console.WriteLine("The person is already in the group or an error occurred.");
        }

        //check if a person and a group exists and if the person is in the group
        public string CheckPersonAndGroup(string personName, string groupName)
        {
            Person person = _database.GetAllPersons().Find(p => p.GetStageName() == personName);
            if (person == null)
            {
                Console.WriteLine("Person not found.");
                return "Person not found";
            }
            Group group = _database.GetAllGroups().Find(g => g.GetName() == groupName);
            if (group == null)
            {
                Console.WriteLine("Group not found.");
                return "Group not found";
            }
            bool alreadyInGroup = _database.CheckPersonInGroup(person, group);
            if (alreadyInGroup) return "Person already in group";
            return "success";
        }

        //check if is a valid query
        public bool IsQueryValid(string query) => _compiler.IsValidQuery(query);

        // search albums in database
        public List<string> SearchAlbums(string query)
        {
            _compiler.SetQuery(query);
            _compiler.SearchAlbums();
            List<string> results = new List<string>();
            foreach (Album album in _compiler.GetAlbumsFounded())
            {
                string albumInfo = $"Album: {album.GetName()}, Year: {album.GetYear()}, Path: {album.GetPath()}";
                results.Add(albumInfo);
                List<Rola> associatedRolas = _compiler.GetRolasFounded().Where(r => r.GetIdAlbum() == album.GetIdAlbum()).ToList();
                if (associatedRolas.Count > 0)
                {
                    results.Add("Rolas in this album:");
                    foreach (Rola rola in associatedRolas)
                    {
                        string rolaInfo = $"  - Rola: {rola.GetTitle()}, Performer: {GetPerformerName(rola.GetIdPerformer())}, Track: {rola.GetTrack()}";
                        results.Add(rolaInfo);
                    }
                }
                else results.Add("Not rolas in this album.");
            }
            return results;
        }

        // getAlbumsFounded after the search
        public List<Album> GetAlbumsFounded() => _compiler.GetAlbumsFounded();

        // search rolas in database        
        public List<string> SearchRolas(string query)
        {
            _compiler.SetQuery(query);
            _compiler.SearchRolas();
            List<string> results = new List<string>();
            foreach (Rola rola in _compiler.GetRolasFounded())
            {
                if (rola != null)
                {
                    string rolaInfo = $"Title: {rola.GetTitle()}, Genre: {rola.GetGenre()}, Track: {rola.GetTrack()}, " +
                                    $"Performer: {GetPerformerName(rola.GetIdPerformer())}, Year: {rola.GetYear()}, " +
                                    $"Album: {GetAlbumName(rola.GetIdAlbum())}, Path: {rola.GetPath()}";
                    results.Add(rolaInfo);
                }
            }
            return results;
        }

        //check if the query contains only performer
        public bool OnlyContainsPerformer(string query)
        {
            return query.Contains("Performer:") && 
                !query.Contains("Album:") && 
                !query.Contains("Title:") && 
                !query.Contains("InTitle:");
        }

        //check if the query contains only album
        public bool OnlyContainsAlbum(string query)
        {
            return query.Contains("Album:") && 
                !query.Contains("Performer:") && 
                !query.Contains("Title:") && 
                !query.Contains("InTitle:");
        }

        // search performers in database
        public List<string> SearchPerformers(string query)
        {
            _compiler.SetQuery(query);
            _compiler.SearchPerformers();
            List<string> results = new List<string>();
            foreach (Performer performer in _compiler.GetPerformersFounded())
            {
                if (performer.GetIdType() == 0) // Person
                    results.AddRange(ProcessPerson(performer));
                else if (performer.GetIdType() == 1) // Group
                    results.AddRange(ProcessGroup(performer));
                else
                {
                    string unknownPerformerInfo = $"Performer: {performer.GetName()}";
                    results.Add(unknownPerformerInfo);
                    results.AddRange(ProcessRolasForPerformer(performer));
                }
            }
            return results;
        }

        // continue the search performers process if the performer is a person
        private List<string> ProcessPerson(Performer performer)
        {
            List<string> result = new List<string>();
            Person person = _database.GetPersonByStageName(performer.GetName());
            if (person != null)
            {
                result.Add($"Performer person:");
                result.Add($"Stage name: {person.GetStageName()}");
                result.Add($"Real name: {person.GetRealName()}");
                result.Add($"Birth date: {person.GetBirthDate()}");
                result.Add($"Death date: {person.GetDeathDate()}");
                List<Group> groups = _database.GetGroupsForPerson(person);
                if (groups.Count > 0)
                {
                    result.Add("Groups:");
                    foreach (Group group in groups)
                    {
                        result.Add($" - {group.GetName()}");
                        result.AddRange(ProcessRolasForGroup(group));
                    }
                }
                else 
                    result.Add("This person does not belong to any group.");

                result.Add("Rolas by performer:");
                result.AddRange(ProcessRolasForPerformer(performer));
            }
            return result;
        }

        // continue the search performers process if the performer is a group
        private List<string> ProcessGroup(Performer performer)
        {
            List<string> result = new List<string>();
            Group group = _database.GetGroupByName(performer.GetName());
            if (group != null)
            {
                result.Add($"Performer group:");
                result.Add($"Name: {group.GetName()}");
                result.Add($"Start date: {group.GetStartDate()}");
                result.Add($"End date: {group.GetEndDate()}");
                result.Add("Members:");
                List<Person> members = _database.GetPersonsInGroup(group);
                foreach (Person member in members)
                {
                    result.Add($" - {member.GetStageName()}");
                }
                result.Add("Rolas by group:");
                result.AddRange(ProcessRolasForGroup(group));
            }
            return result;
        }

        // get rolas from the performer
        private List<string> ProcessRolasForPerformer(Performer performer)
        {
            List<string> result = new List<string>();
            List<Rola> performerRolas = _database.GetRolasByPerformer(performer.GetIdPerformer());
            if (performerRolas.Count > 0)
            {
                foreach (Rola rola in performerRolas)
                {
                    result.Add($"  - Rola: {rola.GetTitle()}, Track: {rola.GetTrack()}");
                }
            }
            else
                result.Add("No rolas found for this performer.");
            return result;
        }

        // get rolas from the group
        private List<string> ProcessRolasForGroup(Group group)
        {
            List<string> result = new List<string>();
            List<Rola> groupRolas = _database.GetRolasByGroup(group.GetIdGroup());
            if (groupRolas.Count > 0)
            {
                foreach (Rola rola in groupRolas)
                {
                    result.Add($"  - Rola: {rola.GetTitle()}, Track: {rola.GetTrack()}");
                }
            }
            else
                result.Add("No rolas found for this group.");
            return result;
        }
    }
}