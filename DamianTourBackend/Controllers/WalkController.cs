using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using DamianTourBackend.Infrastructure.Mailer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IConfiguration _configuration;

        public WalkController(
            IUserRepository userRepository,
            IWalkRepository walkRepository,
            IRegistrationRepository registrationRepository,
            IRouteRepository routeRepository,
             IConfiguration config)
        {
            _userRepository = userRepository;
            _walkRepository = walkRepository;
            _registrationRepository = registrationRepository;
            _routeRepository = routeRepository;
            _configuration = config;
        }

        [HttpGet("{email}")]
        [AllowAnonymous]
        public IActionResult SearchWalk(string email)
        {
            //if (!User.Identity.IsAuthenticated) return Unauthorized(); // wachtend optiesysteem (publieke wandelaar)

            var walker = _userRepository.GetBy(email);
            if (walker == null) return NotFound();

            var registration = _registrationRepository.GetLast(email);
            if (registration == null) return NotFound();

            var walk = _walkRepository.GetByUserAndRoute(walker.Id, registration.RouteId);
            if (walk == null) return NotFound();

            return Ok(walk);
        }


        //begone
        [AllowAnonymous]
        [HttpPut(nameof(StopWalkAsync))]
        public async Task<IActionResult> StopWalkAsync()
        {
            /*if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();*/


            MailService mailer = new MailService(_configuration);

            await mailer.CreateMail();


            return Ok();
        }

        [HttpPost(nameof(StartWalk))]
        public IActionResult StartWalk()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return NotFound("User not found");

            var registration = _registrationRepository.GetLast(mailAdress);
            if (registration == null) return NotFound("Registration not found");

            var route = _routeRepository.GetBy(registration.RouteId);
            if (route == null) return NotFound("Route not found");

            var walk = _walkRepository.GetByUserAndRoute(user.Id, route.Id);
            var now = DateTime.Now;
            if (walk == null && DateCheckHelper.CheckEqualsDate(route.Date, now))
            {
                walk = new Walk(DateTime.Now, route);

                _walkRepository.Add(mailAdress, walk);
            }
            return Ok();
        }

        [HttpPut(nameof(Update))]
        public IActionResult Update(List<double[]> coords)
        {
            User user = _userRepository.GetBy(User.Identity.Name);
            if (user == null) return NotFound("User not found");

            var routeid = _registrationRepository.GetLast(user.Email).RouteId;
            var route = _routeRepository.GetBy(routeid);

            Walk walk = _walkRepository.GetByUserAndRoute(user.Id, route.Id);
            if (walk == null) return NotFound("Walk not found for user");

            walk.AddCoords(coords);

            _walkRepository.Update(user.Email, walk);

            return Ok(walk);
        }

    }
}
