#nullable disable
namespace CompilerApp
{
    using DataBaseApp;
    public class Compiler
    {
        private string _query;
        private DataBase _database = DataBase.Instance();
        private List<Rola> _rolasFounded = new List<Rola>();
        private List<Album> _albumsFounded = new List<Album>();
        private List<Performer> _performersFounded = new List<Performer>();

        public void SetQuery(string query) => _query = query;
        public string GetQuery() => _query;
        public List<Rola> GetRolasFounded() => _rolasFounded;
        public List<Album> GetAlbumsFounded() => _albumsFounded;
        public List<Performer> GetPerformersFounded() => _performersFounded;

        public bool IsValidQuery(string query)
        {
            string[] parts = query.Split(new[] { '|', '^' }, StringSplitOptions.RemoveEmptyEntries);
            bool hasValidField = false;

            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();

                if (trimmedPart.StartsWith("Title:") && IsValidField(trimmedPart))
                {
                    hasValidField = true;
                    continue;
                }

                if (trimmedPart.StartsWith("Performer:") && IsValidField(trimmedPart))
                {
                    hasValidField = true;
                    continue;
                }

                if (trimmedPart.StartsWith("Album:") && IsValidField(trimmedPart))
                {
                    hasValidField = true;
                    continue;
                }


                if (trimmedPart.StartsWith("InTitle:") && IsValidField(trimmedPart))
                {
                    hasValidField = true;
                    continue;
                }

                return false;
            }

            return hasValidField;
        }

        private bool IsValidField(string part)
        {
            int colonIndex = part.IndexOf(':');
            if (colonIndex == -1) return false;

            string value = part.Substring(colonIndex + 1).Trim();
            return value.StartsWith("\"") && value.EndsWith("\"") && value.Length > 2;
        }
    
        public void SearchAlbums()
        {
            _albumsFounded.Clear();
            _rolasFounded.Clear();

            string[] parts = _query.Split('|', StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();

                if (trimmedPart.StartsWith("Album:"))
                {
                    string albumName = trimmedPart.Substring(trimmedPart.IndexOf(":") + 1).Trim().Trim('"');

                    List<Album> albums = _database.GetAllAlbums().Where(a => a.GetName() == albumName).ToList();
                    foreach (var album in albums)
                    {
                        _albumsFounded.Add(album);
                        List<Rola> rolas = _database.GetAllRolas().Where(r => r.GetIdAlbum() == album.GetIdAlbum()).ToList();
                        foreach (var rola in rolas)
                        {
                            _rolasFounded.Add(rola);
                        }
                    }
                }
            }
        }

        public void SearchPerformers()
        {
            _performersFounded.Clear();
            _rolasFounded.Clear();
            string[] parts = _query.Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                if (trimmedPart.StartsWith("Performer:"))
                {
                    string performerName = trimmedPart.Substring(trimmedPart.IndexOf(":") + 1).Trim().Trim('"');
                    List<Performer> performers = _database.GetAllPerformers().Where(p => p.GetName() == performerName).ToList();
                    foreach (var performer in performers)
                    {
                        _performersFounded.Add(performer);
                        if (performer.GetIdType() == 0)
                        {
                            Person person = _database.GetAllPersons().Find(p => p.GetStageName() == performerName);
                            if (person != null)
                            {
                                _rolasFounded.AddRange(_database.GetAllRolas().Where(r => r.GetIdPerformer() == performer.GetIdPerformer()).ToList());                                
                                List<Group> groups = _database.GetGroupsForPerson(person);
                                foreach (var group in groups)
                                {
                                    List<Rola> groupRolas = _database.GetAllRolas().Where(r => r.GetIdPerformer() == group.GetIdGroup()).ToList();
                                    _rolasFounded.AddRange(groupRolas);
                                }
                            }
                        }
                        else if (performer.GetIdType() == 1)
                        {
                            Group group = _database.GetAllGroups().Find(g => g.GetName() == performerName);
                            if (group != null) _rolasFounded.AddRange(_database.GetAllRolas().Where(r => r.GetIdPerformer() == group.GetIdGroup()).ToList());
                        }
                        else if (performer.GetIdType() == 2)
                            _rolasFounded.AddRange(_database.GetAllRolas().Where(r => r.GetIdPerformer() == performer.GetIdPerformer()).ToList());
                    }
                }
            }
        }


        public void SearchRolas()
        {
            _rolasFounded.Clear();
            string[] parts = _query.Split('^', StringSplitOptions.RemoveEmptyEntries);
            List<Rola> filteredRolas = _database.GetAllRolas();
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim(); 
                if (trimmedPart.StartsWith("Title:"))
                {
                    string title = trimmedPart.Substring(trimmedPart.IndexOf(":") + 1).Trim().Trim('"');
                    filteredRolas = filteredRolas.Where(r => r.GetTitle() == title).ToList();
                }
                else if (trimmedPart.StartsWith("InTitle:"))
                {
                    string partOfTitle = trimmedPart.Substring(trimmedPart.IndexOf(":") + 1).Trim().Trim('"');
                    filteredRolas = filteredRolas.Where(r => r.GetTitle().Contains(partOfTitle)).ToList();
                }
                else if (trimmedPart.StartsWith("Performer:"))
                {
                    string performerName = trimmedPart.Substring(trimmedPart.IndexOf(":") + 1).Trim().Trim('"');
                    List<Performer> performers = _database.GetAllPerformers().Where(p => p.GetName() == performerName).ToList();
                    List<int> performerIds = performers.Select(p => p.GetIdPerformer()).ToList();
                    filteredRolas = filteredRolas.Where(r => performerIds.Contains(r.GetIdPerformer())).ToList();
                }
                else if (trimmedPart.StartsWith("Album:"))
                {
                    string albumName = trimmedPart.Substring(trimmedPart.IndexOf(":") + 1).Trim().Trim('"');
                    List<Album> albums = _database.GetAllAlbums().Where(a => a.GetName() == albumName).ToList();
                    List<int> albumIds = albums.Select(a => a.GetIdAlbum()).ToList();
                    filteredRolas = filteredRolas.Where(r => albumIds.Contains(r.GetIdAlbum())).ToList();
                }
            }
            _rolasFounded.AddRange(filteredRolas);
        }
    }
}