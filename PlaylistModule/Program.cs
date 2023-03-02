using Microsoft.EntityFrameworkCore;
using PlaylistModule;
using PlaylistModule.Services;

var builder = WebApplication.CreateBuilder(args);

string connStr = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationContext>(option => option.UseNpgsql(connStr));

builder.Services.AddGrpc();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<PlaylistApiService>();
app.MapGet("/", () => "The server is up and running");

app.Run();

// https://localhost:32768 - сервис доступен по адресу