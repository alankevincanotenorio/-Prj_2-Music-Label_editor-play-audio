namespace MinerApp
{
    using System;
    using System.IO;
    using TagLib;
    using DataBaseApp;
    using System.Collections;
    using System.Collections.Generic;
    public class Miner
    {
        private string _path;
        private List<Rola> _rolas = new List<Rola>();
        private List<Performer> _performers = new List<Performer>();
        private List<Album> _albums = new List<Album>();
        private DataBase _database = DataBase.Instance();

        //constructor
        public Miner(string path)
        {
            _path = path;
        }

        //browse directories and add the rola in rolas mining metadata
        public bool Mine(string path)
        {
            try
            {
                var mp3Files = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
                foreach (var file in mp3Files)
                {
                    bool IsValidFile = Path.GetExtension(file).Equals(".mp3", StringComparison.OrdinalIgnoreCase);
                    if (IsValidFile)
                    {
                        Rola? rola = GetMetadata(file);
                        if (rola != null)
                        {
                            if (!_rolas.Exists(r => r.GetPath() == rola.GetPath()))
                            {
                                _rolas.Add(rola);
                                Console.WriteLine($"Rola added: {rola.GetTitle()}");
                            }
                            else
                            {
                                Console.WriteLine($"Rola '{rola.GetTitle()}' ya existe en la lista de rolas.");
                            }
                        }
                    }
                }
                Console.WriteLine("Mining finish");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mining error: " + ex.Message);
                return false;
            }
        }

        //save metadata 
        public void SaveMetadata()
        {
            foreach (Rola rola in _rolas)
            {
                Rola? check = _database.GetRolaByTitleAndPath(rola.GetTitle(), rola.GetPath());
                if (check == null)
                {
                    _database.InsertRola(rola);
                    Console.WriteLine("Rola inserted");
                }
                else
                {
                    Console.WriteLine($"Rola '{rola.GetTitle()}' already exists with the same path.");
                }
            }
        }

        //Get metadata
        private Rola? GetMetadata(string rola_str)
        {
            Rola? rola = null;
            try
            {
                var file = TagLib.File.Create(rola_str);
                //TPE1
                string performer = file.Tag.FirstPerformer ?? "Unknown";
                //TIT2
                string title = file.Tag.Title ?? "Unknown";
                //TALB
                string album = file.Tag.Album ?? "Unknown";
                //TDRC
                uint year = file.Tag.Year != 0 ? file.Tag.Year : (uint)System.IO.File.GetCreationTime(rola_str).Year;
                //TCON
                string genre = file.Tag.FirstGenre ?? "Unknown";
                //TRCK
                uint track = file.Tag.Track != 0 ? file.Tag.Track : 0;
                uint totalTracks = file.Tag.TrackCount;
                string trackInfo = totalTracks > 0 ? $"{track}/{totalTracks}" : $"{track}";

                int performerId = InsertPerformerIfNotExists(performer);
                int albumId = InsertAlbumIfNotExists(album, rola_str, (int)year);
                rola = new Rola(performerId, albumId, rola_str, title, (int)track, (int)year, genre);

                Console.WriteLine($"Artista: {performer}");
                Console.WriteLine($"Título: {title}");
                Console.WriteLine($"Álbum: {album}");
                Console.WriteLine($"Año: {year}");
                Console.WriteLine($"Pista: {trackInfo}");
                Console.WriteLine($"Genero: {genre}");
                return rola;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error mining metadata: " + ex.Message);
            }
            return rola;
        }

        // insert performer
        private int InsertPerformerIfNotExists(string performer_name)
        {
            Performer? performer = _database.GetPerformerByName(performer_name);
            if (performer != null)
            { 
                _performers.Add(performer);  //check this cause is for reopen the app
                return performer.GetIdPerformer();
            }
            else
            {
                performer = new Performer(performer_name);
                _database.InsertPerformer(performer);
                _performers.Add(performer);
                return performer.GetIdPerformer();
            }
        }

        // insert album
        private int InsertAlbumIfNotExists(string album_name, string album_path, int year)
        {
            Album? album = _database.GetAlbumByName(album_name);
            if (album != null)
            {
                _albums.Add(album); //check this cause is for reopen the app
                return album.GetIdAlbum();
            }
            else
            {
                album = new Album(album_path, album_name, year);
                _database.InsertAlbum(album);   
                _albums.Add(album); //
                return album.GetIdAlbum();
            }
        }


        // SETTERS & GETTERS

        public void SetPath(string path)
        {
            _path = path;
        }

        public string GetPath()
        {
            return _path;
        }

        public List<Rola> GetRolas()
        {
            return _rolas;
        }
            
        public List<Performer> GetPerformers()
        {
            return _performers;
        }

        public List<Album> GetAlbums()
        {
            return _albums;
        }

        public DataBase GetDataBase()
        {
            return _database;
        }
    }
}