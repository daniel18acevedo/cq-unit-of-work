using MongoDB.Bson.Serialization.Attributes;

namespace CQ.UnitOfWork.Api.MongoDriver.DataAccess
{
    [BsonIgnoreExtraElements]
    public class UserMongo
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public BookMongo Book { get; set; }

        public UserMongo()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }

    [BsonIgnoreExtraElements]
    public class BookMongo
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public BookMongo()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}