using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InflaStoreWebAPI.DTOs;

public class UserVerifyDTO
{
    public string Email { get; set; } = string.Empty;
}
