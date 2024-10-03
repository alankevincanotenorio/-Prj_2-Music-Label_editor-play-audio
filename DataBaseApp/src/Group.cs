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

    //getters
    public int GetIdGroup() => IdGroup;
    public string GetName() => Name;
    public string GetStartDate() => StartDate;
    public string GetEndDate() => EndDate;

    // setters
    public void SetIdGroup(int id_group) => IdGroup = id_group;
    public void SetName(string name) => Name = name;
    public void SetStartDate(string start_date) => StartDate = start_date;
    public void SetEndDate(string end_date) => EndDate = end_date;
}