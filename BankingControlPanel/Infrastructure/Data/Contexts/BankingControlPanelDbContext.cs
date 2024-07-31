using BankingControlPanel.Application.Authorization;
using BankingControlPanel.Domain.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankingControlPanel.Infrastructure.Data.Contexts
{
    public class BankingControlPanelDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public BankingControlPanelDbContext(DbContextOptions<BankingControlPanelDbContext> options) : base(options) { }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<FilteredClientsHistory> FilteredClientsHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.BankAccounts)
                .WithOne(a => a.Client)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Addresses)
                .WithOne(a => a.Client)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
