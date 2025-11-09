namespace Labubu.Auth.Extensions;

public static class ApiRoutes
{
    private const string ApiBase = "/api/v1";
    
    public static class Auth
    {
        private const string Base = $"{ApiBase}/auth";
        
        public const string Signup = $"{Base}/signup";
        public const string Signin = $"{Base}/signin";
        public const string Refresh = $"{Base}/refresh";
        public const string Logout = $"{Base}/logout";
    }
}

