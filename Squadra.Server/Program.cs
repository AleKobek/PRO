using Microsoft.EntityFrameworkCore;
using Squadra;
using Squadra.Server.Context;
using Squadra.Server.Repositories;
using Squadra.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IKrajRepository, KrajRepository>();
builder.Services.AddScoped<IKrajService, KrajService>();

builder.Services.AddScoped<IProfilRepository, ProfilRepository>();
builder.Services.AddScoped<IProfilService, ProfilService>();

builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IStatusService, StatusService>();


builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IRegionService, RegionService>();

builder.Services.AddScoped<IJezykRepository, JezykRepository>();
builder.Services.AddScoped<IJezykService, JezykService>();

builder.Services.AddScoped<IStopienBieglosciJezykaRepository, StopienBieglosciJezykaRepository>();
builder.Services.AddScoped<IStopienBieglosciJezykaService, StopienBieglosciJezykaService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/openapi/v1.json", "v1");
//     });
//
// }

if (app.Environment.IsDevelopment())
{
    // jest pod adresem /swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();


app.Run();