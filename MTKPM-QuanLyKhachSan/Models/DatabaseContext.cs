﻿using Microsoft.EntityFrameworkCore;

namespace MTKPM_QuanLyKhachSan.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<PermissionGroup>()
				.HasKey(o => new { o.PermissionId, o.RoleId });
            modelBuilder.Entity<EmployeePermission>()
                .HasKey(o => new { o.PermissionId, o.EmployeeId });
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomTypeImage> RoomTypeImages { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BookRoom> BookRooms { get; set; }
        public DbSet<BookRoomDetails> BookRoomDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public DbSet<EmployeePermission> EmployeePermissions { get; set; }
    }
}
