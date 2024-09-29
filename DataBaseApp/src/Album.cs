public class Album
{
    public int IdAlbum { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }
    public int Year { get; set; } 

    //constructor
    public Album(int idAlbum, string path, string name, int year)
    {
        IdAlbum = idAlbum;
        Path = path;
        Name = name;
        Year = year;
    }

    // Constructor to new rolas
    public Album(string path, string name, int year)
    {
        IdAlbum = 0;
        Path = path;
        Name = name;
        Year = year;
    }

    //Show rola info
    public void ShowInfo()
    {
        Console.WriteLine($"Título: {Name}");
        Console.WriteLine($"Álbum ID: {IdAlbum}");
        Console.WriteLine($"Ruta: {Path}");
        Console.WriteLine($"Año: {Year}");
    }
}