using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskFlow.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.ColumnId).NotEmpty().WithMessage("Column id is required.");
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MinimumLength(2).WithMessage("Task title must be at least 2 characters.")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
        RuleFor(x => x.Priority).IsInEnum().WithMessage("Priority is invalid.");
    }
}