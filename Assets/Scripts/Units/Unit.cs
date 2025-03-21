using static RankManager;

public class Unit
{
    public string unitName;
    public int age;
    public Rank rank;
    public Job job;
    public bool isAlive = true;

    public Unit(Rank rank)
    {
        this.rank = rank;
        this.job = Job.Unassigned;
        this.age = 18; // Default starting age
    }
}
