using Microsoft.AspNetCore.Mvc;
using UserMgr.Domain;
using UserMgr.Infrastructure;

namespace UserMgr.WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly UserDomainService _userService;

    public LoginController(UserDomainService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 根據手機號與密碼進行登入驗證
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [UnitOfWork(typeof(UserDbContext))]
    [HttpPost]
    public async Task<IActionResult> LoginByPhoneAndPassword(LoginByPhoneAndPasswordRequest request)
    {
        // 驗證邏輯請另外寫
        // 不要寫到controller的Action中
        if (request.Password.Length <= 3)
        {
            return BadRequest($"密碼長度過短");
        }

        var res = await _userService.CheckLoginAsync(request.PhoneNumber, request.Password);

        switch (res)
        {
            case UserAccessResult.Ok:
                return Ok("登入成功");

            case UserAccessResult.NoPassword:
            case UserAccessResult.PasswordError:
            case UserAccessResult.PhoneNumberNotFound:
                return BadRequest("登入失敗");
            case UserAccessResult.LockOut:
                return BadRequest("帳戶被鎖定");
            default:
                throw new ApplicationException($"未知值{res}");
        }
    }
}