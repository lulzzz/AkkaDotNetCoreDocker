# TODO

### API Client
Currently the Loaner project is embedded with an ASP.NET Web API client, but perhaps we want to split those out, and keep the API interface to the service in a separate project. The issue is deciding how to interface Akka and the WebAPI. 

### Client Cluster
I've already included Lighthouse, and it's set up to cluster the actor system. However, we really want to demo a client-per-machine scenario where failures across accounts, portfolios or whole machines can be demostrated to not crash the cluster.

### Cross-platform
Show there development can be done using VS2017 on windows, and deployment, unchanged, on Docker.

### UI
Need a friendly-looking UI to showcase in the demo. Perhaps one for the Business Rules?
