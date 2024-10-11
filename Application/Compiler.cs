namespace CompilerClass
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
            string[] parts = query.Split('|', StringSplitOptions.RemoveEmptyEntries);
            bool hasValidField = false;

            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();

                if (trimmedPart.StartsWith("Title:") && IsValidField(trimmedPart, "Title"))
                {
                    hasValidField = true;
                    continue;
                }

                if (trimmedPart.StartsWith("Performer:") && IsValidField(trimmedPart, "Performer"))
                {
                    hasValidField = true;
                    continue;
                }

                if (trimmedPart.StartsWith("Album:") && IsValidField(trimmedPart, "Album"))
                {
                    hasValidField = true;
                    continue;
                }

                return false;
            }

            return hasValidField;
        }

        private bool IsValidField(string part, string fieldName)
        {
            int colonIndex = part.IndexOf(':');
            if (colonIndex == -1) return false;

            string value = part.Substring(colonIndex + 1).Trim();
            return value.StartsWith("\"") && value.EndsWith("\"") && value.Length > 2;
        }

        public void SearchRolas()
        {
            _rolasFounded.Clear();

            string[] parts = _query.Split('|', StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();

                if (trimmedPart.StartsWith("Title:"))
                {
                    string title = trimmedPart.Substring(trimmedPart.IndexOf(":") + 1).Trim().Trim('"');
                    _rolasFounded.AddRange(_database.GetAllRolas().Where(r => r.GetTitle().Contains(title)).ToList());
                }

                if (trimmedPart.StartsWith("Performer:"))
                {
                    string performerName = trimmedPart.Substring(trimmedPart.IndexOf(":") + 1).Trim().Trim('"');
                    List<Performer> performers = _database.GetAllPerformers().Where(p => p.GetName().Contains(performerName)).ToList();

                    foreach (var performer in performers)
                    {
                        _rolasFounded.AddRange(_database.GetAllRolas().Where(r => r.GetIdPerformer() == performer.GetIdPerformer()).ToList());
                    }
                }

                if (trimmedPart.StartsWith("Album:"))
                {
                    string albumName = trimmedPart.Substring(trimmedPart.IndexOf(":") + 1).Trim().Trim('"');
                    List<Album> albums = _database.GetAllAlbums().Where(a => a.GetName().Contains(albumName)).ToList();

                    foreach (var album in albums)
                    {
                        _rolasFounded.AddRange(_database.GetAllRolas().Where(r => r.GetIdAlbum() == album.GetIdAlbum()).ToList());
                    }
                }
            }
        }

    }
}