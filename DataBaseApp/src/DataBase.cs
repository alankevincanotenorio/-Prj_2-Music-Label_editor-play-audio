﻿namespace DataBaseApp
{
    using System;
    using System.Data.SQLite;
    using System.IO;
    public class DataBase
    {
        private static DataBase? _instance = null;
        private SQLiteConnection _connection;

        //Constructor
        private DataBase()  
        {
            string dbPath = "./DataBaseApp/database/DataBase.db";
            bool dbExists = File.Exists(dbPath);
            string connectionString = $"Data Source={dbPath};Version=3;";
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
            Console.WriteLine("Data base connection open");
            if (!dbExists)
            {
                CreateTables();
            }
        }

        // Singleton design pattern
        public static DataBase Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new DataBase();
                }
                return _instance;
            }
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

        public SQLiteConnection GetConnection()
        {
            return _connection;
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


        //addPerformer
        public bool InsertPerformer (Performer performer)
        {
            bool added = false;
            try
            {
                string query = "INSERT INTO performers (id_performer, id_type, name)" +
                        "VALUES (@id_performer, @id_type, @name)";
                using(SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id_performer", performer.IdPerformer);
                    command.Parameters.AddWithValue("@id_type", performer.IdType);
                    command.Parameters.AddWithValue("@name", performer.Name);
                    int rowsAffected = command.ExecuteNonQuery();
                    added = rowsAffected > 0;
                }
                Console.WriteLine("Performer added");
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
                string query = "INSERT INTO rolas (id_rola, id_performer, id_album, path, title, track, year, genre) " +
                            "VALUES (@id_rola, @id_performer, @id_album, @path, @title, @track, @year, @genre)";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id_rola", rola.IdRola);
                    command.Parameters.AddWithValue("@id_performer", rola.IdPerformer);
                    command.Parameters.AddWithValue("@id_album", rola.IdAlbum);
                    command.Parameters.AddWithValue("@path", rola.Path);
                    command.Parameters.AddWithValue("@title", rola.Title);
                    command.Parameters.AddWithValue("@track", rola.Track);
                    command.Parameters.AddWithValue("@year", rola.Year);
                    command.Parameters.AddWithValue("@genre", rola.Genre);
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

        //get performer by id
        public Performer? GetPerformerById(int id_performer)
        {
            Performer? performer = null;
            try
            {
                string query = "SELECT * FROM performers WHERE id_performer = @id_performer";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id_performer", id_performer);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int idType = reader.GetInt32(1);
                            string name = reader.GetString(2);
                            performer = new Performer(id_performer, name, idType);
                            return performer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while searching performer: " + ex.Message);
            }
            return performer;
        }
    }
}