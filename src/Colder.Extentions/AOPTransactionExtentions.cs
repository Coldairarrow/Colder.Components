using Microsoft.Extensions.DependencyInjection;

namespace Colder.Extentions
{
    /// <summary>
    /// AOP事务拓展
    /// </summary>
    public static class AOPTransactionExtentions
    {
        /// <summary>
        /// 注入AOP事务
        /// </summary>
        /// <param name="services">services</param>
        /// <returns></returns>
        public static IServiceCollection AddAOPTransaction(this IServiceCollection services)
        {
            return services.AddScoped<TransactionContainer>();
        }
    }
}
