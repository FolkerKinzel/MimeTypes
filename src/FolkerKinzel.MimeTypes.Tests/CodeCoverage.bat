dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./reports/ /p:ExcludeByAttribute=\"Obsolete,GeneratedCode\" /p:SkipAutoProps=true /p:DoesNotReturnAttribute="DoesNotReturnAttribute"
reportgenerator -reports:./reports/coverage.net7.0.cobertura.xml -targetdir:./reports/html/
cd reports/html/
index.htm