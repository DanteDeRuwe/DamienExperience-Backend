using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Application.DTOs;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DamianTourBackend.Api.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly IValidator<RegisterDTO> _registerValidator;

        public UsersController(
            IUserRepository userRepository,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration config,
            IValidator<LoginDTO> loginValidator,
            IValidator<RegisterDTO> registerValidator
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = config;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model">the login details</param>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var validation = _loginValidator.Validate(model);
            if (!validation.IsValid) return BadRequest(validation);

            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null) return BadRequest();

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return BadRequest();

            string token = TokenHelper.GetToken(user, _configuration);
            return Ok(token);
        }

        /// <summary>
        /// Logged in user can ask his/her account details
        /// </summary>
        /// <returns>User details (id,firstname,lastname,email,tel)</returns>
        [HttpGet("getProfile")]
        public IActionResult GetProfile()
        {
            if (!User.Identity.IsAuthenticated) return BadRequest();
            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();
            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();
            return Ok(user);
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="model">the user details</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            var validation = _registerValidator.Validate(model);
            if (!validation.IsValid) return BadRequest(validation);

            var existingUser = _userRepository.GetBy(model.Email);
            if (existingUser != null) return BadRequest();

            IdentityUser identityUser = new IdentityUser { UserName = model.Email, Email = model.Email };
            User user = new User { Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };

            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded) return BadRequest();

            _userRepository.Add(user);
            _userRepository.SaveChanges();

            string token = TokenHelper.GetToken(identityUser, _configuration);
            return Created("", token);
        }

        /// <summary>
        /// Checks if an email is available as username
        /// </summary>
        /// <returns>true if the email is not registered yet</returns>
        /// <param name="email">Email.</param>/
        [AllowAnonymous]
        [HttpGet("checkusername")]
        public async Task<ActionResult<bool>> CheckAvailableUserName(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            return user == null;
        }
    }
}
