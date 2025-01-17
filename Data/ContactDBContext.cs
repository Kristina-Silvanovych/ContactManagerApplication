using ContactManagerApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ContactManagerApp.Data
{
    public class ContactDBContext : DbContext
    {
        public DbSet<Contact> Contact { get; set; }

        public ContactDBContext(DbContextOptions<ContactDBContext> options) : base(options) { }

        public class ContactDBContextFactory : IDesignTimeDbContextFactory<ContactDBContext>
        {
            public ContactDBContext CreateDbContext(string[] args)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("ContactManagerDBConnection");

                var optionsBuilder = new DbContextOptionsBuilder<ContactDBContext>();
                optionsBuilder.UseSqlServer(connectionString);
                return new ContactDBContext(optionsBuilder.Options);
            }
        }
    }
}
