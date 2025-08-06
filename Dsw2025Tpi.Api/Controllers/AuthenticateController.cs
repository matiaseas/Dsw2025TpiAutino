using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/auth")]

public class AuthenticateController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtTokenServices _jwtTokenServices;
    public AuthenticateController( 
        UserManager<IdentityUser> userManager, 
        SignInManager<IdentityUser> signInManager,
        JwtTokenServices jwtTokenServices)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenServices = jwtTokenServices;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
        {
            return Unauthorized("Usuario o contraseña invalido.");
        }

        if(string.IsNullOrWhiteSpace(request.Username) 
          || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Usuario y contraseña son obligatorios.");
        } 

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Usuario o contraseña invalido.");
        }
        var roles= await _userManager.GetRolesAsync(user!);
        var role = roles.FirstOrDefault();

        var token = _jwtTokenServices.GenerateToken(request.Username,role!);
        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if(string.IsNullOrWhiteSpace(model.Username) 
          || string.IsNullOrWhiteSpace(model.Password) 
          || string.IsNullOrWhiteSpace(model.Email))
        {
            return BadRequest("Todos los campos son obligatorios.");
        }

        if(!model.Email.Contains("@"))
        {
            return BadRequest("El correo electrónico debe ser válido.");
        }

        if (model == null)
        {
            return BadRequest("Modelo de registro no puede ser nulo.");
        }

        var user = new IdentityUser
        {
            UserName = model.Username,
            Email = model.Email
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok("Usuario registrado exitosamente.");

    }
}
