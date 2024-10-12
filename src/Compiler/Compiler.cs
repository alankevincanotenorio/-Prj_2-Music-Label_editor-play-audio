#nullable disable
namespace CompilerApp
{
    using DataBaseApp;

    /// <summary>
    /// The Compiler class is responsible for compiling user queries into SQL commands 
    /// and interacting with the database to retrieve rolas, albums, performers.
    /// </summary>
    public class Compiler
    {
        private string _query;
        private DataBase _database = DataBase.Instance();
        private List<Rola> _rolasFounded = new List<Rola>();
        private List<Album> _albumsFounded = new List<Album>();
        private List<Performer> _performersFounded = new List<Performer>();
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private int _paramCounter = 1;

        /// <summary>
        /// Sets the user query to be used for searches.
        /// </summary>
        /// <param name="query">The query string provided by the user.</param>
        public void SetQuery(string query) => _query = query;

        // Getters
        public string GetQuery() => _query;
        public List<Rola> GetRolasFounded() => _rolasFounded;
        public List<Album> GetAlbumsFounded() => _albumsFounded;
        public List<Performer> GetPerformersFounded() => _performersFounded;

        /// <summary>
        /// Checks if a user query is valid based on the format. 
        /// A valid query must include "Title:", "Performer:", or "Album:" with proper quotes around the values.
        /// </summary>
        /// <param name="query">The query string to validate.</param>
        /// <returns>True if the query is valid, false otherwise.</returns>
        public bool IsValidQuery(string query)
        {
            string[] parts = query.Split(new[] { '|', '^' }, StringSplitOptions.RemoveEmptyEntries);
            bool hasValidField = false;
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                if ((trimmedPart.StartsWith("Title:") || trimmedPart.StartsWith("Performer:") || trimmedPart.StartsWith("Album:")) && IsValidField(trimmedPart))
                    hasValidField = true;
                else
                    return false;
            }
            return hasValidField;
        }

        /// <summary>
        /// Checks if the field in the query is valid. A valid field must have a value enclosed in quotes.
        /// </summary>
        /// <param name="part">The query field part to validate.</param>
        /// <returns>True if the field is valid, false otherwise.</returns>
        private bool IsValidField(string part)
        {
            int colonIndex = part.IndexOf(':');
            if (colonIndex == -1) return false;
            string value = part.Substring(colonIndex + 1).Trim();
            return value.StartsWith("\"") && value.EndsWith("\"") && value.Length > 2;
        }

        /// <summary>
        /// Extracts the value from a query field (e.g., extracts "value" from Title:"value").
        /// </summary>
        /// <param name="part">The query field part to extract the value from.</param>
        /// <returns>The extracted value.</returns>
        private string ExtractValueFromField(string part)
        {
            int colonIndex = part.IndexOf(':');
            if (colonIndex == -1) return string.Empty;
            string value = part.Substring(colonIndex + 1).Trim();
            return value.Substring(1, value.Length - 2);
        }

        /// <summary>
        /// Searches for rolas based on the compiled SQL query.
        /// Clears any previous search results and stores the new ones.
        /// </summary>
        public void SearchRolas()
        {
            _rolasFounded.Clear();
            if (!IsValidQuery(_query))
            {
                Console.WriteLine("Invalid query format.");
                return;
            }
            string sql = CompileRolasQuery();
            if (sql.Contains("Invalid"))
            {
                Console.WriteLine(sql);
                return;
            }
            _rolasFounded = _database.GetRolasByQuery(sql, _parameters);
        }

        /// <summary>
        /// Compiles the SQL query to search for rolas based on user input.
        /// It supports search by title, performer, or album.
        /// </summary>
        /// <returns>The compiled SQL query string.</returns>
        private string CompileRolasQuery()
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
                    string paramName = $"@param{_paramCounter++}";
                    if (trimmedPart.StartsWith("Title:"))
                    {
                        string title = ExtractValueFromField(trimmedPart);
                        orConditions.Add($"title LIKE {paramName}");
                        _parameters.Add(paramName.Substring(1), $"%{title}%");
                    }
                    else if (trimmedPart.StartsWith("Performer:"))
                    {
                        string performer = ExtractValueFromField(trimmedPart);
                        orConditions.Add($"id_performer = (SELECT id_performer FROM performers WHERE name LIKE {paramName})");
                        _parameters.Add(paramName.Substring(1), $"%{performer}%");
                    }
                    else if (trimmedPart.StartsWith("Album:"))
                    {
                        string album = ExtractValueFromField(trimmedPart);
                        orConditions.Add($"id_album = (SELECT id_album FROM albums WHERE name LIKE {paramName})");
                        _parameters.Add(paramName.Substring(1), $"%{album}%");
                    }
                }
                if (orConditions.Count > 0)
                    conditions.Add($"({string.Join(" OR ", orConditions)})");
            }
            return conditions.Count > 0 ? $"SELECT * FROM rolas WHERE {string.Join(" AND ", conditions)}" : "Invalid SQL";
        }

        /// <summary>
        /// Searches for albums based on the compiled SQL query. Clears any previous search results
        /// and retrieves albums and their associated rolas
        /// </summary>
        public void SearchAlbums()
        {
            _albumsFounded.Clear();
            if (!IsValidQuery(_query))
            {
                Console.WriteLine("Invalid query format.");
                return;
            }
            string sql = CompileAlbumsQuery();
            if (sql.Contains("Invalid"))
            {
                Console.WriteLine(sql);
                return;
            }
            _albumsFounded = _database.GetAlbumsByQuery(sql, _parameters);
            foreach (var album in _albumsFounded)
            {
                string rolaSql = "SELECT * FROM rolas WHERE id_album = @id_album";
                var rolaParams = new Dictionary<string, string> { { "id_album", album.GetIdAlbum().ToString() } };
                List<Rola> rolasInAlbum = _database.GetRolasByQuery(rolaSql, rolaParams);
                _rolasFounded.AddRange(rolasInAlbum);
            }
        }

        /// <summary>
        /// Compiles the SQL query to search for albums based on user input.
        /// </summary>
        /// <returns>The compiled SQL query string for albums.</returns>
        private string CompileAlbumsQuery()
        {
            _parameters.Clear();
            _paramCounter = 1;
            if (_query.StartsWith("Album:"))
            {
                string albumName = ExtractValueFromField(_query);
                string paramName = $"@name";
                _parameters.Add(paramName.Substring(1), $"%{albumName}%");
                return "SELECT * FROM albums WHERE name LIKE @name";
            }
            return "Invalid SQL for albums";
        }

        /// <summary>
        /// Searches for performers based on the compiled SQL query. Clears any previous search results
        /// and retrieves matching performers.
        /// </summary>
        public void SearchPerformers()
        {
            _performersFounded.Clear();
            if (!IsValidQuery(_query))
            {
                Console.WriteLine("Invalid query format.");
                return;
            }
            string sql = CompilePerformerQuery();
            if (sql.Contains("Invalid"))
            {
                Console.WriteLine(sql);
                return;
            }
            _performersFounded = _database.GetPerformersByQuery(sql, _parameters);
        }

        /// <summary>
        /// Compiles the SQL query to search for performers based on user input.
        /// </summary>
        /// <returns>The compiled SQL query string for performers.</returns>
        private string CompilePerformerQuery()
        {
            _parameters.Clear();
            _paramCounter = 1;
            if (_query.StartsWith("Performer:"))
            {
                string performerName = ExtractValueFromField(_query);
                string paramName = $"@name";
                _parameters.Add(paramName.Substring(1), performerName);
                return "SELECT * FROM performers WHERE name LIKE @name";
            }
            return "Invalid SQL for performer query";
        }
    }
}