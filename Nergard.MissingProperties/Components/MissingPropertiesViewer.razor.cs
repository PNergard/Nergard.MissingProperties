using Microsoft.AspNetCore.Components;
using MudBlazor;
using Nergard.MissingProperties.Services;

namespace Nergard.MissingProperties.Components;

public partial class MissingPropertiesViewer
{
    [Inject]
    private IMissingPropertyService MissingPropertyService { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private List<GroupViewModel> _groups = new();
    private bool _isLoading;
    private bool _isDeleting;
    private string _statusMessage = string.Empty;
    private Severity _messageSeverity = Severity.Success;

    private int TotalMissing => _groups.Sum(g => g.Properties.Count);
    private int TotalSelected => _groups.Sum(g => g.Properties.Count(p => p.IsSelected));
    private int PageTypesAffected => _groups.Count(g => g.Kind == ContentKind.PageType);
    private int BlockTypesAffected => _groups.Count(g => g.Kind == ContentKind.BlockType);

    protected override void OnInitialized()
    {
        Scan();
    }

    private void Scan()
    {
        _isLoading = true;

        try
        {
            _groups = MissingPropertyService.ScanForMissingProperties()
                .Select(r => new GroupViewModel
                {
                    ContentTypeName = r.ContentTypeName,
                    Kind = r.Kind,
                    Properties = r.Properties
                        .Select(p => new PropertyViewModel
                        {
                            PropertyDefinitionId = p.PropertyDefinitionId,
                            PropertyName = p.PropertyName
                        })
                        .ToList()
                })
                .ToList();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void ToggleSelectAll(GroupViewModel group, bool selected)
    {
        foreach (var prop in group.Properties)
            prop.IsSelected = selected;
    }

    private void ToggleSelectAllGlobal(bool selected)
    {
        foreach (var group in _groups)
        {
            foreach (var prop in group.Properties)
                prop.IsSelected = selected;
            if (selected)
                group.IsExpanded = true;
        }
    }

    private void ExpandAll() { foreach (var g in _groups) g.IsExpanded = true; }
    private void CollapseAll() { foreach (var g in _groups) g.IsExpanded = false; }

    private async Task DeleteSelectedAsync()
    {
        var selectedCount = TotalSelected;
        if (selectedCount == 0) return;

        if (!await ConfirmAsync(
            $"Delete {selectedCount} selected {(selectedCount == 1 ? "property" : "properties")}?",
            "This will permanently remove the selected property definitions including any existing content from the database. This action cannot be undone."))
            return;

        var ids = _groups
            .SelectMany(g => g.Properties)
            .Where(p => p.IsSelected)
            .Select(p => p.PropertyDefinitionId)
            .ToList();

        await ExecuteDeleteAsync(ids, $"Deleted {selectedCount} {(selectedCount == 1 ? "property" : "properties")} successfully.");
    }

    private async Task DeleteAllAsync()
    {
        var total = TotalMissing;
        if (total == 0) return;

        if (!await ConfirmAsync(
            $"Delete all {total} missing {(total == 1 ? "property" : "properties")}?",
            "This will permanently remove ALL missing property definitions from the database. This action cannot be undone."))
            return;

        var ids = _groups
            .SelectMany(g => g.Properties)
            .Select(p => p.PropertyDefinitionId)
            .ToList();

        await ExecuteDeleteAsync(ids, $"Deleted all {total} missing {(total == 1 ? "property" : "properties")} successfully.");
    }

    private async Task ExecuteDeleteAsync(List<int> ids, string successMessage)
    {
        _isDeleting = true;
        _statusMessage = string.Empty;
        StateHasChanged();

        try
        {
            MissingPropertyService.DeletePropertyDefinitions(ids);
            _messageSeverity = Severity.Success;
            _statusMessage = successMessage;
            Scan();
        }
        catch (Exception ex)
        {
            _messageSeverity = Severity.Error;
            _statusMessage = $"Error during deletion: {ex.Message}";
        }
        finally
        {
            _isDeleting = false;
            StateHasChanged();
        }
    }

    private async Task<bool> ConfirmAsync(string title, string message)
    {
        var parameters = new DialogParameters<ConfirmDialog>
        {
            { x => x.ContentText, message },
            { x => x.ConfirmText, "Delete" },
            { x => x.Color, Color.Error }
        };
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(title, parameters, options);
        var result = await dialog.Result;
        return result is { Canceled: false };
    }

    private class PropertyViewModel
    {
        public int PropertyDefinitionId { get; init; }
        public string PropertyName { get; init; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    private class GroupViewModel
    {
        public string ContentTypeName { get; init; } = string.Empty;
        public ContentKind Kind { get; init; }
        public List<PropertyViewModel> Properties { get; init; } = new();
        public bool AllSelected => Properties.Count > 0 && Properties.All(p => p.IsSelected);
        public bool AnySelected => Properties.Any(p => p.IsSelected);
        public bool IsExpanded { get; set; }
    }
}
