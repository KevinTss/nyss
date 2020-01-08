﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Web.Features.User.Dto;
using RX.Nyss.Web.Services.Authorization;
using RX.Nyss.Web.Utils.DataContract;
using static RX.Nyss.Web.Utils.DataContract.Result;

namespace RX.Nyss.Web.Features.User
{
    public interface IUserService
    {
        Task<Result<List<GetNationalSocietyUsersResponseDto>>> GetUsers(int nationalSocietyId);
        Task<Result<NationalSocietyUsersBasicDataResponseDto>> GetBasicData(int nationalSocietyUserId);
        Task<Result> AddExisting(int nationalSocietyId, string userEmail);
        Task<string> GetUserApplicationLanguageCode(string userIdentityName);
    }

    public class UserService : IUserService
    {
        private readonly INyssContext _dataContext;
        private readonly IAuthorizationService _authorizationService;

        public UserService(INyssContext dataContext, IAuthorizationService authorizationService)
        {
            _dataContext = dataContext;
            _authorizationService = authorizationService;
        }

        public async Task<Result<List<GetNationalSocietyUsersResponseDto>>> GetUsers(int nationalSocietyId)
        {
            var usersQuery = _authorizationService.IsCurrentUserInRole(Role.GlobalCoordinator)
                ? _dataContext.UserNationalSocieties.Where(u => u.User.Role != Role.Supervisor)
                : _dataContext.UserNationalSocieties;

            var users = await usersQuery
                .Where(uns => uns.NationalSocietyId == nationalSocietyId)
                .Select(uns => new GetNationalSocietyUsersResponseDto
                {
                    Id = uns.User.Id,
                    Name = uns.User.Name,
                    Email = uns.User.EmailAddress,
                    PhoneNumber = uns.User.PhoneNumber,
                    Role = uns.User.Role.ToString(),
                    Project = (uns.User is SupervisorUser)
                        ? ((SupervisorUser)uns.User).SupervisorUserProjects
                        .Where(sup => sup.Project.State == ProjectState.Open)
                        .Select(sup => sup.Project.Name)
                        .SingleOrDefault()
                        : null,
                    IsHeadManager = uns.NationalSociety.HeadManager != null && uns.NationalSociety.HeadManager.Id == uns.User.Id,
                    IsPendingHeadManager = uns.NationalSociety.PendingHeadManager != null && uns.NationalSociety.PendingHeadManager.Id == uns.User.Id
                })
                .ToListAsync();

            return new Result<List<GetNationalSocietyUsersResponseDto>>(users, true);
        }

        public async Task<Result<NationalSocietyUsersBasicDataResponseDto>> GetBasicData(int nationalSocietyUserId)
        {
            var user = await _dataContext.Users
                .Where(u => u.Id == nationalSocietyUserId)
                .Select(u => new NationalSocietyUsersBasicDataResponseDto { Email = u.EmailAddress, Role = u.Role })
                .SingleOrDefaultAsync();

            return Success(user);
        }

        public async Task<Result> AddExisting(int nationalSocietyId, string userEmail)
        {
            var userData = await _dataContext.Users
                .Where(u => u.EmailAddress == userEmail)
                .Select(u => new { u.Id, u.Role })
                .SingleOrDefaultAsync();

            if (userData == null)
            {
                return Error(ResultKey.User.Registration.UserNotFound);
            }

            if (userData.Role != Role.TechnicalAdvisor && userData.Role != Role.DataConsumer)
            {
                return Error(ResultKey.User.Registration.NoAssignableUserWithThisEmailFound);
            }

            var userAlreadyIsInThisNationalSociety = await _dataContext.UserNationalSocieties
                .AnyAsync(uns => uns.NationalSocietyId == nationalSocietyId && uns.UserId == userData.Id);

            if (userAlreadyIsInThisNationalSociety)
            {
                return Error(ResultKey.User.Registration.UserIsAlreadyInThisNationalSociety);
            }

            var userNationalSociety = new UserNationalSociety { NationalSocietyId = nationalSocietyId, UserId = userData.Id };
            await _dataContext.UserNationalSocieties.AddAsync(userNationalSociety);
            await _dataContext.SaveChangesAsync();
            return Success();
        }

        public async Task<string> GetUserApplicationLanguageCode(string userIdentityName) =>
            await _dataContext.Users
                .Where(u => u.EmailAddress == userIdentityName)
                .Select(u => u.ApplicationLanguage.LanguageCode)
                .SingleAsync();
    }
}
