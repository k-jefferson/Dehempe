using Dehempe.Application.Common.Interfaces;

namespace Dehempe.Infrastructure.Dmp.Soap;

internal sealed class SoapRequestCapture : ISoapRequestCapture
{
    public string? LastRequest { get; set; }
    public string? LastResponse { get; set; }
}
