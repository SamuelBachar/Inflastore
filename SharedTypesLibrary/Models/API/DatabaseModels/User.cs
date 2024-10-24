﻿namespace SharedTypesLibrary.Models.API.DatabaseModels;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public string PasswordHashWithSalt { get; set; } = string.Empty;

    public string TempResetPasswordHashWithSalt { get; set; } = string.Empty;

    public int Region { get; set; }

    public string VerificationToken { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
}
