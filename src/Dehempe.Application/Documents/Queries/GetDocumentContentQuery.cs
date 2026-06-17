using Dehempe.Domain.Interfaces;
using Dehempe.Domain.ValueObjects;
using MediatR;

namespace Dehempe.Application.Documents.Queries;

public record GetDocumentContentQuery(
    string UniqueId,
    string RepositoryUniqueId,
    string? HomeCommunityId = null
) : IRequest<DocumentContentResult>;

public record DocumentContentResult(
    string UniqueId,
    string MimeType,
    byte[] Data
);

internal sealed class GetDocumentContentQueryHandler
    : IRequestHandler<GetDocumentContentQuery, DocumentContentResult>
{
    private readonly IDmpDocumentRepository _repository;

    public GetDocumentContentQueryHandler(IDmpDocumentRepository repository)
        => _repository = repository;

    public async Task<DocumentContentResult> Handle(
        GetDocumentContentQuery request,
        CancellationToken cancellationToken)
    {
        var content = await _repository.RetrieveDocumentAsync(
            new DocumentUniqueId(request.UniqueId),
            new RepositoryUniqueId(request.RepositoryUniqueId),
            request.HomeCommunityId,
            cancellationToken);

        return new DocumentContentResult(content.UniqueId.Value, content.MimeType, content.Data);
    }
}
