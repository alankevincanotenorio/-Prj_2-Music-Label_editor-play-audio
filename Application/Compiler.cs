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

    }
}