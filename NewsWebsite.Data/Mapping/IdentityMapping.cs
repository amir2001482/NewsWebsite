using Microsoft.EntityFrameworkCore;
using NewsWebsite.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public static class IdentityMapping
    {
        public static void AddCustomIdentityMapping(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("AppUsers");
            modelBuilder.Entity<Role>().ToTable("AppRoles");
            modelBuilder.Entity<UserRole>().ToTable("AppUserRoles");
            modelBuilder.Entity<UserClaim>().ToTable("AppUserClaims");
            modelBuilder.Entity<RoleClaim>().ToTable("AppRoleClaims");

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.Users).HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.Roles).HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<RoleClaim>()
                .HasOne(rc => rc.Role)
                .WithMany(r => r.Claims).HasForeignKey(rc => rc.RoleId);

            modelBuilder.Entity<UserClaim>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Claims).HasForeignKey(uc => uc.UserId);




           

        }
    }
}
