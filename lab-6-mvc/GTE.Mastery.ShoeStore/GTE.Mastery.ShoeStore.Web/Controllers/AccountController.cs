using AutoMapper;
using GTE.Mastery.ShoeStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GTE.Mastery.ShoeStore.Business.Dtos;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.AspNetCore;
using GTE.Mastery.ShoeStore.Domain.Enums;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IValidator<RegisterDto> registerValidator, IMapper mapper)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _registerValidator = registerValidator ?? throw new ArgumentNullException(nameof(registerValidator));
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

            await _userManager.AddToRoleAsync(user,RoleTypes.User.ToString());
            return RedirectToAction("Login", "Account");
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
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "The user with such Email does not exist");
                return View(loginDto);
            }

            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (checkPasswordResult == false)
            {
                ModelState.AddModelError("Password", "You have typed not valid Password");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

            if (signInResult.Succeeded)
            {
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
