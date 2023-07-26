namespace SharedTypesLibrary.DTOs.API;

public class UserForgotPasswordDTO
{
    public string Email { get; set; } = string.Empty;

    public string PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
}
