using System.Reactive;
using andecr.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace andecr.Views;

/// <summary>
/// The view for editing an English dictionary deck, bound to <see cref="EnglishDeckViewModel"/>.
/// </summary>
/// <remarks>
/// Registers a handler for <see cref="EnglishDeckViewModel.ClearMarkersInteraction"/> that clears the
/// embedded <see cref="andecr.Controls.MarkerList"/> control's chip list whenever the view model resets
/// the card editor.
/// </remarks>
public partial class EnglishDeckView : ReactiveUserControl<EnglishDeckViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnglishDeckView"/> class.
    /// </summary>
    /// <remarks>
    /// Initializes the XAML-defined components and, while the view is activated, registers a handler
    /// for <see cref="EnglishDeckViewModel.ClearMarkersInteraction"/> that calls
    /// <see cref="andecr.Controls.MarkerList.ClearMarkers"/> on <c>MarkerListControl</c>.
    /// </remarks>
    public EnglishDeckView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
            d(ViewModel!.ClearMarkersInteraction.RegisterHandler(context =>
            {
                MarkerListControl.ClearMarkers();
                context.SetOutput(Unit.Default);
            })));
    }
}