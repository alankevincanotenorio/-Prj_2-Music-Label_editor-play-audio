public class Album
{
    private int IdAlbum;
    private string Path;
    private string Name;
    private int Year;

    // Constructor
    public Album(int idAlbum, string path, string name, int year)
    {
        IdAlbum = idAlbum;
        Path = path;
        Name = name;
        Year = year;
    }

    // Constructor for new albums
    public Album(string path, string name, int year)
    {
        Path = path;
        Name = name;
        Year = year;
    }

    // getters
    public int GetIdAlbum() => IdAlbum;
    public string GetPath() => Path;
    public string GetName() => Name;
    public int GetYear() => Year;

    // setters
    public void SetIdAlbum(int id_album) => IdAlbum = id_album;
    public void SetPath(string path) => Path = path;
    public void SetName(string name) => Name = name;
    public void SetYear(int year) => Year = year;
}