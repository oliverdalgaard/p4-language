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
}