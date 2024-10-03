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

    //getters
    public int GetIdPerson() => IdPerson;
    public string GetStageName() => StageName;
    public string GetRealName() => RealName;
    public string GetBirthDate() => BirthDate;
    public string GetDeathDate() => DeathDate;

    // setters
    public void SetIdPerson(int id_person) => IdPerson = id_person;
    public void SetStageName(string stage_name) => StageName = stage_name;
    public void SetRealName(string real_name) => RealName = real_name;
    public void SetBirthDate(string birth_date) => BirthDate = birth_date;
    public void SetDeathDate(string death_date) => DeathDate = death_date;
}