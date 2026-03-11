using CosmeticApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CosmeticApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost("Login")]
        public ActionResult Login(string login, string pass)
        {
            using (var context = new DbCosmeticContext())
            {
                var user = context.Users.Include(a => a.Role).FirstOrDefault(a => a.UserLogin == login && a.UserPass == pass);
                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest("Логин или пароль неверный");
                }
            }
        }

        [HttpPost("Register")]
        public ActionResult Register([FromBody] User user)
        {
            try
            {
                using (var context = new DbCosmeticContext())
                {
                    var existingUser = context.Users.FirstOrDefault(u => u.UserLogin == user.UserLogin);
                    if (existingUser != null)
                    {
                        return BadRequest("Пользователь с таким email уже существует");
                    }

                    if (string.IsNullOrEmpty(user.UserSurname) ||
                        string.IsNullOrEmpty(user.UserName) ||
                        string.IsNullOrEmpty(user.UserLastname) ||
                        string.IsNullOrEmpty(user.UserLogin) ||
                        string.IsNullOrEmpty(user.UserPass))
                    {
                        return BadRequest("Все поля должны быть заполнены");
                    }

                    user.RoleId = 1;
                    context.Users.Add(user);
                    context.SaveChanges();

                    context.Entry(user).Reference(u => u.Role).Load();

                    return Ok(new
                    {
                        user.UserId,
                        user.UserSurname,
                        user.UserName,
                        user.UserLastname,
                        user.UserLogin,
                        user.RoleId
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }
    }
}
