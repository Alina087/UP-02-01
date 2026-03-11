using CosmeticApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        // Создание заказа из корзины
        [HttpPost("CreateOrder")]
        public ActionResult CreateOrder(int userId, int? pickUpPointId = null)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    // Получаем корзину пользователя
                    var cartItems = context.Carts
                        .Include(c => c.TovarArticleNavigation)
                        .Where(c => c.UserId == userId)
                        .ToList();

                    if (cartItems == null || cartItems.Count == 0)
                        return BadRequest("Корзина пуста");

                    // Проверяем наличие товаров
                    foreach (var item in cartItems)
                    {
                        var tovar = context.Tovars.Find(item.TovarArticle);
                        if (tovar == null)
                            return BadRequest($"Товар {item.TovarArticle} не найден");

                        if (item.CartTovarCount > tovar.TovarCount)
                            return BadRequest($"Недостаточно товара {tovar.TovarName} на складе. Доступно: {tovar.TovarCount}");
                    }

                    // Генерируем короткий код заказа
                    string orderCode = GenerateOrderCode();

                    // Проверяем уникальность кода
                    while (context.Orders.Any(o => o.OrderCode == orderCode))
                    {
                        orderCode = GenerateOrderCode();
                    }

                    // Создаем заказ
                    var order = new Order
                    {
                        UserId = userId,
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        OrderStatus = "Новый",
                        PickUpPointId = pickUpPointId,
                        OrderCode = orderCode
                    };

                    context.Orders.Add(order);
                    context.SaveChanges();

                    // Создаем структуру заказа и обновляем количество товаров
                    foreach (var item in cartItems)
                    {
                        if (string.IsNullOrEmpty(item.TovarArticle))
                            continue;

                        var structureOrder = new StructureOrder
                        {
                            OrderId = order.OrderId,
                            TovarArticle = item.TovarArticle,
                            StructureOrderTovarCount = item.CartTovarCount ?? 1
                        };
                        context.StructureOrders.Add(structureOrder);

                        var tovar = context.Tovars.Find(item.TovarArticle);
                        if (tovar != null)
                        {
                            tovar.TovarCount -= item.CartTovarCount ?? 1;
                        }
                    }

                    // Очищаем корзину
                    context.Carts.RemoveRange(cartItems);
                    context.SaveChanges();

                    return Ok(new
                    {
                        message = "Заказ успешно создан",
                        orderId = order.OrderId,
                        orderCode = order.OrderCode
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(500, $"Ошибка при создании заказа: {ex.Message}. Inner: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Ошибка при создании заказа: {ex.Message}");
            }
        }

        // Получение всех заказов (для админа и менеджера) с полной информацией о товарах
        [HttpGet("GetAllOrders")]
        public ActionResult GetAllOrders()
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var orders = context.Orders
                        .Include(o => o.User)
                        .Include(o => o.PickUpPoint)
                        .Include(o => o.StructureOrders)
                            .ThenInclude(s => s.TovarArticleNavigation)
                                .ThenInclude(t => t.Supplier)
                        .Include(o => o.StructureOrders)
                            .ThenInclude(s => s.TovarArticleNavigation)
                                .ThenInclude(t => t.Manufacturer)
                        .Include(o => o.StructureOrders)
                            .ThenInclude(s => s.TovarArticleNavigation)
                                .ThenInclude(t => t.TovarCategory)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();

                    // Создаем DTO объекты для безопасной сериализации
                    var result = orders.Select(o => new
                    {
                        o.OrderId,
                        o.OrderDate,
                        o.OrderDateDelivery,
                        o.PickUpPointId,
                        o.UserId,
                        o.OrderCode,
                        o.OrderStatus,
                        // Информация о пользователе
                        User = o.User != null ? new
                        {
                            o.User.UserId,
                            o.User.UserSurname,
                            o.User.UserName,
                            o.User.UserLastname,
                            o.User.UserLogin,
                            o.User.RoleId
                        } : null,
                        // Информация о пункте выдачи
                        PickUpPoint = o.PickUpPoint != null ? new
                        {
                            o.PickUpPoint.PickUpPointId,
                            o.PickUpPoint.PickUpPointAdress
                        } : null,
                        // Товары в заказе с полной информацией
                        StructureOrders = o.StructureOrders.Select(s => new
                        {
                            s.StructureOrderId,
                            s.OrderId,
                            s.TovarArticle,
                            s.StructureOrderTovarCount,
                            // Полная информация о товаре
                            Tovar = s.TovarArticleNavigation != null ? new
                            {
                                s.TovarArticleNavigation.TovarArticle,
                                s.TovarArticleNavigation.TovarName,
                                s.TovarArticleNavigation.TovarDescription,
                                s.TovarArticleNavigation.TovarCost,
                                s.TovarArticleNavigation.TovarDiscount,
                                s.TovarArticleNavigation.TovarCount,
                                s.TovarArticleNavigation.TovarUnit,
                                s.TovarArticleNavigation.TovarImage,
                                // Информация о связанных сущностях
                                SupplierName = s.TovarArticleNavigation.Supplier != null ?
                                    s.TovarArticleNavigation.Supplier.SupplierName : null,
                                ManufacturerName = s.TovarArticleNavigation.Manufacturer != null ?
                                    s.TovarArticleNavigation.Manufacturer.ManufacturerName : null,
                                CategoryName = s.TovarArticleNavigation.TovarCategory != null ?
                                    s.TovarArticleNavigation.TovarCategory.TovarCategoryName : null
                            } : null
                        })
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении заказов: {ex.Message}");
            }
        }

        // Получение заказов конкретного пользователя с полной информацией о товарах
        [HttpGet("GetUserOrders/{userId}")]
        public ActionResult GetUserOrders(int userId)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var orders = context.Orders
                        .Include(o => o.PickUpPoint)
                        .Include(o => o.StructureOrders)
                            .ThenInclude(s => s.TovarArticleNavigation)
                                .ThenInclude(t => t.Supplier)
                        .Include(o => o.StructureOrders)
                            .ThenInclude(s => s.TovarArticleNavigation)
                                .ThenInclude(t => t.Manufacturer)
                        .Include(o => o.StructureOrders)
                            .ThenInclude(s => s.TovarArticleNavigation)
                                .ThenInclude(t => t.TovarCategory)
                        .Where(o => o.UserId == userId)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();

                    // Создаем DTO объекты для безопасной сериализации
                    var result = orders.Select(o => new
                    {
                        o.OrderId,
                        o.OrderDate,
                        o.OrderDateDelivery,
                        o.PickUpPointId,
                        o.UserId,
                        o.OrderCode,
                        o.OrderStatus,
                        // Информация о пункте выдачи
                        PickUpPoint = o.PickUpPoint != null ? new
                        {
                            o.PickUpPoint.PickUpPointId,
                            o.PickUpPoint.PickUpPointAdress
                        } : null,
                        // Товары в заказе с полной информацией
                        StructureOrders = o.StructureOrders.Select(s => new
                        {
                            s.StructureOrderId,
                            s.OrderId,
                            s.TovarArticle,
                            s.StructureOrderTovarCount,
                            // Полная информация о товаре
                            Tovar = s.TovarArticleNavigation != null ? new
                            {
                                s.TovarArticleNavigation.TovarArticle,
                                s.TovarArticleNavigation.TovarName,
                                s.TovarArticleNavigation.TovarDescription,
                                s.TovarArticleNavigation.TovarCost,
                                s.TovarArticleNavigation.TovarDiscount,
                                s.TovarArticleNavigation.TovarCount,
                                s.TovarArticleNavigation.TovarUnit,
                                s.TovarArticleNavigation.TovarImage,
                                // Информация о связанных сущностях
                                SupplierName = s.TovarArticleNavigation.Supplier != null ?
                                    s.TovarArticleNavigation.Supplier.SupplierName : null,
                                ManufacturerName = s.TovarArticleNavigation.Manufacturer != null ?
                                    s.TovarArticleNavigation.Manufacturer.ManufacturerName : null,
                                CategoryName = s.TovarArticleNavigation.TovarCategory != null ?
                                    s.TovarArticleNavigation.TovarCategory.TovarCategoryName : null
                            } : null
                        })
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении заказов пользователя: {ex.Message}");
            }
        }

        [HttpPut("UpdateOrderStatus")]
        public ActionResult UpdateOrderStatus(int orderId, string status)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var order = context.Orders.Find(orderId);
                    if (order == null)
                        return NotFound("Заказ не найден");

                    if (status != "Новый" && status != "Завершен")
                        return BadRequest("Недопустимый статус заказа");

                    if (order.OrderStatus != "Новый")
                        return BadRequest("Можно изменить статус только нового заказа");

                    order.OrderStatus = status;

                    if (status == "Завершен")
                    {
                        order.OrderDateDelivery = DateOnly.FromDateTime(DateTime.Now);
                    }

                    context.SaveChanges();

                    return Ok("Статус заказа обновлен");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обновлении статуса: {ex.Message}");
            }
        }

        // Отмена заказа (только для нового заказа)
        [HttpDelete("CancelOrder/{orderId}")]
        public ActionResult CancelOrder(int orderId)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var order = context.Orders
                        .Include(o => o.StructureOrders)
                        .FirstOrDefault(o => o.OrderId == orderId);

                    if (order == null)
                        return NotFound("Заказ не найден");

                    if (order.OrderStatus != "Новый")
                        return BadRequest("Можно отменить только новый заказ");

                    // Возвращаем товары на склад
                    foreach (var item in order.StructureOrders)
                    {
                        var tovar = context.Tovars.Find(item.TovarArticle);
                        if (tovar != null && item.StructureOrderTovarCount.HasValue)
                        {
                            tovar.TovarCount += item.StructureOrderTovarCount.Value;
                        }
                    }

                    context.StructureOrders.RemoveRange(order.StructureOrders);
                    context.Orders.Remove(order);
                    context.SaveChanges();

                    return Ok("Заказ отменен");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при отмене заказа: {ex.Message}");
            }
        }

        // Вспомогательный метод для генерации кода заказа
        private string GenerateOrderCode()
        {
            var random = new Random();
            int randomNumber = random.Next(100, 999);
            return randomNumber.ToString();
        }
    }
}