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
    /// <summary>
    /// Registra um novo usuário. Não exige autenticação (sem <c>[Authorize]</c>),
    /// já que é o próprio ponto de entrada para obter credenciais.
    /// </summary>
    /// <param name="registerDto">Dados de registro (usuário, senha, e opcionalmente uma pessoa para associar).</param>
    /// <returns>O usuário criado, sem senha/hash no corpo da resposta.</returns>
    [HttpPost(ApiRoutes.Auth.Register)]
    [SwaggerOperation(Summary = "Registra um novo usuário")]
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

    /// <summary>
    /// Autentica um usuário e retorna um token JWT válido para chamadas subsequentes
    /// aos endpoints protegidos com <c>[Authorize]</c>.
    /// </summary>
    /// <param name="loginDto">Credenciais informadas pelo usuário.</param>
    /// <returns>Um objeto <c>{ token }</c> em caso de sucesso, ou 401 se as credenciais forem inválidas.</returns>
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