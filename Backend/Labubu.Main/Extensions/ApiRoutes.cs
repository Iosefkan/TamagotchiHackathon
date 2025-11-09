namespace Labubu.Main.Extensions;

public static class ApiRoutes
{
    private const string ApiBase = "/api/v1";
    
    public static class Labubu
    {
        private const string Base = $"{ApiBase}/labubu";
        
        public const string Health = $"{Base}/health";
        public const string Clothes = $"{Base}/clothes";
        public const string ClothesCurrent = $"{Base}/clothes/current";
        public const string Actions = $"{Base}/actions";
        public const string MessagesRead = $"{Base}/messages/read";
    }
    
    public static class Users
    {
        private const string Base = $"{ApiBase}/users";
        
        public const string Info = $"{Base}/info";
        public const string Energy = $"{Base}/energy";
    }
    
    public static class Clothes
    {
        private const string Base = $"{ApiBase}/clothes";
        
        public const string List = Base;
    }
    
    public static class Achievements
    {
        private const string Base = $"{ApiBase}/achievements";
        
        public const string List = Base;
    }
    
    public static class Games
    {
        private const string Base = $"{ApiBase}/games";
        
        public const string Start = $"{Base}/start";
        public const string End = $"{Base}/end";
    }
    
    public static class BattlePasses
    {
        private const string Base = $"{ApiBase}/battle-passes";
        
        public const string Active = $"{Base}/active";
        public const string Info = $"{Base}/{{battlePassId}}";
        public const string Progress = $"{Base}/{{battlePassId}}/progress";
        public const string Rewards = $"{Base}/{{battlePassId}}/rewards";
        public const string Purchase = $"{Base}/{{battlePassId}}/purchase";
        public const string ClaimReward = $"{Base}/rewards/{{rewardId}}/claim";
    }
    
    public static class Hubs
    {
        private const string Base = $"{ApiBase}/hubs";
        
        public const string Message = $"{Base}/message";
    }
}

