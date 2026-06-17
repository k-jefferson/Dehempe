using Dehempe.Domain.Entities;
using Dehempe.Domain.Interfaces;
using Dehempe.Domain.ValueObjects;
using Dehempe.Infrastructure.Dmp.Soap;

namespace Dehempe.Infrastructure.Dmp;

/// <summary>
/// Implémentation de IDmpDocumentRepository qui délègue aux clients SOAP XDS.b.
/// </summary>
internal sealed class DmpDocumentRepository : IDmpDocumentRepository
{
    private readonly XdsRegistryClient _registry;
    private readonly XdsRepositoryClient _repository;

    // L'INS du patient est nécessaire pour le VIHF dans ITI-43.
    // On le récupère depuis le contexte (à injecter selon le flux applicatif).
    private readonly IVihfContextAccessor _vihfCtxAccessor;

    public DmpDocumentRepository(
        XdsRegistryClient registry,
        XdsRepositoryClient repository,
        IVihfContextAccessor vihfCtxAccessor)
    {
        _registry = registry;
        _repository = repository;
        _vihfCtxAccessor = vihfCtxAccessor;
    }

    public Task<IReadOnlyList<DocumentEntry>> FindDocumentsAsync(
        Ins patientIns,
        DocumentSearchCriteria? criteria = null,
        CancellationToken ct = default)
        => _registry.FindDocumentsAsync(patientIns, criteria, ct);

    public Task<DocumentContent> RetrieveDocumentAsync(
        DocumentUniqueId uniqueId,
        RepositoryUniqueId repositoryUniqueId,
        string? homeCommunityId = null,
        CancellationToken ct = default)
    {
        // Pour ITI-43 on a besoin de l'INS du patient pour le VIHF.
        // Dans ce projet, le contexte VIHF est résolu par IVihfContextAccessor
        // qui lit le token Bearer de la requête HTTP courante.
        var vihfCtx = _vihfCtxAccessor.GetContext(null!);
        var patientIns = Ins.CreateNir(vihfCtx.PatientIns);

        return _repository.RetrieveDocumentAsync(
            uniqueId, repositoryUniqueId, homeCommunityId, patientIns, ct);
    }
}
