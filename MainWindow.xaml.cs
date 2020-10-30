//x64版本
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
//using System.Timers;

namespace WpfApp_Repeat_Test_Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        UInt32 mbopen = 0;//设备开关
        bool istarted = false;//can通道开关

        UInt32 mdevicetype = 3;//设备类型
        UInt32 mdeviceind = 0;//设备编号
        UInt32 mdevicereserved = 0;//保留参数
        UInt32 mcanind = 0;//can通道编号

        private DispatcherTimer timer;



        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 连接/断开按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_open_Click(object sender, RoutedEventArgs e)
        {
            if (mbopen == 1)//如果设备已开启就关闭设备
            {
                controlCAN.VCI_CloseDevice(mdevicetype, mdeviceind);//关闭设备
                timer.Stop();
                mbopen = 0;
                //Btn_Connect.Content = "连接";
            }
            else
            {
                UInt32 rel = controlCAN.VCI_OpenDevice(mdevicetype, mdeviceind, mdevicereserved);//打开设备
                if (rel != 1)
                {
                    MessageBox.Show(("打开设备失败!"), ("警告"),
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();//初始化can参数的结构体

                config.AccCode = 0x00000000;//验收码
                config.AccMask = 0xffffffff;//屏蔽码
                config.Timing0 = 0x00;//波特率
                config.Timing1 = 0x1c;//500k
                config.Filter = 1;//滤波方式，单滤波
                config.Mode = 0;//模式，正常模式

                UInt32 ret = controlCAN.VCI_InitCAN(mdevicetype, mdeviceind, mcanind, ref config);//初始化can
                if (ret != 1)
                {
                    MessageBox.Show("InitCAN失败", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                mbopen = 1;


                //}

                if (istarted)
                {
                    controlCAN.VCI_ResetCAN(mdevicetype, mdeviceind, mcanind);
                }
                else
                {
                    UInt32 rtn = controlCAN.VCI_StartCAN(mdevicetype, mdeviceind, mcanind);
                    istarted = !istarted;
                    if (rtn != 1)
                    {
                        MessageBox.Show("StartCAN失败", "错误",
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }

                //实例化对象
                timer = new DispatcherTimer();
                //设置触发时间
                timer.Interval = TimeSpan.FromSeconds(0.0001);
                //设置触发事件
                timer.Tick += timer_Tick;
                //启动
                timer.Start();

                //Btn_Connect.Content = "断开";
            }

            
            Btn_Open.Content = mbopen == 1 ? "断开" : "连接";//调节按钮
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            UInt32 res = new UInt32();
            res = controlCAN.VCI_GetReceiveNum(mdevicetype, mdeviceind, mcanind);
            if (res == 0)//res==0，退出，原因未知
                return;
            //res = VCI_Receive(m_devtype, m_devind, m_canind, ref m_recobj[0],50, 100);

            /////////////////////////////////////
            UInt32 con_maxlen = 50;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);
            //UInt32 pp = (UInt32)pt;




            res = controlCAN.VCI_Receive(mdevicetype, mdeviceind, mcanind, pt, con_maxlen, 100);
            ////////////////////////////////////////////////////////

            String str = "";
            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)(pt.ToInt64() + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));//在x64环境中pt需要通过.ToInt64()转换！

                str = "接收到数据: ";         
                str += "  帧ID:0x" + System.Convert.ToString((Int32)obj.ID, 16);
                str += "  帧格式:";
                if (obj.RemoteFlag == 0)
                    str += "数据帧 ";
                else
                    str += "远程帧 ";
                if (obj.ExternFlag == 0)
                    str += "标准帧 ";
                else
                    str += "扩展帧 ";

                //////////////////////////////////////////
                if (obj.RemoteFlag == 0)
                {
                    str += "数据: ";
                    byte len = (byte)(obj.DataLen % 9);
                    byte j = 0;
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[0], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[1], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[2], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[3], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[4], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[5], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[6], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[7], 16);

                }

                Lbx_CanFrame.Items.Add(str);
                
                //自动滚动Lbx_CanFrame的滚动条
                Lbx_CanFrame.SelectedIndex = Lbx_CanFrame.Items.Count - 1;
                Lbx_CanFrame.SelectionChanged += ListBox_SourceUpdated;
                //this.Lbx_CanFrame.to = this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight);
                //Lbx_CanFrame.SelectedIndex = -1;
            }
            Marshal.FreeHGlobal(pt);
        }

        //DataSource.CollectionChanged += ListBox_SourceUpdated;
        private void ListBox_SourceUpdated(object sender, EventArgs e)
        {
            Decorator decorator = (Decorator)VisualTreeHelper.GetChild(Lbx_CanFrame, 0);
            ScrollViewer scrollViewer = (ScrollViewer)decorator.Child;
            scrollViewer.ScrollToEnd();
        }

    }
}
