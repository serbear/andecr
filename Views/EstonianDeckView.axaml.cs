using andecr.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace andecr.Views;

public partial class EstonianDeckView : ReactiveUserControl<EstonianDeckViewModel>
{
    public EstonianDeckView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
