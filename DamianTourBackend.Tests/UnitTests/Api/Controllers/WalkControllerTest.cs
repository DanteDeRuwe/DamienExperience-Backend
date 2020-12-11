using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Linq;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Api.Controllers
{
    public class WalkControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalkRepository _walkRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly WalkController _sut;
        private readonly IRouteRepository _routeRepository;

        public WalkControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _walkRepository = Substitute.For<IWalkRepository>();
            _registrationRepository = Substitute.For<IRegistrationRepository>();
            _routeRepository = Substitute.For<IRouteRepository>();
            _mailService = Substitute.For<IMailService>();
            _configuration = Substitute.For<IConfiguration>();

            _sut = new WalkController(_userRepository, _walkRepository, _registrationRepository, _routeRepository, _mailService, _configuration);
        }

        [Fact]
        public void SearchWalk_UserExistsAndHasWalk_ReturnsWalk()
        {
            // Arrange
            var walker = DummyData.UserFaker.Generate();
            var lastRegistration = walker.Registrations.Last();
            var route = DummyData.RouteFaker.Generate();
            route.Id = lastRegistration.RouteId;
            var walk = new Walk(DateTime.Now, route);

            _userRepository.GetBy(walker.Email).Returns(walker);
            _registrationRepository.GetLast(walker.Email).Returns(lastRegistration);
            _walkRepository.GetByUserAndRoute(walker.Id, lastRegistration.RouteId).Returns(walk);

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
            _walkRepository.DidNotReceiveWithAnyArgs().GetByUserAndRoute(default, default);

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
            _walkRepository.DidNotReceiveWithAnyArgs().GetByUserAndRoute(default, default);
        }

        [Fact]
        public void SearchWalk_WalkNotFound_ReturnsNotFound()
        {
            // Arrange
            var walker = DummyData.UserFaker.Generate();
            var lastRegistration = walker.Registrations.Last();

            _userRepository.GetBy(walker.Email).Returns(walker);
            _registrationRepository.GetLast(walker.Email).Returns(lastRegistration);
            _walkRepository.GetByUserAndRoute(walker.Id, lastRegistration.RouteId).ReturnsNull(); //!

            // Act
            var result = _sut.SearchWalk(walker.Email);

            // Assert
            result.Should().BeOfType<NotFoundResult>();

            _userRepository.Received().GetBy(walker.Email);
            _registrationRepository.Received().GetLast(walker.Email);
            _walkRepository.Received().GetByUserAndRoute(walker.Id, lastRegistration.RouteId);
        }


        [Fact]
        public void StartWalk_UserAndRegistrationAndRouteExists_ReturnsOk()
        {
            //Arrange
            var user = DummyData.UserFaker.Generate();
            var route = DummyData.RouteFaker.Generate();
            var reg = user.Registrations.Last();
            route.Id = reg.RouteId;
            _sut.ControllerContext = FakeControllerContext.For(user);
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetLast(user.Email).Returns(reg);
            _routeRepository.GetBy(route.Id).Returns(route);
            _walkRepository.GetByUserAndRoute(user.Id, route.Id).ReturnsNull();

            //Act
            var result = _sut.Start();

            //Assert
            result.Should().BeOfType<OkResult>();

            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.Received().GetLast(user.Email);
            _walkRepository.Received().GetByUserAndRoute(user.Id, route.Id);

        }

        [Fact]
        public void StartWalk_UserAndRegistrationAndRouteAndWalkExists_ReturnsOk()
        {
            //Arrange
            var user = DummyData.UserFaker.Generate();
            var route = DummyData.RouteFaker.Generate();
            var reg = user.Registrations.Last();
            route.Id = reg.RouteId;
            _sut.ControllerContext = FakeControllerContext.For(user);
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetLast(user.Email).Returns(reg);
            _routeRepository.GetBy(route.Id).Returns(route);
            _walkRepository.GetByUserAndRoute(user.Id, route.Id).Returns(new Walk());

            //Act
            var result = _sut.Start();

            //Assert
            result.Should().BeOfType<OkResult>();

            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.Received().GetLast(user.Email);
            _routeRepository.Received().GetBy(route.Id);
            _walkRepository.Received().GetByUserAndRoute(user.Id, route.Id);
        }

        [Fact]
        public void StartWalk_UserDoesNotExist_ReturnsOk()
        {
            //Arrange
            var user = DummyData.UserFaker.Generate();
            var route = DummyData.RouteFaker.Generate();
            var reg = user.Registrations.Last();
            route.Id = reg.RouteId;
            _sut.ControllerContext = FakeControllerContext.For(user);
            _userRepository.GetBy(user.Email).ReturnsNull();
            _registrationRepository.GetLast(user.Email).Returns(reg);
            _routeRepository.GetBy(route.Id).Returns(route);
            _walkRepository.GetByUserAndRoute(user.Id, route.Id).Returns(new Walk());

            //Act
            var result = _sut.Start();

            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();

            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.DidNotReceive().GetLast(user.Email);
            _routeRepository.DidNotReceive().GetBy(route.Id);
            _walkRepository.DidNotReceive().GetByUserAndRoute(user.Id, route.Id);
        }
        [Fact]
        public void StartWalk_ResitrationDoesNotExist_ReturnsOk()
        {
            //Arrange
            var user = DummyData.UserFaker.Generate();
            var route = DummyData.RouteFaker.Generate();
            var reg = user.Registrations.Last();
            route.Id = reg.RouteId;
            _sut.ControllerContext = FakeControllerContext.For(user);
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetLast(user.Email).ReturnsNull();
            _routeRepository.GetBy(route.Id).Returns(route);
            _walkRepository.GetByUserAndRoute(user.Id, route.Id).Returns(new Walk());

            //Act
            var result = _sut.Start();

            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();

            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.Received().GetLast(user.Email);
            _routeRepository.DidNotReceive().GetBy(route.Id);
            _walkRepository.DidNotReceive().GetByUserAndRoute(user.Id, route.Id);

        }

        [Fact]
        public void StartWalk_RouteDoesNotExist_ReturnsOk()
        {
            //Arrange
            var user = DummyData.UserFaker.Generate();
            var route = DummyData.RouteFaker.Generate();
            var reg = user.Registrations.Last();
            route.Id = reg.RouteId;
            _sut.ControllerContext = FakeControllerContext.For(user);
            _userRepository.GetBy(user.Email).Returns(user);
            _registrationRepository.GetLast(user.Email).Returns(reg);
            _routeRepository.GetBy(route.Id).ReturnsNull();
            _walkRepository.GetByUserAndRoute(user.Id, route.Id).Returns(new Walk());

            //Act
            var result = _sut.Start();

            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();

            _userRepository.Received().GetBy(user.Email);
            _registrationRepository.Received().GetLast(user.Email);
            _routeRepository.Received().GetBy(route.Id);
            _walkRepository.DidNotReceive().GetByUserAndRoute(user.Id, route.Id);

        }
    }
}