using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add SQLite support
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// ≈÷«›… Œœ„… MessageBusSubscriber
builder.Services.AddSingleton<MessageBusSubscriber>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//  √ﬂœ „‰  ‘€Ì· Œœ„… RabbitMQ Subscriber ⁄‰œ »œ¡ «· ‘€Ì·
var subscriber = app.Services.GetRequiredService<MessageBusSubscriber>();
app.Run();
