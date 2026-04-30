cd ./Matilda.Test/
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
reportgenerator -reports:./TestResults/Coverage/coverage.cobertura.xml -targetdir:./TestResults/Coverage/Html -reporttypes:Html_Dark