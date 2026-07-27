using ExpenseControl.Api.Data;
using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Mapper;
using ExpenseControl.Api.Model.Entity;
using ExpenseControl.Api.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ExpenseControl.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Root + "/[controller]")]
[SwaggerTag("Autenticação")]
public class AuthController(
    AuthService authService,
    AppDbContext context,
    UserMapper userMapper) : ControllerBase
{
    [HttpPost(ApiRoutes.Auth.Register)]
    [SwaggerOperation(Summary = "Registra um novo usuário")]
    [HttpPost(ApiRoutes.Auth.Register)]
    [HttpPost(ApiRoutes.Auth.Register)]
    [HttpPost(ApiRoutes.Auth.Register)]
    public async Task<ActionResult<UserDto>> Register(RegisterUserDto registerDto)
    {
        if (await context.Users.AnyAsync(u => u.Username == registerDto.Username))
        {
            return BadRequest("Username already exists.");
        }

        People? people = null;
        if (registerDto.PeopleId.HasValue)
        {
            people = await context.Peoples.FindAsync(registerDto.PeopleId.Value);
            if (people == null)
            {
                return NotFound($"People with ID {registerDto.PeopleId.Value} does not exist.");
            }
        }

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = authService.RegisterPassword(registerDto.Password),
            People = people
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Ok(userMapper.MapToDto(user));
    }

    [HttpPost(ApiRoutes.Auth.Login)]
    [SwaggerOperation(Summary = "Realiza login e retorna o token JWT")]
    public async Task<ActionResult> Login(LoginDto loginDto)
    {
        var user = await context.Users
            .Include(u => u.People)
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = authService.LoginAndGenerateToken(user, loginDto.Password);

        if (token == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(new { token });
    }
}