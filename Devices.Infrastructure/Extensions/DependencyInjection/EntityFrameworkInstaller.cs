using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Devices.Infrastructure.Extensions.DependencyInjection
{
    public static class EntityFrameworkInstaller
    {
        public static IServiceCollection AddEFDbContext(this IServiceCollection serviceCollection, string connectionString)
        {
            connectionString = connectionString ?? throw new ArgumentNullException(connectionString);

            serviceCollection.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(connectionString));

            return serviceCollection;
        }
    }
}