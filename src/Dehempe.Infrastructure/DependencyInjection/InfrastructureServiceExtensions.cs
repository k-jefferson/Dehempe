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

        // Handler factory qui attache le cert CPS au handshake mTLS (quand il a une clé privée extractable).
        // Sur macOS avec cert CTK, la clé n'est pas extractable → le handshake mTLS ratera côté .NET runtime,
        // l'erreur est interceptée dans XdsSoapClientBase et remontée en DmpAuthException.
        HttpClientHandler BuildClientHandler(IServiceProvider sp)
        {
            var handler = new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Manual };
            try
            {
                var cpsAuth = sp.GetRequiredService<ICpsAuthService>();
                var cert    = cpsAuth.GetCertificateAsync().GetAwaiter().GetResult();
                if (cert.HasPrivateKey)
                    handler.ClientCertificates.Add(cert);
            }
            catch
            {
                // L'auto-détection est différée à la première vraie requête.
                // Ici on est dans la factory du HttpClient (lazy), pas grave si ça rate.
            }
            return handler;
        }

        services.AddHttpClient<XdsRegistryClient>()
            .ConfigurePrimaryHttpMessageHandler(BuildClientHandler);

        services.AddHttpClient<XdsRepositoryClient>()
            .ConfigurePrimaryHttpMessageHandler(BuildClientHandler);

        services.AddHttpClient<GdpExistenceClient>()
            .ConfigurePrimaryHttpMessageHandler(BuildClientHandler);

        services.AddScoped<IDmpDocumentRepository, DmpDocumentRepository>();
        services.AddScoped<IDmpExistenceRepository>(sp => sp.GetRequiredService<GdpExistenceClient>());
        services.AddScoped<ICpsCardReaderService, CpsCardReaderService>();

        return services;
    }
}
