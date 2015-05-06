# Angiris

What's Project Angiris: Proof-Of-Concept of Backend(Crawler)-as-a-Service (BaaS) on Azure 

● Crawling external resources on-demand or by schedule with pre-defined logics and rules 

● Storing and indexing entities in NoSQL database 

● A distributed Azure solution with high scalability & availability 

● Hosting an agent farm to handle extensible crawlers and query-tasks asynchronously 

Demo: http://angiris.ranger.im/

How to host:
1. Clone
2. Save Angiris/src/Angiris.Core.Config/config.sample.json as Angiris/src/Angiris.Core.Config/config.release.json
3. Replace your own Azure service endpoints and credentials in config.release.json
4. Build and deploy
5. Don't attempt to try the hardcoded credentials in legacy commits :)



![What's Angiris](http://angiris.ranger.im/Images/Overview.PNG)

Architecture with Azure Services
![Sln Arch](http://angiris.ranger.im/Images/SlnArch.PNG)
