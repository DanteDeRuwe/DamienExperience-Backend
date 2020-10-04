using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Core.DTOs;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DamianTourBackend.Api.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UsersController(
            IUserRepository userRepository,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration config
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = config;
            _userRepository = userRepository;
        }


        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model">the login details</param>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null) return BadRequest();
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded) return BadRequest();

            string token = TokenHelper.GetToken(user, _configuration);
            return Ok(token);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(Guid id)
        {
            var user = _userRepository.GetBy(id);
            return (ActionResult<User>) user ?? NotFound();
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="model">the user details</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterDTO model)
        {
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
