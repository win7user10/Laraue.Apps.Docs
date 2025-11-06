---
title: Saint Petersburg Real Estate
type: project
projectType: application
githubLink: https://github.com/win7user10/Laraue.Apps.RealEstate
applicationLink: https://laraue.com/crawled-apartments
tags: [Crawler,AI,Ollama]
description: A real estate crawler and AI-powered analysis application built with Ollama and Crawler library.
createdAt: 2025-11-01
updatedAt: 2025-11-04
---
## Key features
|              |                                                                                |
|--------------|--------------------------------------------------------------------------------|
| Language     | C#                                                                             |
| Framework    | .NET 9                                                                         |
| Project type | Application (Real Estate Api + Worker Host + GpuWorkerHost)                    |
| Status       | Proof of concept                                                               |
| License      | MIT                                                                            |
| Github       | [Laraue.Apps.RealEstate](https://github.com/win7user10/Laraue.Apps.RealEstate) |
| Application  | [Crawled Apartments](https://laraue.com/crawled-apartments)                    |

## About The Real Estate Market
The market is always the battle of two sides: one wants to sell, the other wants to buy.
The deal usually happens when the price is enough for both sides.
But sometimes it is hard for an uninitiated person to determine whether the price is high, low, or represents a good deal.
The professionals in the market usually know different markers that allow them to estimate the price and compare it with
the real price. They see good and bad offers and understand where they can earn. The regular person often makes wrong 
decisions that lead to financial loss. 

## The User Search Problem
But how to help the regular user understand whether that deal is good or not? A bit ago, it was almost impossible
because the tasks of fuzzy estimates were poorly resolved by computers. But with AI availability, it is now possible
to delegate the task of making estimates to the models. Photos of the objects and their meta-information can tell
a lot about cost and whether the deal will be good or not.

## The Application Vision
The application should collect advertisements from real estate sites, analyze all images, and estimate renovation
on these photos, to make a verdict about whether the deal is good or not. Then it should return aggregated
advertisements that allow comparing them by advertisement level or ideality (the relationship between renovation,
price, and location).

## Application Services
There are some functions should be implemented:

### Advertisement Collector
The service that periodically launches and collects data from the advertisement aggregators. The logic is straightforward:
1. Launch the crawler every 4 hours. The crawler is built upon the [crawler](crawler) library, representing a
[job](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Crawling.Impl/Cian/CianCrawlerJob.cs)
that launches the crawling process with the defined crawling [schema](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Crawling.Impl/Cian/CianCrawlingSchema.cs) 
2. Try to store the result in a database. When some advertisements are not new, it means all items are crawled and
the job should wait for the next execution. This logic works because the crawler requests the newest advertisements.
The system can have as many different sites for crawlers as needed. Each is a separate job.

### Image processor
The service that makes predictions for the images of crawled apartments.
1. Launch the predictor every minute. Take the next batch of unpredicted images and send them sequentially to the 
[predictor](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Prediction.Impl/OllamaRealEstatePredictor.cs).
The class launches prediction on the local hosted Ollama [qwen2.5](https://ollama.com/library/qwen2.5) model passing
image bytes and use the [prompt](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Prediction.Impl/OllamaRealEstatePredictor.cs#L15)
that describes what signs are good on a photo and what is bad, then returns the prediction result.
2. For each predicted image, store the result.
3. When there are no images for prediction, wait for the next job execution.

Each prediction is represented by the following class:
```csharp
public record OllamaPredictionResult
{
    public double RenovationRating { get; init; } // The value from 0 to 1
    public string[] Tags { get; init; } = []; // What the model found on image
    public string Description { get; init; } = string.Empty; // Description of why the values were chosen
}
```
Only one property is used in the final result, but the others help understand how to tune the prompt/model to
increase recognition accuracy.

### The ranking system
The service gets results when they are ready and gives the final assessment for the advertisement. Since this moment,
the item has participated in the result returned from the API. It works with the following flow:
1. Get the advertisements where all images have been predicted.
2. Calculate the ideality. The [AdvertisementComputedFieldsCalculator](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Crawling.Impl/AdvertisementComputedFieldsCalculator.cs) 
sets the final ideality score using the system of fines. When there is no subway station nearby, or it is too far from the center,
it is bad and sets a fine. The bad renovation rating is also a fine. The more fines, the less is ideality. 
3. Calculate the renovation rating. The renovation is just an average rating from the photos. Flats with too few pictures
are not included in the rating due to higher possible errors.

### The system API
The service returns requested items with different filters and sorting. The usual API and nothing particularly interesting.

## Application Architecture
Unify all described services and take the following:
1. **[ApiHost](https://github.com/win7user10/Laraue.Apps.RealEstate/tree/main/src/Laraue.Apps.RealEstate.ApiHost):** the host serving frontend requests
2. **[WorkerHost](https://github.com/win7user10/Laraue.Apps.RealEstate/tree/main/src/Laraue.Apps.RealEstate.WorkerHost):** the host that launches crawlers and rating calculations
3. **[GpuWorkerHost](https://github.com/win7user10/Laraue.Apps.RealEstate/tree/main/src/Laraue.Apps.RealEstate.GpuWorkerHost):** the host that runs prediction job

## Challenges
The main solved problems will be described in separate articles:
- How to select the suitable model for the task
- How to make a ranking system formula for the project
- How to test this big system to ensure it works

## Timeline
- **Feb 2023** Attempts to collect dataset and train a model with TensorFlow.
- **Oct 2023** Launch of the first app version that uses three trained models with ~22M parameters to get predictions.
Works fast but has poor accuracy. It is tough to collect a large and correct dataset.
- **Sen 2025** Replacement of self-trained models with open-source ones

## Real Use Cases
Sometimes the application is used with me and my friends to observe prices on the real estate market. It does not work
absolutely correctly — for example, predictions often have errors. But because the average ranking system is used,
it works good enough to rank advertisements from the bad offers to the good.