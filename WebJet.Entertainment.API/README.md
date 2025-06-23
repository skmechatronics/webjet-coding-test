# README

## Introduction
This is a demo application for the WebJet API Coding test. It consists of multiple projects as defined below.

## Assumptions
- Both Cinemaworld and Filmworld APIs are composed of the same structures
	- However, given that they are separate clients, it would be easy to separate them out if needed
- The titles across movie sources match exactly	

## TL;DR Launching TODO
Run the launch command to dockerize this app and run the API

## Considerations
- APIs are unreliable, can return 503
- The APIs can return a different set of movies so the prices aren't always available
- Health check for build number (for this app)
- Separate health check for whether the APIs are online


## Requirements
.NET 8.0 SDK (build) and .NET 8.0 Runtime

## Features
- Using package.lock files for deterministic restores and Nuget.config to set upstream sources (best practices)
- Environment variables to facilitate docker deployments with one image build and protect secrets
- HealthChecks
	- Basic health check: for liveness / health probes
	- Advanced health check: simulate production environment with external dependencies, used to find out which sources are unhealthy and report overall status
	```
	{
	  "status": "Healthy",
	  "sources": [
		{
		  "source": "Cinemaworld",
		  "status": "Healthy",
		  "message": "Healthy"
		},
		{
		  "source": "Filmworld",
		  "status": "Healthy",
		  "message": "Healthy"
		}
	  ]
	}
	```
- Both Cinemaworld and Filmworld are injected with the same interface but discriminated by a Source property so they can be injected as an IEnumerable
	- This would make it possible to consume future APIs in parallel and makes this easily extensible
	- Provided of course the assumptions match; if they return different data structures
	- Assuming of course they are all similar or at least contain the Title and Price as common properties
	- Can use marker interfaces that contain the minimum subset of properties that are common amongst sources (e.g. IPriceModel)
- Custom converters are used to deserialize 
	- For properties such as Released (DateTime), Rating, Votes and Year
- Use of caching
	- Can use a Redis cache with this solution with some extra code
	- This caches the entire movie collection for each source, individual movie and movie metadata
	- Cache expiration is set appropriately
- Sample tests are provided
	- Since this is a coding test, full coverage was not targeted with 2 unit tests provided for showcase
	- Using Shouldly rather than FluentAssertions since the latter was deprecated 
	- Ideally, the Service layer would have the most unit tests
	- Integration tests (with various boundaries) could be tested
- Using Result<T> with subclasses for a more functional style
	- As much as possible avoid null values for ease of use
	- Code is more verbose but gives it added reliability
	- A Unit class is provided if SuccessResult does not need a return value

## Configuration
Several configuration values are expected to launch this application correctly.
- For ease of development (to avoid environment conflicts), appSettings.Development.json and appSettings.Uat.json are specified. 
	- Note that the Build Properties on this file in Visual Studio must be set to (Copy To Output Directory : Always)
Ideally for deployed environments the corresponding environment variables would be set and the json files will not be deployed (they are ignored by .gitignore).
Secrets would be placed into KeyVault and injected into Azure

The following values are required (they can be specified as JSON too):
- WebJetConfiguration__FilmworldApiBaseUrl (the base url including the /api/<>)
- WebJetConfiguration__CinemaworldApiBaseUrl (the base url including the /api/<>)
- WebJetConfiguration__ApiKey (the API Key in the headers as required by the third party APIs)

## Further improvements
- Rather than using an In Memory cache, create separate "caches" for scalability and usability
  - Notice that the extenral APIs are not paginated - normally they would be given the number of movies that are present
  - Fetching all of these at once would be a significant issue and you would want to "crawl" them a page at a time
- A Database cache (a bit more permanent)
	- An Azure Function can regularly call the /movies endpoint for both and populate them here
	- Full price history etc. for analysis
- Azure Cognitive Search to help assist with Faceted Navigation
	- So for example, you can filter by Genre and search by Title
	- This is primarily a "read" store, and is to be linked to the SQL database
- Cache control endpoint
	- A protected admin only endopint that would clear the cache for a particular source
	- Can also trigger a re-cache