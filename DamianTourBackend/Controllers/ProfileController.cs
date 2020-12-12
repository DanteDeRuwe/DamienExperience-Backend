using AspNetCore.Identity.Mongo.Model;
using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateProfile;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly RoleManager<MongoRole> _roleManager;
        private readonly IRegistrationRepository _registrationRepository;

        public ProfileController(IUserRepository userRepository, 
            IValidator<UpdateProfileDTO> updateProfileValidator, 
            UserManager<AppUser> userManager,
            RoleManager<MongoRole> roleManager,
            IRegistrationRepository registrationRepository)
        {
            _userRepository = userRepository;
            _updateProfileValidator = updateProfileValidator;
            _userManager = userManager;
            _roleManager = roleManager;
            _registrationRepository = registrationRepository;
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

        [HttpPut(nameof(UpdateFriends))]
        public IActionResult UpdateFriends(ICollection<string> friends)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            user.Friends = friends;

            // Update User
            _userRepository.Update(user);

            return Ok(user.Friends);
        }

        [HttpPut(nameof(UpdatePrivacy))]
        public IActionResult UpdatePrivacy(string privacy)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            Privacy updatedPrivacy = Privacy.PRIVATE;
            Enum.TryParse(privacy, out updatedPrivacy);

            user.Privacy = updatedPrivacy;

            var last = _registrationRepository.GetLast(mailAdress);
            if (last != null)
            {
                last.Privacy = updatedPrivacy;
                _registrationRepository.Update(last, mailAdress);
                user.Registrations = _registrationRepository.GetAllFromUser(mailAdress);
            }

            // Update User
            _userRepository.Update(user);

            return Ok(user.Privacy);
        }

        [HttpPost(nameof(AddAdmin))]
        public async Task<ActionResult> AddAdmin(string email) {

            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null || mailAdress.Equals("")) return Unauthorized();

            AppUser admin = await _userManager.FindByEmailAsync(mailAdress);
            if (admin == null ) return BadRequest();

            if (!await _roleManager.RoleExistsAsync("admin"))
                await _roleManager.CreateAsync(new MongoRole("admin"));

            if (!admin.IsAdmin()) return Unauthorized();


            await _userManager.AddToRoleAsync(user, "admin");
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "admin"));

            return Ok();
        }

        [HttpPost(nameof(RemoveAdmin))]
        public async Task<ActionResult> RemoveAdmin(string email)
        {

            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null || mailAdress.Equals("")) return Unauthorized();

            AppUser admin = await _userManager.FindByEmailAsync(mailAdress);
            if (admin == null) return BadRequest();
            

            if (!admin.IsAdmin()) return Unauthorized();

            await _userManager.RemoveFromRoleAsync(user, "admin");
            await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, "admin"));

            return Ok();
        }

        [HttpGet(nameof(IsAdmin))]
        public async Task<ActionResult> IsAdmin()
        {
            string mailAdress = User.Identity.Name;
            if (mailAdress == null || mailAdress.Equals("")) return Unauthorized();

            AppUser admin = await _userManager.FindByEmailAsync(mailAdress);
            if (admin == null) return BadRequest();


            return Ok(admin.IsAdmin());
        }        
     }
}
