namespace datastructures_project.Search.Score;

public interface IScore
{
    public Dictionary<int, double> Calculate(string[] tokens);
}