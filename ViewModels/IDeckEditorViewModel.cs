using System.Reactive;
using ReactiveUI;

namespace andecr.ViewModels;

/// <summary>
/// Общий контракт для ViewModel'ей редакторов языковых колод (эстонской, английской
/// и последующих). Реализуя этот интерфейс, каждый редактор автоматически получает
/// поддержку общего бокового меню <see cref="andecr.Views.Controls.SideMenuView"/>.
/// </summary>
public interface IDeckEditorViewModel
{
    /// <summary>Создать новую карточку — сбросить редактор в исходное состояние.</summary>
    ReactiveCommand<Unit, Unit> NewCardCommand { get; }

    /// <summary>Сохранить редактируемую карточку в текущую колоду.</summary>
    ReactiveCommand<Unit, Unit> SaveCardCommand { get; }

    /// <summary>Перейти на экран выбора колоды.</summary>
    ReactiveCommand<Unit, Unit> GoToDecksCommand { get; }

    /// <summary>Выйти из приложения.</summary>
    ReactiveCommand<Unit, Unit> ExitCommand { get; }
}
