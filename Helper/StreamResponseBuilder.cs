using Microsoft.AspNetCore.Mvc;
using Netflix_BackendAPI.DTO;

namespace Netflix_BackendAPI.Helper
{
    public static class StreamResponseBuilder
    {
        public static BaseResponse<object> FromStream(ResponseCode success, FileStreamResult streamResult)
        {
            return new BaseResponse<object>
            {
                Success = true,
                Code = ResponseCode.Success,
                Message = "Video stream ready",
                Data = streamResult
            };
        }
        public static BaseResponse<object> FromFile(ResponseCode success, string localPath, string v1, string v2)
        {
               
            return new BaseResponse<object>
            {
                Success = true,
                Code = success,
                Message = "File stream ready",
                Data = new { LocalPath = localPath, FileName = v1, ContentType = v2 }
            };
        }
        public static BaseResponse<object> FromError(ResponseCode code, string message, List<string>? errors = null)
        {
            return new BaseResponse<object>
            {
                Success = false,
                Code = code,
                Message = message,
                Errors = errors,
                Data = null
            };
        }
        public static BaseResponse<T> FromSuccess<T>(T data, string message = "Success", ResponseCode code = ResponseCode.Success)
        {
            return new BaseResponse<T>
            {
                Success = true,
                Code = code,
                Message = message,
                Data = data
            };
        }
        public static (FileStream?, string?, BaseResponse<T>) ErrorResponse<T>(ResponseCode code, string message)
        {
            return (null, null, new BaseResponse<T>
            {
                Success = false,
                Message = message,
                Errors = new List<string> { message },
                Code = code,
                Data = default
            });
        }



    }

}
