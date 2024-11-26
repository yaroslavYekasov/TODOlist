using Microsoft.EntityFrameworkCore;
using TODOlist.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MySQL Database Context
builder.Services.AddDbContext<AuthContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25))
    )
);

// **Add CORS Policy**
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000") // Frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// **Use the CORS policy**
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
