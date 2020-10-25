using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DamianTourBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    public class RouteRegistrationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRouteRepository _routeRepository;

        public RouteRegistrationController(IUserRepository userRepository, IRouteRepository routeRepository)
        {
            _userRepository = userRepository;
            _routeRepository = routeRepository;
        }

        [HttpPost("")]
        public IActionResult Post(RouteRegistrationDTO registrationDTO)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            var route = _routeRepository.GetBy(registrationDTO.RouteId);
            if (route == null) return BadRequest();

            return Ok();
        }
    }
}
