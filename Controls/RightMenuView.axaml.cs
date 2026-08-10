using Avalonia.Controls;

namespace andecr.Controls;

/// <summary>
/// Right menu of the main window. The DataContext is inherited from MainWindow (= MainWindowViewModel)
/// and is not set separately.
/// The control is unaware of specific screens; see RightMenuView.axaml and IHasRightMenu.
/// </summary>
public partial class RightMenuView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RightMenuView"/> class.
    /// </summary>
    public RightMenuView()
    {
        InitializeComponent();
    }
}