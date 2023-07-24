namespace SharedTypesLibrary.Models.API.ServiceResponseModel;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public string ExceptionMessage { get; set; } = string.Empty;
}
