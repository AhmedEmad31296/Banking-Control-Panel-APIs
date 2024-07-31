using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System.IO;

namespace BankingControlPanel.Infrastructure.Data.Contexts
{
    public class BankingControlPanelDbContextFactory : IDesignTimeDbContextFactory<BankingControlPanelDbContext>
    {
        public BankingControlPanelDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BankingControlPanelDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new BankingControlPanelDbContext(optionsBuilder.Options);
        }
    }
}
