using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Features.Columns.Dtos;

public record UpdateColumnRequest(string Name, int? Position);