using DamianTourBackend.Api.Hubs;
using DamianTourBackend.Application.StopWalk;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using DamianTourBackend.Infrastructure.Mailer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;


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

        /// <summary>
        /// Looks for walk with given email
        /// </summary>
        /// <param name="email">email of person who walks</param>
        /// <returns>ok with walk or notFound if given email isn't valid</returns>
        [HttpGet("{email}")]
        [AllowAnonymous]
        public IActionResult SearchWalk(string email)
        {
            var walker = _userRepository.GetBy(email);
            if (walker == null) return NotFound();

            var registration = _registrationRepository.GetLast(email);
            if (registration == null) return NotFound();

            switch (registration.Privacy)
            {
                case Privacy.PRIVATE:
                    return NotFound(); // don't return walk
                case Privacy.FRIENDS when !User.Identity.IsAuthenticated:
                    return Unauthorized();
                case Privacy.FRIENDS:
                    {
                        string mailAdress = User.Identity.Name;
                        if (mailAdress == null) return BadRequest();

                        var user = _userRepository.GetBy(mailAdress);
                        if (user == null) return BadRequest();

                        if (!walker.IsFriend(user.Email))
                            return BadRequest(); // don't return walk
                        break;
                    }
            }

            var walk = _walkRepository.GetByUserAndRoute(walker.Id, registration.RouteId);
            if (walk == null) return NotFound();

            return Ok(walk);
        }


        //begone 
        //Jordy schreef dit ^
        /// <summary>
        /// Stops the walk of the current user
        /// </summary>
        /// <returns>Ok or Unauthorized if user isn't logged in or BadRequest if user isn't valid or NotFound if current walk is invalid</returns>
        [HttpPut(nameof(Stop))]
        public IActionResult Stop()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            var walk = user.Walks.Last();
            if (walk == null) return NotFound();

            walk.EndTime = DateTime.Now;
            _walkRepository.Update(user.Email, walk);

            var registration = _registrationRepository.GetLast(user.Email);
            var route = _routeRepository.GetBy(registration.RouteId);

            var certificateDTO = CertificateMapper.DTOFrom(user, walk, route);
            _mailService.SendCertificate(certificateDTO);

            return Ok();
        }

        /// <summary>
        /// Starts the walk from the current user
        /// </summary>
        /// <returns>Ok or Unauthorized if user isn't logged in or BadRequest if user isn't valid or NotFound if route/registration/user is not found</returns>
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

            //TODO this check needs to happen in rproduction
            //test
            //if(!registration.Paid) return BadRequest("Registration has not been paid");

            var route = _routeRepository.GetBy(registration.RouteId);
            if (route == null) return NotFound("Route not found");

            var walk = _walkRepository.GetByUserAndRoute(user.Id, route.Id);

            if (walk == null
                // && DateCheckHelper.CheckEqualsDate(route.Date, now)
                )
            {
                walk = new Walk(DateTime.Now, route);
                _walkRepository.Add(mailAdress, walk);
            }
            return Ok();
        }

        /// <summary>
        /// Updates the users current walk with given coordinates
        /// </summary>
        /// <param name="coords">coordinates that need to be pushed onto the walk</param>
        /// <returns>ok with current walk or NotFound if user/walk is invalid</returns>
        [HttpPut(nameof(Update))]
        public IActionResult Update(List<double[]> coords)
        {
            var user = _userRepository.GetBy(User.Identity.Name);
            if (user == null) return NotFound("User not found");

            var routeid = _registrationRepository.GetLast(user.Email).RouteId;
            var route = _routeRepository.GetBy(routeid);

            var walk = _walkRepository.GetByUserAndRoute(user.Id, route.Id);
            if (walk == null) return NotFound("Walk not found for user");

            walk.AddCoords(coords);

            _walkRepository.Update(user.Email, walk);

            //Invoke signalr to notify people that track this walker
            _trackingHub.Clients.Group(User.Identity.Name)
                .SendAsync("updateWalk", walk);

            return Ok(walk);
        }
    }
}
