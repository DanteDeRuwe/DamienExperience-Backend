using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IValidator<RouteRegistrationDTO> _routeRegistrationDTOValidator;

        public RouteRegistrationController(IUserRepository userRepository, IRouteRepository routeRepository,
            IRegistrationRepository registrationRepository, IValidator<RouteRegistrationDTO> routeRegistrationDTOValidator)
        {
            _userRepository = userRepository;
            _routeRepository = routeRepository;
            _registrationRepository = registrationRepository;
            _routeRegistrationDTOValidator = routeRegistrationDTOValidator;
        }

        [HttpPost("")]
        public IActionResult Post(RouteRegistrationDTO registrationDTO)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            var validation = _routeRegistrationDTOValidator.Validate(registrationDTO);
            if (!validation.IsValid) return BadRequest(validation);


            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            var route = _routeRepository.GetBy(registrationDTO.RouteId);
            if (route == null) return BadRequest();

            //should happen in frontend 
            //validator checks if size is a part of an array! check validator!
            //size should not be filled in the case (OrderedShirt == false)
            if (!registrationDTO.OrderedShirt) registrationDTO.Size = "no shirt";

            var registration = registrationDTO.MapToRegistration(user, route);

            _registrationRepository.Add(registration, mailAdress);

            return Ok(registration);
        }

        //test method
        [HttpDelete("")]
        public IActionResult Delete(Guid id)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            var registration = _registrationRepository.GetBy(id, mailAdress);

            _registrationRepository.Delete(registration, user.Email);

            return Ok(registration);
        }
    }
}
