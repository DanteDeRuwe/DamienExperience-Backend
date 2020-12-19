using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateWaypoint;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamianTourBackend.Application.Helpers;

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

        /// <summary>
        /// Get waypoints with given tour name
        /// </summary>
        /// <param name="tourName">name of the tour of the waypoints you want to get</param>
        /// <returns>ok with all the waypoints of the selectedRoute</returns>
        [AllowAnonymous]
        [HttpGet(nameof(Get))]
        public IActionResult Get(string tourName)
        {
            var route = _routeRepository.GetByName(tourName);
            if (route == null) return BadRequest();

            IEnumerable<Waypoint> waypoints = route.Waypoints;
            return Ok(waypoints);
        }

        /// <summary>
        /// Add waypoints using UpdateWaypointDTO
        /// </summary>
        /// <param name="updateWaypoint">UpdateWaypointDTO with list of waypoints and info and name of route</param>
        /// <returns>ok with waypoints or Unauthorized if user isn't admin or BadRequest if route name isn't valid</returns>
        [HttpPost(nameof(AddWaypoints))]
        public IActionResult AddWaypoints(UpdateWaypointDTO updateWaypoint)
        {
            //needed to put it in an object 
            if (!IsAdmin().Result) return Unauthorized();

            var route = _routeRepository.GetByName(updateWaypoint.TourName);
            if (route == null) return BadRequest();

            var waypoints = updateWaypoint.Dtos.Select(dto => dto.MapToWaypoint()).ToList();

            route.Waypoints = waypoints;
            _routeRepository.Update(route);

            return Ok(waypoints);
        }

        /// <summary>
        /// Add waypoints to given route
        /// </summary>
        /// <param name="routename">name of route you want to add waypoints to</param>
        /// <param name="waypointDTO">WaypointDTO containg longitude, latitude and Dictionary of info</param>
        /// <returns>Ok with route, or Unauthorized if current user isn't admin or BadRequest if route isn't valid</returns>
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

        /// <summary>
        /// Deletes waypoint from given route
        /// </summary>
        /// <param name="routename">Name of the route you want to delete waypoints from</param>
        /// <param name="waypoint">Waypoint you want to delete</param>
        /// <returns>Ok or Unauthorized if current user isn't admin orBadRequest if routename isn't valid</returns>
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

        /// <summary>
        /// Checks if current user is admin
        /// </summary>
        /// <returns>Boolean if user is admin</returns>
        private async Task<bool> IsAdmin()
        {
            if (User.Identity.Name == null) return false;
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.IsAdmin();
        }
    }
}
