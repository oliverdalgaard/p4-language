# p4-language - Matilda

### Main Branch Test Status

[![main](https://github.com/oliverdalgaard/p4-language/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/oliverdalgaard/p4-language/actions/workflows/dotnet.yml)
[![main](https://codecov.io/github/oliverdalgaard/p4-language/branch/main/graph/badge.svg?token=H46BXUDL7I)](https://codecov.io/github/oliverdalgaard/p4-language)

### Dev Branch Test Status

[![dev](https://github.com/oliverdalgaard/p4-language/actions/workflows/dotnet.yml/badge.svg?branch=dev)](https://github.com/oliverdalgaard/p4-language/actions/workflows/dotnet.yml)
[![dev](https://codecov.io/github/oliverdalgaard/p4-language/branch/dev/graph/badge.svg?token=H46BXUDL7I)](https://codecov.io/github/oliverdalgaard/p4-language)

---

## What is Matilda?

Matilda is a statically typed, domain-specific language designed for tabular data processing. It supports reading CSV files, filtering and transforming tables, joining datasets, and grouping with aggregation — all within a clean, readable syntax. Matilda programs are parsed, type-checked, and interpreted using a custom-built compiler pipeline written in C# (.NET 10).

---

## Language Features

- **Primitive types:** `int`, `float`, `bool`, `string`
- **Schema declarations:** Define named table structures with typed columns
- **Table declarations:** Load CSV files into typed tables using `read(...)`
- **Relational operations:**
  - `FILTER(table, predicate)` — filter rows by a boolean expression
  - `SUM(table, groupByColumn, sumColumn, resultSchema)` — group and aggregate
  - `JOIN(table1, table2, key1, key2, resultSchema)` — join two tables on a key
- **Functions:** Declare reusable functions with typed parameters and return types, including table-typed parameters and return values
- **Control flow:** `if` / `else` statements
- **Variable assignment and reassignment**
- **Type casting:** Implicit widening between `int` and `float`
- **Boolean expressions:** `==`, `!=`, `<`, `&&`, `||`, `!`

---

## Example

```matilda
schema transactions = {
    customer_id : int,
    account_id  : int,
    transaction_date : int,
    amount : float
}

schema customers = {
    customer_id : int,
    name : string
}

schema sumResult = {
    name   : string,
    amount : float
}

schema joinResult = {
    account_id       : int,
    customer_id      : int,
    name             : string,
    transaction_date : int,
    amount           : float
}

function table<sumResult> detectFraud(int days_ago, int flag_amount, table<transactions> transactions_table, table<customers> customers_table) {
    table<joinResult> joined  = JOIN(transactions_table, customers_table, customer_id, customer_id, joinResult);
    table<joinResult> filtered = FILTER(joined, transaction_date < days_ago);
    table<sumResult>  summed  = SUM(filtered, name, amount, sumResult);
    summed = FILTER(summed, flag_amount < amount);
    return summed;
}

table<customers>    customers    = read("customers.csv");
table<transactions> transactions = read("transactions.csv");

table<sumResult> Medium_Risk = detectFraud(8, 200,  transactions, customers);
table<sumResult> High_Risk   = detectFraud(8, 1000, transactions, customers);
```

---

## Project Structure

```
p4-language/
├── Matilda/                  # Compiler and runtime
│   ├── src/
│   │   ├── AbstractSyntax/   # AST node definitions (Expr, Stmt, Type, etc.)
│   │   ├── Interpreter/      # Interpreter + environments
│   │   ├── TypeChecker/      # Static type checker
│   │   └── lib/              # Shared utilities
│   ├── Matilda.cs.ATG        # Coco/R grammar definition
│   ├── Parser.cs             # Generated parser
│   └── Scanner.cs            # Generated scanner
│
└── Matilda.Test/             # Test suite
    ├── UnitTests/            # Unit tests
    ├── IntegrationTests/     # End-to-end interpreter tests
    ├── AcceptanceTests/      # Snapshot-based tests
    └── TestMatildaScripts/   # .matilda files used in tests
```

---

## Building and Running

**Build** (regenerates parser/scanner from grammar, then builds):
```sh
# Linux/macOS
sh build.sh

# Windows
build.bat
```

**Run a Matilda program:**
```sh
# Linux/macOS
sh run.sh

# Windows
run.bat
```

**Run tests:**
```sh
sh test.sh       # Linux/macOS
test.bat         # Windows
dotnet test      # Direct
```

**Run tests with coverage report:**
```sh
sh testCoverage.sh   # Linux/macOS
testCoverage.bat     # Windows
```

---

## Testing

The project uses **MSTest** with **Verify** for snapshot-based acceptance testing and **Coverlet** for code coverage. Tests are organised into three layers:

- **Unit tests** — cover individual AST nodes, interpreter evaluation, type checker rules, and helper functions
- **Integration tests** — run full programs and assert on final variable values
- **Acceptance tests** — run full programs and compare the entire environment state against verified snapshots

Coverage reports are uploaded to [Codecov](https://codecov.io/github/oliverdalgaard/p4-language) on every push to `main` and `dev`.

---

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Coco/R (included as `Coco.exe` in the `Matilda/` directory) for regenerating the parser and scanner from the grammar file

*README.md partially generated by Claude AI*