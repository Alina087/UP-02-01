using CosmeticApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        // GET: api/Cart/GetCart/5
        [HttpGet("GetCart/{userId}")]
        public ActionResult GetCart(int userId)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var cartItems = context.Carts
                        .Include(c => c.TovarArticleNavigation)
                            .ThenInclude(t => t.Supplier)
                        .Include(c => c.TovarArticleNavigation)
                            .ThenInclude(t => t.Manufacturer)
                        .Include(c => c.TovarArticleNavigation)
                            .ThenInclude(t => t.TovarCategory)
                        .Where(c => c.UserId == userId)
                        .Select(c => new
                        {
                            c.UserId,
                            c.TovarArticle,
                            c.CartTovarCount,
                            // Явно указываем все нужные поля товара
                            tovarName = c.TovarArticleNavigation.TovarName,
                            tovarUnit = c.TovarArticleNavigation.TovarUnit,
                            tovarCost = c.TovarArticleNavigation.TovarCost,
                            tovarDiscount = c.TovarArticleNavigation.TovarDiscount,
                            tovarCount = c.TovarArticleNavigation.TovarCount,
                            tovarImage = c.TovarArticleNavigation.TovarImage,
                            tovarDescription = c.TovarArticleNavigation.TovarDescription,
                            manufacturerName = c.TovarArticleNavigation.Manufacturer != null ?
                                c.TovarArticleNavigation.Manufacturer.ManufacturerName : null,
                            supplierName = c.TovarArticleNavigation.Supplier != null ?
                                c.TovarArticleNavigation.Supplier.SupplierName : null,
                            tovarCategoryName = c.TovarArticleNavigation.TovarCategory != null ?
                                c.TovarArticleNavigation.TovarCategory.TovarCategoryName : null
                        })
                        .ToList();

                    return Ok(cartItems);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении корзины: {ex.Message}");
            }
        }

        // POST: api/Cart/AddToCart
        [HttpPost("AddToCart")]
        public ActionResult AddToCart([FromBody] CartDto cartDto)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    // Проверяем существование пользователя
                    var user = context.Users.Find(cartDto.UserId);
                    if (user == null)
                        return BadRequest("Пользователь не найден");

                    // Проверяем существование товара
                    var tovar = context.Tovars.Find(cartDto.TovarArticle);
                    if (tovar == null)
                        return BadRequest("Товар не найден");

                    // Проверяем наличие товара
                    if (tovar.TovarCount <= 0)
                        return BadRequest("Товара нет в наличии");

                    // Ищем существующий элемент в корзине
                    var existingItem = context.Carts
                        .FirstOrDefault(c => c.UserId == cartDto.UserId && c.TovarArticle == cartDto.TovarArticle);

                    if (existingItem != null)
                    {
                        // Обновляем количество
                        int newCount = (existingItem.CartTovarCount ?? 0) + (cartDto.CartTovarCount ?? 1);

                        if (newCount > tovar.TovarCount)
                            return BadRequest("Достигнуто максимальное количество товара на складе");

                        existingItem.CartTovarCount = newCount;
                        context.SaveChanges();

                        return Ok("Количество товара в корзине обновлено");
                    }
                    else
                    {
                        // Добавляем новый товар в корзину
                        var cartItem = new Cart
                        {
                            UserId = cartDto.UserId,
                            TovarArticle = cartDto.TovarArticle,
                            CartTovarCount = cartDto.CartTovarCount ?? 1
                        };

                        if (cartItem.CartTovarCount > tovar.TovarCount)
                            return BadRequest("Недостаточно товара на складе");

                        context.Carts.Add(cartItem);
                        context.SaveChanges();

                        return Ok("Товар добавлен в корзину");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при добавлении в корзину: {ex.Message}");
            }
        }

        // PUT: api/Cart/UpdateCartItem?userId=1&tovarArticle=ART001&quantity=2
        [HttpPut("UpdateCartItem")]
        public ActionResult UpdateCartItem(int userId, string tovarArticle, int quantity)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var cartItem = context.Carts
                        .Include(c => c.TovarArticleNavigation)
                        .FirstOrDefault(c => c.UserId == userId && c.TovarArticle == tovarArticle);

                    if (cartItem == null)
                        return NotFound("Товар не найден в корзине");

                    if (quantity <= 0)
                    {
                        context.Carts.Remove(cartItem);
                        context.SaveChanges();
                        return Ok("Товар удален из корзины");
                    }

                    if (quantity > cartItem.TovarArticleNavigation.TovarCount)
                        return BadRequest("Недостаточно товара на складе");

                    cartItem.CartTovarCount = quantity;
                    context.SaveChanges();

                    return Ok("Количество обновлено");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обновлении количества: {ex.Message}");
            }
        }

        // DELETE: api/Cart/RemoveFromCart?userId=1&tovarArticle=ART001
        [HttpDelete("RemoveFromCart")]
        public ActionResult RemoveFromCart(int userId, string tovarArticle)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var cartItem = context.Carts
                        .FirstOrDefault(c => c.UserId == userId && c.TovarArticle == tovarArticle);

                    if (cartItem == null)
                        return NotFound("Товар не найден в корзине");

                    context.Carts.Remove(cartItem);
                    context.SaveChanges();

                    return Ok("Товар удален из корзины");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при удалении из корзины: {ex.Message}");
            }
        }

        // DELETE: api/Cart/ClearCart/5
        [HttpDelete("ClearCart/{userId}")]
        public ActionResult ClearCart(int userId)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var cartItems = context.Carts.Where(c => c.UserId == userId).ToList();

                    if (!cartItems.Any())
                        return Ok("Корзина уже пуста");

                    context.Carts.RemoveRange(cartItems);
                    context.SaveChanges();

                    return Ok("Корзина очищена");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при очистке корзины: {ex.Message}");
            }
        }
    }

    // DTO класс для добавления в корзину
    public class CartDto
    {
        public int UserId { get; set; }
        public string TovarArticle { get; set; } = null!;
        public int? CartTovarCount { get; set; }
    }
}