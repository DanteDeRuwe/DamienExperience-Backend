using System;
using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Application.UpdateWalk;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Api.Controllers
{
    public class WalkControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalkRepository _walkRepository;
        private readonly UserManager<AppUser> _um;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly WalkController _sut;
        private IRouteRepository _routeRepository;

        public WalkControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _walkRepository = Substitute.For<IWalkRepository>();
            _registrationRepository = Substitute.For<IRegistrationRepository>();
            _routeRepository = Substitute.For<IRouteRepository>();
            _um = Substitute.For<FakeUserManager>();

            _sut = new WalkController(_userRepository, _walkRepository, _um, _registrationRepository, _routeRepository);
        }

        [Fact]
        public void SearchWalk_UserExistsAndHasWalk_ReturnsWalk()
        {
            // Arrange
            var walker = DummyData.UserFaker.Generate();
            var route = DummyData.RouteFaker.Generate();
            var walk = new Walk(DateTime.Now, route);
            
            var routeRegistrationDTO = DummyData.RouteRegistrationDTOFaker.Generate();
            var registration = routeRegistrationDTO.MapToRegistration(walker, route);
            
            _userRepository.GetBy(walker.Email).Returns(walker);
            _registrationRepository.GetLast(walker.Email).Returns(registration);
            _walkRepository.GetByUserAndRoute(walker.Id, registration.RouteId).Returns(walk);

            // Act
            var result = _sut.SearchWalk(walker.Email);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(walk);
        }
        
        [Fact]
        public void SearchWalk_WalkerNotFound_ReturnsNotFound()
        {
            // Arrange
            var walker = DummyData.UserFaker.Generate();
            _userRepository.GetBy(walker.Email).ReturnsNull(); //!

            // Act
            var result = _sut.SearchWalk(walker.Email);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            
            _userRepository.Received().GetBy(walker.Email);
            _registrationRepository.DidNotReceive().GetLast(walker.Email);
            _walkRepository.DidNotReceiveWithAnyArgs().GetByUserAndRoute(default,default);

        }
        
        [Fact]
        public void SearchWalk_RegistrationNotFound_ReturnsNotFound()
        {
            // Arrange
            var walker = DummyData.UserFaker.Generate();
            
            _userRepository.GetBy(walker.Email).Returns(walker);
            _registrationRepository.GetLast(walker.Email).ReturnsNull(); //!

            // Act
            var result = _sut.SearchWalk(walker.Email);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            
            _userRepository.Received().GetBy(walker.Email);
            _registrationRepository.Received().GetLast(walker.Email);
            _walkRepository.DidNotReceiveWithAnyArgs().GetByUserAndRoute(default,default);
        }
        
        [Fact]
        public void SearchWalk_WalkNotFound_ReturnsNotFound()
        {
            // Arrange
            var walker = DummyData.UserFaker.Generate();
            var route = DummyData.RouteFaker.Generate();
            var walk = new Walk(DateTime.Now, route);
            
            var routeRegistrationDTO = DummyData.RouteRegistrationDTOFaker.Generate();
            var registration = routeRegistrationDTO.MapToRegistration(walker, route);
            
            _userRepository.GetBy(walker.Email).Returns(walker);
            _registrationRepository.GetLast(walker.Email).Returns(registration);
            _walkRepository.GetByUserAndRoute(walker.Id, registration.RouteId).ReturnsNull(); //!

            // Act
            var result = _sut.SearchWalk(walker.Email);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            
            _userRepository.Received().GetBy(walker.Email);
            _registrationRepository.Received().GetLast(walker.Email);
            _walkRepository.Received().GetByUserAndRoute(walker.Id, route.Id);
        }
    }
}