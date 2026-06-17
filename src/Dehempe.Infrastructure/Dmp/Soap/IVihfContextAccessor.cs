using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.ValueObjects;

namespace Dehempe.Infrastructure.Dmp.Soap;

/// <summary>
/// Fournit le VihfContext à partir des informations du praticien authentifié
/// (résolu depuis le token ou la session courante).
/// </summary>
public interface IVihfContextAccessor
{
    VihfContext GetContext(Ins patientIns);
}
