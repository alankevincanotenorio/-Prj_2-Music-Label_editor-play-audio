public class Performer
{
    private int IdPerformer = 0;
    private string Name;
    private int IdType = 2;

    // Constructor
    public Performer(int id_performer, string name, int id_type)
    {
        IdPerformer = id_performer;
        Name = name;
        IdType = id_type;
    }

    // Constructor for new performer
    public Performer(string name)
    {
        Name = name;
    }

    // getters
    public int GetIdPerformer() => IdPerformer;
    public string GetName() => Name;
    public int GetIdType() => IdType;

    //setters
    public void SetIdPerformer(int id_performer) => IdPerformer = id_performer;
    public void SetName(string name) => Name = name;
    public void SetIdType(int id_type) => IdType = id_type;
}