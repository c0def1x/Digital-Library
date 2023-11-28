using Digital_Library.Domain.Entities;
using Digital_Library.Domain.Services;
using Digital_Library.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace Digital_Library.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private const int _adminRoleId = 2;
        private const int _clientRoleId = 1;
        private readonly ILogger<UserController> _logger;

        public UserController (IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public IActionResult Login ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login (LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            User? user = await _userService.GetUserAsync(loginViewModel.Username, loginViewModel.Password);
            if (user is not null)
            {
                await SignIn(user);
                return View();
            }
            else
            {
                _logger.LogError("User not found");
                ModelState.AddModelError("user_null", "Пользователь не найден");
                return View(loginViewModel);
            }
            
        }

        [HttpGet]
        public IActionResult Registration ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration (RegistrationViewModel registration)
        {
            if (!ModelState.IsValid)
            {
                return View(registration);
            }

            if (await _userService.IsUserExistsAsync(registration.Username))
            {
                _logger.LogError("User name not exist");
                ModelState.AddModelError("user_exists", $"Имя пользователя {registration.Username} уже существует!");
                return View(registration);
            }


            try
            {
                await _userService.RegistrationAsync(registration.Fullname, registration.Username, registration.Password);
                return RedirectToAction("RegistrationSuccess", "User");
            } catch
            {
                _logger.LogError("Registratin failed, try later");
                ModelState.AddModelError("reg_error", $"Не удалось зарегистрироваться, попробуйте попытку регистрации позже");
                return View(registration);
            }
        }

        public IActionResult RegistrationSuccess ()
        {
            return View();
        }

        private async Task SignIn (User user)
        {
            string role = user.RoleId switch
            {
                _adminRoleId => "admin",
                _clientRoleId => "client",
                _ => throw new ApplicationException("invalid user role")
            };

            List<Claim> claims = new List<Claim>
            {
                new Claim("fullname", user.Fullname),
                new Claim("id", user.Id.ToString()),
                new Claim("role", role),
                new Claim("username", user.Login)
            };
            string authType = CookieAuthenticationDefaults.AuthenticationScheme;
            IIdentity identity = new ClaimsIdentity(claims, authType, "username", "role");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);
        }

        public async Task<IActionResult> Logout ()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public IActionResult AccessDenied ()
        {
            return View();
        }
    }
}
