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
using System.Linq;
using System.Threading.Tasks;
using DamianTourBackend.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

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
        private readonly IMailService _mailService;
        private readonly IHubContext<TrackingHub> _trackingHub;

        public WalkController(
            IUserRepository userRepository,
            IWalkRepository walkRepository,
            IRegistrationRepository registrationRepository,
            IRouteRepository routeRepository,
            IMailService mailService,
            IConfiguration config, 
            IHubContext<TrackingHub> trackingHub)
        {
            _userRepository = userRepository;
            _walkRepository = walkRepository;
            _registrationRepository = registrationRepository;
            _routeRepository = routeRepository;
            _configuration = config;
            _mailService = mailService;
            _trackingHub = trackingHub;

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
        [HttpPut(nameof(Stop))]
        public IActionResult Stop()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            Walk walk = user.Walks.Last();
            if (walk == null) return NotFound();

            walk.EndTime = DateTime.Now;
            _walkRepository.Update(user.Email, walk);

            Registration reg = _registrationRepository.GetLast(user.Email);
            Route route = _routeRepository.GetBy(reg.RouteId);

            //_mailService.SendCertificate();
            _mailService.SendCertificate(new CertificateDTO()
            {
                Id = walk.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Distance = (route.DistanceInMeters/1000).ToString() + " KM",
                Date = DateTime.Now.ToString(),
            });

            return Ok();
        }

        [HttpPost(nameof(Start))]
        public IActionResult Start()
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

            if (walk == null 
               // && DateCheckHelper.CheckEqualsDate(route.Date, now)
                )
            {
                walk = new Walk(DateTime.Now, route);

                _walkRepository.Add(mailAdress, walk);
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost(nameof(TestMail))]
        public IActionResult TestMail(string firstname, string lastname, string email, string distance, string date, string tourname)
        {
            _mailService.SendRegistrationConfirmation(new RegistrationMailDTO()
            {
                Email = email,
                FirstName = firstname,
                LastName = lastname,
                Tourname = tourname,
                Distance = distance,
                Date = date,
            });
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
            
            //Invoke signalr to notify people that track this walker
            //TODO only send to tracking clients
            _trackingHub.Clients.All.SendAsync("updateWalk", walk);

            return Ok(walk);
        }

    }
}
