using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class ProfilePictureDto
{
    public Guid Id { get; set; }
    public required PhotoPath PhotoPath { get; set; }
}