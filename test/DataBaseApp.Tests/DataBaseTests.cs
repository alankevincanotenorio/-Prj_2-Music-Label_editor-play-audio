using Xunit;
using DataBaseApp;

public class DataBaseTests
{
    [Fact]
    public void TestInsertPerformer()
    {
        var database = DataBase.TestInstance();
        Performer test1 = new Performer("PerformerTest");
        bool response = database.InsertPerformer(test1);
        Assert.True(response);
    }

    [Fact]
    public void TestInsertAlreadyPerformer()
    {
        var database = DataBase.TestInstance();
        Performer test1 = new Performer("PerformerTest");
        database.InsertPerformer(test1);
        bool secondResponse = database.InsertPerformer(test1);
        Assert.False(secondResponse);
    }

    [Fact]
    public void TestGetPerformer()
    {
        var database = DataBase.TestInstance();
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
        var database = DataBase.TestInstance();
        Performer? retrieved = database.GetPerformerByName("NonExistent");
        Assert.Null(retrieved);
    }

    [Fact]
    public void TestInsertRola()
    {
        var database = DataBase.TestInstance();
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
        var database = DataBase.TestInstance();
        Performer performer = new Performer("Rola Performer");
        database.InsertPerformer(performer);
        Album album = new Album("/path/to/album", "Rola Album", 2024);
        database.InsertAlbum(album);
        Rola rola = new Rola(performer.GetIdPerformer(), album.GetIdAlbum(), "/path/to/rola", "TestRola", 1, 2024, "Rock");
        database.InsertRola(rola);
        bool secondResponse = database.InsertRola(rola);
        Assert.False(secondResponse);
    }

    [Fact]
    public void TestGetRola()
    {
        var database = DataBase.TestInstance();
        Performer performer = new Performer("Rola Performer 2");
        database.InsertPerformer(performer);
        Album album = new Album("/path/to/album", "Rola Album 2", 2024);
        database.InsertAlbum(album);
        Rola rola = new Rola(performer.GetIdPerformer(), album.GetIdAlbum(), "/path/to/rola", "TestRola2", 1, 2024, "Rock");
        database.InsertRola(rola);
        Rola? retrievedRola = database.GetRolaByTitleAndPath("TestRola2", "/path/to/rola");
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
        var database = DataBase.TestInstance();
        Rola? retrieved = database.GetRolaByTitleAndPath("NonExistent", "path/non/existent");
        Assert.Null(retrieved);
    }

    [Fact]
    public void TestInsertAlbum()
    {
        var database = DataBase.TestInstance();
        Album album = new Album("/path/to/album", "AlbumTest", 2024);
        bool response = database.InsertAlbum(album);
        Assert.True(response);
    }

    [Fact]
    public void TestInsertAlreadyAlbum()
    {
        var database = DataBase.TestInstance();
        Album album = new Album("/path/to/album", "AlbumTest", 2024);
        database.InsertAlbum(album);
        bool secondResponse = database.InsertAlbum(album);
        Assert.False(secondResponse);
    }

    [Fact]
    public void TestGetAlbum()
    {
        var database = DataBase.TestInstance();
        Album album = new Album("/path/to/album", "Album retrieved", 2024);
        database.InsertAlbum(album);
        Album? retrievedAlbum = database.GetAlbumByNameAndPath("Album retrieved", "/path/to/album");
        Assert.NotNull(retrievedAlbum);
        Assert.Equal(album.GetIdAlbum(), retrievedAlbum?.GetIdAlbum());
        Assert.Equal(album.GetPath(), retrievedAlbum?.GetPath());
        Assert.Equal(album.GetName(), retrievedAlbum?.GetName());
        Assert.Equal(album.GetYear(), retrievedAlbum?.GetYear());
    }

    [Fact]
    public void TestGetInexistentAlbumByName()
    {
        var database = DataBase.TestInstance();
        Album? retrieved = database.GetAlbumByNameAndPath("NonExistent", "path/non/existent");
        Assert.Null(retrieved);
    }

    [Fact]
    public void TestInsertPerson()
    {
        var database = DataBase.TestInstance();
        Person person = new Person("PersonExample", "Real Name", "01/01/1990", "01/01/2020");
        bool response = database.InsertPerson(person);
        Assert.True(response);
    }

    [Fact]
    public void TestInsertAlreadyPerson()
    {
        var database = DataBase.TestInstance();
        Person person = new Person("Stage Name", "Real Name", "01/01/1990", "01/01/2020");
        database.InsertPerson(person);
        bool secondResponse = database.InsertPerson(person);
        Assert.False(secondResponse);
    }

    [Fact]
    public void TestGetPerson()
    {
        var database = DataBase.TestInstance();
        Person person = new Person("Person Retrieved", "Real Name", "01/01/1990", "01/01/2020");
        database.InsertPerson(person);
        Person? retrieved = database.GetPersonByStageName("Person Retrieved");
        Assert.NotNull(retrieved);
        Assert.Equal(person.GetIdPerson(), retrieved?.GetIdPerson());
        Assert.Equal(person.GetStageName(), retrieved?.GetStageName());
        Assert.Equal(person.GetRealName(), retrieved?.GetRealName());
        Assert.Equal(person.GetBirthDate(), retrieved?.GetBirthDate());
        Assert.Equal(person.GetDeathDate(), retrieved?.GetDeathDate());
    }

    [Fact]
    public void TestInsertGroup()
    {
        var database = DataBase.TestInstance();
        Group group = new Group("Group example", "01/01/2000", "01/01/2010");
        bool response = database.InsertGroup(group);
        Assert.True(response);
    }

    [Fact]
    public void TestInsertAlreadyGroup()
    {
        var database = DataBase.TestInstance();
        Group group = new Group("Group Name", "01/01/2000", "01/01/2010");
        database.InsertGroup(group);
        bool secondResponse = database.InsertGroup(group);
        Assert.False(secondResponse);
    }

    [Fact]
    public void TestGetGroup()
    {
        var database = DataBase.TestInstance();
        Group group = new Group("Group retrieved", "01/01/2000", "01/01/2010");
        database.InsertGroup(group);
        Group? retrieved = database.GetGroupByName("Group retrieved");
        Assert.NotNull(retrieved);
        Assert.Equal(group.GetIdGroup(), retrieved?.GetIdGroup());
        Assert.Equal(group.GetName(), retrieved?.GetName());
        Assert.Equal(group.GetStartDate(), retrieved?.GetStartDate());
        Assert.Equal(group.GetEndDate(), retrieved?.GetEndDate());
    }
}