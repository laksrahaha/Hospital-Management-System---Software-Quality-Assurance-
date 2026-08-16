using Microsoft.EntityFrameworkCore;

namespace ReserveHealth.Api.Data;

public class ReserveHealthContext : DbContext
{
    public ReserveHealthContext(DbContextOptions<ReserveHealthContext> options)
        : base(options)
    {
    }
}