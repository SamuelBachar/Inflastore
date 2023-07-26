namespace SharedTypesLibrary.DTOs.API;

public class UserRegisterDTO
{
    public string Email { get; set; } = string.Empty;

    public string VerificationToken { get; set; }
}
