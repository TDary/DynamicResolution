using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicResolution
{
    class MainController:Form
    {
        static Dictionary<int, string> supportedResolutions = new Dictionary<int, string>();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        // 定义DEVMODE结构体[citation:1][citation:2]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }

        // 分辨率列表初始化
        public static void MainForm()
        {
            //InitializeComponent();
            LoadSupportedResolutions();
        }

        // 加载支持的分辨率[citation:2]
        private static void LoadSupportedResolutions()
        {
            DEVMODE devMode = new DEVMODE();
            int modeIndex = 0;
            while (EnumDisplaySettings(null, modeIndex, ref devMode))
            {
                string resolution = $"{devMode.dmPelsWidth}x{devMode.dmPelsHeight}";
                int currentHashCode = resolution.GetHashCode();
                if (!supportedResolutions.ContainsKey(currentHashCode))
                {
                    supportedResolutions.Add(currentHashCode,resolution);
                }
                modeIndex++;
            }
        }



        // 应用分辨率按钮事件
        private static void btnApply_Click()
        {
            var selectedResolution = "";
            foreach (var item in supportedResolutions.Values)
            {
                Console.WriteLine($"SupportedResolution:{item}");
                selectedResolution = item;
            }
            //if (comboBoxResolutions.SelectedItem == null) return;
            string[] res = selectedResolution.Split('x');
            SetResolution(int.Parse(res[0]), int.Parse(res[1]));
        }

        void AddResolutionInPool(in string resolution_val)
        {
            foreach (var item in supportedResolutions.Values)
            {

            }
        }

        // 设置分辨率核心方法[citation:4]
        private static void SetResolution(int width, int height)
        {
            DEVMODE devMode = new DEVMODE();
            devMode.dmSize = (short)Marshal.SizeOf(devMode);
            devMode.dmPelsWidth = width;
            devMode.dmPelsHeight = height;
            devMode.dmFields = 0x00080000 | 0x00100000; // DM_PELSWIDTH | DM_PELSHEIGHT

            int result = ChangeDisplaySettingsEx(null, ref devMode, IntPtr.Zero, 0, IntPtr.Zero);
            if (result == 0) // DISP_CHANGE_SUCCESSFUL
            {
                MessageBox.Show("分辨率设置成功");
            }
            else
            {
                MessageBox.Show($"设置失败，错误代码：{result}");
            }
        }

        static void Main(string[] args)
        {
            MainForm();
            btnApply_Click();
        }

    }
}
