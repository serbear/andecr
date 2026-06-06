using Avalonia;
using System;
using ReactiveUI.Avalonia;

namespace andecr;

internal class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            /*
             * В ReactiveUI.Avalonia метод UseReactiveUI() принимает Action<ReactiveUIBuilder> и namespace для импорта — Avalonia.ReactiveUI. Но в документации также показан Alternative: Traditional Setup — без RxAppBuilder, что идеально подходит для простого приложения без DI.
             * Для вашего приложения без DI достаточно передать пустой колбэк.
             */
            .UseReactiveUI(_ => { });
}
