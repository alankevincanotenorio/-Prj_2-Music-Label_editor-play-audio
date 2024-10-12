namespace CompilerApp
{
    using DataBaseApp;
    public class Compiler
    {
        private string _query;
        private DataBase _database = DataBase.Instance();
        private List<Rola> _rolasFounded = new List<Rola>();
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private int _paramCounter = 1;

        public void SetQuery(string query) => _query = query;

        public List<Rola> GetRolasFounded() => _rolasFounded;

        public void SearchRolas()
        {
            _rolasFounded.Clear();
            if (!IsValidQuery(_query))
            {
                Console.WriteLine("Invalid query format.");
                return;
            }
            string sql = CompileToSQL();
            if (sql.Contains("Invalid"))
            {
                Console.WriteLine(sql);
                return;
            }
            _rolasFounded = _database.GetRolasByQuery(sql, _parameters);
        }

        private string CompileToSQL()
        {
            _parameters.Clear();
            _paramCounter = 1;

            string[] andParts = _query.Split(new[] { '^' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> conditions = new List<string>();

            foreach (string andPart in andParts)
            {
                string[] orParts = andPart.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> orConditions = new List<string>();

                foreach (string orPart in orParts)
                {
                    string trimmedPart = orPart.Trim();

                    if (trimmedPart.StartsWith("Title:"))
                    {
                        string title = ExtractValueFromField(trimmedPart);
                        string paramName = $"@title{_paramCounter++}";
                        orConditions.Add($"title LIKE {paramName}");
                        _parameters.Add(paramName.Substring(1), $"%{title}%");
                    }
                    else if (trimmedPart.StartsWith("Performer:"))
                    {
                        string performer = ExtractValueFromField(trimmedPart);
                        string paramName = $"@performer{_paramCounter++}";
                        orConditions.Add($"id_performer = (SELECT id_performer FROM performers WHERE name LIKE {paramName})");
                        _parameters.Add(paramName.Substring(1), $"%{performer}%");
                    }
                    else if (trimmedPart.StartsWith("Album:"))
                    {
                        string album = ExtractValueFromField(trimmedPart);
                        string paramName = $"@album{_paramCounter++}";
                        orConditions.Add($"id_album = (SELECT id_album FROM albums WHERE name LIKE {paramName})");
                        _parameters.Add(paramName.Substring(1), $"%{album}%");
                    }
                }
                if (orConditions.Count > 0)
                    conditions.Add($"({string.Join(" OR ", orConditions)})");
            }

            if (conditions.Count > 0)
                return $"SELECT * FROM rolas WHERE {string.Join(" AND ", conditions)}";
            else
                return "Invalid SQL";
        }

        private string ExtractValueFromField(string part)
        {
            int colonIndex = part.IndexOf(':');
            if (colonIndex == -1) return string.Empty;

            string value = part.Substring(colonIndex + 1).Trim();
            return value.Substring(1, value.Length - 2);
        }

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
    }
}