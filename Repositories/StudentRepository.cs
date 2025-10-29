using d22rest.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Globalization;

namespace d22rest.Repositories
{
    public interface IStudentRepository
    {
        public Task Add(Student std);
        public Task<Student?> Get(int studentId);
        Task<List<Student>> GetAll();
    }

    public class StudentRepository : IStudentRepository
    {
        //private List<Student> _students;
        private readonly IMongoCollection<Student> _students;

        //Test environment variable usage
        string envar = "";

        public StudentRepository(IOptions<D22RestDatabase> d22RestDatabase)
        {
            //Non mongodb test version
            //_students = new List<Student>();
            
            //TBD test code
            //Guid guid = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa7");
            //_students.Add(new Student { Id = guid, Name = "Linus", Major = "Programming" });

            var mongoClient = new MongoClient(d22RestDatabase.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(d22RestDatabase.Value.DatabaseName);

            _students = mongoDatabase.GetCollection<Student>(d22RestDatabase.Value.StudentsCollectionName);

            //create an index for performance reasons
            var indexKeysDefinition = Builders<Student>.IndexKeys.Descending("Version");
            _students.Indexes.CreateOneAsync(new CreateIndexModel<Student>(indexKeysDefinition));

            //in mongosh use:
            //db.COLLECTION_NAME.createIndex({KEY:1})

            //Test environment variable usage
            envar = d22RestDatabase.Value.TestENVVar;
        }

        public async Task Add(Student std)
        {
            //Using query builders with string property names
            //FilterDefinition<Student> filter = Builders<Student>.Filter.Eq("StudentID", std.StudentID);
            //var sort = Builders<Student>.Sort.Descending("Version");                        
            //var lastStd = await _students.Find<Student>(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();

            //Using query builders with lambda
            //FilterDefinition<Student> filter = Builders<Student>.Filter.Eq(ent => ent.StudentID, std.StudentID);
            //var sort = Builders<Student>.Sort.Descending(ent => ent.Version);
            //var lastStd = await _students.Find<Student>(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();

            //Using query without builders 
            var lastStd = await _students.Find<Student>(ent=> ent.StudentID == std.StudentID)
                                            .SortByDescending(ent => ent.Version)
                                            .Limit(1)
                                            .FirstOrDefaultAsync();

            if (lastStd != null)
            {
                std.Version = lastStd.Version + 1;
                
            }
            std.Id = new Guid();    //Generate a new uniqe ID, better yet, use a DTO so this field isnt part of the API

            await _students.InsertOneAsync(std);
        }



        public async Task<Student?> Get(int studentId)
        {
            //return _students.FirstOrDefault(s => s.Id == id);
            //return await _students.Find(x => x.Id == id).FirstOrDefaultAsync();

            return await _students.Find<Student>(ent => ent.StudentID == studentId)
                                            .SortByDescending(ent => ent.Version)
                                            .Limit(1)
                                            .FirstOrDefaultAsync();
        }





        public async Task<List<Student>> GetAll()
        {
            return await _students.Find(std => true).ToListAsync(); ;
        }


        #region Test environment variable usage 
        public async Task<Student?> Get2(Guid id)
        {
            //return _students.FirstOrDefault(s => s.Id == id);
            Student std =  await _students.Find(x => x.Id == id).FirstOrDefaultAsync();

            std.Major = envar;

            return std;
        }

       

        #endregion


    }
}
