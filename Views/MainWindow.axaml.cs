using andecr.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace andecr.Views;

/// <summary>
/// Represents the main window view of the application.
/// </summary>
/// <remarks>
/// This class serves as the primary view for the application, bound to MainWindowViewModel.
/// </remarks>
public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(_ => { });
    }
}
