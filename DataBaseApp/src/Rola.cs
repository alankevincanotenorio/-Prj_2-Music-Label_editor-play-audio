public class Rola
{
    public int IdRola { get; set; }        
    public int IdPerformer { get; set; }   
    public int IdAlbum { get; set; }       
    public string Path { get; set; }       
    public string Title { get; set; }      
    public int Track { get; set; }         
    public int Year { get; set; }          
    public string Genre { get; set; }      

    //constructor
    public Rola(int idRola, int idPerformer, int idAlbum, string path, string title, int track, int year, string genre)
    {
        IdRola = idRola;
        IdPerformer = idPerformer;
        IdAlbum = idAlbum;
        Path = path;
        Title = title;
        Track = track;
        Year = year;
        Genre = genre;
    }

    // Constructor to new rolas
    public Rola(int idPerformer, int idAlbum, string path, string title, int track, int year, string genre)
    {
        IdRola = 0;
        IdPerformer = idPerformer;
        IdAlbum = idAlbum;
        Path = path;
        Title = title;
        Track = track;
        Year = year;
        Genre = genre;
    }

    //Show rola info
    public void ShowInfo()
    {
        Console.WriteLine($"Título: {Title}");
        Console.WriteLine($"Intérprete ID: {IdPerformer}");
        Console.WriteLine($"Álbum ID: {IdAlbum}");
        Console.WriteLine($"Ruta: {Path}");
        Console.WriteLine($"Número de pista: {Track}");
        Console.WriteLine($"Año: {Year}");
        Console.WriteLine($"Género: {Genre}");
    }
}
