using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Outbox.Common;
using Outbox.Order.Data;
using Outbox.Order.Dtos;

namespace Outbox.Order.Routing;

public static partial class OrderRoute
{
    public static void MapOrderControllers(WebApplication app, IConfiguration configuration)
    {
        app.MapGet("/order", async (OrderContext db) => await db.Orders.Include(o => o.OrderItems).ToListAsync())
            .WithName("GetOrders");


        app.MapGet("/order/{id}", async (Guid id, OrderContext db) =>
            await db.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id)
                is Models.Order order
                ? Results.Ok(order)
                : Results.NotFound()).WithName("GetOrder");

        app.MapPost("/order/{customerId}", async (int customerId, OrderDTO inputOrder, OrderContext db) => {
                var orderId = Guid.NewGuid();
                double orderAmount = 0;

                foreach (var orderItem in inputOrder.Items.ToList())
                {
                    orderAmount += (double)(orderItem.Units * orderItem.UnitPrice);
                }

                Models.Order order = new Models.Order
                {
                    Id = orderId,
                    CustomerId = customerId,
                    ResturantId = inputOrder.ResturantId,
                    OrderDate = DateTime.Now,
                    OrderAmount = orderAmount,
                    OrderItems = inputOrder.Items.ToList().Select(x => new Models.OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderId,
                        ItemId = x.ItemId,
                        UnitPrice = x.UnitPrice,
                        Units = x.Units
                    }).ToList()

                };

                var orderEvent = new OrderEvent
                {
                    OrderId = orderId,
                    CustomerId = customerId, 
                    RestaurantId = inputOrder.ResturantId,
                    OrderAmount = orderAmount, 
                    OrderItems = inputOrder.Items.Select(x => 
                        new OrderItem 
                        {   ItemId = x.ItemId, 
                            UnitPrice = x.UnitPrice, 
                            Units = x.Units 
                        }).ToList()
                };

                using var transaction = db.Database.BeginTransaction();

                db.Orders.Add(order);
                await db.SaveChangesAsync();

                db.OutboxEntity.Add(
                    new OutboxEventEntity 
                    { 
                        Event = "order.add",
                        Data = JsonSerializer.Serialize(orderEvent)
                    });
                await db.SaveChangesAsync();

                transaction.Commit();

                //return Results.Created($"/order/{order.Id}", order);
                return Results.Ok(order.Id);
            })
            .WithName("CreateOrder");

        app.MapDelete("/order/{id}", async (Guid id, OrderContext db) =>
            {
                var order = await db.Orders.FindAsync(id);
                if (order is null)
                {
                    return Results.NotFound();
                }

                db.Orders.Remove(order);
                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("CancelOrder");
    }
}