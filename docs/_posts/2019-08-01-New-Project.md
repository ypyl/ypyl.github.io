---
layout: post
title: 'New project'
date: 2019-08-01
categories: documentation
---

Joining a new project is always stressful and not easy. As a new joiner, it is needed to learn a lot of information about the project: how it works, it's main functionality, how to build it, monitor, how to make changes and run tests, how to run the product locally and so on. Unfortunately, there isn't always good documentation which covers all these details, so you have to be prepared to learn and get all the details by yourself.

This article is to help a newcomer to understand the project and learn it. It describes the main steps and points, which can be used to getting knowledge about a project. Additionally, it can help to improve existing documentation of the project to fill gaps in the knowledge base.

# Introduction

## New team and who is responsible for what

The project is not able to evolve without people who are working on it. So it is quite important to know the whole team and who is responsible for what. Using this information it will be much easy to identify the right person who should be asked about questions or some particular point.

All teams are different, but there are usually the next roles:

- Research & Development team - a team which works on developing, writing code, adding new features to the product;
- Technical lead or architect - a person who is responsible for most important technical decisions in the project;z
- Project manager - a person who is responsible for the project, its timeline and teams which works on the product;
- QA team - quality assurance team or testers, who works on testing the product;
- Product owner or business representative - a person who knows business requirements of the product, how the product makes money and how to improve the product to make it more profitable;
- Release manager - a person who is responsible for release process of the product, usually only this person can change the production environment;
- DevOps member - a person who is responsible to automate the process of building, releasing of the product or any other repetitive task (e.g. CI/CD), additionally this person is responsible for non-prod environments (installing the product on it)
- SaaSOps member - a person who is supporting a product in the production, this person is responsible for SLA of a product, makes RCA (root cause analyses) of any issue in the production, sets up monitoring for a product and check logs from it.

Some of these roles can be covered by one person or a dedicated team.

# Product overview

It is important to see the goal of the product, why it exists and what types of problems it solves. Besides, it is great to understand what market/niche the product suite covers. It will allow understanding the project, some decisions what made earlier and future tasks better. Moreover, significant competitors should be pointed to see the difference between our product and theirs.

The type of the product should be described (web service, PLM solution, ERP solution and so on) which will uncover the nature of the product. It worths to read the history of the product if it exists.

Last 2-3 releases should be checked and the list of released features, fixes should be scanned for better understanding last changes in the product and what potentially will bring a new type of bugs.

List of topics:

- Type of the product
- Goal of the product
- Market of the product
- History of the product
- Main competitors
- Recent releases

## Key product objectives and capabilities

This chapter should try to answer the questions why the product exists and how it helps customers to solve their problems. Usually, it allows to understand the main interface of the product, which clients work with. Additionally, it shows which functions the product suite performs. It worths to check the demo of the product which is usually showed to potential customers.

Moreover, it will be great to have a list of a specific customer or consumer, business needs which are being satisfied. A list of differences from other similar solutions and what the product is doing which distinguish it from them should be prepared.

Also, the list of direct competitors or other players in the same market, a niche of the market should be discovered and their products should be investigated more closely.

## Key customers

Knowing the main customers and how they are using the product allows to understand what are the main workflows and features of the product.

So the next question should be answered:

- Which is the top 5-10 customer?
- Why are they using the product and not using competitor's product A, B, C?
- How are they using it? What is their main workflow?
- What are the most important features that customers use?
- How is all information about customer's usage collected?

## Architecture

This part is quite complex and comprehensive. But it will be great to have any type of diagrams or descriptions of how the whole application is working, how it is connected with other external systems or internally. Additionally, the list of application/system components can be covered in this section and how they depend on each other. All technical details about the application/system should be described here.

There are a couple of ways to understand application architecture. Using any of it or combination will be valuable.

Consider checking [C4 Model](https://c4model.com/) to see examples of diagrams.

There is a also a great article [Documenting software architecture](https://herbertograca.com/2019/08/12/documenting-software-architecture/) about this topic.

### Applications overview (Level 1: System Context)

It is important to understand how a customer or user sees the application or product. So the description of all different solutions and/or applications which are available in the product can be covered in this chapter. All descriptions should be done from a customer's perspective without any technical details.

### Product implementation (Level 2: ContainerS)

It is important to have an information in general about how the product/application depends on the infrastructure. This section should include all special requirements to infrastructure so the application will work on it well.

Describe how the product is instantiated across the servers, networks and other IT resources that allow the product to function.

Describe the cloud services that are used for the product and how they are used.

### Product components (Level 3: Component)

When you are joining a project, you are starting to take tasks that are related to a specific component(s) of the system. So this paragraph covers a description of components, one by one, as it is a completely independent product. It is almost impossible to finish this part as it will grow as you are learning a new project and application/system.

This chapter should include the summary of the languages and technologies used for each of the components in the application/product suite. Also there can be a description of frameworks and libraries which are used in the each component.

List all open-source, non open-source or 3rd-party services, components which the product embeds, uses or that are instantiated. Also capture any insights about why these particular frameworks, components and services were used.

Describe how all the components, 3rd-party and cloud services that were previously mentioned are deployed on the production infrastructure, how they connect and how they interact to deliver the product’s capabilities.

So each of the components that make up the product should be described here.

### Data model and storage

This chapter should include a description of any data which is used inside the application/system. What types of data is used and how it is used by a different part of the application. Also, this paragraph should cover what kinds of data exist, and where they are stored.

### Security

The processes of authentication and authorization should be described and the whole data flow related to this process. Consider collect and understand where different types of credentials are saved and how they are delivered to the client/customer and spread along different part of the product. What is a retention policy of them and how to rotate such type of information?

### Playground Infrastructure

It is almost always mandatory for a developer to have a playground environment to play/test/run changes that he is doing in the product. This chapter should cover the minimal valuable infrastructure that can be used for such purposes.
Ideally, it should be one script that allows to install all required components to run the product (and, in addition, it will be great to have a workflow which builds the source code, package it and run it in the playground environment).

## Use Cases

This chapter should cover all the customer and operations scenarios that define the product and its usage, especially those that have or could have an impact on operations. It is basically a list of all the test scenarios to validate that the product is functioning as it should and all the places where synthetic monitoring is required. It is not needed to describe what is going on at code level, the goal is to understand the process what is behind of this use case and provide a start point for root case analyses. The set of scenarios should cover all customer end-points.

Each use case description should cover the following topics:

1. Business Requirement

This paragraph contains description of the use case purpose. It covers and describes the behinds why it is important to have the particular functionality or feature from business perspective. Consider describing the value added to end user or customer.

2. Customer View

This part of the document describes the workflow of the use case from user perspective, it should cover the following topics:

- how to initiate the use case using the product
- how to observe and check results of the use case via application interface.

Ideally it should contain a list of sequential commands that can be executed by end user and mapped to a function test.

3. Data Flow

This paragraph contains information about data flows besides the use case, which components/services are used. Any type of diagrams, like a call sequence diagram, can be added here and will be useful.

4. Common Incidents

This paragraph contains the list of the most frequent types of incidents related to this customer scenario. It can just contain a set of notes or records that might be helpful during troubleshooting of the issue related to the used case. Any of the following entities can be added here:

- concerns
- recommendations
- playbooks
- runbooks

## Limitations & Problems

The best way to know the product it is to know its problems. So knowing high-level weaknesses of the product's architecture and implementation helps a lot with it. All components that incur operation cost should be covered by this chapter. Additionally, all recurring problems should be mentioned.

# Improvement plans

## Current ideal end-state

The ideal end-state of the current view of the product should be described here. It will be required to talk with different members of the team or even different departments if there is no any documented information about this topic. Consider collection as much information as possible. There are many sides of the product and its ideal end-state:

- number of members in different teams
- number of customers
- functionality in the ideal end-state product
- non-functional points (e.g CI/CD, logs, alarms, HA, performance)

The following topics should be covered in more details if possible:

1. Complexity reduction

The list of all the things that should be done to reduce operational complexity, e.g.

- improved automation
- automated deployment
- component consolidation
- component elimination

2. Cost reduction

The list of all the things that should be done to the product to reduce operational costs, e.g.

- improved automation
- component consolidation
- component elimination
- replacing services

3. Performance improvement

The list of all the things that should be done to the product to improve scalability, resilience, uptime.

# Operational overview

General overview of activities that are related to non-functional requirements.

1. Deployment strategy - an overview of how the product is installed or deployed into production and how it is provisioned and configured;
2. Testing strategy - an overview of how to check CI/CD, smoke and functional testing results, how to validate that a deployment was successful;
3. Monitoring strategy - an overview of how to monitor the existing infrastructure, e.g. check logs, APM;
4. SLA, uptime SLO and any maintenance window considerations;
5. Disaster recovery and business continuity - an overview of how approach to DR, backup, restore and data retention;
6. High availability and scalability - an overview of redundancy, load balancing, database replication, etc;
7. Automated and manual OAM activities (Operations, Administration and Management) - the list of all repetitive jobs that are performed automatically or need to be performed manually, e.g disk or database clean up etc;
8. Troubleshooting tools and approaches
9. Incidents analysis - an overview of the last issues and incidents with the product and RCS of them ([5 whys](https://en.wikipedia.org/wiki/Five_whys))

# References

- [Documenting Software Architecture](https://herbertograca.com/2019/08/12/documenting-software-architecture/)
- [The Twelve-Factor App](https://12factor.net/)
- [Five whys](https://en.wikipedia.org/wiki/Five_whys)
