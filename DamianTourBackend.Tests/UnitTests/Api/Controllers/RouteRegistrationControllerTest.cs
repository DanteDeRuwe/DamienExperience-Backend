using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Api.Controllers
{
    public class RouteRegistrationControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IValidator<RouteRegistrationDTO> _routeRegistrationValidator;
        private readonly RouteRegistrationController _sut;

        public RouteRegistrationControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _routeRepository = Substitute.For<IRouteRepository>();
            _registrationRepository = Substitute.For<IRegistrationRepository>();
            _routeRegistrationValidator = Substitute.For<IValidator<RouteRegistrationDTO>>();
            _sut = new RouteRegistrationController(_userRepository, _routeRepository, _registrationRepository, _routeRegistrationValidator);
        }

        [Fact]
        public void PostRouteRegistration_RouteExistsUserLoggedInDTOGood_RouteRegistrationPosted()
        {
            // Arrange 
            var route = DummyData.RouteFaker.Generate();
            var user = DummyData.UserFaker.Generate();
            var routeRegistrationDTO = DummyData.RouteRegistrationDTOFaker.Generate();
            routeRegistrationDTO.OrderedShirt = true;
            routeRegistrationDTO.RouteId = route.Id;
            _sut.ControllerContext = FakeControllerContext.For(user);
            _userRepository.GetBy(user.Email).Returns(user);
            _routeRepository.GetBy(route.Id).Returns(route);
            _routeRegistrationValidator.SetupPass();

            // Act 
            var meResult = _sut.Post(routeRegistrationDTO);

            // Assert 
            meResult.Should().BeOfType<OkResult>();
            _userRepository.Received().GetBy(user.Email);
            _routeRepository.Received().GetBy(route.Id);
            //registration is added to user 
            user.Registrations.Count.Should().Be(1);
        }
    }
}
