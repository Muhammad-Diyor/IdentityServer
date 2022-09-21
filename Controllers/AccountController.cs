using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ilmhub.Identity.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        ILogger<AccountController> logger,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register(string returnUrl) => View(new RegisterViewModel() { ReturnUrl = returnUrl });

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var user = new IdentityUser(model.Username);
        var result = await _userManager.CreateAsync(user, model.Password);
        if(!result.Succeeded)
        {
            _logger.LogError($"User create bo'lmadi. {result.Errors.First().Description}");
            ModelState.AddModelError("1","Something went wrong!");
            return View();
        }
        return LocalRedirect($"/account/login?returnUrl={model.ReturnUrl}");
    }

    [HttpGet]
    public IActionResult Login(string returnUrl) => View(new LoginViewModel() { ReturnUrl = returnUrl });

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if(user is null)
        {
            return RedirectToAction(nameof(Register));
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if(!result.Succeeded)
        {
            _logger.LogError($"{result.IsNotAllowed} Nimadirlar bo'layapti");
            return View();
        }

        return LocalRedirect($"{model.ReturnUrl ?? "/"}");
    }

    
}