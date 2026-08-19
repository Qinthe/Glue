namespace Glue.API.Models.Dtos;

public class ApiResponseDto<T>
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? PromptTextCode { get; set; }
    public T? Data { get; set; }


    public static ApiResponseDto<T> Ok(T data, string message = "Success", string? promptTextCode = null)
    {
        return new ApiResponseDto<T>
        {
            StatusCode = 200,
            Success = true,
            Message = message,
            PromptTextCode = promptTextCode,
            Data = data
        };
    }
    public static ApiResponseDto<T> Error(string message, int statusCode = 400, string? promptTextCode = null, T? data = default)
    {
        return new ApiResponseDto<T>
        {
            StatusCode = statusCode,
            Success = false,
            Message = message,
            PromptTextCode = promptTextCode,
            Data = data
        };
    }
}