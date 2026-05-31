var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

Find engine = new Find();

app.MapGet("/search", (string word) =>
{
    var res = engine.Search(word);

    if (res.Count() == 0)
        return Results.BadRequest("Keyword not indexed");

    return Results.Ok(engine.Search(word));
    //DBHandler db = new DBHandler();

    //int kwId = db.GetKeywordId(word);

    //if (kwId == -1)
    //    return Results.BadRequest("Keyword cannot be processed");

    //var results = db.GetArticleKeywordCount(kwId, 2);

    //return Results.Ok(results);
});

app.Run();