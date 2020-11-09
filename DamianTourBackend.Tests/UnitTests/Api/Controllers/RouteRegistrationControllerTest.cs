using System.Linq;
using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Api.Controllers
{
    public class RouteRegistrationControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IValidator<RouteRegistrationDTO> _validator;
        private readonly RouteRegistrationController _sut;

        public RouteRegistrationControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _routeRepository = Substitute.For<IRouteRepository>();
            _registrationRepository = Substitute.For<IRegistrationRepository>();
            _validator = Substitute.For<IValidator<RouteRegistrationDTO>>();
            _sut = new RouteRegistrationController(_userRepository, _routeRepository, _registrationRepository, _validator);
        }

        [Fact]
        public void Post_LoggedInUserWithGoodRoute_ShouldRegisterAndReturnsOk()
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
            _validator.SetupPass();

            // Act 
            var numberOfRegistrations = user.Registrations.Count;
            var result = _sut.Post(routeRegistrationDTO);

            // Assert 
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(
                    routeRegistrationDTO,
                    options => options.Using(new EnumAsStringAssertionRule()) //treat enums as strings
                );

            _userRepository.Received().GetBy(user.Email);
            _routeRepository.Received().GetBy(route.Id);
            //registration is added to user 
            user.Registrations.Count.Should().Be(numberOfRegistrations+1);
        }
        
        [Fact]
        public void Post_ValidationFailed_ReturnsBadRequest()
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
            
            _validator.SetupFail();

            // Act 
            var numberOfRegistrations = user.Registrations.Count;
            var result = _sut.Post(routeRegistrationDTO);

            // Assert 
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ValidationResult>()
                .Which.IsValid.Should().BeFalse();

            _userRepository.DidNotReceive().GetBy(user.Email);
            user.Registrations.Count.Should().Be(numberOfRegistrations);
        }
        
        [Fact]
        public void Post_UserNotLoggedIn_ReturnsUnauthorized()
        {
            // Arrange 
            var routeRegistrationDTO = DummyData.RouteRegistrationDTOFaker.Generate();
            _sut.ControllerContext = FakeControllerContext.NotLoggedIn; //!

            // Act 
            var result = _sut.Post(routeRegistrationDTO);

            // Assert 
            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        
        [Fact]
        public void Post_RouteNotFound_ReturnsBadRequest()
        {
            // Arrange 
            var route = DummyData.RouteFaker.Generate();
            var user = DummyData.UserFaker.Generate();
            var routeRegistrationDTO = DummyData.RouteRegistrationDTOFaker.Generate();
            routeRegistrationDTO.OrderedShirt = true;
            routeRegistrationDTO.RouteId = route.Id;
            _sut.ControllerContext = FakeControllerContext.For(user); 
            _userRepository.GetBy(user.Email).Returns(user);
            _routeRepository.GetBy(route.Id).ReturnsNull(); //!
            _validator.SetupPass();

            // Act 
            var numberOfRegistrations = user.Registrations.Count;
            var result = _sut.Post(routeRegistrationDTO);

            // Assert 
            result.Should().BeOfType<BadRequestResult>();
            
            _userRepository.Received().GetBy(user.Email);
            _routeRepository.Received().GetBy(route.Id);

            user.Registrations.Count.Should().Be(numberOfRegistrations);
        }
        
        [Fact]
        public void Delete_LoggedInUserWithGoodRegistration_ShouldDeleteRegistrationAndReturnsOk()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            var registration = user.Registrations.Last();
            
            _sut.ControllerContext = FakeControllerContext.For(user); 
            
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetBy(registration.Id, user.Email).Returns(registration);

            // Act 
            var numberOfRegistrations = user.Registrations.Count;
            var result = _sut.Delete(registration.Id);

            // Assert 
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(
                    registration,
                    options => options.Using(new EnumAsStringAssertionRule()) //treat enums as strings
                );

            user.Registrations.Count.Should().Be(numberOfRegistrations-1);
            user.Registrations.Should().NotContain(registration);
            
            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.Received().Delete(registration, user.Email);
        }
    }
}