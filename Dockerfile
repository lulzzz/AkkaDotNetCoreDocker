FROM microsoft/dotnet

WORKDIR /AkkaDotNetCoreDocker

COPY . .
run ls -lt
RUN dotnet restore 

RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "AkkaDotNetCoreDocker/out/AkkaDotNetCoreDocker.dll"]

