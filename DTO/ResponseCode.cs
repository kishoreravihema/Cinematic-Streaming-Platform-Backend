namespace Netflix_BackendAPI.DTO
{
    public enum ResponseCode
    {
        // ✅ Success
        Success = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,
        // 📄 Partial Content
        PartialContent = 206,

        // 🔄 Redirection (rarely used but available)
        MovedPermanently = 301,
        Found = 302,

        // ⚠️ Client Errors
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        Conflict = 409,
        Gone = 410,
        UnsupportedMediaType = 415,
        UnprocessableEntity = 422,
        TooManyRequests = 429,

        // 💥 Server Errors
        ServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
       
    }


}
