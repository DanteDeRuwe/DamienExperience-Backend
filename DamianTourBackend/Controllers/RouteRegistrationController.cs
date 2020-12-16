﻿using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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
        private readonly IMailService _mailService;

        public RouteRegistrationController(IUserRepository userRepository, IRouteRepository routeRepository,
            IRegistrationRepository registrationRepository,
            IMailService mailService, IValidator<RouteRegistrationDTO> routeRegistrationDTOValidator)
        {
            _userRepository = userRepository;
            _routeRepository = routeRepository;
            _registrationRepository = registrationRepository;
            _routeRegistrationDTOValidator = routeRegistrationDTOValidator;
            _mailService = mailService;
        }

        /// <summary>
        /// Creates a new RouteRegistration for the current user using RouteRegistrationDTO
        /// </summary>
        /// <param name="registrationDTO">RouteRegistrationDTO containing the id of the chosen route, and shirtsize</param>
        /// <returns>ok with the registration or Unauthorized if user isn't logged in, or BadRequest if user/registration isn't valid</returns>
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
            //if (!registrationDTO.OrderedShirt) registrationDTO.ShirtSize = "no shirt";

            var registration = registrationDTO.MapToRegistration(user, route);

            _registrationRepository.Add(registration, mailAdress);

            _mailService.SendRegistrationConfirmation(new RegistrationMailDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Tourname = route.TourName,
                Distance = (route.DistanceInMeters/1000).ToString(),
                Date = route.Date.ToString()
            });

            return Ok(registration);
        }

        /// <summary>
        /// Deletes a RouteRegistration with the given id
        /// </summary>
        /// <param name="id">Guid id of the registration to be deleted</param>
        /// <returns>Ok with registration that got deleted, or Unauthorized if user isn't logged in or BadRequest if user/registration is invalid</returns>
        [HttpDelete("")]
        public IActionResult Delete(Guid id)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string email = User.Identity.Name;
            if (email == null) return BadRequest();

            var user = _userRepository.GetBy(email);
            if (user == null) return BadRequest();

            var registration = _registrationRepository.GetBy(id, email);
            if (registration == null) return BadRequest();

            try
            {
                _registrationRepository.Delete(registration, email);
            }
            catch (Exception)
            {

                return BadRequest();
            }

            return Ok(registration);
        }
        
        /// <summary>
        /// Returns all RouteRegistrations from current user
        /// </summary>
        /// <returns>Ok with all Registrations from current user</returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            var all = _registrationRepository.GetAllFromUser(mailAdress);
            if (all == null || !all.Any()) return NotFound();

            return Ok(all);
        }

        /// <summary>
        /// Returns the last registration from the current user
        /// </summary>
        /// <returns>Ok with last registration</returns>
        [HttpGet("GetLast")]
        public IActionResult GetLast()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            var last = _registrationRepository.GetLast(mailAdress);
            if (last == null) return NotFound();

            return Ok(last);
        }

        /// <summary>
        /// Looks for registrations in the future
        /// </summary>
        /// <returns>Ok with boolean if user has future registrations</returns>
        [HttpGet("CheckCurrentRegistered")]
        public IActionResult CheckCurrentRegistered()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string mailAdress = User.Identity.Name;
            if (mailAdress == null) return BadRequest();

            var user = _userRepository.GetBy(mailAdress);
            if (user == null) return BadRequest();

            Registration reg = _registrationRepository.GetLast(mailAdress);
            if (reg == null) return Ok(false);

            Route route = _routeRepository.GetBy(reg.RouteId);
            if (route == null) return NotFound();

            return Ok(DateCheckHelper.CheckGreaterThenOrEqualsDate(route.Date));
        }
    }
}
