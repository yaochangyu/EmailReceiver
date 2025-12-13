// using EmailReceiver.WebApi.Infrastructure.TraceContext;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace EmailReceiver.IntegrationTest;
//
// public static class ServiceCollectionExtension
// {
//     public static IServiceCollection AddFakeContextAccessor(this IServiceCollection services, string userId)
//     {
//         services.AddSingleton<ContextAccessor<TraceContext>>(p =>
//         {
//             var traceContext = new TraceContext
//             {
//                 TraceId = "測試",
//                 UserId = userId
//             };
//             var accessor = new ContextAccessor<TraceContext>();
//             accessor.Set(traceContext);
//             return accessor;
//         });
//         services.AddSingleton<IContextGetter<TraceContext>>(p => p.GetService<ContextAccessor<TraceContext>>());
//         services.AddSingleton<IContextSetter<TraceContext>>(p => p.GetService<ContextAccessor<TraceContext>>());
//         return services;
//     }
// }