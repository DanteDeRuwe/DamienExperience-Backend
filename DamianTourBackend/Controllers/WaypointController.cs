using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateWaypoint;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DamianTourBackend.Api.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WaypointController : ControllerBase
    {

        private readonly IRouteRepository _routeRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;

        public WaypointController(IRouteRepository routeRepository, IUserRepository userRepository, UserManager<AppUser> userManager)
        {
            _routeRepository = routeRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet(nameof(Get))]
        public IActionResult Get(string tourName)
        {

            var route = _routeRepository.GetByName(tourName);
            if (route == null) return BadRequest();

            IEnumerable<Waypoint> waypoints = route.Waypoints;
            return Ok(waypoints);
        }

        [HttpPost(nameof(AddWaypoints))]
        public IActionResult AddWaypoints(UpdateWaypointDTO updateWaypoint)
        {
            //needed to put it in an object 
            if (!IsAdmin().Result) return Unauthorized();

            var route = _routeRepository.GetByName(updateWaypoint.TourName);
            if (route == null) return BadRequest();

            ICollection<Waypoint> waypoints = new List<Waypoint>();

            foreach (var dto in updateWaypoint.Dtos)
            {
                waypoints.Add(dto.MapToWaypoint());
            }

            route.Waypoints = waypoints;
            _routeRepository.Update(route);

            return Ok(waypoints);
        }


        [HttpPut(nameof(AddWaypoint))]
        public IActionResult AddWaypoint(string routename, WaypointDTO waypointDTO)
        {
            if (!IsAdmin().Result) return Unauthorized();

            var route = _routeRepository.GetByName(routename);
            if (route == null) return BadRequest();

            route.Waypoints.Add(waypointDTO.MapToWaypoint());

            _routeRepository.Update(route);

            return Ok(route);
        }

        [HttpDelete(nameof(DeleteWaypoint))]
        public IActionResult DeleteWaypoint(string routename, Waypoint waypoint)//If this doesn't work, Lucas will fix
        {
            if (!IsAdmin().Result) return Unauthorized();

            var route = _routeRepository.GetByName(routename);
            if (route == null) return BadRequest();

            route.Waypoints.Remove(waypoint);
            _routeRepository.Update(route);

            return Ok();
        }


        private async Task<bool> IsAdmin()
        {
            if (User.Identity.Name == null) return false;

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            return user.IsAdmin();
        }
    }
}
