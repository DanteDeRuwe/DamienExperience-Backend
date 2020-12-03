using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DamianTourBackend.Application.UpdateWaypoint;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Application;
using Microsoft.AspNetCore.Identity;

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
        public IActionResult AddWaypoints(string tourName, ICollection<WaypointDTO> dtos) 
        {
            if (!IsAdmin().Result) return Unauthorized();

            var route = _routeRepository.GetByName(tourName);
            if (route == null) return BadRequest();

            ICollection<Waypoint> waypoints = new List<Waypoint>();

            foreach (WaypointDTO dto in dtos) {
                waypoints.Add(dto.MapToWaypoint());
            }

            route.Waypoints = waypoints;
            _routeRepository.Update(route);

            return Ok();
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
