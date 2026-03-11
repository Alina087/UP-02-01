using CosmeticApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CosmeticApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TovarController : ControllerBase
    {
        [HttpGet("GetTovar")]
        public ActionResult GetTovar()
        {
            using (var context = new DbCosmeticContext())
            {
                var tovars = context.Tovars.Include(a => a.Supplier).Include(b => b.Manufacturer).Include(a => a.TovarCategory).ToList();
                if (tovars != null)
                {
                    return Ok(tovars);
                }
                else
                {
                    return BadRequest("Товары не найдены");
                }
            }
        }

        [HttpGet("GetTovarId")]
        public ActionResult GetTovarId(string id)
        {
            using (var context = new DbCosmeticContext())
            {
                var tovars = context.Tovars.Include(a => a.Supplier).Include(b => b.Manufacturer).Include(a => a.TovarCategory).FirstOrDefault(a => a.TovarArticle == id);
                if (tovars != null)
                {
                    return Ok(tovars);
                }
                else
                {
                    return BadRequest("Товар не найден");
                }
            }
        }

        [HttpPost("Add")]
        public ActionResult Add([FromBody] Tovar tovar)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    if (tovar == null)
                        return BadRequest("Данные товара не предоставлены");

                    if (string.IsNullOrWhiteSpace(tovar.TovarArticle))
                        return BadRequest("Артикул товара является обязательным полем");

                    if (string.IsNullOrWhiteSpace(tovar.TovarName))
                        return BadRequest("Название товара является обязательным полем");

                    if (tovar.TovarCost <= 0)
                        return BadRequest("Стоимость товара должна быть больше 0");

                    if (tovar.TovarCount < 0)
                        return BadRequest("Количество товара не может быть меньше 0");

                    if (tovar.TovarDiscount < 0)
                        return BadRequest("Скидка товара не может быть меньше 0");

                    if (string.IsNullOrWhiteSpace(tovar.TovarUnit))
                        return BadRequest("Единица измерения не может быть пустой");

                    if (string.IsNullOrWhiteSpace(tovar.TovarDescription))
                        return BadRequest("Описание не может быть пустым");



                    var existingTovar = context.Tovars
                        .FirstOrDefault(t => t.TovarArticle == tovar.TovarArticle);

                    if (existingTovar != null)
                        return Conflict($"Товар с артикулом {tovar.TovarArticle} уже существует");
                    context.Tovars.Add(tovar);
                    context.SaveChanges();

                    return Ok("Товар добавлен успешно!");
                }
            }
            catch { return Ok(); }
               
        }

        [HttpPut("Update")]
        public ActionResult Update([FromBody] Tovar tovar)
        {
            using (var context = new DbCosmeticContext())
            {
                if (tovar == null)
                    return BadRequest("Данные товара не предоставлены");

                if (string.IsNullOrWhiteSpace(tovar.TovarArticle))
                    return BadRequest("Артикул товара является обязательным полем");

                if (string.IsNullOrWhiteSpace(tovar.TovarName))
                    return BadRequest("Название товара является обязательным полем");

                if (tovar.TovarCost <= 0)
                    return BadRequest("Стоимость товара должна быть больше 0");

                if (tovar.TovarCount < 0)
                    return BadRequest("Количество товара не может быть меньше 0");

                if (tovar.TovarDiscount < 0)
                    return BadRequest("Скидка товара не может быть меньше 0");

                if (string.IsNullOrWhiteSpace(tovar.TovarUnit))
                    return BadRequest("Единица измерения не может быть пустой");

                if (string.IsNullOrWhiteSpace(tovar.TovarDescription))
                    return BadRequest("Описание не может быть пустым");

                var existingProduct = context.Tovars
                    .FirstOrDefault(p => p.TovarArticle == tovar.TovarArticle);

                if (existingProduct == null)
                    return NotFound("Товар не найден.");

                existingProduct.TovarName = tovar.TovarName;
                existingProduct.ManufacturerId = tovar.ManufacturerId;
                existingProduct.TovarCategoryId = tovar.TovarCategoryId;
                existingProduct.TovarDescription = tovar.TovarDescription;
                existingProduct.TovarCost = tovar.TovarCost;
                existingProduct.TovarCount = tovar.TovarCount;
                existingProduct.TovarUnit = tovar.TovarUnit;
                existingProduct.SupplierId = tovar.SupplierId;
                existingProduct.TovarImage = tovar.TovarImage;

                context.SaveChanges();

                return Ok("Данные товара обновлены успешно!");
            }
               
        }

        [HttpDelete("DeleteTovar")]
        public ActionResult Add(string article)
        {
            using (var context = new DbCosmeticContext())
            {
                var tovar = context.Tovars.FirstOrDefault(a => a.TovarArticle == article);
                if (tovar == null) return BadRequest("Товар не найден");
                else
                {
                    context.Tovars.Remove(tovar);
                    context.SaveChanges();
                    return Ok("Товар успешно удален"); 
                }
            }
        }

        [HttpGet("FilterTovars")]
        public ActionResult<IEnumerable<Tovar>> FilterTovars([FromQuery] string? searchText = null, [FromQuery] int? supplierId = null, [FromQuery] string? sortBy = null, [FromQuery] bool? sortDescending = null)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var query = context.Tovars
                        .Include(a => a.Supplier)
                        .Include(b => b.Manufacturer)
                        .Include(a => a.TovarCategory)
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        var searchLower = searchText.ToLower();
                        query = query.Where(t =>
                            (t.TovarName != null && t.TovarName.ToLower().Contains(searchLower)) ||
                            (t.TovarDescription != null && t.TovarDescription.ToLower().Contains(searchLower)) ||
                            (t.Supplier != null && t.Supplier.SupplierName != null &&
                             t.Supplier.SupplierName.ToLower().Contains(searchLower)) ||
                            (t.TovarCategory != null && t.TovarCategory.TovarCategoryName != null &&
                             t.TovarCategory.TovarCategoryName.ToLower().Contains(searchLower)) ||
                            (t.Manufacturer != null && t.Manufacturer.ManufacturerName != null &&
                             t.Manufacturer.ManufacturerName.ToLower().Contains(searchLower)));
                    }

                    if (supplierId.HasValue && supplierId > 0)
                    {
                        query = query.Where(t => t.SupplierId == supplierId);
                    }

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        query = sortDescending == true
                                   ? query.OrderByDescending(t => t.TovarCount)
                                   : query.OrderBy(t => t.TovarCount);
                    }
                    else
                    {
                        query = query.OrderBy(t => t.TovarName);
                    }

                    var result = query.ToList();
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при фильтрации товаров: {ex.Message}");
            }
        }

        [HttpGet("GetSuppliers")]
        public ActionResult<IEnumerable<Supplier>> GetSuppliers()
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var suppliers = context.Suppliers.OrderBy(s => s.SupplierName).ToList();
                    return Ok(suppliers);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении поставщиков: {ex.Message}");
            }
        }


        [HttpGet("GetManufacturers")]
        public ActionResult<IEnumerable<Manufacturer>> GetManufacturers()
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var manufacturers = context.Manufacturers.OrderBy(m => m.ManufacturerName).ToList();
                    return Ok(manufacturers);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении производителей: {ex.Message}");
            }
        }

        [HttpGet("GetCategories")]
        public ActionResult<IEnumerable<TovarCategory>> GetCategories()
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var categories = context.TovarCategories.OrderBy(c => c.TovarCategoryName).ToList();
                    return Ok(categories);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении категорий: {ex.Message}");
            }
        }

    }
}
