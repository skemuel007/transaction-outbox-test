using Microsoft.EntityFrameworkCore;
using Outbox.Order.Data;
using Outbox.Order.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

builder.Services.AddDbContext<OrderContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")), ServiceLifetime.Singleton);

builder.Services.AddHostedService<OutboxPublisher>();
builder.Services.AddSingleton<IPublisher, Publisher>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

try
{
    var context = app.Services.GetService<OrderContext>();
    await context.Database.MigrateAsync();
}
catch(Exception ex)
{
    app.Logger.LogError(ex.ToString());
}

app.Run();