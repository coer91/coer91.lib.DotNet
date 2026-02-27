using Microsoft.Extensions.DependencyInjection;

namespace coer91.NET
{
    public static class ExceptionFilterService
    {
        public static IServiceCollection AddExceptionFilter(this IServiceCollection filter)
        {
            filter.AddTransient<ExceptionFilter>().AddControllers(config => config.Filters.Add<ExceptionFilter>());
            return filter;
        }
    }
}