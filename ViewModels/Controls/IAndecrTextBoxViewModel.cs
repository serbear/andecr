namespace andecr.ViewModels.Controls;

using System.Reactive;
using ReactiveUI;

public interface IAndecrTextBoxViewModel
{
    ReactiveCommand<Unit, Unit> PasteTextCommand { get; }
}