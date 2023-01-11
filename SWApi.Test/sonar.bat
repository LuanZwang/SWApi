dotnet test --collect:"XPlat Code Coverage;Format=opencover;"
dotnet sonarscanner begin /k:"SWApi" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="TOKEN_VALUE" /d:sonar.cs.opencover.reportsPaths="./**/coverage.opencover.xml"
dotnet build "../SWApi.sln"
dotnet sonarscanner end /d:sonar.login="TOKEN_VALUE"
PAUSE