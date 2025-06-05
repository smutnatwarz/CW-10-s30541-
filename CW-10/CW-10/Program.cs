using CW_10.Data;
using CW_10.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddDbContext<_2019sbdContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IDbService, DbService>();

var app = builder.Build();



app.UseAuthorization();

app.MapControllers();

app.Run();