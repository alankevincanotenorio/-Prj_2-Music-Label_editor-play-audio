public class Performer
{
    private int IdPerformer { get; set; }
    private string Name { get; set; }
    private int IdType { get; set; }

    // Constructor
    public Performer(int id_performer, string name, int id_type)
    {
        IdPerformer = id_performer;
        Name = name;
        IdType = id_type;
    }

    // Constructor to new performer
    public Performer(string name)
    {
        IdPerformer = 0;
        Name = name;
        IdType = 2;
    }

    // SETTERS & GETTERS

    public void SetIdPerformer(int id_performer)
    {
        IdPerformer = id_performer;
    }

    public int GetIdPerformer()
    {
        return IdPerformer;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public string GetName()
    {
        return Name;
    }

    public void SetIdType(int id_type)
    {
        IdType = id_type;
    }

    public int GetIdType()
    {
        return IdType;
    }
}