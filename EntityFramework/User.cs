namespace Easy.Net.Starter.EntityFramework;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime RegistrationDate { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsLocked { get; set; }
}
