using DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);

var app = builder.Build();

app.Run();
