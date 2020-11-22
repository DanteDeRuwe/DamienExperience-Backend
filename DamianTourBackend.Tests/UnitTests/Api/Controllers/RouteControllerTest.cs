using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateRoute;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Api.Controllers
{
    public class RouteControllerTest
    {
        private readonly IRouteRepository _routeRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RouteController _routeController;

        public RouteControllerTest()
        {
            _routeRepository = Substitute.For<IRouteRepository>();
            _userManager = Substitute.For<FakeUserManager>();
            _routeController = new RouteController(_routeRepository, _userManager);
        }

        [Fact]
        public async Task GetRouteByName_RouteExists_ReceivesRoute()
        {
            // Arrange
            var route = DummyData.RouteFaker.Generate();
            _routeRepository.GetByName(route.TourName).Returns(route);

            // Act
            var result = _routeController.GetRouteByName(route.TourName);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

        }

        [Fact]
        public async Task GetRouteByName_RouteNotFound_ReturnsBadRequest()
        {
            // Arrange
            var route = DummyData.RouteFaker.Generate();
            _routeRepository.GetByName(route.TourName).ReturnsNull();

            // Act
            var result = _routeController.GetRouteByName(route.TourName);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        /*
        [Fact]
        public async Task Add_UserIsAdmin_RouteAdded()
        {
            // Arrange
            var user = DummyData.UserFaker.Generate();
            var routeDTO = DummyData.RouteFaker.Generate().MapToRouteDTO();
            
            //_userManager.FindByNameAsync(user.Email).Returns(new AppUser() { Claims = "admin"});

            // Act
            var result = _routeController.Add(routeDTO);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
        */
        
    }
}
