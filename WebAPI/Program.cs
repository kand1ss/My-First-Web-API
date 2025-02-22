using Application;
using Infrastructure;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddAPIControllers();
builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<PerformanceMiddleware>();
app.UseHealthChecks();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();