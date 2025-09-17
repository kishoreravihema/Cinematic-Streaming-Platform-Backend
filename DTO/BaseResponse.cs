using Microsoft.AspNetCore.Mvc;

namespace Netflix_BackendAPI.DTO
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public ResponseCode Code { get; set; } = ResponseCode.Success;

        public List<string>? Errors { get; set; }

       // public string? FileName { get; internal set; }
    }

}


