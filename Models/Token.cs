namespace Easy.Net.Starter.Models
{
    public record UserToken
    {
        public string Token { get; init; }

        public UserToken(string token)
        {
            Token = token;
        }
    }
}
