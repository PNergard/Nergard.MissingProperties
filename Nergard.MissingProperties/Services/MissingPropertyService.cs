using EPiServer.DataAbstraction;

namespace Nergard.MissingProperties.Services;

public class MissingPropertyService : IMissingPropertyService
{
    private readonly IContentTypeRepository _contentTypeRepository;
    private readonly IPropertyDefinitionRepository _propertyDefinitionRepository;

    public MissingPropertyService(
        IContentTypeRepository contentTypeRepository,
        IPropertyDefinitionRepository propertyDefinitionRepository)
    {
        _contentTypeRepository = contentTypeRepository;
        _propertyDefinitionRepository = propertyDefinitionRepository;
    }

    public IReadOnlyList<MissingContentTypeResult> ScanForMissingProperties()
    {
        var results = new List<MissingContentTypeResult>();
        var allTypes = _contentTypeRepository.List();

        foreach (var pageType in allTypes.OfType<PageType>())
        {
            // Only check code-defined content types; skip manually created ones
            if (pageType.ModelType == null) continue;

            var missing = pageType.PropertyDefinitions
                .Where(p => !p.ExistsOnModel)
                .Select(p => new MissingPropertyInfo(p.ID, p.Name))
                .ToList();

            if (missing.Count > 0)
                results.Add(new MissingContentTypeResult(pageType.Name, ContentKind.PageType, missing));
        }

        foreach (var blockType in allTypes.OfType<BlockType>())
        {
            // Only check code-defined content types; skip manually created ones
            if (blockType.ModelType == null) continue;

            var missing = blockType.PropertyDefinitions
                .Where(p => !p.ExistsOnModel)
                .Select(p => new MissingPropertyInfo(p.ID, p.Name))
                .ToList();

            if (missing.Count > 0)
                results.Add(new MissingContentTypeResult(blockType.Name, ContentKind.BlockType, missing));
        }

        return results
            .OrderBy(r => r.Kind)
            .ThenBy(r => r.ContentTypeName)
            .ToList();
    }

    public void DeletePropertyDefinitions(IEnumerable<int> propertyDefinitionIds)
    {
        foreach (var id in propertyDefinitionIds)
        {
            var definition = _propertyDefinitionRepository.Load(id);
            if (definition != null)
                _propertyDefinitionRepository.Delete(definition.CreateWritableClone());
        }
    }
}
