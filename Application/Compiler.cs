namespace CompilerClass
{
    using DataBaseApp;
    public class Compiler
    {
        string _query;
        DataBase _database = DataBase.Instance();
        List<Rola> _rolasFounded = new List<Rola>();
        List<Album> _albumsFounded = new List<Album>();
        List<Performer> _performersFounded = new List<Performer>();

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

                if (trimmedPart.StartsWith("Year:") && IsValidYearField(trimmedPart))
                {
                    hasValidField = true;
                    continue;
                }
                return false;
            }

            return hasValidField;
        }

        // aux method
        private bool IsValidField(string part, string fieldName)
        {
            int colonIndex = part.IndexOf(':');
            if (colonIndex == -1) return false;

            string value = part.Substring(colonIndex + 1).Trim();
            return value.StartsWith("\"") && value.EndsWith("\"") && value.Length > 2;
        }

        // aux method
        private bool IsValidYearField(string part)
        {
            int colonIndex = part.IndexOf(':');
            if (colonIndex == -1) return false;

            string value = part.Substring(colonIndex + 1).Trim();
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Trim('\"');
                return int.TryParse(value, out _);
            }
            return false;
        }
    }
}