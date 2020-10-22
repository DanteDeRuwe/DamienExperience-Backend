using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateProfile;
using DamianTourBackend.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DamianTourBackend.Api.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UpdateProfileDTO> _updateProfileValidator;

        public ProfileController(IUserRepository userRepository, IValidator<UpdateProfileDTO> updateProfileValidator, UserManager<AppUser> userManager)
        {
            _userRepository = userRepository;
            _updateProfileValidator = updateProfileValidator;
            _userManager = userManager;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            return Ok(user);
        }

        [HttpDelete(nameof(Delete))]
        public async Task<IActionResult> Delete()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            var identityUser = await _userManager.FindByNameAsync(mailAdress);
            if (user == null || identityUser == null) return BadRequest();

            // Delete User
            _userRepository.Delete(user);

            // Delete IdentityUser
            var result = await _userManager.DeleteAsync(identityUser);
            if (!result.Succeeded) return BadRequest();

            return Ok();
        }

        [HttpPut(nameof(Update))]
        public async Task<IActionResult> Update(UpdateProfileDTO updateProfileDTO)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            var validation = _updateProfileValidator.Validate(updateProfileDTO);
            if (!validation.IsValid) return BadRequest(validation);

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            var identityUser = await _userManager.FindByNameAsync(mailAdress);
            if (user == null || identityUser == null) return BadRequest();

            // Update User
            updateProfileDTO.UpdateUser(ref user);
            _userRepository.Update(user);

            // Update IdentityUser
            updateProfileDTO.UpdateIdentityUser(ref identityUser);
            var result = await _userManager.UpdateAsync(identityUser);
            if (!result.Succeeded) return BadRequest();

            return Ok(user);
        }
    }
}
