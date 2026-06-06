using andecr.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace andecr.Views;

public partial class DeckSelectionView : ReactiveUserControl<DeckSelectionViewModel>
{
    public DeckSelectionView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
