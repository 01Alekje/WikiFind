# WikiFind

WikiFind is a simple crawler, indexer and search engine for Wikipedia. WikiFind can start from virtually any single Wikipedia-article and crawl and index it's way through thousands of related articles. 

## How to run WikiFind

WikiFind is made up of three parts; The WebCrawler, The WebAPI and the simple Frontend. 

### The WebCrawler

Running the WebCrawler:
```
cd RonkleCrawler
dotnet run
```

Also notice (main.cs): 

```
// limit amount of pages to crawl per run
await Crawl(100);
```

### The WebApi
Running the WebAPI:
```
cd RonkleAPI
dotnet run
```

### The Frontend
```
cd RonkleWeb
firefox index.html
```

Obviously, both the WebAPI and the Frontend are very simple, but it's enough to have a function search engine. 

## Requirements

_These are not strict requirements, but rather the only version of packages that I've used for this project_

 - .NET minimum 8.0.421
 - Microsoft.Data.Sqlite
 - HtmlAgilityPack