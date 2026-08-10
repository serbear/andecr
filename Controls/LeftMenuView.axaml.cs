using Avalonia.Controls;

namespace andecr.Controls;

/// <summary>
/// Left menu of the main window. The DataContext is inherited from MainWindow (= MainWindowViewModel)
/// and is not set separately.
/// The control is unaware of specific screens; see LeftMenuView.axaml and IHasLeftMenu.
/// </summary>
public partial class LeftMenuView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LeftMenuView"/> class.
    /// </summary>
    public LeftMenuView()
    {
        InitializeComponent();
    }
}