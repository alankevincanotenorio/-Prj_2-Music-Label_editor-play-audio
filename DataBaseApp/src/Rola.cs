public class Rola
{
    private int IdRola { get; set; }
    private int IdPerformer { get; set; }
    private int IdAlbum { get; set; }
    private string Path { get; set; }
    private string Title { get; set; }
    private int Track { get; set; }
    private int Year { get; set; }
    private string Genre { get; set; }

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

    // SETTERS & GETTERS

    public void SetIdRola(int id_rola)
    {
        IdRola = id_rola;
    }

    public int GetIdRola()
    {
        return IdRola;
    }

    public void SetIdPerformer(int id_performer)
    {
        IdPerformer = id_performer;
    }

    public int GetIdPerformer()
    {
        return IdPerformer;
    }

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

    public void SetTitle(string title)
    {
        Title = title;
    }

    public string GetTitle()
    {
        return Title;
    }

    public void SetTrack(int track)
    {
        Track = track;
    }

    public int GetTrack()
    {
        return Track;
    }

    public void SetYear(int year)
    {
        Year = year;
    }

    public int GetYear()
    {
        return Year;
    }

    public void SetGenre(string genre)
    {
        Genre = genre;
    }

    public string GetGenre()
    {
        return Genre;
    }
}