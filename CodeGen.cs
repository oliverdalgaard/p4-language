using System;

namespace Matilda {

public enum Type {
    INT, BOOL
}

public enum Op {
    ADD, SUB
}

public class Var {
    private string name;
    private Type type;
    private int value;

    public Var(string name, Type type) {
        this.name = name;
        this.type = type;
    }

    public void setValue(int value) {
        this.value = value;
    }

    public int getValue() {
        return this.value;
    }

    public void print() {
        Console.WriteLine("Name: "+this.name);
        Console.WriteLine("Value: "+this.value);
    }
}


public class CodeGenerator {

    public CodeGenerator() {
        Console.WriteLine("CodeGenerator created");
    }

}

}