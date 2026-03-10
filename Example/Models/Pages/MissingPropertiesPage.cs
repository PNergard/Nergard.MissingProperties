namespace YourProject.Models.Pages;

/// <summary>
/// A standalone page that renders the Missing Properties tool.
/// Create a page of this type in the CMS and navigate to it to use the tool.
/// </summary>
[ContentType(
    GUID = "D4E5F6A7-890B-CDEF-0123-456789012345",  // Replace with your own GUID
    DisplayName = "Missing Properties Tool",
    Description = "Find and delete orphaned property definitions left over after code refactoring")]
public class MissingPropertiesPage : PageData
{
}
