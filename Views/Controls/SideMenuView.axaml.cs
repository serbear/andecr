using Avalonia.Controls;

namespace andecr.Views.Controls;

/// <summary>
/// Общее вертикальное меню редактора колоды. Не привязан к конкретной ViewModel —
/// работает с любым DataContext, реализующим <see cref="andecr.ViewModels.IDeckEditorViewModel"/>,
/// поэтому переиспользуется во всех редакторах языковых колод.
/// </summary>
public partial class SideMenuView : UserControl
{
    public SideMenuView()
    {
        InitializeComponent();
    }
}
