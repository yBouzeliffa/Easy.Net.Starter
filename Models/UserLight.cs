namespace Easy.Net.Starter.Models
{
    public record UserLight
    {
        public Guid Id { get; init; } = Guid.Empty;
        public string Email { get; init; } = string.Empty;
        public string? Name { get; init; }

        public UserLight(Guid id, string email, string? name)
        {
            Id = id;
            Email = email;
            Name = name;
        }
    }
}
