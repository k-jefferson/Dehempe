using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.Interfaces;
using Dehempe.Infrastructure.Dmp;
using Dehempe.Infrastructure.Dmp.Auth;
using Dehempe.Infrastructure.Dmp.Auth.Pkcs11;
using Dehempe.Infrastructure.Dmp.Card;
using Dehempe.Infrastructure.Dmp.Soap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CpsOptions>(configuration.GetSection(CpsOptions.SectionName));
        services.Configure<DmpOptions>(configuration.GetSection(DmpOptions.SectionName));

        services.AddSingleton<Pkcs11CpsKeyStore>();
        services.AddSingleton<ICpsAuthService, CpsAuthService>();
        services.AddScoped<IVihfService, VihfService>();
        services.AddScoped<ISoapRequestCapture, SoapRequestCapture>();

        // IVihfContextAccessor lit l'identité depuis le certificat CPS branché à la machine
        services.AddHttpContextAccessor();
        services.AddSingleton<IVihfContextAccessor, CpsVihfContextAccessor>();

        // Détermine en avance si on utilise le tunnel stunnel : ça change la wiring de tous les HttpClients.
        var tunnelEndpoint = configuration.GetSection(DmpOptions.SectionName)[nameof(DmpOptions.TunnelEndpoint)];
        var useTunnel = !string.IsNullOrWhiteSpace(tunnelEndpoint);

        services.AddTransient<DmpTunnelHandler>(sp => new DmpTunnelHandler(
            tunnelEndpoint!,
            sp.GetRequiredService<ILoggerFactory>().CreateLogger<DmpTunnelHandler>()));

        // Quand le tunnel est actif, stunnel s'occupe du mTLS : .NET ne doit PAS
        // tenter d'attacher de cert client (et on parle en HTTP loopback simple).
        // Sinon : on attache le cert CPS au handshake mTLS direct (Windows/Linux PKCS#11 ou .p12).
        HttpClientHandler BuildClientHandler(IServiceProvider sp)
        {
            var handler = new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Manual };
            if (useTunnel) return handler;

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
            }
            return handler;
        }

        IHttpClientBuilder ConfigureDmpClient<TClient>() where TClient : class
        {
            var b = services.AddHttpClient<TClient>()
                .ConfigurePrimaryHttpMessageHandler(BuildClientHandler);
            if (useTunnel) b = b.AddHttpMessageHandler<DmpTunnelHandler>();
            return b;
        }

        ConfigureDmpClient<XdsRegistryClient>();
        ConfigureDmpClient<XdsRepositoryClient>();
        ConfigureDmpClient<GdpExistenceClient>();

        services.AddScoped<IDmpDocumentRepository, DmpDocumentRepository>();
        services.AddScoped<IDmpExistenceRepository>(sp => sp.GetRequiredService<GdpExistenceClient>());
        services.AddScoped<ICpsCardReaderService, CpsCardReaderService>();

        return services;
    }
}
