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
        TODO: Fix or delete comment
        
        [Fact]
        public async Task Add_UserIsAdmin_RouteAdded()
        {
            // Arrange
            var user = DummyData.UserFaker.Generate();
            var routeDTO = DummyData.RouteFaker.Generate().MapToRouteDTO();

            // Werkt nie......
            var appUser = new AppUser() { Claims = new List<IdentityUserClaim<string>>() };
            appUser.Claims.Add(new IdentityUserClaim<string>() { ClaimType = "admin"});
            _userManager.FindByNameAsync(user.Email).Returns(appUser);

            // Act
            var result = _routeController.Add(routeDTO);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
        */
        /*
        [Fact]
        public async Task Add_UserIsNotAdmin_Fails()
        {
            // Arrange
            var user = DummyData.UserFaker.Generate();
            var routeDTO = DummyData.RouteFaker.Generate().MapToRouteDTO();

            // Werkt nie................
            var appUser = new AppUser();
            _userManager.FindByNameAsync(user.Email).Returns(appUser);

            // Act
            var result = _routeController.Add(routeDTO);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }
        */
        [Fact]
        private async Task GetFutureRoutes_Works()
        {
            // Arrange
            var route = DummyData.RouteFaker.Generate();
            var pastRoute = DummyData.PastRouteFaker.Generate();
            var routes = new List<Route>();
            routes.Add(route);
            routes.Add(pastRoute);
            _routeRepository.GetAll().Returns(routes);

            // Act
            var result = _routeController.GetAll();

            // Assert
            routes.Remove(pastRoute);

            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeSameAs(routes);

        }
    }
}