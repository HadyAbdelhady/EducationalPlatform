namespace Domain.enums
{
    public enum ErrorType
    {
        None,
        NotFound = 404,
        BadRequest = 400,
        UnAuthorized = 401,
        Validation = 422,
        Conflict = 409,
        InternalServerError = 500
    }
}
