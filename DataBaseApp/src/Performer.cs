public class Performer
{
    public int IdPerformer { get; set; }
    public string Name { get; set; }
    public int IdType { get; set; }

    // Constructor
    public Performer(int id_performer, string name, int id_type)
    {
        IdPerformer = id_performer;
        Name = name;
        IdType = id_type;
    }

    // Constructor to new performer
    public Performer(string name, int idType)
    {
        IdPerformer = 0;
        Name = name;
        IdType = idType;
    }

    public void ShowInfo()
    {
        Console.WriteLine($"Nombre: {Name}");
        Console.WriteLine($"Tipo de intérprete (Id): {IdType}");
    }
}