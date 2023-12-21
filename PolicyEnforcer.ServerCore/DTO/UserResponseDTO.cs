namespace PolicyEnforcer.ServerCore.DTO
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }
        public string Login { get; set; }
        public int AccessLevel { get; set; }
    }
}
