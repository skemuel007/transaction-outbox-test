using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Outbox.Notification.Data;
using Outbox.Notification.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//InMemory Db Context
builder.Services.AddDbContext<NotificationContext>(opt => opt.UseInMemoryDatabase("Notifications"), ServiceLifetime.Singleton);
//
builder.Services.AddHostedService<EventsListener>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
    {
        return Results.Ok(new
        {
            Status = true,
            Message = "Default route reached"
        });
    })
    .WithName("Default")
    .WithOpenApi();

app.MapGet("/notification", async (NotificationContext db) =>
        await db.Notifications.ToListAsync()
    ).WithName("GetNotifications");

app.Run();