using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateRoute;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public RouteController(IRouteRepository routeRepository,UserManager<AppUser> userManager)
        {
            _routeRepository = routeRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns one route with the specified name.
        /// </summary>
        /// <param name="tourName">Expects a tourname as parameter.</param>
        /// <returns>Returns a route with the corresponding name or gives a Bad Request when there is no route with given name.</returns>
        [HttpGet(nameof(GetRouteByName))]
        public IActionResult GetRouteByName(string routeName) 
        {

            var route = _routeRepository.GetByName(routeName); 

            if (route == null) return BadRequest();

            return Ok(route);
        }


        [HttpPost(nameof(Add))]
        public IActionResult Add(RouteDTO routeDTO) 
        {
            if (!IsAdmin().Result) return Unauthorized();

            if (_routeRepository.GetByName(routeDTO.TourName) != null) return BadRequest();

            Route route = routeDTO.MapToRoute();
            _routeRepository.Add(route);

            return Ok();
        }

        [HttpDelete(nameof(Delete))]
        public IActionResult Delete(string routeName)
        {
            if (!IsAdmin().Result) return Unauthorized();
            var route = _routeRepository.GetByName(routeName);

            if (route == null) return BadRequest();

            _routeRepository.Delete(route);

            return Ok();
        }

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

        [HttpGet("")]
        public IActionResult GetAll() 
        {
            return Ok(_routeRepository.GetAll());
        }

        //temp method to check claims
        private async Task<bool> IsAdmin() {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            return user.Claims.Any(c => c.ClaimValue.Equals("admin"));
        }
    }
}
