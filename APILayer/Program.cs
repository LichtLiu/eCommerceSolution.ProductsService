using APILayer.APIEndpoints;
using APILayer.Middlewares;
using BusinessLogicLayer;
using DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

// ← 新增:Swagger(Minimal API 要先加 EndpointsApiExplorer 才找得到 endpoint)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ← 新增:CORS,只允許 Angular 前端
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandlingMiddleware();

// ← 新增:產生 /swagger/v1/swagger.json 與 /swagger 網頁
app.UseSwagger();
app.UseSwaggerUI();

// ← 新增:要在 endpoint 之前
app.UseCors();

app.MapProductAPIEndpoints();

app.Run();
