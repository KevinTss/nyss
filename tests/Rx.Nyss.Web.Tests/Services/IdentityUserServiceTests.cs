﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Common.Utils.Logging;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Web.Configuration;
using RX.Nyss.Web.Services;
using Xunit;

namespace RX.Nyss.Web.Tests.Services
{
    public class IdentityUserServiceTests
    {
        private readonly ILoggerAdapter _loggerAdapterMock;
        private readonly INyssWebConfig _configMock;
        private readonly IEmailPublisherService _emailPublisherServiceMock;
        private readonly IEmailTextGeneratorService _emailTextGeneratorServiceMock;
        private readonly INyssContext _nyssContextMock;

        public IdentityUserServiceTests()
        {
            _loggerAdapterMock = Substitute.For<ILoggerAdapter>();
            _configMock = new ConfigSingleton();
            _emailPublisherServiceMock = Substitute.For<IEmailPublisherService>();
            _nyssContextMock = Substitute.For<INyssContext>();
            _emailTextGeneratorServiceMock = Substitute.For<IEmailTextGeneratorService>();
        }

        [Fact]
        public async Task CreateIdentityUser_WhenEmptyUserList_ShouldReturnNewUser()
        {
            var userEmail = "emailTest1@domain.com";
            var existingUserList = new List<IdentityUser>();
            var identityUserService = GetIdentityUserRegistrationServiceWithMockedDependencies(existingUserList);

            var identityUser = await identityUserService.CreateIdentityUser(userEmail, Role.GlobalCoordinator);

            Assert.NotNull(identityUser);
        }

        [Fact]
        public async Task CreateIdentityUser_WhenUserAlreadyExists_ShouldThrowException()
        {
            var userEmail = "emailTest1@domain.com";
            var existingUserList = new List<IdentityUser>
            {
                new IdentityUser
                {
                    UserName = userEmail,
                    Email = userEmail
                }
            };
            var identityUserService = GetIdentityUserRegistrationServiceWithMockedDependencies(existingUserList);

            await Assert.ThrowsAsync<ResultException>(() => identityUserService.CreateIdentityUser(userEmail, Role.GlobalCoordinator));
        }

        private IdentityUserRegistrationService GetIdentityUserRegistrationServiceWithMockedDependencies(List<IdentityUser> users)
        {
            var userManager = MockUserManager(users);

            var userService = new IdentityUserRegistrationService(
                userManager, _loggerAdapterMock, _configMock, _emailPublisherServiceMock, _nyssContextMock, _emailTextGeneratorServiceMock);
            return userService;
        }

        public UserManager<IdentityUser> MockUserManager(List<IdentityUser> users)
        {
            var store = MockUserEmailStore(users);

            var manager = new UserManager<IdentityUser>(store, null, null, null, null, null, null, null, null);

            var userValidator = Substitute.For<IUserValidator<IdentityUser>>();
            userValidator.ValidateAsync(manager, Arg.Any<IdentityUser>()).Returns(IdentityResult.Success);
            manager.UserValidators.Add(userValidator);
            manager.PasswordValidators.Add(new PasswordValidator<IdentityUser>());
            manager.Logger = Substitute.For<ILogger>();

            return manager;
        }

        private IUserEmailStore<IdentityUser> MockUserEmailStore(List<IdentityUser> users)
        {
            var store = Substitute.For<IUserEmailStore<IdentityUser>, IUserRoleStore<IdentityUser>>();

            store.FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(ci => users.FirstOrDefault(u => u.Email == ci.Arg<string>()));

            store.CreateAsync(Arg.Any<IdentityUser>(), Arg.Any<CancellationToken>()).Returns(IdentityResult.Success);
            store.When(s => s.CreateAsync(Arg.Any<IdentityUser>(), Arg.Any<CancellationToken>()))
                .Do(ci => { users.Add(ci.Arg<IdentityUser>()); });

            ((IUserRoleStore<IdentityUser>)store).UpdateAsync(Arg.Any<IdentityUser>(), Arg.Any<CancellationToken>())
                .Returns(IdentityResult.Success);

            return store;
        }
    }
}
