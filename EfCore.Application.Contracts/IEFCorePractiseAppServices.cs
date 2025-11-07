using EfCore.Application.Contracts.Dtos;

namespace EfCore.Application.Contracts
{
    public interface IEFCorePractiseAppServices
    {
        Task<List<CustomerDto>> PgSql_SelectDemo();

        Task<List<MonthDayDto>> Partition();
    }
}
