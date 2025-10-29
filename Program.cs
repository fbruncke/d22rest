using d22rest;
using d22rest.Model;
using d22rest.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<D22RestDatabase>(
    builder.Configuration.GetSection("D22RestDatabaseSettings"));

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

app.MapGet("/student/{studentId}", (int studentId, IStudentRepository sr) =>
{
    return sr.Get(studentId);
});

app.MapGet("/student", async (IStudentRepository sr) =>
{
    return await sr.GetAll();
});


app.Run();





