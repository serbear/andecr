using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace andecr.ViewModels.Controls;

public class AndecrTextBoxViewModel: ViewModelBase, IAndecrTextBoxViewModel
{
    public ReactiveCommand<Unit, Unit> PasteTextCommand { get; }

    public AndecrTextBoxViewModel(Func<Task<string?>> getClipboardTextAsync, Action<string?> setText)
    {
        PasteTextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var text = await getClipboardTextAsync();
            if (text is not null)
            {
                setText(text);
            }
        });
    }
}