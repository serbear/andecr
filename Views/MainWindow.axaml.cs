using andecr.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace andecr.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
