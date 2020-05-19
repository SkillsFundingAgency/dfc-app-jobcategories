# Digital First Careers – Job Categories App

## Introduction

This project provides a Job Categories App for use in the Composite UI (Shell application) to output markup for the "Explore careers" job categories list, and the job profiles by job category list pages.

Details of the Composite UI application may be found here https://github.com/SkillsFundingAgency/dfc-composite-shell

Job category, and related job profile data is retrieved from an API over the top of a Neo4j graph database cluster. The Neo4j database itself is populated by a Orchard Core CMS web application.

Details of the API application may be found here https://github.com/SkillsFundingAgency/dfc-api-content
Details of the Orchard Core CMS application may be found here https://github.com/SkillsFundingAgency/dfc-servicetaxonomy-editor

The app also provisions the following for consumption by the Composite UI:

* Sitemap.xml for all Help documents
* Robots.txt

## Getting Started

This is a self-contained Visual Studio 2019 solution containing a number of projects (web application, service and repository layers, with associated unit test and integration test projects).

### Installing

Clone the project and open the solution in Visual Studio 2019.

## List of dependencies

|Item	|Purpose|
|-------|-------|
|Azure Cosmos DB | Document storage |
|Comp UI Shell App | Hosting the content rendered from this app |
|API Content App | Serving the data that this app fetches and caches in Cosmos DB on startup |
|Neo4j Graph Database | Serving the data that the API Content App serves as above |

## Local Config Files

Once you have cloned the public repo you need to remove the -template part from the configuration file names listed below.

| Location | Repo Filename | Rename to |
|-------|-------|-------|
| DFC.App.JobCategories.IntegrationTests | appsettings-template.json | appsettings.json |
| DFC.App.JobCategories.MessageFunctionApp | local.settings-template.json | local.settings.json |
| DFC.App.JobCategories | appsettings-template.json | appsettings.json |

## Configuring to run locally

The project contains a number of "appsettings-template.json" files which contain sample appsettings for the web app and the integration test projects. To use these files, rename them to "appsettings.json" and edit and replace the configuration item values with values suitable for your environment.

By default, the appsettings include a local Azure Cosmos Emulator configuration using the well known configuration values. These may be changed to suit your environment if you are not using the Azure Cosmos Emulator.

On startup, by default the app will query the API content app for Job Category and Profile information to cache in Cosmos DB. You can disable this after the first startup by changing ```JobCategories:LoadDataOnStartup``` to ```"false"``` in DFC.App.JobCategories.appsettings.json

## Running locally

To run this product locally, you will need to configure the list of dependencies, once configured and the configuration files updated, it should be F5 to run and debug locally. The application can be run using IIS Express or full IIS.

To run the project, start the web application. Once running, browse to the main entrypoint which is the "https://localhost:44329/explore-careers". This will list all of the Job Categories available and from here, you can navigate to the individual job profile listing pages.

The app is designed to be run from within the Composite UI, therefore running it outside of the Composite UI shell will only show simple views of the data.

## Deployments

This app will be deployed as an individual deployment for consumption by the Composite UI.

## Assets

CSS, JS, images and fonts used in this site can found in the following repository https://github.com/SkillsFundingAgency/dfc-digital-assets

## Built With

* Microsoft Visual Studio 2019
* .Net Core 3.1

## References

Please refer to https://github.com/SkillsFundingAgency/dfc-digital for additional instructions on configuring individual components like Cosmos.
