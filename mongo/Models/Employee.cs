using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mongo.Models
{

    [BsonIgnoreExtraElements]
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = String.Empty;
        [BsonElement("Name")]
        public string Name { get; set; } = String.Empty;
        [BsonElement("Address")]
        public string Address { get; set; } = String.Empty;
        [BsonElement("Phone")]
        public string Phone { get; set; } = String.Empty;
        [BsonElement("Email")]
        public string Email { get; set; } = String.Empty;
    }
}
