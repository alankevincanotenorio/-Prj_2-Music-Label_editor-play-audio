public class Group{
    private int IdGroup = 0;
    private string Name;
    private string StartDate;
    private string EndDate;

    public Group(int id_group, string name, string start_date, string end_date)
    {
        IdGroup = id_group;
        Name = name;
        StartDate = start_date;
        EndDate = end_date;
    }

    public Group(string name, string start_date, string end_date)
    {
        Name = name;
        StartDate = start_date;
        EndDate = end_date;
        
    }
}