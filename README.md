# WebJet coding challenge
This is my submission for the WebJet coding challenge.
It contains two folders
- webjet-entertainment-ui (React based UI with NextJS)
- WebJet.Entertainment.API (.NET 8 based backend 

- Development was done entirely on Windows with the following pre-requisites installed.
- ***Important:*** ideally these would be in two separate repositories but for the sake of this coding challenge, it was kept in one to avoid any issues with missed code etc.

## Pre-requisites for development

### Frontend
- npm: 9.7.1
- node: v20.9.0

### Backend
- .NET 8.0 SDK (https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Docker version 28.1.1
- Ensure that dotnet dev-certs https --trust is run so that there's a local development certificate

### Important
- The frontend runs on port 3000
- The backend runs on port 7090
- The backend URL is configured on .env.local in the React app
- There are further READMEs in each app


## Running the app
- At the root of the repository you can run "pwsh Bootstrap.ps1" and this will auto run both apps
	- You will be prompted for the configuration for the backend
	- If port 3000 is busy, the frontend won't run
	- If port 7090 is busy, the backend won't run