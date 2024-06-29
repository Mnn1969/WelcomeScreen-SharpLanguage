using System.Windows;
using System.Windows.Interop;
using WelcomeScreenСSharpLanguage.API;
using WelcomeScreenСSharpLanguage.API.pInvoke;

namespace WelcomeScreenСSharpLanguage;
public partial class App
{
    private enum Mode
    {
        Configuration,
        FullScreen,
        Preview
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mode = Mode.Configuration;

        nint parent_handle = default;

        //var args = Environment.GetCommandLineArgs();
        var args = e.Args;
        var args_count = args.Length;
        for (var i = 0; i < args_count; i++)
        {
            var arg = args[i].AsSpan();
            if (arg.StartsWith("/c", StringComparison.CurrentCultureIgnoreCase))
            {
                mode = Mode.Configuration;

                if (arg.IndexOf(':') is > 0 and var s_index && nint.TryParse(arg[(s_index + 1)..], out var s_handle))
                    parent_handle = s_handle;
                else if (i + 1 < args_count && nint.TryParse(args[i + 1], out var p_handle))
                    parent_handle = p_handle;
            }
            else if (arg.StartsWith("/p", StringComparison.CurrentCultureIgnoreCase))
            {
                mode = Mode.Preview;

                if (arg.IndexOf(':') is > 0 and var s_index && nint.TryParse(arg[(s_index + 1)..], out var s_handle))
                    parent_handle = s_handle;
                else if (i + 1 < args_count && nint.TryParse(args[i + 1], out var p_handle))
                    parent_handle = p_handle;
            }
            else if (arg.StartsWith("/s", StringComparison.CurrentCultureIgnoreCase))
                mode = Mode.FullScreen;
        }

        switch (mode)
        {
            case Mode.Configuration:
                MessageBox.Show("Конфигурация...");
                Shutdown();
                return;

            case Mode.Preview:
                CreateWindow(parent_handle).Show();
                break;

            case Mode.FullScreen:
                CreateMainWindos(parent_handle);
                break;

        }

    }

    private static void CreateMainWindos(nint ParentHandle)
    {
        foreach (var (left, top, width, height) in Screen.AllScreens.Select(s => s.Bounds))
        {
            var window = CreateWindow(ParentHandle);
            (window.Left, window.Top, window.Width, window.Height) = (left, top, width, height);

            window.Show();
        }

    }

    private static Window CreateWindow(nint ParentHandle)
    {
        var window = new MainWindow();

        if (ParentHandle == default)
        {
            window.Loaded += (s, _) => ((Window)s).WindowState = WindowState.Maximized;
            window.MouseDown += (_, _) => Current.Shutdown();
            window.KeyDown += (_, _) => Current.Shutdown();

            return window;
        }

        User32.GetClientRect(ParentHandle, out var parent_rect);

        var parent = new HwndSource(new("sourceParams")
        {
            PositionX = 0,
            PositionY = 0,
            Height = parent_rect.Bottom - parent_rect.Top,
            Width = parent_rect.Right - parent_rect.Left,
            ParentWindow = ParentHandle,
            WindowStyle = (int)(WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN)
        })
        {
            RootVisual = window.MainBorder
        };

        parent.Disposed += (_, _) => window.Close();

        return window;
    }
}

