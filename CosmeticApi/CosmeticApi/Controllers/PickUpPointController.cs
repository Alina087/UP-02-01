using CosmeticApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmeticApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PickUpPointController : ControllerBase
    {
        // GET: api/PickUpPoint/GetAll
        [HttpGet("GetAll")]
        public ActionResult GetAll()
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    // Загружаем все пункты выдачи и сортируем на клиенте
                    var points = context.PickUpPoints.ToList();

                    // Формируем адрес для каждого пункта
                    var result = points.Select(p => new
                    {
                        p.PickUpPointId,
                        p.PickUpPointIndex,
                        p.PickUpPointCity,
                        p.PickUpPointStreet,
                        p.PickUpPointHome,
                        PickUpPointAdress = $"{p.PickUpPointIndex}, {p.PickUpPointCity}, {p.PickUpPointStreet}, {p.PickUpPointHome}"
                    })
                    .OrderBy(p => p.PickUpPointAdress)
                    .ToList();

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении пунктов выдачи: {ex.Message}");
            }
        }

        // GET: api/PickUpPoint/GetById/5
        [HttpGet("GetById/{id}")]
        public ActionResult GetById(int id)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var point = context.PickUpPoints.Find(id);

                    if (point == null)
                        return NotFound("Пункт выдачи не найден");

                    var result = new
                    {
                        point.PickUpPointId,
                        point.PickUpPointIndex,
                        point.PickUpPointCity,
                        point.PickUpPointStreet,
                        point.PickUpPointHome,
                        PickUpPointAdress = $"{point.PickUpPointIndex}, {point.PickUpPointCity}, {point.PickUpPointStreet}, {point.PickUpPointHome}"
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении пункта выдачи: {ex.Message}");
            }
        }
    }
}