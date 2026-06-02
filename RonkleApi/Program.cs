var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

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
});

app.Run();