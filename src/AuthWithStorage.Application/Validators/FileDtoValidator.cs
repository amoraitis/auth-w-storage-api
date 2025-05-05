using AuthWithStorage.Application.DTOs;
using FluentValidation;

namespace AuthWithStorage.Application.Validators
{
    public class FileDtoValidator : AbstractValidator<FileDto>
    {
        public FileDtoValidator()
        {
            // TODO: Add more validation rules as needed
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("File name is required.")
                .Length(1, 255).WithMessage("File name must be between 1 and 255 characters.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content type is required.")
                .Matches(@"^[a-zA-Z0-9/]+$").WithMessage("Invalid content type format.");
        }
    }

    public class FileRequestValidator : AbstractValidator<FileRequest>
    {
        public FileRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("File name is required.")
                .Length(1, 255).WithMessage("File name must be between 1 and 255 characters.");

            RuleFor(x => x.FormFile)
                .NotNull().WithMessage("File is required.")
                .Must(file => file.Length > 0).WithMessage("File cannot be empty.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("File type is required.")
                .IsInEnum().WithMessage("Invalid file type format.");

            RuleFor(x => x.UploadedByUserId)
                .NotEmpty().WithMessage("Uploaded by user ID is required.");
        }
    }
}
