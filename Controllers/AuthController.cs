using ExpenseControl.Api.Data;
using ExpenseControl.Api.Dto;
using ExpenseControl.Api.Mapper;
using ExpenseControl.Api.Model.Entity;
using ExpenseControl.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ExpenseControl.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Auth.Base)]
[SwaggerTag("Autenticação")]
public class AuthController(
    AuthService authService,
    AppDbContext context,
    UserMapper userMapper) : ControllerBase
{
    [Authorize]
    [HttpPost(ApiRoutes.Auth.Register)]
    [SwaggerOperation(Summary = "Registra um novo usuário (requer estar autenticado)")]
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

    /// <summary>
    /// Retorna todos os usuários cadastrados no sistema. Requer autenticação.
    /// </summary>
    [Authorize]
    [HttpGet(ApiRoutes.Auth.Users)]
    [SwaggerOperation(Summary = "Retorna todos os usuários cadastrados no sistema")]
    [SwaggerResponse(200, "Lista de usuários retornada com sucesso", typeof(IEnumerable<UserDto>))]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await context.Users.Include(u => u.People).ToListAsync();
        return Ok(users.Select(userMapper.MapToDto));
    }

    /// <summary>
    /// Exclui um usuário pelo seu identificador único. Requer autenticação.
    /// Não é permitido excluir o próprio usuário logado, nem o último usuário restante.
    /// </summary>
    [Authorize]
    [HttpDelete($"{ApiRoutes.Auth.Users}/{{id}}")]
    [SwaggerOperation(Summary = "Exclui um usuário")]
    [SwaggerResponse(204, "Usuário excluído com sucesso")]
    [SwaggerResponse(400, "Não é possível excluir o próprio usuário ou o último usuário restante")]
    [SwaggerResponse(404, "Usuário não encontrado")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (user.Username == User.Identity?.Name)
        {
            return BadRequest("Você não pode excluir o próprio usuário enquanto estiver logado com ele.");
        }

        if (await context.Users.CountAsync() <= 1)
        {
            return BadRequest("Não é possível excluir o último usuário do sistema.");
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return NoContent();
    }
}