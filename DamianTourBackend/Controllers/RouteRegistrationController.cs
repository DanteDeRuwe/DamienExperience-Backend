using DamianTourBackend.Api.Helpers;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
        private readonly IConfiguration _config;

        public RouteRegistrationController(IUserRepository userRepository, IRouteRepository routeRepository,
            IRegistrationRepository registrationRepository,
            IMailService mailService, IValidator<RouteRegistrationDTO> routeRegistrationDTOValidator, IConfiguration config)
        {
            _userRepository = userRepository;
            _routeRepository = routeRepository;
            _registrationRepository = registrationRepository;
            _routeRegistrationDTOValidator = routeRegistrationDTOValidator;
            _mailService = mailService;
            _config = config;
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
            //if (!registrationDTO.OrderedShirt) registrationDTO.ShirtSize = "no shirt";

            var registration = registrationDTO.MapToRegistration(user, route);

            _registrationRepository.Add(registration, mailAdress);

            _mailService.SendRegistrationConfirmation(new RegistrationMailDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Tourname = route.TourName,
                Distance = (route.DistanceInMeters / 1000).ToString(),
                Date = route.Date.ToString()
            });

            return Ok(registration);
        }

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

        [HttpPost("RegistrationIsPaid")]
        public IActionResult RegistrationIsPaid(string registrationId, string email)
        {
            var id = Guid.Parse(registrationId);

            var user = _userRepository.GetBy(email);
            if (user == null) return BadRequest();

            var registration = _registrationRepository.GetBy(id, email);
            if (registration == null) return BadRequest();

            registration.Paid = true;
            _registrationRepository.Update(registration, email);

            return Ok();
        }

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

        [HttpGet("GeneratePaymentData")]
        public IActionResult GeneratePaymentData(string language)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            string email = User.Identity.Name;
            var user = _userRepository.GetBy(email);
            if (user == null) return BadRequest();

            var registration = _registrationRepository.GetLast(email);

            if (registration == null) return BadRequest();
            if (registration.Paid) return BadRequest();


            var route = _routeRepository.GetBy(registration.RouteId);
            string amount = registration.OrderedShirt ? "6500" : "5000";
            string shasign = CalculateShaSign(amount, "EUR", email, language, registration.Id.ToString(), "damiaanacties", user.Id.ToString());

            RegistrationPaymentDTO registrationPaymentDTO = new RegistrationPaymentDTO()
            {
                Amount = amount,
                Currency = "EUR",
                Email = email,
                Language = language,
                OrderId = registration.Id.ToString(),
                PspId = "damiaanactie",
                UserId = user.Id.ToString(),
                ShaSign = shasign,
                RouteName = route.TourName
            };
            return Ok(registrationPaymentDTO);
        }



        public string CalculateShaSign(string amount, string currency, string email, string language, string orderid, string pspid, string userid)
        {
            string hash;
            string key = _config["Payment:Key"];
            string input =
                "AMOUNT=" + amount + key +
                "CURRENCY=" + currency + key +
                "EMAIL=" + email + key +
                "LANGUAGE=" + language + key +
                "ORDERID=" + orderid + key +
                "PSPID=" + pspid + key +
                "USERID=" + userid + key;
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }
            return hash;
        }
    }
}
