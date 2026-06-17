using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.Interfaces;
using Dehempe.Infrastructure.Dmp;
using Dehempe.Infrastructure.Dmp.Auth;
using Dehempe.Infrastructure.Dmp.Card;
using Dehempe.Infrastructure.Dmp.Soap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dehempe.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CpsOptions>(configuration.GetSection(CpsOptions.SectionName));
        services.Configure<DmpOptions>(configuration.GetSection(DmpOptions.SectionName));

        services.AddSingleton<ICpsAuthService, CpsAuthService>();
        services.AddScoped<IVihfService, VihfService>();

        // IVihfContextAccessor lit l'identité depuis le certificat CPS branché à la machine
        services.AddHttpContextAccessor();
        services.AddSingleton<IVihfContextAccessor, CpsVihfContextAccessor>();

        services.AddHttpClient<XdsRegistryClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual
            });

        services.AddHttpClient<XdsRepositoryClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual
            });

        services.AddScoped<IDmpDocumentRepository, DmpDocumentRepository>();
        services.AddScoped<ICpsCardReaderService, CpsCardReaderService>();

        return services;
    }
}
