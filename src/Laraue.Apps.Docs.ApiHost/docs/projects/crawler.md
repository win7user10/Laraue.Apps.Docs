---
title: Crawler
type: project
tags: [Crawler,.NET,C#]
description: A highly customizable crawler built with C# that lets you extract data from any website, even those with complex JavaScript rendering.
---
## Key features
| Feature       | Value                                                                     |
|---------------|---------------------------------------------------------------------------|
| Language      | C#                                                                        |
| Framework     | NET9                                                                      |
| Project type  | Library                                                                   |
| Status        | Proof of Concept                                                          |
| License       | MIT                                                                       |
| Nuget         | ![latest version](https://img.shields.io/nuget/v/Laraue.Crawling.Common)  |
| Downloads     | ![latest version](https://img.shields.io/nuget/dt/Laraue.Crawling.Common) |
| Github        | [Laraue.Crawling](https://github.com/win7user10/Laraue.Crawling)          |

## What is the crawling
Crawling, also known as web scraping, is the automated process of systematically browsing the internet to extract data
from websites. It involves a program, called a crawler or spider, following links and downloading content from pages.
This extracted data can then be analyzed, stored, or used for various purposes like market research or price comparison.

## Initial implementation vision
There were two ideas how to implement the crawling program

#### Application implementation
It is the user-friendly way to allow non-programmatic users to work with software. Should have an interface where user
can make the sequence of blocks to make a crawling schema.

Pros
1. **User-friendly:** easy to start for user without programming knowledge
2. **Easier to promote:** when the interface is ready, it is straightforward to make GIFs and videos with it, make
tutorials, etc.

Cons
1. **Hard implementation:** to make an interface means not only to make a frontend (which is definitely hard), but to make
an additional architecture that will transform human-like view to programming code. Any edit on Backend can lead to 
edits on this layer and on Frontend. It seems like this part can be added in the future, when Backend will be stable.
2. **Limitations:** not all that can be described on the NP-full Programming language can be described with interface. 
I even think almost nothing (but it can be enough for most common cases). As soon as the first target was to make 
something that can grab data in any situation the limit sounds bad.

#### Library implementation
The programmer-friendly way to work with software. It Will be written in the specific language that only for users that
use that language.

Pros
1. **Flexible:** when the library follows software principals, it can allow user to make almost all he wants
2. **Less development time:** the product can be made on the clear C#
3. **Cheaper support:** no need to have Database or Domain to share development results
4. **Can be self-hosted:** MIT License allows using the library for any purposes

Cons
1. **Limited audience:** sharply reduces possible users to the C# programmers
2. **Harder to start:** it is required to download the repo and write a batch of code to get a working example

## The main problems tried to be solved
1. **Decrease the amount of routine work:** the typical crawler writing is not hard but takes a lot of time for the engineer.
2. **Simplify support:** sometimes requested resources change their structure, and it leads to code
rewriting. Fast-developed crawlers often have a bad architecture, and it's easier to rewrite them fully than make the changes.
3. **Better testability:** strongly typed library should show type errors as soon as possible, and the models defined
properties can be tested as usual C# classes.

## How to use the library
1. Users should choose the type of crawling schema builder. The chosen class defines what actions will be available for node.
Inbuilt builders are made for 
[Static html](https://github.com/win7user10/Laraue.Crawling/blob/master/src/Laraue.Crawling.Static.AngleSharp/AngleSharpSchemaBuilder.cs), 
[Dynamic html](https://github.com/win7user10/Laraue.Crawling/blob/master/src/Laraue.Crawling.Dynamic.PuppeterSharp/PuppeterSharpSchemaBuilder.cs) 
or even [Static xml](https://github.com/win7user10/Laraue.Crawling/blob/master/src/Laraue.Crawling.Static.Xml/XmlSchemaBuilder.cs). 
Actually, an implementation almost for any tree structure can be added; the user needs to implement the parser class, like
[that](https://github.com/win7user10/Laraue.Crawling/blob/master/src/Laraue.Crawling.Static.Xml/XmlParser.cs) for the required node type and a batch of related classes to define node methods. 
2. Then the user builds schema as on these examples for [static html](https://github.com/win7user10/Laraue.Crawling/blob/master/tests/Laraue.Crawling.Static.Tests/AngleSharpParserTests.cs#L16) 
and [dynamic html](https://github.com/win7user10/Laraue.Crawling/blob/master/tests/Laraue.Crawling.Dynamic.Tests/PuppeterSharpParserTests.cs)
3. The schema can be run via parser class for the specified schema: [static html](https://github.com/win7user10/Laraue.Crawling/blob/master/tests/Laraue.Crawling.Dynamic.Tests/PuppeterSharpParserTests.cs) or 
[dynamic html](https://github.com/win7user10/Laraue.Crawling/blob/master/tests/Laraue.Crawling.Dynamic.Tests/PuppeterSharpParserTests.cs#L100). 
Or the mini-example

```csharp
public record OnePage(string Title) : ICrawlingModel;

var schema = new AngleSharpSchemaBuilder<OnePage>()
    .HasProperty(x => x.Title, ".title")
    .Build();

var parser = new AngleSharpParser(new NullLoggerFactory());
var model = await parser.RunAsync(schema, "<html><p class='title'>Hi</html>");

Assert.Equal("Hi", model.Title);
```

## Challenges
The main solved problems will be described in the separated articles 
- How the library can be strongly-typed for the client despite the inner layer will use untyped delegates and work with
`object`
- How to make common API for sync and async cases not increasing the code amount
- How to make API flexible enough to allow the customer to do even the developer did not cover

## Timeline
- **Jun 2022** Base version with dynamic and static HTML parsing support
- **Oct 2022** Refactoring that separates common crawling logic from the specific mode details
- **Apr 2023** Made the base class for crawler job - ASP NET Host that runs crawling with the scheduling
- **May 2024** Refactoring that allowed to add new tree-structures crawlers in one-two hours 

## Real use cases
The library is widely used in the [SPB Real Estate](real-estate) project. Crawlers of the main advertisements sites,
[Avito](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Crawling.Impl/Avito/AvitoCrawlingSchema.cs), and
[Cian](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Crawling.Impl/Cian/CianCrawlingSchema.cs)
made with the library and launches as [jobs](https://github.com/win7user10/Laraue.Apps.RealEstate/blob/main/src/Laraue.Apps.RealEstate.Crawling.Impl/BaseRealEstateCrawlerJob.cs).