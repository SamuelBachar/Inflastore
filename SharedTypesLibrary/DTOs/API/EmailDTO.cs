using SharedTypesLibrary.Models.API;

namespace SharedTypesLibrary.DTOs.API;

public class EmailDTO
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public EEmailType EmailType { get; set; }
}
