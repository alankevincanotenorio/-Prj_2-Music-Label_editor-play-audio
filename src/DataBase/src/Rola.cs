public class Rola
{
    private int IdRola;
    private int IdPerformer;
    private int IdAlbum;
    private string Path;
    private string Title;
    private int Track;
    private int Year;
    private string Genre;

    // Constructor
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

    // Constructor for new rolas
    public Rola(int idPerformer, int idAlbum, string path, string title, int track, int year, string genre)
    {
        IdPerformer = idPerformer;
        IdAlbum = idAlbum;
        Path = path;
        Title = title;
        Track = track;
        Year = year;
        Genre = genre;
    }

    // getters
    public int GetIdRola() => IdRola;
    public int GetIdPerformer() => IdPerformer;
    public int GetIdAlbum() => IdAlbum;
    public string GetPath() => Path;
    public string GetTitle() => Title;
    public int GetTrack() => Track;
    public int GetYear() => Year;
    public string GetGenre() => Genre;

    // setters
    public void SetIdRola(int id_rola) => IdRola = id_rola;
    public void SetIdPerformer(int id_performer) => IdPerformer = id_performer;
    public void SetIdAlbum(int id_album) => IdAlbum = id_album;
    public void SetPath(string path) => Path = path;
    public void SetTitle(string title) => Title = title;
    public void SetTrack(int track) => Track = track;
    public void SetYear(int year) => Year = year;
    public void SetGenre(string genre) => Genre = genre;
}