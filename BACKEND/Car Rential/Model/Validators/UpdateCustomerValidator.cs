using Car_Rential.Entieties;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Car_Rential.Model.Validators
{
    public class UpdateCustomerValidator : AbstractValidator<InputCustomerDto>
    {
        public UpdateCustomerValidator(RentalDbContext dbContext)
        {
            RuleFor(c => c.FirstName)
                .Matches(@"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$")
                .WithMessage("FirstName must not contain white spaces")
                .MaximumLength(30);

            RuleFor(c => c.LastName)
                .Matches(@"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$")
                .WithMessage("LastName must not contain white spaces")
                .MaximumLength(30);

            RuleFor(c => c.PhoneNumber)
                .Matches(@"^[\s-]?\d{3}[\s-]?\d{3}[\s-]?\d{3}$")
                .Custom(
                    (value, contex) =>
                    {
                        var result = dbContext.Custormers
                            .Where(c => c.PhoneNumber == value)
                            .ToList();

                        var id = contex.InstanceToValidate.Identyfire;
                        if (!result.IsNullOrEmpty())
                        {
                            foreach (var item in result)
                            {
                                if (item.Id != id)
                                    contex.AddFailure("PhoneNumber", "PhoneNumber must be uniqe");
                            }
                        }
                    }
                );

            RuleFor(c => c.Email)
                .Matches(@"^[^\s]+$")
                .WithMessage("Email must not contain white spaces")
                .EmailAddress()
                .Custom(
                    (value, contex) =>
                    {
                        var result = dbContext.Custormers.Where(c => c.Email == value).ToList();
                        var id = contex.InstanceToValidate.Identyfire;
                        if (!result.IsNullOrEmpty())
                        {
                            foreach (var item in result)
                            {
                                if (item.Id != id)
                                    contex.AddFailure("Email", "Email must be uniqe");
                            }
                        }
                    }
                );

            RuleFor(c => c.Pesel)
                .Matches(@"^(\d{11})$")
                .WithMessage("Pesel must be 11 digits")
                .Custom(
                    (value, contex) =>
                    {
                        var result = dbContext.Custormers.Where(c => c.Pesel == value).ToList();
                        var id = contex.InstanceToValidate.Identyfire;
                        if (!result.IsNullOrEmpty())
                        {
                            foreach (var item in result)
                            {
                                if (item.Id != id)
                                    contex.AddFailure("Pesel", "Pesel must be uniqe");
                            }
                        }
                    }
                );

            RuleFor(c => c.Password)
                .Matches(
                    @"^(null|(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,})$"
                )
                .WithMessage(
                    "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character [@$!%*?&], and be at least 8 characters long."
                );

            RuleFor(c => c.ConfirmPassword).Equal(p => p.Password);

            RuleFor(a => a.Country).MaximumLength(255);
            RuleFor(a => a.City).MaximumLength(255);
            RuleFor(a => a.StreetName).MaximumLength(255);
            RuleFor(a => a.BuildingNumber)
                .Matches(@"^[0-9]+[a-zA-Z]?$")
                .WithMessage("Building number can only contain digits and one optional letter");

            RuleFor(a => a.ZipCode)
                .Matches(@"^\d{5}$|^\d{2}-\d{3}$")
                .WithMessage("ZipCode if format XXXXX where X is a digit");

            RuleFor(a => a.State).MaximumLength(255);
        }
    }
}
