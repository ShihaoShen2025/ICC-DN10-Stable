using OSVersionExtension;
using System;
using System.Windows;

namespace ICC_DN10.Helpers
{
    internal class WindowsVersionChecker
    {
        // 要求的最低Windows版本：10.0.19044.6093
        private const int RequiredMajorVersion = 10;
        private const int RequiredMinorVersion = 0;
        private const int RequiredBuildNumber = 19044;
        private const int RequiredUBR = 6093; // Update Build Revision

        /// <summary>
        /// 检查当前Windows版本是否满足最低要求
        /// </summary>
        /// <returns>如果满足要求返回true，否则返回false</returns>
        public static bool CheckWindowsVersion()
        {
            try
            {
                // 获取当前操作系统版本
                var osVersion = OSVersion.GetOSVersion();
                var majorVersion10Props = OSVersion.MajorVersion10Properties();
                int currentUBR = 0;
                // UBR属性可能是字符串类型，尝试解析为整数
                if (majorVersion10Props.UBR != null && int.TryParse(majorVersion10Props.UBR.ToString(), out int parsedUBR))
                {
                    currentUBR = parsedUBR;
                }

                // 检查主要版本号
                if (osVersion.Version.Major < RequiredMajorVersion)
                    return false;

                // 如果主要版本号相同，检查次要版本号
                if (osVersion.Version.Major == RequiredMajorVersion && osVersion.Version.Minor < RequiredMinorVersion)
                    return false;

                // 如果主要和次要版本号相同，检查构建号
                if (osVersion.Version.Major == RequiredMajorVersion && 
                    osVersion.Version.Minor == RequiredMinorVersion && 
                    osVersion.Version.Build < RequiredBuildNumber)
                    return false;

                // 如果主要、次要和构建版本号相同，检查UBR
                if (osVersion.Version.Major == RequiredMajorVersion && 
                    osVersion.Version.Minor == RequiredMinorVersion && 
                    osVersion.Version.Build == RequiredBuildNumber && 
                    currentUBR < RequiredUBR)
                    return false;

                // 所有检查都通过
                return true;
            }
            catch (Exception ex)
            {
                // 如果发生异常，记录日志并返回false
                LogHelper.NewLog($"检查Windows版本时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 显示版本不兼容的错误消息并退出应用程序
        /// </summary>
        public static void ShowVersionErrorAndExit()
        {
            string message = $"此应用程序需要Windows 10版本10.0.19044.6093或更高版本。\n\n" +
                           $"请更新您的Windows系统后再运行此应用程序。";

            MessageBox.Show(message, "系统版本不兼容", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }
}