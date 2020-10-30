using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateWalk;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DamianTourBackend.Api.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WalkController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalkRepository _walkRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IRouteRepository _routeRepository;

        public WalkController(
            IUserRepository userRepository,
            IWalkRepository walkRepository,
            UserManager<AppUser> userManager,
            IRegistrationRepository registrationRepository,
            IRouteRepository routeRepository)
        {
            _userRepository = userRepository;
            _walkRepository = walkRepository;
            _userManager = userManager;
            _registrationRepository = registrationRepository;
            _routeRepository = routeRepository;
        }

        [HttpGet("{email}")]
        public IActionResult SearchWalker(string email) // mag ook searchwalk zijn 
        {

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var searcher = _userRepository.GetBy(mailAdress);
            if (searcher == null) return BadRequest();

            var walker = _userRepository.GetBy(email);
            if (walker == null) return BadRequest();

            Registration registration = _registrationRepository.GetLast(email);
            if (registration == null) return NotFound();

            Walk walk =_walkRepository.GetByUserAndRoute(walker.Id, registration.RouteId);
            if (walk == null) return NotFound();


            //if (!User.Identity.IsAuthenticated) return Unauthorized(); // wachtend optiesysteem (publieke wandelaar)


            return Ok(walk.MapToWalkDTO());
        }

        [HttpPut(nameof(StopWalk))]
        public IActionResult StopWalk()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();



            return Ok(user);
        }

        [HttpPost(nameof(StartWalk))]
        public IActionResult StartWalk()
        {

            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();



            return Ok(user);
        }

        [HttpPost(nameof(AddTest))]
        public IActionResult AddTest(WalkDTO walkDTO) {
            User user = _userRepository.GetBy(User.Identity.Name);
            Route route = _routeRepository.GetByName("RouteZero");
            Registration registration = new Registration(
                    DateTime.Now,
                    route,
                    user,
                    true,
                    "L"
                );
            Walk walk = new Walk(DateTime.Now, route, user);
            walk.SetCoords(walkDTO.Coordinates);
            user.Registrations.Add(registration);

            _userRepository.Update(user);
            _registrationRepository.Add(registration, user.Email);

           _walkRepository.Add(walk);

            return Ok();
        }

        [HttpPut(nameof(Update))]
        public IActionResult Update(WalkDTO walkDTO) {
            User user = _userRepository.GetBy(User.Identity.Name);
            if (user == null) return NotFound("User not found");
            Walk walk = _walkRepository.GetByUserAndRoute(user.Id, user.Registrations.Last().Id);
            if (walk == null) return NotFound("Walk not found for user");

            walkDTO.UpdateWalk(ref walk);
            _walkRepository.Update(walk);

            return Ok(walk);
        }
    }
}
