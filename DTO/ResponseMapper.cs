using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Netflix_BackendAPI.DTO
{
    public static class ResponseMapper
    {
        public static IActionResult Map<T>(ControllerBase controller, BaseResponse<T> result)
        {
            return result.Code switch
            {
                ResponseCode.Success => controller.Ok(result),
                ResponseCode.Created => controller.StatusCode(201, result),
                ResponseCode.Accepted => controller.StatusCode(202, result),
                ResponseCode.NoContent => controller.NoContent(),
                ResponseCode.BadRequest => controller.BadRequest(result),
                ResponseCode.Unauthorized => controller.Unauthorized(result),
                ResponseCode.PaymentRequired => controller.StatusCode(402, result),
                ResponseCode.Forbidden => controller.StatusCode(403, result),
                ResponseCode.NotFound => controller.NotFound(result),
                ResponseCode.MethodNotAllowed => controller.StatusCode(405, result),
                ResponseCode.Conflict => controller.StatusCode(409, result),
                ResponseCode.Gone => controller.StatusCode(410, result),
                ResponseCode.UnsupportedMediaType => controller.StatusCode(415, result),
                ResponseCode.UnprocessableEntity => controller.StatusCode(422, result),
                ResponseCode.TooManyRequests => controller.StatusCode(429, result),
                ResponseCode.ServerError => controller.StatusCode(500, result),
                ResponseCode.NotImplemented => controller.StatusCode(501, result),
                ResponseCode.BadGateway => controller.StatusCode(502, result),
                ResponseCode.ServiceUnavailable => controller.StatusCode(503, result),
                ResponseCode.GatewayTimeout => controller.StatusCode(504, result),
                _ => controller.StatusCode(500, result)
            };
        }

        internal static BaseResponse<T> FromModelState<T>(ModelStateDictionary modelState)
        {
            throw new NotImplementedException();
        }
    }
}
