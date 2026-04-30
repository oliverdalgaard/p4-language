namespace Matilda;

public class Program
{
    public List<TopLevelDeclaration> TopLevelDeclarations { get; }
    public Stmt Stmt { get; }

    public Program(List<TopLevelDeclaration> topLevelDeclarations, Stmt stmt)
    {
        TopLevelDeclarations = topLevelDeclarations;
        Stmt = stmt;
    }
    public Program(Stmt stmt)
    {
        TopLevelDeclarations = new List<TopLevelDeclaration>();
        Stmt = stmt;
    }

    public Program(List<TopLevelDeclaration> topLevelDeclarations)
    {
        TopLevelDeclarations = topLevelDeclarations;
        Stmt = Skip.Instance;
    }
}