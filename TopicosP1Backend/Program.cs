using Microsoft.EntityFrameworkCore;
using CareerApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using TopicosP1Backend.Scripts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Context>(opt =>
    opt.UseInMemoryDatabase("main"));
//    opt.UseSqlite("Data Source=test.db"));
builder.Services.AddSingleton<APIQueue>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.CreateDbIfNotExists();

app.Run();
