FROM microsoft/dotnet:2.1.2-runtime
COPY . /app
WORKDIR /app

EXPOSE 8080/tcp
ENV ASPNETCORE_URLS http://*:8080

ENTRYPOINT ["dotnet", "Hello.dll"]