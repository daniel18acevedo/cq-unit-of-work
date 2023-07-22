using CQ.UnitOfWork.Core;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

var mongoClient = new MongoClient(new MongoClientSettings
{
    Server = new MongoServerAddress("localhost", 27017),
    ReadPreference = ReadPreference.Primary,
    ApplicationName = "MongoDB%20Compass",
    DirectConnection = true,
    ClusterConfigurator = cb =>
    {
        cb.Subscribe<CommandStartedEvent>(e =>
        {
            Console.WriteLine($"{e.CommandName} - {e.Command.ToJson(new JsonWriterSettings { Indent = true })}");
            Console.WriteLine(new String('-', 32));
        });
    },
});

var playerFinderDatabase = mongoClient.GetDatabase("PlayerFinder");

var userRepository = new MongoRepository<User>(playerFinderDatabase);

async Task SearchUsers()
{
    try
    {
        var users = await userRepository.GetAllAsync(user => user.Name.ToLower().Contains("da")).ConfigureAwait(false);
    }catch(Exception ex) { 
        Console.WriteLine(ex.ToString());
    }
}

SearchUsers();

Console.ReadLine();

public class User
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
}