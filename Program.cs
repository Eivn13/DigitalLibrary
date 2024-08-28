using Digital_library.Data;
using Digital_library.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSqlite<BookContext>("Data Source=DigitalLibrary.db");

builder.Services.AddScoped<BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.CreateDbIfNotExists();

app.MapGet("/", () => @"Navigate to /swagger to open the Swagger test UI.");

app.Run();
