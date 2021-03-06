using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Data;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Data.Models;
using RX.Nyss.Data.Queries;
using RX.Nyss.Web.Features.Authentication.Dto;
using RX.Nyss.Web.Services;
using static RX.Nyss.Common.Utils.DataContract.Result;


namespace RX.Nyss.Web.Features.Authentication
{
    public interface IAuthenticationService
    {
        Task<Result> Login(LoginRequestDto dto);
        Task<Result> Logout();
        Task<Result<StatusResponseDto>> GetStatus(ClaimsPrincipal user);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly INyssContext _nyssContext;
        private readonly IUserIdentityService _userIdentityService;

        public AuthenticationService(
            IUserIdentityService userIdentityService,
            INyssContext nyssContext)
        {
            _userIdentityService = userIdentityService;
            _nyssContext = nyssContext;
        }

        public async Task<Result> Login(LoginRequestDto dto)
        {
            try
            {
                await _userIdentityService.Login(dto.UserName, dto.Password);
                return Success();
            }
            catch (ResultException exception)
            {
                return exception.GetResult();
            }
        }

        public async Task<Result<StatusResponseDto>> GetStatus(ClaimsPrincipal user) =>
            user.Identity.IsAuthenticated
                ? await GetAuthenticatedStatus(user)
                : GetAnonymousStatus();

        public async Task<Result> Logout()
        {
            await _userIdentityService.Logout();
            return Success();
        }

        private static Result<StatusResponseDto> GetAnonymousStatus() =>
            Success(new StatusResponseDto { IsAuthenticated = false });

        private async Task<Result<StatusResponseDto>> GetAuthenticatedStatus(ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Name);

            var userEntity = await _nyssContext.Users.FilterAvailable()
                .Include(x => x.ApplicationLanguage)
                .SingleOrDefaultAsync(u => u.EmailAddress == email);

            if (userEntity == null)
            {
                return Error<StatusResponseDto>(ResultKey.User.Common.UserNotFound);
            }

            var hasPendingHeadManagerConsents = await _nyssContext.NationalSocieties
                .Where(ns => ns.PendingHeadManager.IdentityUserId == userEntity.IdentityUserId).AnyAsync();

            var homePageData = await GetHomePageData(userEntity);
            return Success(new StatusResponseDto
            {
                IsAuthenticated = user.Identity.IsAuthenticated,
                UserData = user.Identity.IsAuthenticated
                    ? new StatusResponseDto.UserDataDto
                    {
                        Id = userEntity.Id,
                        Name = userEntity.Name,
                        Email = email,
                        LanguageCode = userEntity.ApplicationLanguage?.LanguageCode ?? "en",
                        Roles = user.FindAll(m => m.Type == ClaimTypes.Role).Select(x => x.Value).ToArray(),
                        HasPendingHeadManagerConsents = hasPendingHeadManagerConsents,
                        HomePage = user.Identity.IsAuthenticated
                            ? homePageData
                            : null
                    }
                    : null
            });
        }

        private async Task<StatusResponseDto.HomePageDto> GetHomePageData(User userEntity) =>
            userEntity switch
            {
                SupervisorUser user => await GetProjectHomePage(user),
                ManagerUser user => await GetNationalSocietyHomePage(user),
                TechnicalAdvisorUser user => await GetNationalSocietyHomePage(user),
                DataConsumerUser user => await GetNationalSocietyHomePage(user),
                GlobalCoordinatorUser user => GetRootHomePage(),
                AdministratorUser user => GetRootHomePage(),
                _ => GetRootHomePage()
            };

        private async Task<StatusResponseDto.HomePageDto> GetNationalSocietyHomePage<T>(T user) where T : User
        {
            var nationalSocietyIds = await _nyssContext.UserNationalSocieties
                .Where(uns => uns.UserId == user.Id)
                .Select(uns => uns.NationalSocietyId)
                .ToListAsync();

            if (nationalSocietyIds.Count == 0 || nationalSocietyIds.Count > 1)
            {
                return new StatusResponseDto.HomePageDto { Page = HomePageType.Root };
            }

            return new StatusResponseDto.HomePageDto
            {
                Page = HomePageType.ProjectList,
                NationalSocietyId = nationalSocietyIds.Single()
            };
        }

        private async Task<StatusResponseDto.HomePageDto> GetProjectHomePage(SupervisorUser user)
        {
            var supervisorActiveProject = await _nyssContext.SupervisorUserProjects
                .Where(sup => sup.SupervisorUserId == user.Id)
                .Where(sup => sup.Project.State == ProjectState.Open)
                .Select(sup => new
                {
                    projectId = sup.ProjectId,
                    nationalSocietyId = sup.Project.NationalSociety.Id
                })
                .SingleOrDefaultAsync();

            if (supervisorActiveProject != null)
            {
                return new StatusResponseDto.HomePageDto
                {
                    Page = HomePageType.Project,
                    ProjectId = supervisorActiveProject.projectId,
                    NationalSocietyId = supervisorActiveProject.nationalSocietyId
                };
            }

            return new StatusResponseDto.HomePageDto
            {
                Page = HomePageType.ProjectList,
                NationalSocietyId = await _nyssContext.UserNationalSocieties
                    .Where(uns => uns.UserId == user.Id)
                    .Select(uns => uns.NationalSocietyId)
                    .SingleAsync()
            };
        }

        private StatusResponseDto.HomePageDto GetRootHomePage() =>
            new StatusResponseDto.HomePageDto { Page = HomePageType.Root };
    }
}
