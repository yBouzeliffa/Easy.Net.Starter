namespace Easy.Net.Starter.EntityFramework
{
    public class EmailValidation
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? VerifiedAt { get; set; }

        public virtual User User { get; set; }
    }
}
