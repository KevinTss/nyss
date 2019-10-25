﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MockQueryable.NSubstitute;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RX.Nyss.Data;
using RX.Nyss.Data.Models;
using RX.Nyss.Web.Features.Authentication;
using RX.Nyss.Web.Features.Authentication.Dto;
using RX.Nyss.Web.Services;
using RX.Nyss.Web.Utils.DataContract;
using Shouldly;
using Xunit;

namespace Rx.Nyss.Web.Tests.Features.Authentication
{
    public class AuthenticationServiceTests
    {
        private const string UserName = "user";
        private const string UserEmail = "user@user.com";
        private const string Password = "password";
        private readonly IUserIdentityService _userIdentityService;
        private readonly INyssContext _nyssContext;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationServiceTests()
        {
            _userIdentityService = Substitute.For<IUserIdentityService>();
            _nyssContext = Substitute.For<INyssContext>();
            _authenticationService = new AuthenticationService(_userIdentityService, _nyssContext);
        }

        [Fact]
        public async Task Login_WhenUserIdentityServiceThrowsResultException_ReturnsResult()
        {
            var resultException = new ResultException("key");
            _userIdentityService.Login(UserName, Password).Throws(resultException);

            var result = await _authenticationService.Login(new LoginRequestDto
            {
                UserName = UserName,
                Password = Password
            });

            result.IsSuccess.ShouldBeFalse();
            result.Message.Key.ShouldBe(resultException.Result.Message.Key);
        }

        [Fact]
        public async Task Login_WhenSuccessful_ReturnsToken()
        {
            var nyssUsers = new List<User>();
            var usersDbSet = nyssUsers.AsQueryable().BuildMockDbSet();
            _nyssContext.Users.Returns(usersDbSet);

            var user = new IdentityUser { UserName = UserName };
            var roles = new List<string> { "Admin" };
            var additionalClaims = new List<Claim>();
            const string expectedToken = "token1";

            _userIdentityService.Login(UserName, Password).Returns(user);
            _userIdentityService.GetRoles(user).Returns(roles);
            _userIdentityService.CreateToken(UserName, roles, Arg.Any<IEnumerable<Claim>>()).Returns(expectedToken);

            var result = await _authenticationService.Login(new LoginRequestDto
            {
                UserName = UserName,
                Password = Password
            });

            result.IsSuccess.ShouldBeTrue();
            result.Value.AccessToken.ShouldBe(expectedToken);
        }

        [Fact]
        public async Task Logout_CallsLogoutOnIdentityService()
        {
            var result = await _authenticationService.Logout();

            await _userIdentityService.Received(1).Logout();
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public void GetStatus_CallsLogoutOnIdentityService()
        {
            const string role = "Administrator";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.Email, UserEmail),
                new Claim(ClaimTypes.Role, role)
            }, "JWT"));

            var result = _authenticationService.GetStatus(user);

            result.IsAuthenticated.ShouldBe(true);
            result.Data.Email.ShouldBe(UserEmail);
            result.Data.Name.ShouldBe(UserName);
            result.Data.Roles[0].ShouldBe(role);
        }
    }
}