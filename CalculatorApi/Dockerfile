FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

COPY bin/Release/netcoreapp3.1/publish/ ./

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "CalculatorApi.dll"]

# Run this command in a command prompt window to build the image:
# docker build -t calculator-api .

# Run the image in a container with this command
# docker run -p 80:80 -it --name calculator-api --rm calculator-api

# To try the web service in a browser load this URL:
# http://localhost/api/Calculator/square/6