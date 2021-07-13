using Microsoft.Extensions.DependencyInjection;
using OdinPlugs.ApiLinkMonitor.OdinAspectCore;
using OdinPlugs.ApiLinkMonitor.OdinAspectCore.IOdinAspectCoreInterface;
using OdinPlugs.ApiLinkMonitor.OdinLinkMonitor.OdinLinkMonitorInterface;

namespace OdinPlugs.ApiLinkMonitor.OdinMiddleware.Inject
{
    public static class MiddlewareInject
    {
        /// <summary>
        /// inject OdinAspectCoreInterceptorAttribute
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddSingletonOdinAspectCoreInterceptorAttribute(this IServiceCollection services)
        {
            services.AddSingleton<IOdinAspectCoreInterceptorAttribute>(new OdinAspectCoreInterceptorAttribute());
            return services;
        }


        /// <summary>
        /// inject OdinAspectCoreInterceptorAttribute
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddTransientOdinAspectCoreInterceptorAttribute(this IServiceCollection services)
        {
            services.AddTransient<IOdinAspectCoreInterceptorAttribute>(provider => new OdinAspectCoreInterceptorAttribute());
            return services;
        }

        /// <summary>
        /// inject OdinAspectCoreInterceptorAttribute
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddScopedOdinAspectCoreInterceptorAttribute(this IServiceCollection services)
        {
            services.AddScoped<IOdinAspectCoreInterceptorAttribute>(provider => new OdinAspectCoreInterceptorAttribute());
            return services;
        }


        /// <summary>
        /// inject OdinLinkMonitor
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddSingletonOdinApiLinkMonitor(this IServiceCollection services)
        {
            services.AddSingleton<IOdinApiLinkMonitor>();
            return services;
        }


        /// <summary>
        /// inject OdinLinkMonitor
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddTransientOdinApiLinkMonitor(this IServiceCollection services)
        {
            services.AddTransient<IOdinApiLinkMonitor>();
            return services;
        }

        /// <summary>
        /// inject OdinLinkMonitor
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddScopedOdinApiLinkMonitor(this IServiceCollection services)
        {
            services.AddScoped<IOdinApiLinkMonitor>();
            return services;
        }
    }
}