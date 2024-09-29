using Xunit;
using DataBaseApp;

public class DataBaseTests
{
    [Fact]
    public void TestInsertPerformer()
    {
        var database = DataBase.Instance(":memory:");
        Performer test1 = new Performer("PerformerTest");
        bool response = database.InsertPerformer(test1);
        Assert.True(response);
    }

    [Fact]
    public void TestInsertAlreadyPerformer()
    {
        var database = DataBase.Instance(":memory:");
        Performer test1 = new Performer("PerformerTest");
        bool secondResponse = database.InsertPerformer(test1);
        Assert.False(secondResponse);
    }

    [Fact]
    public void TestGetPerformer()
    {
        var database = DataBase.Instance(":memory:");
        Performer test1 = new Performer("Test1");
        database.InsertPerformer(test1);
        Performer? retrieved = database.GetPerformerByName("Test1");
        Assert.NotNull(retrieved);
        Assert.Equal(test1.GetIdPerformer(), retrieved?.GetIdPerformer());
        Assert.Equal(test1.GetName(), retrieved?.GetName());
        Assert.Equal(test1.GetIdType(), retrieved?.GetIdType());
    }

    [Fact]
    public void TestGetInexistentPerformer()
    {
        var database = DataBase.Instance(":memory:");
        Performer? retrieved = database.GetPerformerByName("NonExistent");
        Assert.Null(retrieved);
    }

    [Fact]
    public void TestInsertRola()
    {
        var database = DataBase.Instance(":memory:");
        Performer performer = new Performer("Rola Performer");
        database.InsertPerformer(performer);
        Album album = new Album("/path/to/album", "Rola Album", 2024);
        database.InsertAlbum(album);
        Rola rola = new Rola(performer.GetIdPerformer(), album.GetIdAlbum(), "/path/to/rola", "TestRola", 1, 2024, "Rock");
        bool response = database.InsertRola(rola);
        Assert.True(response);
    }

    [Fact]
    public void TestInsertAlreadyRola()
    {
        var database = DataBase.Instance(":memory:");
        Performer performer = new Performer("Rola Performer"); 
        database.InsertPerformer(performer);
        Album album = new Album("/path/to/album", "Rola Album", 2024);
        database.InsertAlbum(album);
        Rola rola = new Rola(performer.GetIdPerformer(), album.GetIdAlbum(), "/path/to/rola", "TestRola", 1, 2024, "Rock");
        bool secondResponse = database.InsertRola(rola);
        Assert.False(secondResponse);
    }

    [Fact]
    public void TestGetRola()
    {
        var database = DataBase.Instance(":memory:");
        Performer performer = new Performer("Rola Performer 2");
        database.InsertPerformer(performer);
        Album album = new Album("/path/to/album", "Rola Album 2", 2024);
        database.InsertAlbum(album);
        Rola rola = new Rola(performer.GetIdPerformer(), album.GetIdAlbum(), "/path/to/rola", "TestRola2", 1, 2024, "Rock");
        database.InsertRola(rola);
        Rola? retrievedRola = database.GetRolaByName("TestRola2");
        Assert.NotNull(retrievedRola);
        Assert.Equal(rola.GetIdRola(), retrievedRola?.GetIdRola());
        Assert.Equal(rola.GetIdPerformer(), retrievedRola?.GetIdPerformer());
        Assert.Equal(rola.GetIdAlbum(), retrievedRola?.GetIdAlbum());
        Assert.Equal(rola.GetPath(), retrievedRola?.GetPath());
        Assert.Equal(rola.GetTitle(), retrievedRola?.GetTitle());
        Assert.Equal(rola.GetTrack(), retrievedRola?.GetTrack());
        Assert.Equal(rola.GetYear(), retrievedRola?.GetYear());
        Assert.Equal(rola.GetGenre(), retrievedRola?.GetGenre());
    }

    [Fact]
    public void TestGetInexistentRolaByName()
    {
        var database = DataBase.Instance(":memory:");
        Rola? retrieved = database.GetRolaByName("NonExistent");
        Assert.Null(retrieved);
    }

    [Fact]
    public void TestInsertAlbum()
    {
        var database = DataBase.Instance(":memory:");
        Album album = new Album("/path/to/album", "AlbumTest", 2024);
        bool response = database.InsertAlbum(album);
        Assert.True(response);
    }

    [Fact]
    public void TestInsertAlreadyAlbum()
    {
        var database = DataBase.Instance(":memory:");
        Album album = new Album("/path/to/album", "AlbumTest", 2024);
        bool secondResponse = database.InsertAlbum(album);
        Assert.False(secondResponse);
    }

    [Fact]
    public void TestGetAlbum()
    {
        var database = DataBase.Instance(":memory:");
        Album album = new Album("/path/to/album", "Album r", 2024);
        bool response = database.InsertAlbum(album);
        Assert.True(response);
        Album? retrievedAlbum = database.GetAlbumByName("Album r");
        Assert.NotNull(retrievedAlbum);
        Assert.Equal(album.GetIdAlbum(), retrievedAlbum?.GetIdAlbum());
        Assert.Equal(album.GetPath(), retrievedAlbum?.GetPath());
        Assert.Equal(album.GetName(), retrievedAlbum?.GetName());
        Assert.Equal(album.GetYear(), retrievedAlbum?.GetYear());
    }

    [Fact]
    public void TestGetInexistentAlbumByName()
    {
        var database = DataBase.Instance(":memory:");
        Album? retrieved = database.GetAlbumByName("NonExistent");
        Assert.Null(retrieved);
    }
}