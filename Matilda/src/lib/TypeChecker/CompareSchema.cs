namespace Matilda;

public class CompareSchema
{
    public static bool Compare(List<Column>? l1, List<Column>? l2)
    {
        if (l1 == null || l2 == null)
        {
            return false;
        }

        if (l1.Count != l2.Count)
        {
            return false;
        }

        for (int i = 0; i < l1.Count; i++)
        {
            if (l1[i].Id != l2[i].Id || l1[i].Type != l2[i].Type)
            {
                return false;
            }
        }

        return true;
    }

    public static bool ContainsDuplicate(List<Column> l1)
    {

        List<string> names = new List<string>();

        foreach (Column col in l1)
        {
            names.Add(col.Id);
        }

        if (names.Count != names.Distinct().Count())
        {
            return true;
        }
        return false;
    }
}