namespace ControllerApp
{
    using MinerApp;
    using DataBaseApp;
    public class Controller
    {
        private string _currentPath;
        private string _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "MusicLibraryEditorConfg", "config.txt");
        private Miner _miner;
        private DataBase _database = DataBase.Instance();

        // Constructor
        public Controller()
        {
            _currentPath = LoadPathFromConfig();
            _miner = new Miner();
            CheckForDeletedFiles();
        }

        // read the path config
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
                if(Directory.Exists(pathFromFile)) return pathFromFile;
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
        public Miner GetMiner() => _miner;
        public DataBase GetDataBase() => _database;
        public List<string> GetLog() => _miner.GetLog();
        public int GetTotalMp3FilesInPath() => _miner.GetTotalMp3FilesCount(_currentPath);
        public List<Rola> GetAllRolasInDB() => _database.GetAllRolas();
        public float GetProgress(float processedFiles, int totalFiles) => processedFiles / totalFiles;

        public void SetProcessedFilesNumber(int count) => _miner.SetProcessedFilesCount(count);

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

        // start mining method
        public void StartMining(Action<int> onFileProcessed)
        {
            _miner.GetLog().Clear();
            SetProcessedFilesNumber(0);
            _miner.Mine(_currentPath, onFileProcessed);
            Console.WriteLine("Mining finished.");
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

        public List<string> GetRolasOptions(string title)
        {
            List<Rola> matchedRolas = _database.GetAllRolas().Where(r => r.GetTitle() == title).ToList();
            return matchedRolas.Select(r => r.GetPath()).ToList();
        }
    
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

      public bool UpdateRolaDetails(string title, string path, string newTitle, string newGenre, string newTrack, string performerName, string year, string albumName)
        {
            Rola? rolaToEdit = _database.GetRolaByTitleAndPath(title, path);
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

            if (!string.IsNullOrEmpty(newTitle)) rolaToEdit.SetTitle(newTitle);
            if (!string.IsNullOrEmpty(newGenre)) rolaToEdit.SetGenre(newGenre);

            if (!string.IsNullOrEmpty(performerName)) UpdatePerformer(rolaToEdit, performerName);

            if (!string.IsNullOrEmpty(albumName)) UpdateAlbum(rolaToEdit, albumName);

            _database.UpdateRola(rolaToEdit);

            UpdateMp3Metadata(rolaToEdit);
            return true;
        }


        private void UpdatePerformer(Rola rolaToEdit, string performerName)
        {
            List<Performer> performers = _database.GetAllPerformers();
            Performer? existingPerformer = performers.Find(p => p.GetName() == performerName);

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
            else rolaToEdit.SetIdPerformer(existingPerformer.GetIdPerformer());
        }

       private void UpdateAlbum(Rola rolaToEdit, string albumName)
        {
            List<Album> albums = _database.GetAllAlbums();
            Album rolaAlbum = albums.Find(a => a.GetIdAlbum() == rolaToEdit.GetIdAlbum());

            Album? existingAlbum = albums.Find(a => a.GetName() == albumName && a.GetPath() == rolaAlbum.GetPath());

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
            else rolaToEdit.SetIdAlbum(existingAlbum.GetIdAlbum());
        }

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

        //aux method for Update metadata
        private string GetPerformerName(int performerId)
        {
            List<Performer> performers = _database.GetAllPerformers();
            Performer? performer = performers.Find(p => p.GetIdPerformer() == performerId);
            return performer != null ? performer.GetName() : "Unknown Performer";
        }

        //aux method for Update metadata
        private string GetAlbumName(int albumId)
        {
            List<Album> albums = _database.GetAllAlbums();
            Album? album = albums.Find(a => a.GetIdAlbum() == albumId);
            return album != null ? album.GetName() : "Unknown Album";
        }


        public List<string> GetAlbumsOptions(string name)
        {
            List<Album> matchedAlbums = _database.GetAllAlbums().Where(r => r.GetName() == name).ToList();
            return matchedAlbums.Select(r => r.GetPath()).ToList();
        }

        public bool UpdateAlbumDetails(string oldName, string newName, string path, string year)
        {
            Album? albumToEdit = _database.GetAlbumByNameAndPath(oldName, path);
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
            if (isUpdated) Console.WriteLine("Album details successfully updated.");
            else
            {
                Console.WriteLine("Failed to update album details.");
                return false;
            }
            UpdateRolasMetadataForAlbum(albumToEdit);
            return true;
        }

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


        public List<string> ShowPerformerDetails(string performerName)
        {
            List<string> performerDetails = new List<string>();

            Performer? performer = _database.GetPerformerByName(performerName);
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


        public bool ExistsPerformer(string performerName)
        {
            Performer? performer = _database.GetPerformerByName(performerName);
            return performer != null ? true : false;
        }

        public bool IsDefined(string performerName)
        {   
            Performer? performer = _database.GetPerformerByName(performerName);
            return performer.GetIdType() == 2 ? false : true;
        }

        public int GetTypePerformer(string performerName)
        {
            Performer? performer = _database.GetPerformerByName(performerName);
            return performer.GetIdType();
        }

        public void DefinePerformerAsPerson(string performerName, string stage_name, string real_name, string birth_date, string death_date)
        {
            Performer? performer = _database.GetPerformerByName(performerName);
            List<Person> allPersons = _database.GetAllPersons();

            Person? existingPerson = allPersons.Find(p => p.GetStageName() == stage_name);

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


        public void DefinePerformerAsGroup(string performerName, string name, string start_date, string end_date)
        {
            Performer? performer = _database.GetPerformerByName(performerName);
            List<Group> allGroups = _database.GetAllGroups();
            Group? existingGroup = allGroups.Find(p => p.GetName() == name);
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

        public void addPersonToGroup()
        {
            //verificar si la persona ya esta en el grupo
        }
        //this method is for the compiler
        public void search()
        {

        }
    }
}