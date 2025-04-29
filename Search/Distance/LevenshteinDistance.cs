namespace datastructures_project.Search.Distance;

public class LevenshteinDistance
{
    public static int Distance(string a, string b)
    {
        int[,] matrix = new int[a.Length + 1, b.Length + 1];
        
        for (int i = 0; i <= a.Length; i++)
        {
            matrix[i, 0] = i;
        }

        for (int j = 0; j <= b.Length; j++)
        {
            matrix[0, j] = j;
        }

        for (int j = 1; j <= b.Length; j++)
        {
            for (int i = 1; i <= a.Length; i++)
            {
                int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost
                );
            }
        }

        return matrix[a.Length, b.Length];
    }
}