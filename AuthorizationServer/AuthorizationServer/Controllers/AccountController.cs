using AuthorizationServer.Database;
using AuthorizationServer.Models;
using AuthorizationServer.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class AccountController : Controller
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private static bool _databaseChecked;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ApplicationDbContext applicationDbContext)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _applicationDbContext = applicationDbContext;
    }

    //
    // POST: /Account/Register
    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
      EnsureDatabaseCreated(_applicationDbContext);
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByNameAsync(model.Email);
        if (user != null)
        {
          return StatusCode(StatusCodes.Status409Conflict);
        }

        user = new User { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          return Ok();
        }
        AddErrors(result);
      }

      // If we got this far, something failed.
      return BadRequest(ModelState);
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
      EnsureDatabaseCreated(_applicationDbContext);
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByNameAsync(model.Email);

        if (user is null)
        {
          return StatusCode(StatusCodes.Status404NotFound);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        return result.Succeeded == true
          ? Ok() 
          : (IActionResult)StatusCode(StatusCodes.Status409Conflict);
      }

      return BadRequest(ModelState);
    }

    #region Helpers
    private static void EnsureDatabaseCreated(ApplicationDbContext context)
    {
      if (!_databaseChecked)
      {
        _databaseChecked = true;
        context.Database.EnsureCreated();
      }
    }

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }
    }

    #endregion
  }
}
