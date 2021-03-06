using FluentValidation;

namespace RX.Nyss.Web.Features.NationalSocieties.Dto
{
    public class EditNationalSocietyRequestDto
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
        public int ContentLanguageId { get; set; }

        public class Validator : AbstractValidator<EditNationalSocietyRequestDto>
        {
            public Validator()
            {
                RuleFor(r => r.Name).NotEmpty().MinimumLength(3);
                RuleFor(r => r.ContentLanguageId).GreaterThan(0);
                RuleFor(r => r.CountryId).GreaterThan(0);
            }
        }
    }
}
