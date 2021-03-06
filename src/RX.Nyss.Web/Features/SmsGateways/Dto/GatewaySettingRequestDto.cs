﻿using FluentValidation;
using RX.Nyss.Data.Concepts;

namespace RX.Nyss.Web.Features.SmsGateways.Dto
{
    public class GatewaySettingRequestDto
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string EmailAddress { get; set; }
        public GatewayType GatewayType { get; set; }

        public class GatewaySettingValidator : AbstractValidator<GatewaySettingRequestDto>
        {
            public GatewaySettingValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
                RuleFor(x => x.ApiKey).NotEmpty().MaximumLength(100);
                RuleFor(x => x.EmailAddress).MaximumLength(100);
                RuleFor(x => x.GatewayType).IsInEnum();
            }
        }
    }
}
