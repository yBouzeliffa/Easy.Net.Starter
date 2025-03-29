namespace Easy.Net.Starter.EntityFramework
{
    public class TwoFactorCode
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Code { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public TwoFactorCodeType Type { get; set; }

        public virtual User? User { get; set; }
    }

    public enum TwoFactorCodeType
    {
        Email,
        Sms
    }
}
