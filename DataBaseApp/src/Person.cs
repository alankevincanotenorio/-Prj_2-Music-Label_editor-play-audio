public class Person{
    private int IdPerson = 0;
    private string StageName;
    private string RealName;
    private string BirthDate;
    private string DeathDate;

    public Person(int id_person, string stage_name, string real_name, string birth_date, string death_date)
    {
        IdPerson = id_person;
        StageName = stage_name;
        RealName = real_name;
        BirthDate = birth_date;
        DeathDate = death_date;
    }

    public Person(string stage_name, string real_name, string birth_date, string death_date)
    {
        StageName = stage_name;
        RealName = real_name;
        BirthDate = birth_date;
        DeathDate = death_date;
    }
}