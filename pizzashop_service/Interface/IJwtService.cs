namespace pizzashop_service.Interface;

public interface IJwtService
{
     public string GenerateJwtToken(string email, int roleId );
}
