using AutoMapper;
using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Domain;
using GTE.Mastery.ShoeStore.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using GTE.Mastery.ShoeStore.Business.Dtos;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.AspNetCore;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;

        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager,
            IValidator<RegisterDto> registerValidator, IValidator<LoginDto> loginValidator,
            IUserService userService, IMapper mapper)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _registerValidator = registerValidator ?? throw new ArgumentNullException(nameof(registerValidator));
            _loginValidator = loginValidator ?? throw new ArgumentNullException(nameof(loginValidator));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            ValidationResult validationResult = await _registerValidator.ValidateAsync(registerDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return View(registerDto);
            }

            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);

            if (userExists != null)
            {
                ModelState.AddModelError("Email","The user with this Email exists already");
            }

            var user = _mapper.Map<User>(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View(registerDto);
            }

            await _userManager.AddToRoleAsync(user,RoleTypes.User);
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            ValidationResult validationResult = await _loginValidator.ValidateAsync(loginDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return View(loginDto);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
                return RedirectToAction("Index", "Home");
            }

            return View(loginDto);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
