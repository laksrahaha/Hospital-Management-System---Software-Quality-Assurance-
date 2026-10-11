using ReserveHealth.Api.Data;
using ReserveHealth.Api.Models;

namespace ReserveHealth.Api.Services
{
    public class AuditService
    {
        private readonly ReserveHealthContext _context;

        public AuditService(ReserveHealthContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            int userId,
            string actionType,
            string entityType,
            int entityId,
            string? oldValue = null,
            string? newValue = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                ActionType = actionType,
                EntityType = entityType,
                EntityId = entityId,
                OldValue = oldValue,
                NewValue = newValue,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();
        }
    }
}