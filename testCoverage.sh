cd ./Matilda.Test/
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/Coverage/ /p:ExcludeByFile=\"**/Parser.cs,**/Scanner.cs,**/Matilda.cs\"
reportgenerator -reports:./TestResults/Coverage/coverage.cobertura.xml -targetdir:./TestResults/Coverage/Html -reporttypes:Html_Dark