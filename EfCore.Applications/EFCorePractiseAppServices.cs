using EfCore.Application.Contracts;
using EfCore.Application.Contracts.Dtos;
using EfCore.Pgsql;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Applications
{
    public class EFCorePractiseAppServices : IEFCorePractiseAppServices
    {
        private PgDbContext _dbContext;
        public EFCorePractiseAppServices(PgDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<CustomerDto>> PgSql_SelectDemo()
        {
            var customers = await _dbContext.Customers.ToListAsync();
            var data = customers.Adapt<List<CustomerDto>>();
            return data;
        }
    }
}
