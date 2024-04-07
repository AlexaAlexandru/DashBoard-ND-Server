using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

string? connectionString = configuration.GetConnectionString("MongoDb");

var settings = MongoClientSettings.FromConnectionString(connectionString);
settings.ServerApi = new ServerApi(ServerApiVersion.V1);
var mongoClient = new MongoClient(settings);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    var result = mongoClient.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
    app.Logger.LogInformation("Pinged the MongoDB deployment. Successfully connected to MongoDB!");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Failed to connect to MongoDB.");
}

app.Run();