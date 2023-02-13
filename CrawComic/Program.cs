using MongoDB.Bson;
using MongoDB.Driver;

var connectionString = "mongodb+srv://datacraw:superta01@cluster0.bodptox.mongodb.net/?retryWrites=true&w=majority";
var client = new MongoClient(connectionString);
var database = client.GetDatabase("test1");
var collection = database.GetCollection<BsonDocument>("people1");
var document = new BsonDocument
{
    {"name", "John Doe"},
    {"age", 30},
    {"email", "john.doe@example.com"}
};


await collection.InsertOneAsync(document);

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

app.Run();