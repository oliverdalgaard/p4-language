using System.Globalization;

namespace Matilda;

public class Table
{
    public string Identifier { get; }
    public List<Column> Schema { get; }

    public List<string[]> File { get; }

    public List<TableHeader> Headers { get; }
    public List<TableRecord> Records { get; }

    public Table(string identifier, List<Column> schema, List<string[]> file)
    {
        Identifier = identifier;
        Schema = schema;
        File = file;

        Headers = new List<TableHeader>();
        Records = new List<TableRecord>();
    }

    public void ParseTypes()
    {
        if (Schema.Count != File[0].Count())
        {
            throw new Exception("File does not match schema");
        }

        for (int i = 0; i < Schema.Count; i++)
        {
            Headers.Add(new TableHeader(File[0][i], Schema[i].Type));
        }

        for (int i = 1; i < File.Count(); i++) // + 1 i count da vi allerede har tjekket header.
        {
            List<Val> tableRecordVals = new List<Val>();

            for (int j = 0; j < File[i].Count(); j++)
            {
                if (Schema.Count != File[i].Count())
                {
                    throw new Exception("Row does not match schema");
                }

                switch (Headers[j].Type)
                {
                    case IntT:
                        tableRecordVals.Add(new IntVal(Int32.Parse(File[i][j])));
                        break;

                    case FloatT:
                        tableRecordVals.Add(new FloatVal(float.Parse(File[i][j], new CultureInfo("en", false))));
                        break;

                    case BoolT:
                        tableRecordVals.Add(new BoolVal(bool.Parse(File[i][j])));
                        break;

                    case StringT:
                        tableRecordVals.Add(new StringVal(File[i][j]));
                        break;

                    default:
                        throw new Exception("Unknown type");

                }
            }

            Records.Add(new TableRecord(tableRecordVals));
        }
    }
}

public class TableHeader
{
    public string Identifier { get; }
    public Type Type { get; }

    public TableHeader(string identifier, Type type)
    {
        Identifier = identifier;
        Type = type;
    }
}

public class TableRecord
{
    public List<Val> Values { get; }

    public TableRecord(List<Val> values)
    {
        Values = values;
    }
}