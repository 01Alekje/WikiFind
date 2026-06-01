# WikiFind

WikiFind is a simple crawler, indexer and search engine for Wikipedia. WikiFind can start from virtually any single Wikipedia-article and crawl and index it's way through thousands of related articles. 

## How to run WikiFind

WikiFind is made up of three parts; The WebCrawler, The WebAPI and the simple Frontend. 

### The WebCrawler

#### First Crawl
Before running the crawler for the first time, make sure to start from a fun and random wikipedia-page. The default is "Rick Worthy", and can be changed in Ronkle/RonkleCrawler/Main.cs

This line can be commented out after the first run, since the database will most likely already have a decent amount of links to go through, and thus won't need any more guidance. 

__Make sure to set maxPages to a suitable size, and batch to 1 for the very first iteration.__

After the first run, you can set batch to pretty much whatever you want. 

Running the WebCrawler: (same not only for first run obviously)
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
Just open the webpage with your preferred browser. 

Obviously, both the WebAPI and the Frontend are very simple, but it's enough to have a function search engine. 

## Requirements

_These are not strict requirements, but rather the only version of packages that I've used for this project_

 - .NET minimum 8.0.421
 - (Any?) Microsoft.Data.Sqlite
 - (Any?) HtmlAgilityPack