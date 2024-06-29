using System.Windows;

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

            switch (mode)
            {
                case Mode.Configuration:
                    MessageBox.Show("Конфигурация...");
                    Shutdown();
                    return;

                case Mode.Preview:
                    break;

                case Mode.FullScreen:
                    break;
            }

        }

    }
}

