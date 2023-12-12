using Microsoft.EntityFrameworkCore;
using Outbox.Order.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

builder.Services.AddDbContext<OrderContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")), ServiceLifetime.Singleton);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.Run();