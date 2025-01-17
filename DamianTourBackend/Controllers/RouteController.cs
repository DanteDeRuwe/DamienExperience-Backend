﻿using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateRoute;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DamianTourBackend.Api.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteRepository _routeRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRegistrationRepository _registrationRepository;

        public RouteController(IRouteRepository routeRepository, UserManager<AppUser> userManager, IRegistrationRepository registrationRepository)
        {
            _routeRepository = routeRepository;
            _userManager = userManager;
            _registrationRepository = registrationRepository;
        }

        /// <summary>
        /// Returns one route with the specified name.
        /// </summary>
        /// <param name="tourName">Expects a tourname as parameter.</param>
        /// <returns>Returns a route with the corresponding name or gives a Bad Request when there is no route with given name.</returns>
        [AllowAnonymous]
        [HttpGet("GetRouteByName/{routeName}")]
        public IActionResult GetRouteByName(string routeName)
        {
            var route = _routeRepository.GetByName(routeName);
            if (route == null) return BadRequest();

            return Ok(route);
        }

        /// <summary>
        /// Returns one route with the specified routeid
        /// </summary>
        /// <param name="routeId">Expects a tourid as parameter.</param>
        /// <returns>Returns a route with the corresponding id or gives a Bad Request when there is no route with given id.</returns>
        [AllowAnonymous]
        [HttpGet("GetRouteById/{routeId}")]
        public IActionResult GetRouteById(string routeId)
        {
            var id = Guid.Parse(routeId);
            var route = _routeRepository.GetBy(id);
            if (route == null) return BadRequest();

            return Ok(route);
        }

        /// <summary>
        /// Adds a new route
        /// </summary>
        /// <param name="routeDTO">Expects a routeDTO</param>
        /// <returns>Ok request or unauthorized if user is not an admin</returns>
        [HttpPost(nameof(Add))]
        public IActionResult Add(RouteDTO routeDTO)
        {
            if (!IsAdmin().Result) return Unauthorized();

            if (_routeRepository.GetByName(routeDTO.TourName) != null) return BadRequest();

            Route route = routeDTO.MapToRoute();
            _routeRepository.Add(route);

            return Ok(route);
        }

        /// <summary>
        /// Deletes a route with the given name
        /// </summary>
        /// <param name="routeName">Expects a routeName</param>
        /// <returns>Ok request or unauthorized if user is not an admin</returns>
        [HttpDelete(nameof(Delete))]
        public IActionResult Delete(string routeName)
        {
            if (!IsAdmin().Result) return Unauthorized();
            var route = _routeRepository.GetByName(routeName);

            if (route == null) return BadRequest("Route was not found.");

            if (_registrationRepository.GetAllFromRoute(route.Id).Count() != 0)
            {
                return BadRequest("Route can not be deleted, there are registrations for this route.");
            }

            _routeRepository.Delete(route);

            return Ok();
        }

        /// <summary>
        /// Updates a route with using the name of the routeDTO
        /// </summary>
        /// <param name="routeDTO">Expects a routeDTO</param>
        /// <returns>The updated route and ok request or unauthorized </returns>
        [HttpPut(nameof(Update))]
        public IActionResult Update(RouteDTO routeDTO)
        {
            if (!IsAdmin().Result) return Unauthorized();

            var route = _routeRepository.GetByName(routeDTO.TourName);
            if (route == null) return BadRequest();

            routeDTO.UpdateRoute(ref route);
            _routeRepository.Update(route);

            return Ok(route);
        }

        [AllowAnonymous]
        [HttpGet(nameof(GetFutureRoutes))]
        public IActionResult GetFutureRoutes()
        {
            //maybe refactor into repomethod
            return Ok(_routeRepository.GetAll().Where(r => r.Date > DateTime.Now));
        }

        /// <summary>
        /// Gets all the routes
        /// </summary>
        /// <returns>A list of all the routes in an ok request</returns>
        [HttpGet(nameof(GetAll))]
        public IActionResult GetAll()
        {
            return Ok(_routeRepository.GetAll());
        }

        /// <summary>
        /// Checks if user is admin
        /// </summary>
        /// <returns>boolean if user is admin</returns>
        private async Task<bool> IsAdmin()
        {
            if (User.Identity.Name == null) return false;

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            return user.Claims.Any(c => c.ClaimValue.Equals("admin"));
        }
    }
}
