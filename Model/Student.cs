using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace d22rest.Model
{
    public class Student
    {
        [BsonId]        
        public Guid Id { get; set; }

        public int StudentID { get; set; }

        public string? Name { get; set; }
        public string? Major { get; set; }

        public int Version { get; set;}


    }
}
