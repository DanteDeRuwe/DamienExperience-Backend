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
            _validator.SetupPass();

            // Act 
            var result = _sut.Post(routeRegistrationDTO);

            // Assert 
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(routeRegistrationDTO); //the returned registration should at least have same fields as the DTO

            _userRepository.Received().GetBy(user.Email);
            _routeRepository.Received().GetBy(route.Id);
            //registration is added to user 
            user.Registrations.Count.Should().Be(1);
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
            var result = _sut.Post(routeRegistrationDTO);

            // Assert 
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ValidationResult>()
                .Which.IsValid.Should().BeFalse();

            _userRepository.DidNotReceive().GetBy(user.Email);
            user.Registrations.Count.Should().Be(0);
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
            var result = _sut.Post(routeRegistrationDTO);

            // Assert 
            result.Should().BeOfType<BadRequestResult>();
            
            _userRepository.Received().GetBy(user.Email);
            _routeRepository.Received().GetBy(route.Id);

            user.Registrations.Count.Should().Be(0);
        }
    }
}