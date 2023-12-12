using Microsoft.EntityFrameworkCore;
using Outbox.Order.Data;
using Outbox.Order.Routing;
using Outbox.Order.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))/*, ServiceLifetime.Singleton*/);

builder.Services.AddHostedService<OutboxPublisher>();
builder.Services.AddSingleton<IPublisher, Publisher>();

var app = builder.Build();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetService<OrderContext>();
        var logger = services.GetRequiredService<ILogger<OrderContext>>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }
}
catch(Exception ex)
{
    app.Logger.LogError(ex.ToString());
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddMinimalRoutes(app.Configuration);

app.Run();