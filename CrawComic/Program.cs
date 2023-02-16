using System.Text.Json.Serialization;
using CrawComic;
using FirebaseAdmin;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using ReadMangaTest.Data;
using ReadMangaTest.Models;

var connectionString = "mongodb+srv://datacraw:superta01@cluster0.bodptox.mongodb.net/?retryWrites=true&w=majority";
var client = new MongoClient(connectionString);
var database = client.GetDatabase("test1");
var collection = database.GetCollection<BsonDocument>("people1");
var documents = new List<BsonDocument>
{
};
// var comic = new Comic()
// {
//     Name = "comic1",
//     Description = "desComic1",
//     Wallpaper = "walComic1",
//     Url = "http://www.comic1.com",
// };
// var comic2 = new Comic()
// {
//     Name = "comic2",
//     Description = "desComic2",
//     Wallpaper = "walComic2",
//     Url = "http://www.comic2.com",
// };
//
// // var document = new BsonDocument()
// // {
// //     { "name", "John Doe" },
// //     { "age", 30 },
// //     { "email", "john.doe@example.com" }
// // };
// // var document1 = new BsonDocument()
// // {
// //     { "name", "John Does" },
// //     { "age", 30 },
// //     { "email", "john.doe@example.com" }
// // };
//
// documents.Add(comic.ToBsonDocument());
// documents.Add(comic2.ToBsonDocument());
//
// await collection.InsertManyAsync(documents);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddTransient<WebScrapping>();
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient("Nero").ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }

});

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
if (args.Length == 1 && args[0].ToLower() == "scrapecomic")
    ScrapeComic(app);

void ScrapeComic(IHost app)
{
    Console.WriteLine("Starting scraping...");
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    if (scopedFactory != null)
    {
        using (var scope = scopedFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<WebScrapping>();
            if (service != null)
            {
                Console.WriteLine("Scraping...");
                service.ScrapeComicAndArtist();
            }
        }
    }
}
// if (args.Length == 1 && args[0].ToLower() == "downloadcomic")
//     DownloadImage(app);
//
// void DownloadImage(IHost app)
// {
//     Console.WriteLine("Starting scraping...");
//     var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
//
//     if (scopedFactory != null)
//     {
//         using (var scope = scopedFactory.CreateScope())
//         {
//             var service = scope.ServiceProvider.GetService<WebScrapping>();
//             if (service != null)
//             {
//                 Console.WriteLine("Scraping...");
//                 service.DownloadImageToString(
//                     "https://img-3.kunmanga.com/manga_63dbea845c17d/chapter-29/001.webp");
//             }
//         }
//     }
// }

app.Run();