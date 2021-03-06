﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockQueryable.NSubstitute;
using NSubstitute;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Common.Utils.Logging;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Web.Services;
using Shouldly;
using Xunit;

namespace RX.Nyss.Web.Tests.Services
{
    public class NationalSocietyUserServiceTests
    {
        private readonly INyssContext _nyssContext;
        private readonly ILoggerAdapter _loggerAdapter;
        private readonly IIdentityUserRegistrationService _identityUserRegistrationService;
        private readonly INationalSocietyUserService _nationalSocietyUserService;
        private readonly IDeleteUserService _deleteUserService;

        public NationalSocietyUserServiceTests()
        {
            _loggerAdapter = Substitute.For<ILoggerAdapter>();
            _identityUserRegistrationService = Substitute.For<IIdentityUserRegistrationService>();
            _nyssContext = Substitute.For<INyssContext>();
            _deleteUserService = Substitute.For<IDeleteUserService>();
            SetupTestNationalSociety();

            _nationalSocietyUserService = new NationalSocietyUserService(_nyssContext, _loggerAdapter, _identityUserRegistrationService, _deleteUserService);
        }

        private void SetupTestNationalSociety()
        {
            var nationalSociety = new NationalSociety
            {
                Id = 1,
                Name = "Test national society"
            };
            var nationalSocieties = new List<NationalSociety> { nationalSociety };
            var nationalSocietiesDbSet = nationalSocieties.AsQueryable().BuildMockDbSet();
            _nyssContext.NationalSocieties.Returns(nationalSocietiesDbSet);

            _nyssContext.NationalSocieties.FindAsync(1).Returns(nationalSociety);
        }

        private void ArrangeUsersFrom(IEnumerable<User> existingUsers)
        {
            var usersDbSet = existingUsers.AsQueryable().BuildMockDbSet();
            _nyssContext.Users.Returns(usersDbSet);
        }

        private void ArrangeUserNationalSocietiesFrom(IEnumerable<UserNationalSociety> userNationalSocieties)
        {
            var userNationalSocietyDbSet = userNationalSocieties.AsQueryable().BuildMockDbSet();
            _nyssContext.UserNationalSocieties.Returns(userNationalSocietyDbSet);
        }


        [Fact]
        public async Task DeleteNationalSocietyUser_WhenSuccessful_NyssContextSaveChangesShouldBeCalledOnce()
        {
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            ArrangeUserNationalSocietiesFrom(new List<UserNationalSociety> { userNationalSociety });


            await _nationalSocietyUserService.DeleteUser<ManagerUser>(123, new List<string> { Role.Administrator.ToString() });


            await _nyssContext.Received().SaveChangesAsync();
        }


        [Fact]
        public async Task DeleteNationalSocietyUser_WhenSuccessful_NyssContextRemoveUserShouldBeCalledOnce()
        {
            //arrange
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            var usersNationalSocieties = new List<UserNationalSociety> { userNationalSociety };
            ArrangeUserNationalSocietiesFrom(usersNationalSocieties);

            manager.UserNationalSocieties = usersNationalSocieties;

            //act
            await _nationalSocietyUserService.DeleteUser<ManagerUser>(123, new List<string> { Role.Administrator.ToString() });

            //assert
            _nyssContext.Users.Received().Remove(manager);
        }

        [Fact]
        public async Task DeleteNationalSocietyUser_WhenSuccessful_NyssContextRemoveUserNationalSocietiesShouldBeCalledOnce()
        {
            //arrange
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            var usersNationalSocieties = new List<UserNationalSociety> { userNationalSociety };
            ArrangeUserNationalSocietiesFrom(usersNationalSocieties);

            manager.UserNationalSocieties = usersNationalSocieties;

            //act
            await _nationalSocietyUserService.DeleteUser<ManagerUser>(123, new List<string> { Role.Administrator.ToString() });

            //assert
            _nyssContext.UserNationalSocieties.Received().RemoveRange(Arg.Is<IEnumerable<UserNationalSociety>>(x => x.Contains(userNationalSociety)));
        }

        [Fact]
        public async Task DeleteNationalSocietyUser_WhenSuccessful_IdentityUserDeleteShouldBeCalledOnce()
        {
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            ArrangeUserNationalSocietiesFrom(new List<UserNationalSociety> { userNationalSociety });


            await _nationalSocietyUserService.DeleteUser<ManagerUser>(123, new List<string> { Role.Administrator.ToString() });


            await _identityUserRegistrationService.Received().DeleteIdentityUser(Arg.Any<string>());
        }

        [Fact]
        public async Task DeleteNationalSocietyUser_WhenUserExistsAndIsOfRequestedType_ShouldReturnSuccess()
        {
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            ArrangeUserNationalSocietiesFrom(new List<UserNationalSociety> { userNationalSociety });


            var result = await _nationalSocietyUserService.DeleteUser<ManagerUser>(123, new List<string> { Role.Administrator.ToString() });


            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task DeleteNationalSocietyUser_WhenUserExistsAndIsNotOfRequestedType_ShouldReturnError()
        {
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            ArrangeUserNationalSocietiesFrom(new List<UserNationalSociety> { userNationalSociety });


            var result = await _nationalSocietyUserService.DeleteUser<TechnicalAdvisorUser>(123, new List<string> { Role.Administrator.ToString() });


            result.IsSuccess.ShouldBeFalse();
        }

        [Fact]
        public async Task DeleteNationalSocietyUser_WhenUserDoesntExist_ShouldReturnError()
        {
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            ArrangeUserNationalSocietiesFrom(new List<UserNationalSociety> { userNationalSociety });


            var result = await _nationalSocietyUserService.DeleteUser<TechnicalAdvisorUser>(999, new List<string> { Role.Administrator.ToString() });


            result.IsSuccess.ShouldBeFalse();
        }

        [Fact]
        public async Task GetNationalSocietyUser_WhenUserExistsAndIsOfRequestedType_ShouldReturnUser()
        {
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            ArrangeUserNationalSocietiesFrom(new List<UserNationalSociety> { userNationalSociety });


            var user = await _nationalSocietyUserService.GetNationalSocietyUser<ManagerUser>(123);


            user.ShouldBe(manager);
        }

        [Fact]
        public async Task GetNationalSocietyUser_WhenUserDoesntExists_ShouldThrowException()
        {
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            ArrangeUserNationalSocietiesFrom(new List<UserNationalSociety> { userNationalSociety });


            await _nationalSocietyUserService.GetNationalSocietyUser<ManagerUser>(999).ShouldThrowAsync<ResultException>();
        }

        [Fact]
        public async Task GetNationalSocietyUser_WhenUserExistsButIsOfOtherType_ShouldThrowException()
        {
            var manager = new ManagerUser
            {
                Id = 123,
                Role = Role.Manager
            };
            ArrangeUsersFrom(new List<User> { manager });

            var userNationalSociety = new UserNationalSociety
            {
                User = manager,
                UserId = manager.Id,
                NationalSocietyId = 1
            };
            ArrangeUserNationalSocietiesFrom(new List<UserNationalSociety> { userNationalSociety });

            await _nationalSocietyUserService.GetNationalSocietyUser<TechnicalAdvisorUser>(123).ShouldThrowAsync<ResultException>();
        }
    }
}
