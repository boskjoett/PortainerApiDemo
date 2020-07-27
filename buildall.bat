cd CalculatorApi
rmdir /S /Q bin\Release
dotnet publish -c Release
docker build -t calculator-api .
cd ..

cd PortainerApiClient
rmdir /S /Q bin\Release
dotnet publish -c Release
docker build -t portainer-api-client .
cd ..
