namespace HowManIsConnected.Models
{
    /// <summary>
    /// Represents an authenticated user in the system.
    /// </summary>
    public class UserInfo
    {
        public string CircuitId { get; set; } = string.Empty; // Unique Circuit ID
        public string Email { get; set; } = "Guest"; // Default: Guest if not authenticated
        public string Name { get; set; } = "Anonymous"; // Default: Anonymous
        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow; // Time of connection
    }
}
