# AkkaDotNetCoreDocker
This is a POC of Akka.net on .Net Core deployed on Docker.  

The goals of this POC are as follows:
+ Determine viability of modeling a financial servicing domain using actors to hold state instead of using a database.
+ Determine the viability of using Akka.Net in a production-like environment.
+ Identify challenges of using Event Sourcing to model state transitions.


## Running It
```bash
cd AkkaNetPoC/ 
dotnet run --project Loaner
```
## To run it in a cluster use Lighthouse
```bash
cd AkkaNetPoC/ 
dotnet run --project Lighthouse.NetCoreApp
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

## Generating Sample Accounts
```bash
cd SampleData
./GenerateSampleData.pl ####
```
Where #### is the number of accounts you want ot generate.
