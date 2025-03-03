public class Performer
{
    private int IdPerformer;
    private string Name;
    private int IdType = (int)PerformerType.Unknown;

    // Constructor for existing performers
    public Performer(int id_performer, string name, int id_type)
    {
        IdPerformer = id_performer;
        Name = name;
        IdType = id_type;
    }

    // Constructor for new perfomres
    public Performer(string name) => Name = name;

    // getters
    public int GetIdPerformer() => IdPerformer;
    public string GetName() => Name;
    public int GetIdType() => IdType;

    // setters
    public void SetIdPerformer(int id_performer) => IdPerformer = id_performer;
    public void SetName(string name) => Name = name;
    public void SetIdType(PerformerType id_type) => IdType = (int)id_type;
}