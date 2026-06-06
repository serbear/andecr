using andecr.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace andecr.Views;

public partial class EnglishDeckView : ReactiveUserControl<EnglishDeckViewModel>
{
    public EnglishDeckView()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
