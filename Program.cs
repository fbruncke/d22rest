var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IStudentRepository, StudentRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//Endpoints
app.MapPost("/student", (Student std, IStudentRepository sr) => 
{
    sr.Add(std);
});

app.MapGet("/student/{id}", (Guid id, IStudentRepository sr) =>
{
    return sr.Get(id);
});

app.Run();



public interface IStudentRepository
{
    public void Add(Student std);
    public Student? Get(Guid id);
}

public class StudentRepository : IStudentRepository
{
    private List<Student> _students;

    public StudentRepository()
    {
        _students = new List<Student>();
        //TBD test code
        Guid guid = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa7");
        _students.Add(new Student {Id=guid,Name="Linus", Major="Programming" });
    }

    public void Add(Student std)
    {
        _students.Add(std);
    }

    public Student? Get(Guid id)
    {
        return _students.FirstOrDefault(s => s.Id == id);
    }
}

public class Student
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Major { get; set; }
}