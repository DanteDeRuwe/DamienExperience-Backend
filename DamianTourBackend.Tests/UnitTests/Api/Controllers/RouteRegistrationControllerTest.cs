using System;
using System.Collections.Generic;
using System.Linq;
using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace DamianTourBackend.Tests.UnitTests.Api.Controllers
{
    public class RouteRegistrationControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IMailService _mailService;
        private readonly IValidator<RouteRegistrationDTO> _validator;
        private readonly RouteRegistrationController _sut;

        public RouteRegistrationControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _routeRepository = Substitute.For<IRouteRepository>();
            _registrationRepository = Substitute.For<IRegistrationRepository>();
            _validator = Substitute.For<IValidator<RouteRegistrationDTO>>();
            _mailService = Substitute.For<IMailService>();
            _sut = new RouteRegistrationController(_userRepository, _routeRepository, _registrationRepository, _mailService, _validator);
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

            
            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.Received().Delete(registration, user.Email);
        }

        [Fact]
        public void Delete_UserNotLoggedIn_ReturnsUnauthorized()
        {
            // Arrange 
            _sut.ControllerContext = FakeControllerContext.NotLoggedIn; //!

            // Act 
            var result = _sut.Delete(Guid.NewGuid()); //guid shouldn't matter

            // Assert 
            result.Should().BeOfType<UnauthorizedResult>();
        }


        [Fact]
        public void Delete_NoSuchRegistration_ReturnsBadRequest()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            var registrationId = Guid.NewGuid();
            
            _sut.ControllerContext = FakeControllerContext.For(user); 
            
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetBy(Arg.Any<Guid>(), user.Email).ReturnsNull();

            // Act 
            var numberOfRegistrations = user.Registrations.Count;
            var result = _sut.Delete(registrationId);

            // Assert 
            result.Should().BeOfType<BadRequestResult>();
            
            user.Registrations.Count.Should().Be(numberOfRegistrations);
            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.Received().GetBy(registrationId, user.Email);
            _registrationRepository.DidNotReceive().Delete(Arg.Any<Registration>(), user.Email);
        }

        
        [Fact]
        public void GetAll_AtLeastOneRegistration_ReturnsAllRegistrations()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();

            _sut.ControllerContext = FakeControllerContext.For(user); 
            
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetAllFromUser(user.Email).Returns(user.Registrations);
            
            //Act
            var result = _sut.GetAll();
            
            // Assert 
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(
                    user.Registrations,
                    options => options.Using(new EnumAsStringAssertionRule()) //treat enums as strings
                );
        }

        [Fact]
        public void GetAll_NoRegistrations_ReturnsNotFound()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            user.Registrations = new List<Registration>(); //empty
            
            _sut.ControllerContext = FakeControllerContext.For(user); 
            
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetLast(user.Email).ReturnsNull();
            
            //Act
            var result = _sut.GetAll();
            
            // Assert 
            result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public void GetAll_UserNotLoggedIn_ReturnsUnauthorized()
        {
            // Arrange 
            _sut.ControllerContext = FakeControllerContext.NotLoggedIn; //!

            // Act 
            var result = _sut.GetAll();

            // Assert 
            result.Should().BeOfType<UnauthorizedResult>();
        }
        
        [Fact]
        public void GetLast_AtLeastOneRegistration_ReturnsLastRegistration()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            var lastRegistration = user.Registrations.Last();
            
            _sut.ControllerContext = FakeControllerContext.For(user); 
            
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetLast(user.Email).Returns(lastRegistration);
            
            //Act
            var result = _sut.GetLast();
            
            // Assert 
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(
                    lastRegistration,
                    options => options.Using(new EnumAsStringAssertionRule()) //treat enums as strings
                );
        }

        [Fact]
        public void GetLast_NoRegistrations_ReturnsNotFound()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            user.Registrations = new List<Registration>(); //empty
            
            _sut.ControllerContext = FakeControllerContext.For(user); 
            
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetLast(user.Email).ReturnsNull();
            
            //Act
            var result = _sut.GetLast();
            
            // Assert 
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void CheckCurrentRegistered_IsRegisteredForFutureRoute_ReturnsTrue()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            var registration = user.Registrations.Last();
            var route = new Route {Date = DateTime.Today.AddDays(1), Id = registration.RouteId};

            _sut.ControllerContext = FakeControllerContext.For(user);
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetLast(user.Email).Returns(registration);
            _routeRepository.GetBy(registration.RouteId).Returns(route);

            // Act 
            var result = _sut.CheckCurrentRegistered();

            // Assert 
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(true);

            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.Received().GetLast(user.Email);
            _routeRepository.Received().GetBy(registration.RouteId);
        }
        
        [Fact]
        public void CheckCurrentRegistered_LastRouteInPast_ReturnsFalse()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            var registration = user.Registrations.Last();
            var route = new Route {Date = DateTime.Today.AddDays(-1), Id = registration.RouteId};

            _sut.ControllerContext = FakeControllerContext.For(user);
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetLast(user.Email).Returns(registration);
            _routeRepository.GetBy(registration.RouteId).Returns(route);

            // Act 
            var result = _sut.CheckCurrentRegistered();

            // Assert 
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(false);

            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.Received().GetLast(user.Email);
            _routeRepository.Received().GetBy(registration.RouteId);
        }

        [Fact]
        public void CheckCurrentRegistered_UserNotLoggedIn_ReturnsUnauthorized()
        {
            // Arrange 
            _sut.ControllerContext = FakeControllerContext.NotLoggedIn; //!

            // Act 
            var result = _sut.CheckCurrentRegistered();

            // Assert 
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}