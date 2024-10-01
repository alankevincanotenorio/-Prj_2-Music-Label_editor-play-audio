namespace DataBaseApp
{
    using System;
    using System.Data.SQLite;
    using System.IO;
    public class DataBase
    {
        private static DataBase? _instance = null;
        private SQLiteConnection _connection;

        //constuctor
        private DataBase(string dbPath = "./DataBaseApp/database/DataBase.db")  
        {
            bool dbExists = File.Exists(dbPath);
            string connectionString;
            if (dbPath == ":memory:") //database for test
            {
                connectionString = "Data Source=:memory:;Version=3;";
            }
            else //database on disk
            {
                connectionString = $"Data Source={dbPath};Version=3;";
            }
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
            Console.WriteLine("Data base connection open");
            if (!dbExists)
            {
                CreateTables();
            }
        }

        //singleton
        public static DataBase Instance(string dbPath = "./DataBaseApp/database/DataBase.db")
        {
            if (_instance == null)
            {
                _instance = new DataBase(dbPath);
            }
            return _instance;
        }

        //Make tables if data base does not exist
        private void CreateTables()
        {
            string createTablesQuery = @"
                CREATE TABLE types (
                    id_type INTEGER PRIMARY KEY,
                    description TEXT
                );

                INSERT INTO types VALUES(0, 'Person');
                INSERT INTO types VALUES(1, 'Group');
                INSERT INTO types VALUES(2, 'Unknown');

                CREATE TABLE performers (
                    id_performer INTEGER PRIMARY KEY,
                    id_type INTEGER,
                    name TEXT,
                    FOREIGN KEY (id_type) REFERENCES types(id_type)
                );

                CREATE TABLE persons (
                    id_person INTEGER PRIMARY KEY,
                    stage_name TEXT,
                    real_name TEXT,
                    birth_date TEXT,
                    death_date TEXT
                );

                CREATE TABLE groups (
                    id_group INTEGER PRIMARY KEY,
                    name TEXT,
                    start_date TEXT,
                    end_date TEXT
                );

                CREATE TABLE in_group (
                    id_person INTEGER,
                    id_group INTEGER,
                    PRIMARY KEY (id_person, id_group),
                    FOREIGN KEY (id_person) REFERENCES persons(id_person),
                    FOREIGN KEY (id_group) REFERENCES groups(id_group)
                );

                CREATE TABLE albums (
                    id_album INTEGER PRIMARY KEY,
                    path TEXT,
                    name TEXT,
                    year INTEGER
                );

                CREATE TABLE rolas (
                    id_rola INTEGER PRIMARY KEY,
                    id_performer INTEGER,
                    id_album INTEGER,
                    path TEXT,
                    title TEXT,
                    track INTEGER,
                    year INTEGER,
                    genre TEXT,
                    FOREIGN KEY (id_performer) REFERENCES performers(id_performer),
                    FOREIGN KEY (id_album) REFERENCES albums(id_album)
                );
            ";
            using (var command = new SQLiteCommand(createTablesQuery, _connection))
            {
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Data base created successfully");
        }

        //close connection
        public void Disconnect()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
                Console.WriteLine("Data base connection closed");
            }
        }

        private int GetMaxId(string tableName, string columnName)
        {
            int maxId = 0;
            try
            {
                string query = $"SELECT IFNULL(MAX({columnName}), 0) FROM {tableName}";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    var result = command.ExecuteScalar();
                    maxId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while getting max ID from {tableName}: " + ex.Message);
            }
            return maxId;
        }

        //addPerformer
        public bool InsertPerformer(Performer performer)
        {
            bool added = false;
            try
            {
                Performer? existingPerformer = GetPerformerByName(performer.GetName()); // Check if performer exists
                if (existingPerformer != null)
                {
                    Console.WriteLine($"Performer '{performer.GetName()}' already exists with ID: {existingPerformer.GetIdPerformer()}");
                    performer.SetIdPerformer(existingPerformer.GetIdPerformer());
                    return false;
                }
                performer.SetIdPerformer(GetMaxId("performers", "id_performer") + 1);
                string query = "INSERT INTO performers (id_performer, id_type, name) VALUES (@id_performer, @id_type, @name)";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id_performer", performer.GetIdPerformer());
                    command.Parameters.AddWithValue("@id_type", performer.GetIdType());
                    command.Parameters.AddWithValue("@name", performer.GetName());
                    int rowsAffected = command.ExecuteNonQuery();
                    added = rowsAffected > 0;
                }
                if (added)
                {
                    Console.WriteLine($"Performer '{performer.GetName()}' added with ID: {performer.GetIdPerformer()}");
                }
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting performer: " + ex.Message);
            }
            return added;
        }

        //addRola
        public bool InsertRola(Rola rola)
        {   
            bool added = false;
            try
            {
                Rola? existingRola = GetRolaByTitleAndPath(rola.GetTitle(), rola.GetPath());
                if (existingRola != null && existingRola.GetPath() == rola.GetPath())
                {
                    Console.WriteLine($"Rola '{rola.GetTitle()}' already exists with ID: {existingRola.GetIdRola()}");
                    rola.SetIdRola(existingRola.GetIdRola());
                    return false;
                }
                rola.SetIdRola(GetMaxId("rolas", "id_rola") + 1);
                string query = "INSERT INTO rolas (id_rola, id_performer, id_album, path, title, track, year, genre) " +
                               "VALUES (@id_rola, @id_performer, @id_album, @path, @title, @track, @year, @genre)";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id_rola", rola.GetIdRola());
                    command.Parameters.AddWithValue("@id_performer", rola.GetIdPerformer());
                    command.Parameters.AddWithValue("@id_album", rola.GetIdAlbum());
                    command.Parameters.AddWithValue("@path", rola.GetPath());
                    command.Parameters.AddWithValue("@title", rola.GetTitle());
                    command.Parameters.AddWithValue("@track", rola.GetTrack());
                    command.Parameters.AddWithValue("@year", rola.GetYear());
                    command.Parameters.AddWithValue("@genre", rola.GetGenre());
                    int rowsAffected = command.ExecuteNonQuery();
                    added = rowsAffected > 0;
                }            
                Console.WriteLine("Rola inserted correctly");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting rola: " + ex.Message);
            }
            return added;
        }
        
        //addPersons

        //addGroup

        //addInGroup

        //addAlbum
        public bool InsertAlbum(Album album)
        {   
            bool added = false;
            try
            {
                Album? existingAlbum = GetAlbumByName(album.GetName());
                if (existingAlbum != null)
                {
                    Console.WriteLine($"Album '{album.GetName()}' already exists with ID: {existingAlbum.GetIdAlbum()}");
                    album.SetIdAlbum(existingAlbum.GetIdAlbum());
                    return false;
                }
                album.SetIdAlbum(GetMaxId("albums", "id_album") + 1);
                string query = "INSERT INTO albums (id_album, path, name, year) VALUES (@id_album, @path, @name, @year)";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id_album", album.GetIdAlbum());
                    command.Parameters.AddWithValue("@path", album.GetPath());
                    command.Parameters.AddWithValue("@name", album.GetName());
                    command.Parameters.AddWithValue("@year", album.GetYear());
                    int rowsAffected = command.ExecuteNonQuery();
                    added = rowsAffected > 0;
                }            
                Console.WriteLine("Album inserted correctly");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting album: " + ex.Message);
            }
            return added;
        }
        
        //get performer by name
        public Performer? GetPerformerByName(string name)
        {
            Performer? performer = null;
            try
            {
                string query = "SELECT * FROM performers WHERE name = @name";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int idPerformer = reader.GetInt32(0);
                            int idType = reader.GetInt32(1);
                            performer = new Performer(idPerformer, name, idType);
                            return performer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while searching performer by name: " + ex.Message);
            }
            return performer;
        }

        public Rola? GetRolaByTitleAndPath(string title, string path)
        {
            Rola? rola = null;
            try
            {
                string query = "SELECT * FROM rolas WHERE title = @title AND path = @path";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue("@path", path);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int idRola = reader.GetInt32(0);
                            int idPerformer = reader.GetInt32(1);
                            int idAlbum = reader.GetInt32(2);
                            string rolaPath = reader.GetString(3);
                            string rolaTitle = reader.GetString(4);
                            int track = reader.GetInt32(5);
                            int year = reader.GetInt32(6);
                            string genre = reader.GetString(7);

                            rola = new Rola(idRola, idPerformer, idAlbum, rolaPath, rolaTitle, track, year, genre);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while searching for rola by title and path: " + ex.Message);
            }
            return rola;
        }

        public Album? GetAlbumByName(string name)
        {
            Album? album = null;
            try
            {
                string query = "SELECT * FROM albums WHERE name = @name";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int idAlbum = reader.GetInt32(0);
                            string path = reader.GetString(1);
                            string albumName = reader.GetString(2);
                            int year = reader.GetInt32(3);
                            album = new Album(idAlbum, path, albumName, year);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while searching for rola by name: " + ex.Message);
            }
            return album;
        }

        public bool UpdateRola(Rola rola)
        {
            bool updated = false;
            try
            {
                string query = "UPDATE rolas SET title = @title, id_performer = @id_performer, id_album = @id_album, " +
                            "track = @track, year = @year, genre = @genre WHERE id_rola = @id_rola";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@title", rola.GetTitle());
                    command.Parameters.AddWithValue("@id_performer", rola.GetIdPerformer());
                    command.Parameters.AddWithValue("@id_album", rola.GetIdAlbum());
                    command.Parameters.AddWithValue("@track", rola.GetTrack());
                    command.Parameters.AddWithValue("@year", rola.GetYear());
                    command.Parameters.AddWithValue("@genre", rola.GetGenre());
                    command.Parameters.AddWithValue("@id_rola", rola.GetIdRola());

                    int rowsAffected = command.ExecuteNonQuery();
                    updated = rowsAffected > 0;
                }

                if (updated)
                {
                    Console.WriteLine("Rola updated successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to update the rola.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating rola: " + ex.Message);
            }

            return updated;
        }

        public bool UpdateAlbum(Album album)
        {
            bool updated = false;
            try
            {
                string query = "UPDATE albums SET path = @path, name = @name, year = @year WHERE id_album = @id_album";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@path", album.GetPath());
                    command.Parameters.AddWithValue("@name", album.GetName());
                    command.Parameters.AddWithValue("@year", album.GetYear());
                    command.Parameters.AddWithValue("@id_album", album.GetIdAlbum());
                    int rowsAffected = command.ExecuteNonQuery();
                    updated = rowsAffected > 0;
                }

                if (updated)
                {
                    Console.WriteLine("Album updated successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to update the Album.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating Album: " + ex.Message);
            }

            return updated;
        }

    }
}