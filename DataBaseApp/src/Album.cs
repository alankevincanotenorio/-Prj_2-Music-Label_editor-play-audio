public class Album
{
    private int IdAlbum { get; set; }
    private string Path { get; set; }
    private string Name { get; set; }
    private int Year { get; set; } 

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

    //SETTERS & GETTERS

    public void SetIdAlbum(int id_album)
    {
        IdAlbum = id_album;
    }

    public int GetIdAlbum()
    {
        return IdAlbum;
    }

    public void SetPath(string path)
    {
        Path = path;
    }

    public string GetPath()
    {
        return Path;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public string GetName()
    {
        return Name;
    }

    public void SetYear(int year)
    {
        Year = year;
    }

    public int GetYear()
    {
        return Year;
    }
}