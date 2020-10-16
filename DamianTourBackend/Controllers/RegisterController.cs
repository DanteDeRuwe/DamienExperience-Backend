using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Application;
using DamianTourBackend.Application.Register;
using DamianTourBackend.Core.Interfaces;
using FluentValidation;
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
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IValidator<RegisterDTO> _registerValidator;

        public RegisterController(IUserRepository userRepository, IValidator<RegisterDTO> registerValidator, UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _configuration = config;
            _registerValidator = registerValidator;
            _userRepository = userRepository;
        }

        [HttpPost("")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            var validation = _registerValidator.Validate(model);
            if (!validation.IsValid) return BadRequest(validation);

            var existingUser = _userRepository.GetBy(model.Email);
            if (existingUser != null) return BadRequest();

            var identityUser = model.MapToAppUser();
            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded) return BadRequest();

            var user = model.MapToUser();
            _userRepository.Add(user);

            string token = TokenHelper.GetToken(user.Email, _configuration);
            return Created("", token);
        }

        [HttpGet("checkusername")]
        public async Task<ActionResult<bool>> CheckAvailableUserName(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            return user == null;
        }
    }
}
