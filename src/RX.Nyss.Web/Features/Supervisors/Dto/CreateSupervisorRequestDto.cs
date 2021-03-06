﻿using FluentValidation;
using RX.Nyss.Common.Utils.DataContract;
using RX.Nyss.Data.Concepts;
using RX.Nyss.Web.Services;

namespace RX.Nyss.Web.Features.Supervisors.Dto
{
    public class CreateSupervisorRequestDto
    {
        public string Name { get; set; }
        public Sex Sex { get; set; }
        public int DecadeOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        public string Email { get; set; }
        public int? ProjectId { get; set; }
        public string Organization { get; set; }

        public class CreateSupervisorValidator : AbstractValidator<CreateSupervisorRequestDto>
        {
            public CreateSupervisorValidator()
            {
                RuleFor(m => m.Name).NotEmpty().MaximumLength(100);
                RuleFor(m => m.Sex).IsInEnum();
                RuleFor(m => m.DecadeOfBirth).NotEmpty().Must(y => y % 10 == 0).WithMessage(ResultKey.Validation.BirthGroupStartYearMustBeMulipleOf10);
                RuleFor(m => m.PhoneNumber).NotEmpty().MaximumLength(20).PhoneNumber();
                RuleFor(m => m.Email).NotEmpty().MaximumLength(100).EmailAddress();
                RuleFor(m => m.AdditionalPhoneNumber).MaximumLength(20).PhoneNumber().Unless(r => string.IsNullOrEmpty(r.AdditionalPhoneNumber));
            }
        }
    }
}
