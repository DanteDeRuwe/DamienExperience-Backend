using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Application.DTOs;
using DamianTourBackend.Application.Mappers;
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
        private readonly IValidator<UpdateProfileDTO> _updateProfileValidator;

        public UsersController(
            IUserRepository userRepository,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration config,
            IValidator<LoginDTO> loginValidator,
            IValidator<RegisterDTO> registerValidator,
            IValidator<UpdateProfileDTO> updateProfileValidator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = config;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _updateProfileValidator = updateProfileValidator;
            _userRepository = userRepository;
        }
        #region Login & Register

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

            var identityUser = model.MapToIdentityUser();
            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded) return BadRequest();

            var user = model.MapToUser();
            _userRepository.Add(user);
            _userRepository.SaveChanges();

            string token = TokenHelper.GetToken(identityUser, _configuration);
            return Created("", token);
        }
        #endregion

        #region Profile Methods
        /// <summary>
        /// Logged in user can ask his/her account details
        /// </summary>
        /// <returns>User details (id,firstname,lastname,email,tel)</returns>
        [HttpGet("getProfile")]
        public IActionResult GetProfile()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();
            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();
            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();
            return Ok(user);
        }

        /// <summary>
        /// Logged in use can delete his profile
        /// </summary>
        /// <returns>Returns ok when profile is deleted and badrequest in case something goes wrong</returns>
        [HttpDelete("deleteProfile")]
        public async Task<IActionResult> DeleteProfile()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();
            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();
            //find user
            User user = _userRepository.GetBy(mailAdress);
            IdentityUser identityUser = await _userManager.FindByNameAsync(mailAdress);
            if (identityUser == null) return BadRequest();
            if (user == null) return BadRequest();
            //delete identityUser
            var result = await _userManager.DeleteAsync(identityUser);
            if (!result.Succeeded) return BadRequest();
            //delete user
            _userRepository.Delete(user);
            _userRepository.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Allows the user to edit his profile
        /// </summary>
        /// <param name="updateProfileDTO"></param>
        /// <returns>Ok when user is updated and badrequest in case something goes wrong</returns>
        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO updateProfileDTO)
        {
            //validation
            var validation = _updateProfileValidator.Validate(updateProfileDTO);
            if (!validation.IsValid) return BadRequest(validation);
            //user
            if (!User.Identity.IsAuthenticated) return Unauthorized();
            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();
            //find user 
            var user = _userRepository.GetBy(mailAdress);
            var identityUser = await _userManager.FindByNameAsync(mailAdress);
            if (identityUser == null) return BadRequest();
            if (user == null) return BadRequest();


            //update user
            user.Email = updateProfileDTO.Email;
            user.FirstName = updateProfileDTO.FirstName;
            user.LastName = updateProfileDTO.LastName;


            ///identity
            identityUser.Email = updateProfileDTO.Email;
            identityUser.UserName = updateProfileDTO.Email;


            //var result = await _userManager.UpdateAsync(null
            var result = await _userManager.UpdateAsync(identityUser);

            //Testen of gelukt is 
            _userRepository.Update(user);
            _userRepository.SaveChanges();
            return Ok();    //Moet dit geen user teruggeven?
        }
        #endregion

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
