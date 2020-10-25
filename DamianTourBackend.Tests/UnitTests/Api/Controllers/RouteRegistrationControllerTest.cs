using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Api.Controllers
{
    public class RouteRegistrationControllerTest
    {
        private IUserRepository _userRepository;
        private IRouteRepository _routeRepository;
        private RouteRegistrationController _sut;

        public RouteRegistrationControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _routeRepository = Substitute.For<IRouteRepository>();
            _sut = new RouteRegistrationController(_userRepository, _routeRepository);
        }

        [Fact]
        public void PostRouteRegistration_RouteExistsUserLoggedIn_RouteRegistrationPosted()
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

            // Act 
            var meResult = _sut.Post(routeRegistrationDTO);

            // Assert 
            meResult.Should().BeOfType<OkResult>();
            _userRepository.Received().GetBy(user.Email);
            _routeRepository.Received().GetBy(route.Id);
            //user.Registrations.Contains(registration);
        }
    }
}
