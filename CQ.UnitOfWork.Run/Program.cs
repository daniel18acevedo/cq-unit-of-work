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

var playerFinderDatabase = mongoClient.GetDatabase("UnitOfWork");

var userCollection = playerFinderDatabase.GetCollection<User>("Users");

try
{
    UpdateUserEmailInContactsLists("64d2a7542c35c436db1adb27", "daniel@nuevo4.com");
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}


Console.ReadLine();

void AddSeveralUsers()
{
    userCollection.InsertMany(new[]
    {
        new User { Name = "Daniel", Email = "daniel1@gmail.com" },
        new User { Name = "Daniel", Email = "daniel2@gmail.com" },
        new User { Name = "Daniel", Email = "daniel3@gmail.com" },
        new User { Name = "Daniel", Email = "daniel4@gmail.com" },
        new User { Name = "Daniel", Email = "daniel5@gmail.com" }
    });
}

void AddContacts()
{
    var query = Builders<User>.Filter.Eq(user => user.Id, "64d2a7542c35c436db1adb24");
    var update = Builders<User>.Update.Set($"{nameof(User.Contacts)}", new List<MiniUser>
    {
        new MiniUser
        {
            Id = "64d2a7542c35c436db1adb25",
            Email = "daniel2@gmail.com"
        },
        
        new MiniUser
        {
            Id = "64d2a7542c35c436db1adb26",
            Email = "daniel3@gmail.com"
        },
        
        new MiniUser
        {
            Id = "64d2a7542c35c436db1adb27",
            Email = "daniel4@gmail.com"
        },
    });

    userCollection.UpdateOne(query, update);
}

void UpdateUserEmailInContactsLists(string ciUserId, string newEmail)
{
    var userContactQuery = Builders<User>.Filter.ElemMatch(user => user.Contacts, contact => contact.Id == ciUserId);
    var userContactUpdate = Builders<User>.Update.Set($"{nameof(User.Contacts)}.$.{nameof(MiniUser.Email)}", newEmail);

    userCollection.UpdateMany(userContactQuery, userContactUpdate);
}

public class User
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public List<MiniUser> Contacts { get; set; }
}

public class MiniUser
{
    public string Id { get; set; }

    public string Email { get; set; }
}