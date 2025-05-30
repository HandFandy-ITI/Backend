namespace OstaFandy.PL.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<UserTypeDto> UserTypes { get; set; } = new();
    }

    public class UserTypeDto
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = null!;
    }
}
