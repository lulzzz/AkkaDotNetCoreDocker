# AkkaDotNetCoreDocker

## Running It
```bash
dotnet run --project WebClient
```

## Setting up Monitoring (Locally)
```bash
docker run -d --name=grafana  --restart=always -p 3000:3000 -e "GF_SERVER_ROOT_URL=http://localhost" -e "GF_SECURITY_ADMIN_PASSWORD=secret"  grafana/grafana
docker run -d --name=graphite --restart=always -p 80:80 -p 2003-2004:2003-2004 -p 2023-2024:2023-2024 -p 8125:8125/udp -p 8126:8126 hopsoft/graphite-statsd
```
or just run:
```bash
runMonitoring.sh
```

# Generating Sample Accounts
```bash
cd SampleData
./GenerateSampleData.pl ####
```
Where #### is the number of accounts you want ot generate.
