using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserMgr.Domain;
using UserMgr.Domain.Entities;
using UserMgr.Infrastructure;

namespace UserMgr.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CRUDController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserDbContext _userDbContext;

        public CRUDController(IUserRepository userRepository, UserDbContext userDbContext)
        {
            _userRepository = userRepository;
            _userDbContext = userDbContext;
        }

        [UnitOfWork(typeof(UserDbContext))]
        [HttpPost]
        public async Task<IActionResult> AddNewUser(AddUserRequest request)
        {
            var user = await _userRepository.FindOneAsync(request.PhoneNumber);
            if (user != null)
            {
                return BadRequest($"手機號碼已被註冊");
            }

            var newUser = new User(request.PhoneNumber);
            newUser.ChangePassword(request.Password);
            _userDbContext.Set<User>().Add(newUser);
            return Ok("成功完成註冊");
        }
    }
}
