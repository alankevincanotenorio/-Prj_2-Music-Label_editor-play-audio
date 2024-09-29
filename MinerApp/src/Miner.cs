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
        public DataBase db = DataBase.Instance();

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

        public void Add(){
            foreach(Rola rola in _rolas)
            {
                db.InsertRola(rola);
                Console.WriteLine("Rola inserted");
            }
        }

        private Rola? GetMetadata(string rolaStr)
        {
            Rola? rola = null;
            try
            {
                var file = TagLib.File.Create(rolaStr);
                //TPE1
                string performer = file.Tag.FirstPerformer ?? "Unknown";
                //TIT2
                string title = file.Tag.Title ?? "Unknown";
                //TALB
                string album = file.Tag.Album ?? "Unknown";
                //TDRC
                uint year = file.Tag.Year != 0 ? file.Tag.Year : (uint)System.IO.File.GetCreationTime(rolaStr).Year;
                //TCON
                string genre = file.Tag.FirstGenre ?? "Unknown";
                //TRCK
                uint track = file.Tag.Track != 0 ? file.Tag.Track : 0;
                uint totalTracks = file.Tag.TrackCount;
                string trackInfo = totalTracks > 0 ? $"{track} de {totalTracks}" : $"{track}";

                int performerId = InsertPerformerIfNotExists(performer);
                rola = new Rola(performerId, 0, rolaStr, title, (int)track, (int)year, genre);

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

        private int InsertPerformerIfNotExists(string performerName)
        {
            Performer? performer = db.GetPerformerByName(performerName);
            if (performer != null)
            {
                return performer.IdPerformer;
            }
            else
            {
                Performer newPerformer = new Performer(performerName, 0);
                db.InsertPerformer(newPerformer);
                return newPerformer.IdPerformer;
            }
        }
    }
}