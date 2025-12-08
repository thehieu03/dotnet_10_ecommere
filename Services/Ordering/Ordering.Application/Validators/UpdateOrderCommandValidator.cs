using FluentValidation;
using Ordering.Application.Commands;

namespace Ordering.Application.Validators;

public class UpdateOrderCommandValidator:AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(o=>o.Id)
            .GreaterThan(0)
            .WithMessage("{Id} must be greater than 0.");
        RuleFor(o=>o.UserName)
            .NotEmpty()
            .WithMessage("{UserName} is required.")
            .NotNull()
            .MaximumLength(70)
            .WithMessage("{UserName} must not exceed 70 characters.");
        RuleFor(o=>o.TotalPrice)
            .NotEmpty()
            .WithMessage("{TotalPrice} is required.")
            .GreaterThan(-1)
            .WithMessage("{TotalPrice} must be a positive number.");
        RuleFor(o => o.EmailAddress)
            .NotEmpty()
            .WithMessage("{EmailAddress} is required.");
        RuleFor(o=>o.FirstName)
            .NotEmpty()
            .NotNull()
            .WithMessage("{FirstName} is required.");
        RuleFor(o=>o.LastName)
            .NotEmpty()
            .NotNull()
            .WithMessage("{LastName} is required.");
    }
}