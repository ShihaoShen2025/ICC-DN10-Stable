using Hardcodet.Wpf.TaskbarNotification;
using ICC_DN10.Helpers;
using ICC_DN10.Services;
using ICC_DN10.Services;
using iNKORE.UI.WPF.Modern.Controls;
using System.Reflection;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace ICC_DN10
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;

        public static string[] StartArgs = null;
        public static string RootPath = Environment.GetEnvironmentVariable("APPDATA") + "\\ICC-DN10\\";

        public static ISettingsService SettingsService { get; private set; }
        public static IPowerPointService PowerPointService { get; private set; }

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ICC_DN10.MainWindow.ShowNewMessage("抱歉，出现未预期的异常，可能导致 ICC-DN10 运行不稳定。\n建议保存墨迹后重启应用。", true);
            LogHelper.NewLog(e.Exception.ToString());
            e.Handled = true;
        }

        private TaskbarIcon _taskbar;

        void App_Startup(object sender, StartupEventArgs e)
        {
            SettingsService = new SettingsService();
            PowerPointService = new PowerPointService();

            /*if (!StoreHelper.IsStoreApp) */
            RootPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            LogHelper.NewLog(string.Format("ICC-DN10 Starting (Version: {0})", Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            // 检查Windows版本是否满足最低要求
            if (!WindowsVersionChecker.CheckWindowsVersion())
            {
                WindowsVersionChecker.ShowVersionErrorAndExit();
                return;
            }

            bool ret;
            mutex = new System.Threading.Mutex(true, "ICC-DN10", out ret);

            if (!ret && !e.Args.Contains("-m")) //-m multiple
            {
                LogHelper.NewLog("Detected existing instance");
                MessageBox.Show("已有一个程序实例正在运行");
                LogHelper.NewLog("ICC-DN10 automatically closed");
                Environment.Exit(0);
            }

            _taskbar = (TaskbarIcon)FindResource("TaskbarTrayIcon");

            StartArgs = e.Args;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            try
            {
                if (System.Windows.Forms.SystemInformation.MouseWheelScrollLines == -1)
                    e.Handled = false;
                else
                    try
                    {
                        ScrollViewerEx SenderScrollViewer = (ScrollViewerEx)sender;
                        SenderScrollViewer.ScrollToVerticalOffset(SenderScrollViewer.VerticalOffset - e.Delta * 10 * System.Windows.Forms.SystemInformation.MouseWheelScrollLines / (double)120);
                        e.Handled = true;
                    }
                    catch { }
            }
            catch { }
        }
    }
}
