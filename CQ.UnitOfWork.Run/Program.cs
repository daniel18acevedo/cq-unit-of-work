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

IMongoRepository<User> userRepository = new MongoRepository<User>(playerFinderDatabase);
try
{
    SearchUsers();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
Console.ReadLine();

async Task GetById()
{
    var user = await userRepository.GetByPropAsync("QTvkGEhV6eW1WANyj0v0GknOd6l2").ConfigureAwait(false);

    Console.WriteLine(user.Email);
}

async Task SearchUsers()
{
    var users = await userRepository.GetAllAsync<MiniUser>().ConfigureAwait(false);

    Console.WriteLine($"{users.Count} users");
}


public class User
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }
}

public class MiniUser
{
    public string Id { get; set; }
}