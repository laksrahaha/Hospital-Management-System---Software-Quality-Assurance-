namespace ReserveHealth.Api.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        public int UserId { get; set; }

        public string ActionType { get; set; } = "";

        public string EntityType { get; set; } = "";

        public int EntityId { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public DateTime Timestamp { get; set; }
    }
}