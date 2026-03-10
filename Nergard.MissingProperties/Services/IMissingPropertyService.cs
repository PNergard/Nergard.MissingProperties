namespace Nergard.MissingProperties.Services;

public interface IMissingPropertyService
{
    /// <summary>
    /// Scans all page and block types for property definitions that exist in the
    /// database but have no corresponding code-model definition.
    /// </summary>
    IReadOnlyList<MissingContentTypeResult> ScanForMissingProperties();

    /// <summary>
    /// Permanently deletes the specified property definitions from the database.
    /// </summary>
    void DeletePropertyDefinitions(IEnumerable<int> propertyDefinitionIds);
}

public enum ContentKind { PageType, BlockType }

public record MissingPropertyInfo(int PropertyDefinitionId, string PropertyName);

public record MissingContentTypeResult(
    string ContentTypeName,
    ContentKind Kind,
    IReadOnlyList<MissingPropertyInfo> Properties);
