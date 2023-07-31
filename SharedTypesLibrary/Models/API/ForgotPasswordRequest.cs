﻿using System.ComponentModel.DataAnnotations;


namespace SharedTypesLibrary.Models.API;

public class ForgotPasswordRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6, ErrorMessage = "Prosím zadaje aspoň 6 znakov")]
    public string Password { get; set; } = string.Empty;

    [Required, Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
