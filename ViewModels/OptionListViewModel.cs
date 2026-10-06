using System.Collections.ObjectModel;
using andecr.Services;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace andecr.ViewModels;

/// <summary>
/// A sorted list of selectable options (tags, parts of speech, markers, ...) that is loaded
/// from a text file, together with its own loading flag.
/// </summary>
/// <remarks>
/// Replaces the per-list triplet that used to live in <see cref="EnglishDeckViewModel"/>:
/// a <c>SourceList</c>, a bound read-only collection and an <c>IsLoading*</c> property.
/// </remarks>
public sealed class OptionListViewModel : ReactiveObject, IDisposable
{
    private readonly IDisposable _binding;
    private readonly ReadOnlyObservableCollection<string> _items;
    private readonly SourceList<string> _source = new();
    private bool _isLoading;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionListViewModel"/> class.
    /// </summary>
    /// <param name="filePath">Path to the text file with one option per line.</param>
    public OptionListViewModel(string filePath)
    {
        FilePath = filePath;

        // DynamicData keeps Items sorted automatically when the source changes.
        _binding = _source.Connect()
            .Sort(SortExpressionComparer<string>.Ascending(t => t))
            .Bind(out _items)
            .Subscribe();
    }

    /// <summary>
    /// Gets the path of the file this list is loaded from.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the options, sorted in ascending order. Bind UI controls to this collection.
    /// </summary>
    public ReadOnlyObservableCollection<string> Items => _items;

    /// <summary>
    /// Gets a value indicating whether the list is currently being loaded.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _binding.Dispose();
        _source.Dispose();
    }

    /// <summary>
    /// Adds an option; it is placed into <see cref="Items"/> in sorted order.
    /// </summary>
    public void Add(string item)
    {
        _source.Add(item);
    }

    /// <summary>
    /// Removes an option.
    /// </summary>
    /// <returns><c>true</c> if the option was found and removed.</returns>
    public bool Remove(string item)
    {
        return _source.Remove(item);
    }

    /// <summary>
    /// Loads <see cref="Items"/> from <see cref="FilePath"/>, toggling <see cref="IsLoading"/>
    /// for the duration of the operation.
    /// </summary>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="InvalidDataException">The file is not a text file.</exception>
    /// <remarks>
    /// If the file is empty, the method returns without touching the current items.
    /// <see cref="IsLoading"/> is always reset, including when an exception is thrown.
    /// Exceptions are intentionally not handled here; the caller decides how to report them.
    /// </remarks>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;

        try
        {
            // Throws FileNotFoundException / InvalidDataException; false means the file is empty.
            if (!await TextFileValidator.EnsureNonEmptyTextFileAsync(FilePath, cancellationToken))
            {
                return;
            }

            var loaded = (await TextOptionListLoader.LoadAsync(FilePath, cancellationToken)).ToList();

            // Nothing usable in the file (e.g. only blank lines): keep the current contents.
            if (loaded.Count == 0)
            {
                return;
            }

            // The continuation runs on the UI thread, so the bound collection is updated safely.
            _source.Edit(list =>
            {
                list.Clear();
                list.AddRange(loaded);
            });
        }
        finally
        {
            IsLoading = false;
        }
    }
}