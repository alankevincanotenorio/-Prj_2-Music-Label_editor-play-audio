namespace MinerApp
{
    using System;
    using System.IO;
    using TagLib;
    using DataBaseApp;

    public class Miner
    {
        private string _path;
        public List<Rola> _rolas = new List<Rola>();
        public DataBase db = DataBase.Instance;

        public Miner(string path)
        {
            _path = path;
        }

        public bool BrowsePaths(string path)
        {
            try
            {
                var mp3Files = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
                foreach (var file in mp3Files)
                {
                    if (IsValidFile(file))
                    {
                        Rola? rola = GetMetadata(file);
                        if (rola != null)
                        {
                            _rolas.Add(rola);
                            Console.WriteLine($"Rola added: {rola.Title}");
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mining error: " + ex.Message);
                return false;
            }
        }

        public bool IsValidFile(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".mp3", StringComparison.OrdinalIgnoreCase);
        }

        public Rola? GetMetadata(string rolaStr)
        {
            Rola? rola = null;
            try
            {
                var file = TagLib.File.Create(rolaStr);
                string performer = file.Tag.FirstPerformer ?? "Unknown";
                string title = file.Tag.Title ?? "Unknown";
                string album = file.Tag.Album ?? "Unknown";
                uint year = file.Tag.Year != 0 ? file.Tag.Year : (uint)System.IO.File.GetCreationTime(rolaStr).Year;
                string genre = file.Tag.FirstGenre ?? "Unknown";
                uint track = file.Tag.Track != 0 ? file.Tag.Track : 0;

                rola = new Rola(0, 0, 0, rolaStr, title, (int)track, (int)year, genre);  // Los ids de performer y álbum están en 0 por ahora
                Console.WriteLine($"Artista: {performer}");
                Console.WriteLine($"Título: {title}");
                Console.WriteLine($"Álbum: {album}");
                Console.WriteLine($"Año: {year}");
                Console.WriteLine($"Pista: {track}");
                Console.WriteLine($"Genero: {genre}");
                return rola;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error mining metadata: " + ex.Message);
            }
            return rola;
        }
    }
}