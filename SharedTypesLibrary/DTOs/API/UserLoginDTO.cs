﻿namespace SharedTypesLibrary.DTOs.API;

public class UserLoginDTO
{
    public string Email { get; set; } = string.Empty;

    public string JWT { get; set; } = string.Empty;
}