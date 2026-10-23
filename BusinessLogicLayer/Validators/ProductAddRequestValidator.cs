using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators;

public class ProductAddRequestValidator : AbstractValidator<ProductAddRequest>
{
    public ProductAddRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name can't be blank.")
            .MaximumLength(100).WithMessage("Product name can't be longer than 100 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category can't be blank.")
            .MaximumLength(100).WithMessage("Category can't be longer than 100 characters.");

        RuleFor(x => x.UnitPrice)
            .NotNull().WithMessage("Unit price is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Unit price can't be negative.");

        RuleFor(x => x.QuantityInStock)
            .NotNull().WithMessage("Quantity in stock is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Quantity in stock can't be negative.");
    }
}
