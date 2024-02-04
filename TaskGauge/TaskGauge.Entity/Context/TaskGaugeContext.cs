using Microsoft.EntityFrameworkCore;
using TaskGauge.Entity.Entity;

namespace TaskGauge.Entity.Context
{
    public class TaskGaugeContext : DbContext
    {
        public TaskGaugeContext(DbContextOptions<TaskGaugeContext> options) : base(options)
        {

        }
        public virtual DbSet<Entity.Task> Task { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserEstimationLog> UserEstimationLog { get; set; } 
        public virtual DbSet<RoomTaskInformation> RoomTaskInformation { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<PatchTotalEstimationTime> PatchTotalEstimationTime { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestion { get; set; }
        public virtual DbSet<Role> Role { get; set; }
    }
}
