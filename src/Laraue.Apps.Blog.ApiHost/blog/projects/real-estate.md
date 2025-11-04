---
title: Saint Petersburg Real Estate
type: project
projectType: application
githubLink: https://github.com/win7user10/Laraue.Apps.RealEstate
applicationLink: https://laraue.com/crawled-apartments
tags: [Crawler,AI,Ollama]
description: The real project that uses Crawler library
---
## Key features
| Feature      | Value                                                                          |
|--------------|--------------------------------------------------------------------------------|
| Language     | C#                                                                             |
| Framework    | NET9                                                                           |
| Project type | Application (Real Estate Api + Worker Host + GpuWorkerHost)                    |
| Status       | Proof of concept                                                               |
| License      | MIT                                                                            |
| Github       | [Laraue.Apps.RealEstate](https://github.com/win7user10/Laraue.Apps.RealEstate) |
| Application  | [Crawled Apartments](https://laraue.com/crawled-apartments)                    |

## About The Real Estate Market
The market is always the battle of two sides: one wants to sell the second to sell. The deal usually happens when the price 
is enough for both sides. But sometimes it is hard to the not deep-dived man to determine is the price high, low or it is a good deal.
The professional of the market usually knows different markers that allow to estimate the price and compare it with the real price.
They see good and bad offers and understand where they can earn. The regular person often makes wrong decisions that lead to money loss. 

## The user search problem
But how to help the regular user understand is that deal good or not? A bit time ago, it was almost impossible because
the tasks of fuzzy estimates resolved bad by the computers. But with AI distribution, it is possible to delegate to
make estimates to the models. Photos of the objects, its meta-information can tell a lot about cost, and will the deal be good or no. 

## The Application Vision
The application should collect advertisements from real estate sites, see all images, and estimate renovation on these
photos, to make the verdict good or not. Then it should return aggregated advertisements allows to compare them by 
advertisement level or ideality (relation between renovation / price / place).

## Application Architecture
There are some services should be implemented:

### Advertisement Collector
The service that periodically launches and collects the data from the advertisement aggregators. The logic is straightforward
1. Launch the crawler each 4 hours. Crawler is built upon the [crawler](crawler) library, represents
a [job](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Crawling.Impl/Cian/CianCrawlerJob.cs)
that launches the crawling process with the defined crawling [schema](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Crawling.Impl/Cian/CianCrawlingSchema.cs) 
2. Try to store the result to a database. When some advertisements will be not new, it means all items are crawled and the job
should wait for the next execution. This logic works because crawler requests the newest advertisements.
The system can have as much as need different sites for crawlers. Each is one separated Job.

### Image processor
The service

## Challenges
The main problems encountered.

## Timeline
The key moments while developing.

The application collects data from russian real estates sites via [Crawler](crawler) library, estimates renovation with
Ollama, store flats ratings. Provides an API to get aggregated advertisements ranging from estimated renovation, value for money.