FROM microsoft/dotnet

WORKDIR /AkkaDotNetCoreDocker

#Copy all the source files and junk over to the container
COPY . .
#Just to make sure, show me
run ls -lt
#Now restore .net packages
RUN dotnet restore 
#Cut the release
RUN dotnet publish -c Release -o out
#How to run it
ENTRYPOINT ["dotnet", "WebClient/out/WebClient.dll"]

