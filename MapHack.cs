using System;
using System.Windows;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;
//using System.Collections.Generic;

namespace eflayMH_WPF
{
    public partial class Window1
    {
        #region 全图功能

        /*
         * 本来初始化后就直接用初始化的了，不过貌似。。。。
         * 所以还是再枚举次模块，把所有game.dll都改一遍
         * */

        public int War3MHon(bool ON)
        {
            if (war3 == null)
            {
                war3 = new ProcessC(Process.GetProcessesByName("war3")[0]);

            }
            //没找到game.dll的话默认值
            war3.DllBaseAddress = 0x6F000000;
            war3.reflesh();

            foreach (Module md in war3.Modules)
            {
                string filename = Path.GetFileName(md.FullName).ToLower();

                if (filename == "vlanproxy.dll" || filename == "tvlanproxy.dll")
                {
                    if (DisjoyZMR(war3, md))
                    {
                        break;
                    }
                    else
                    {
                        if (MessageBox.Show("不支持的平台版本，是否继续", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question)
                            == MessageBoxResult.No)
                        {
                            return 3;
                        }
                    }
                }
            }


            foreach (Module md in war3.Modules)
            {
                string filename = Path.GetFileName(md.FullName).ToLower();
                if (filename == "game.dll" || filename == "game124.dll")//||filename.Contains("game")
                {

                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(md.FullName);
                    try
                    {
                        //if (fvi.OriginalFilename.ToLower() == "game.dll")
                        //{
                        war3.DllBaseAddress = md.BaseAddress;
                        war3.Version = fvi.FileVersion.Replace(", ", ".");
                        War3MHON(ON);
                        //}
                    }
                    catch (NullReferenceException)
                    { }
                }



            }
            return 1;
        }










        private unsafe int War3MHON(bool ON)
        {
            ProcessVersion = war3.Version;

            Dispatcher.Invoke(new ThreadStart(
                delegate()
                {
                    if (IsCheckedDspt( checkBoxqzbb))
                    {
                        ProcessVersion = ComboBoxVersion.Text;
                    }
                }
            ), null);


            if (ON)
            {


                switch (ProcessVersion)
                {
                    case "1.20.4.6074":
                        //Dispatcher.Invoke(new ThreadStart(delegate() { War3120eMHON(); }), null);
                        War3120eMHON();
                        break;
                    case "1.24.1.6374":
                        //Dispatcher.Invoke(new ThreadStart(delegate() { war3124bMHON(); }), null);
                        war3124bMHON();
                        break;
                    case "1.24.4.6387":
                        //Dispatcher.Invoke(new ThreadStart(delegate() { War3124eMHON(); }), null);
                        War3124eMHON();
                        break;
                    case "1.25.1.6397":
                        //Dispatcher.Invoke(new ThreadStart(delegate() { War3125bMHON(); }), null);
                        War3125bMHON();
                        break;
                    case "1.26.0.6401":
                        //Dispatcher.Invoke(new ThreadStart(delegate() { War3126aMHON(); }), null);
                        War3126aMHON();
                        break;
                    default:
                        //throw new ProcessCException("魔兽版本不支持" + war3.Pid.ToString() + ProcessVersion);
                        Dispatcher.Invoke(new ThreadStart(delegate() { textBlock1.Text = "魔兽版本不支持,PID:" + war3.Pid + ",版本:" + ProcessVersion; }), null);
                        return 0;
                    //break;
                }

                Dispatcher.Invoke(new ThreadStart(delegate() { textBlock1.Text = "MH已经开启,PID:" + war3.Pid + ",版本:" + ProcessVersion; }), null);

                isMHON = true;
            }
            else
            {
                switch (ProcessVersion)
                {
                    case "1.20.4.6074":
                        War3120eMHOff();
                        break;
                    case "1.24.1.6374":
                        war3124bMHOff();
                        break;
                    case "1.24.4.6387":
                        War3124eMHOff();
                        break;
                    case "1.25.1.6397":
                        War3125bMHOff();
                        break;
                    case "1.26.0.6401":
                        War3126aMHOff();
                        break;
                    default:
                        throw new ProcessCException("魔兽版本不支持" + war3.Pid.ToString() + ProcessVersion);
                }
                Dispatcher.Invoke(new ThreadStart(delegate() { textBlock1.Text = "MH已经关闭"; }), null);

                ProcessVersion = "";

                isMHON = false;
            }

            return 0;
        }


        public bool IsCheckedDspt(System.Windows.Controls.CheckBox cb)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                bool tmp = false;
                this.Dispatcher.Invoke((ThreadStart)delegate() {tmp = IsCheckedDspt(cb); }, null);
                return tmp;
            }
            else
            {
                return cb.IsChecked==true;
            }
            
        }



        private void War3126aMHOff()
        {
            war3.patch(0x74ca1a, 0x90, 0x4c);
            war3.patch(0x3a159b, 0x23, 0xca);
            war3.patch(0x399a98, 0x74);//xy
            war3.patch(0x3999cd, 0x75, 0x0c);//xy
            war3.patch(0x3a14f0, 0xeb, 0x09);
            war3.patch(0x3999a8, 0x75);//bx
            war3.patch(0x3999d5, 0x0f, 0x85, 0xb1, 0x00, 0x00, 0x00);//xy

            ////神符
            war3.patch(0x3a14db, 0x75);
            war3.patch(0x409d24, 0x75, 0x0a);

            //分辨幻想
            war3.patch(0x282a5c, 0xc3, 0xcc);


            war3.patch(0x2851b2, 0x75);//dx
 
            ////小地图显示单位
            war3.patch(0x36143b, 0xb8, 0x01, 0x00, 0x00);
            ////显隐
            war3.patch(0x361851, 0x85, 0xC0, 0xF, 0x84);
            ////防秒
            war3.patch(0x5B6c3A, 0x83, 0xf9, 0x12, 0x0f, 0x87);
            //迷雾
            war3.patch(0x356525, 0x88, 0x01);
            ////数显
            war3.patch(0x339114, 0x57, 0x8d, 0x4c, 0x24, 0x18);
            war3.patch(0x3392b4, 0x57, 0x8d, 0x4c, 0x24, 0x18);
            //敌方信号
            war3.patch(0x43ee96, 0x85, 0xc0, 0x0f, 0x84);
            war3.patch(0x43eeA9, 0x85, 0xc0, 0x0f, 0x84);
            //无限取消
            war3.patch(0x57addc, 0x7a);//7a
            war3.patch(0x5B25d7, 0x00);//00
            war3.patch(0x5B25eb, 0x01);//01
            //顶显资源
            war3.patch(0x358fad, 0x85, 0xc0); //恢复：85 C0
            war3.patch(0x35969f, 0x85, 0xc0);
            war3.patch(0x35975F, 0x85, 0xc0);
            war3.patch(0x359890, 0x85, 0xc0);
            war3.patch(0x28DfdA, 0x85, 0xc0);
            //建筑显资
            war3.patch(0x35FA44, 0x75);//恢复： 75

            ////过检测

            //PanCameraToTimed
            war3.patch(0x3B4740, new byte[] { 0x33, 0xd2 });

            //SetUnitAnimation
            war3.patch(0x3c61a0, new byte[] { 0x8b });

            //SetUserControlForceOff
            war3.patch(0x3b3880, new byte[] { 0x33, 0xd2 });

            ////SelectUnit
            war3.patch(0x3c7987, new byte[] { 0x6a, 0x00 });
            war3.patch(0x3c77A7, new byte[] { 0x6a, 0x00 });
            //ClearSelection
            war3.patch(0x3BBaa0, 0x8b);
            //ah
            war3.patch(0x3c616c, 0x3d);
            war3.patch(0x3c6171, 0x76);
            war3.patch(0x3cb642, 0x74);
        }

        private void War3126aMHON()
        {
            uint oldpro = 0;
            war3.VirtualProtectEx(war3.DllBaseAddress + 0x01000, 0x87E000, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldpro);


            //大地图

            if (IsCheckedDspt(checkBoxxsdw))
            {
                ////大地图显示单位
                war3.patch(0x3a159b, 0x90, 0x90);
                war3.patch(0x3a14f0, 0x87, 0xdb);

            }
            else if (IsCheckedDspt(checkBoxbxdw))
            {
                war3.patch(0x3999a8, 0xeb);
            }


            if (IsCheckedDspt(checkBoxxsyx))
            {
                //大地图显示隐形
                war3.patch(0x399a98, 0x71);
                war3.patch(0x3999cd, 0x90, 0x90);
                war3.patch(0x3999d5, 0x75, 0x32, 0x90, 0x90, 0x90, 0x90);
            }

            if (IsCheckedDspt(checkBoxqcmw))
            { // 大地图去除迷雾
                war3.patch(0x74ca1a, 0x15, 0x50);
            }

            if (IsCheckedDspt(checkBoxswdx))
            {
                ////////////////////////////////////////////////视野外点选
                war3.patch(0x2851b2, 0x71);

            }
            if (IsCheckedDspt(checkBoxswxx))
            {
                //视野外显血

            }

            if (IsCheckedDspt(checkBoxfbhx))
            {
                //分辨幻象
                war3.patch(0x282a5c, 0x40, 0xc3);
            }

            if (IsCheckedDspt(checkBoxxssf))
            //显示神符
            {
                war3.patch(0x3a14db, 0xeb);
                war3.patch(0x409d24, 0x90, 0x90);
            }

            //小地图

            if (IsCheckedDspt(checkBoxmxsdw))
            {
                //显示单位
                //war3.patch(0x36143c, 0x00);
                war3.patch(0x36143b, 0x33, 0xc0, 0x90, 0x90, 0x90);

            }

            if (IsCheckedDspt(checkBoxmxsyx))
            {
                //显示隐形
                //war3.patch(0x36143c, 0x00);// 01 xy1
                war3.patch(0x361851, 0x33, 0xC0, 0xF, 0x85);//xi
            }


            if (IsCheckedDspt(checkBoxmqcmw))
            {
                //去除迷雾
                war3.patch(0x356525, 0x90, 0x90);
            }

            //辅助

            //敌方头像
            if (IsCheckedDspt(checkBoxdftx))
            {

            }

            if (IsCheckedDspt(checkBoxyftx))
            {

            }



            if (IsCheckedDspt(checkBoxdfxh))
            {//敌方信号
                war3.patch(0x43ee96, 0x3b, 0xc0, 0x0f, 0x85);
                war3.patch(0x43eeA9, 0x3b, 0xc0, 0x0f, 0x85);
            }

            if (IsCheckedDspt(checkBoxwxqx))
            {
                //无限取消
                war3.patch(0x57addc, 0xeb);//7a
                war3.patch(0x5B25d7, 0x03);//00
                war3.patch(0x5B25eb, 0x03);//01
            }

            if (IsCheckedDspt(checkBoxyxjy))
            {//允许交易
                war3.patch(0x34dda2, 0xb8, 0xc8, 0x00, 0x00, 0x00, 0x90, 0xeb, 0x06, 0xb8, 0x64, 0x00, 0x00, 0x00, 0x90);
            }

            if (IsCheckedDspt(checkBoxxsjn))
            {
                //显示技能
                war3.patch(0x2026dc, 0x87, 0xdb, 0x87, 0xdb, 0x87, 0xdb);
                war3.patch(0x34f2a8, 0x87, 0xdb);
                //技能CD
                war3.patch(0x28e1de, 0x71);
                war3.patch(0x34f2e9, 0x00);
            }

            if (IsCheckedDspt(checkBoxtrts))
            {
                //他人提示

            }
            if (IsCheckedDspt(checkBoxsxys))
            {
                //    //////////////////////////////////////////////////////////数显移速
                war3.patch(0x86c693, 0x25, 0x30, 0x2e, 0x32, 0x66, 0x7c, 0x52, 0x00);
                war3.patch(0x86c6a0, new byte[] { 0x8d, 0x4c, 0x24, 0x18, 0xd9, 0x44, 0x24, 0x60, 0x83, 0xec, 0x08, 0xdd, 0x1c, 0x24, 0x68 });

                int tmp = 0x86c693 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x86c6aF, bt1);
                war3.patch(0x86c6b3, new byte[] { 0x57, 0x51, 0xE8, 0xEC, 0xEE, 0xE7, 0xFF, 0x83, 0xc4, 0x14, 0x58, 0x57, 0x8d, 0x4c, 0x24, 0x18, 0xff, 0xe0 });
                war3.patch(0x339114, 0xE8, 0x87, 0x35, 0x53, 0x00);
            }

            if (IsCheckedDspt(checkBoxsxgs))
            {
                ////////////////////数显攻速
                war3.patch(0x86c693, 0x25, 0x30, 0x2e, 0x32, 0x66, 0x7c, 0x52, 0x00);
                war3.patch(0x86c6a0, new byte[] { 0x8d, 0x4c, 0x24, 0x18, 0xd9, 0x44, 0x24, 0x60, 0x83, 0xec, 0x08, 0xdd, 0x1c, 0x24, 0x68 });
                int tmp = 0x86c693 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x86c6aF, bt1);
                war3.patch(0x86c6b3, new byte[] { 0x57, 0x51, 0xE8, 0xEC, 0xEE, 0xE7, 0xFF, 0x83, 0xc4, 0x14, 0x58, 0x57, 0x8d, 0x4c, 0x24, 0x18, 0xff, 0xe0 });
                war3.patch(0x3392b4, 0xE8, 0xE7, 0x33, 0x53, 0x00);

            }

            //防秒开始
            if (IsCheckedDspt(checkBoxfmks))
            {
                war3.patch(0x86C6eD, 0x83, 0xF9, 0x00, 0x75, 0x03, 0x6A, 0x03, 0x59, 0x83, 0xF9, 0x12, 0x0F, 0x87, 0x91, 0xA7, 0xD4, 0xFF, 0xE9, 0x40, 0xA5, 0xD4, 0xFF);
                war3.patch(0x5B6c3A, 0xE9, 0xAE, 0x5A, 0x2B, 0x00);
            }

            //顶显资源
            if (IsCheckedDspt(checkBoxdxzy))
            {
                war3.patch(0x358fad, 0xeb, 0x02); //恢复：85 C0
                war3.patch(0x35969f, 0xeb, 0x02);
                war3.patch(0x35975F, 0xeb, 0x02);
                war3.patch(0x359890, 0xeb, 0x02);
                war3.patch(0x28DfdA, 0xeb, 0x02);
            }

            //建显资源
            if (IsCheckedDspt(checkBoxjzxz))
            {
                war3.patch(0x35FA44, 0xeb);//恢复： 75
            }


            //特殊

            //过检测

            //PanCameraToTimed
            if (IsCheckedDspt(checkBoxfqpm))
                war3.patch(0x3B4740, new byte[] { 0xc3, 0x90 });//510

            //SetUnitAnimation
            if (IsCheckedDspt(checkBoxfmxs))
                war3.patch(0x3c61a0, 0xc3);

            //SetUserControlForceOff  EnableUserControl(bool)
            if (IsCheckedDspt(checkBoxfsk))
                war3.patch(0x3b3880, 0xc3);

            //SelectUnit
            if (IsCheckedDspt(checkBoxfxdw))
            {
                war3.patch(0x3c7987, new byte[] { 0xeb, 0x11 });
                war3.patch(0x3C79a7, new byte[] { 0xeb, 0x11 });

            }

            //ClearSelection
            if (IsCheckedDspt(checkBoxfqx))
            {
                war3.patch(0x3BBaa0, 0xc3);
            }

            //反-AH
            if (IsCheckedDspt(checkBoxgah))
            {
                war3.patch(0x3c639c, 0xb8);
                war3.patch(0x3c63a1, 0xeb);
                war3.patch(0x3cb872, 0xeb);
            }
        }

        private void War3125bMHOff()
        {

            war3.patch(0x74c7ea, 0x90, 0x4c);
            war3.patch(0x3a136b, 0x23, 0xca);
            war3.patch(0x399868, 0x74);
            war3.patch(0x3a12c0, 0xeb, 0x09);
            war3.patch(0x39979d, 0x75, 0x0c);
            war3.patch(0x3997a5, 0x0f, 0x85, 0xb1, 0x00, 0x00, 0x00);

            //神符
            war3.patch(0x3a12ab, 0x75);
            war3.patch(0x409af4, 0x75, 0x0a);

            war3.patch(0x28282c, 0xc3, 0xcc);


            war3.patch(0x284f82, 0x75);
            war3.patch(0x399778, 0x75); //bx
            //小地图显示单位
            war3.patch(0x36120b, 0xb8, 0x01, 0x00, 0x00);
            //显隐
            war3.patch(0x361621, 0x85, 0xc0, 0x0f, 0x84);
            //小地图迷雾
            war3.patch(0x3562f5, 0x88, 0x01);
            //防秒
            war3.patch(0x5B6A0A, 0x83, 0xf9, 0x12, 0x0f, 0x87);

            //数显
            war3.patch(0x338ee4, 0x57, 0x8d, 0x4c, 0x24, 0x18);//20110402 由339C54改为
            war3.patch(0x339084, 0x57, 0x8d, 0x4c, 0x24, 0x18); // 20110402 修改过
            //敌方信号
            war3.patch(0x43EC66, 0x85,0xc0,0xf,0x84);
            war3.patch(0x43F979, 0x85,0xc0,0xf,0x84);
            //顶显资源
            war3.patch(0x358d7d, 0x85, 0xc0); //恢复：85 C0
            war3.patch(0x35946f, 0x85, 0xc0);
            war3.patch(0x35952F, 0x85, 0xc0);
            war3.patch(0x359660, 0x85, 0xc0);
            war3.patch(0x28DDAA, 0x85, 0xc0);

            //建筑显资源
            war3.patch(0x35f814, 0x75);//恢复： 75
            //过检测

            //PanCameraToTimed
            war3.patch(0x3B4510, new byte[] { 0x33, 0xd2 });

            //SetUnitAnimation
            war3.patch(0x3C5F70, new byte[] { 0x8b });

            //SetUserControlForceOff
            war3.patch(0x3b3650, new byte[] { 0x33, 0xd2 });

            ////SelectUnit
            war3.patch(0x3c7757, new byte[] { 0x6a, 0x00 });
            war3.patch(0x3c7777, new byte[] { 0x6a, 0x00 });
            //ClearSelection
            war3.patch(0x3BB870, 0x8b);
            //ah
            war3.patch(0x3c616c, 0x3d);
            war3.patch(0x3c6171, 0x76);
            war3.patch(0x3cb642, 0x74);
        }

        private void War3125bMHON()
        {
            uint oldpro = 0;
            war3.VirtualProtectEx(war3.DllBaseAddress + 0x01000, 0x87E000, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldpro);


            //大地图

            if (IsCheckedDspt(checkBoxxsdw))
            {
                ////大地图显示单位
                war3.patch(0x3a136b, 0x90, 0x90);
                war3.patch(0x3a12c0, 0x87, 0xdb);

            }
            else if (IsCheckedDspt(checkBoxbxdw))
            {
                war3.patch(0x399778, 0xeb);
            }


            if (IsCheckedDspt(checkBoxxsyx))
            {
                //大地图显示隐形
                war3.patch(0x399868, 0x71);
                war3.patch(0x39979d, 0x90, 0x90);
                war3.patch(0x3997a5, 0x75, 0x32, 0x90, 0x90, 0x90, 0x90);
            }

            if (IsCheckedDspt(checkBoxqcmw))
            { // 大地图去除迷雾
                war3.patch(0x74c7ea, 0x15, 0x50);
            }

            if (IsCheckedDspt(checkBoxswdx))
            {
                ////////////////////////////////////////////////视野外点选
                war3.patch(0x284f82, 0x71);

            }
            if (IsCheckedDspt(checkBoxswxx))
            {
                //视野外显学

            }

            if (IsCheckedDspt(checkBoxfbhx))
            {
                //分辨幻象
                war3.patch(0x28282c, 0x40, 0xc3);
            }

            if (IsCheckedDspt(checkBoxxssf))
            //显示神符
            {
                war3.patch(0x3a12ab, 0xeb);
                war3.patch(0x409af4, 0x90, 0x90);
            }

            //小地图

            if (IsCheckedDspt(checkBoxmxsdw))
            {
                //显示单位
                //war3.patch(0x361F7C, 0x00);
                war3.patch(0x36120b, 0x33, 0xc0, 0x90, 0x90, 0x90);

            }

            if (IsCheckedDspt(checkBoxmxsyx))
            {
                //显示隐形
                //war3.patch(0x36120B, 0x00);// 01 xy1 未确认
                war3.patch(0x361621, 0x33, 0xc0, 0x0f, 0x85);
            }


            if (IsCheckedDspt(checkBoxmqcmw))
            {
                //去除迷雾
                war3.patch(0x3562f5, 0x90, 0x90);
            }

            //辅助

            //敌方头像
            if (IsCheckedDspt(checkBoxdftx))
            {

            }

            if (IsCheckedDspt(checkBoxyftx))
            {

            }



            if (IsCheckedDspt(checkBoxdfxh))
            {//敌方信号
                war3.patch(0x43EC66, 0x3b, 0xc0, 0xf, 0x85);
                war3.patch(0x43F979, 0x3b, 0xc0, 0xf, 0x85);
            }

            if (IsCheckedDspt(checkBoxwxqx))
            {
                //无限取消
                war3.patch(0x57ABAC, 0xeb);// 7a
                war3.patch(0x5B23a7, 0x03);//00
                war3.patch(0x5B23bb, 0x03);//01
            }

            if (IsCheckedDspt(checkBoxyxjy))
            {//允许交易
                war3.patch(0x34db72, 0xb8, 0xc8, 0x00, 0x00, 0x00, 0x90, 0xeb, 0x06, 0xb8, 0x64, 0x00, 0x00, 0x00, 0x90);
            }

            if (IsCheckedDspt(checkBoxxsjn))
            {
                //显示技能
                war3.patch(0x2024ac, 0x87, 0xdb, 0x87, 0xdb, 0x87, 0xdb);
                war3.patch(0x34f078, 0x87, 0xdb);
                //技能CD
                war3.patch(0x28dfae, 0x71);
                war3.patch(0x34f0b9, 0x00);
            }

            if (IsCheckedDspt(checkBoxtrts))
            {
                //他人提示

            }
            if (IsCheckedDspt(checkBoxsxys))
            {
                //    //////////////////////////////////////////////////////////数显移速
                war3.patch(0x86c463, 0x25, 0x30, 0x2e, 0x32, 0x66, 0x7c, 0x52, 0x00);
                war3.patch(0x86c470, new byte[] { 0x8d, 0x4c, 0x24, 0x18, 0xd9, 0x44, 0x24, 0x60, 0x83, 0xec, 0x08, 0xdd, 0x1c, 0x24, 0x68 });

                int tmp = 0x86c463 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x86c47F, bt1);
                war3.patch(0x86c483, new byte[] { 0x57, 0x51,0xE8,0xEC,0xEE,0xE7,0xFF, 0x83, 0xc4, 0x14, 0x58, 0x57, 0x8d, 0x4c, 0x24, 0x18, 0xff, 0xe0 });
                war3.patch(0x338ee4,0xE8,0x87,0x35,0x53,0x00);
            }

            if (IsCheckedDspt(checkBoxsxgs))
            {
                ////////////////////数显攻速
                war3.patch(0x86c463, 0x25, 0x30, 0x2e, 0x32, 0x66, 0x7c, 0x52, 0x00);
                war3.patch(0x86c470, new byte[] { 0x8d, 0x4c, 0x24, 0x18, 0xd9, 0x44, 0x24, 0x60, 0x83, 0xec, 0x08, 0xdd, 0x1c, 0x24, 0x68 });
                int tmp = 0x86c463 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x86c47F, bt1);
                war3.patch(0x86c483, new byte[] { 0x57, 0x51, 0xE8, 0xEC, 0xEE, 0xE7, 0xFF, 0x83, 0xc4, 0x14, 0x58, 0x57, 0x8d, 0x4c, 0x24, 0x18, 0xff, 0xe0 });
                war3.patch(0x339084,0xE8,0xE7,0x33,0x53,0x00);

            }

            //防秒开始
            if (IsCheckedDspt(checkBoxfmks))
            {
                war3.patch(0x86C4BD,0x83,0xF9,0x00,0x75,0x03,0x6A,0x03,0x59,0x83,0xF9,0x12,0x0F,0x87,0x91,0xA7,0xD4,0xFF,0xE9,0x40,0xA5,0xD4,0xFF);
                war3.patch(0x5B6A0A,0xE9,0xAE,0x5A,0x2B,0x00);
            }

            //顶显资源
            if (IsCheckedDspt(checkBoxdxzy))
            {
                war3.patch(0x358d7d, 0xeb, 0x02); //恢复：85 C0
                war3.patch(0x35946f, 0xeb, 0x02);
                war3.patch(0x35952F, 0xeb, 0x02);
                war3.patch(0x359660, 0xeb, 0x02);
                war3.patch(0x28DDAA, 0xeb, 0x02);
            }

            //建显资源
            if (IsCheckedDspt(checkBoxjzxz))
            {
                war3.patch(0x35f814, 0xeb);//恢复： 75
            }


            //特殊

            //过检测

            //PanCameraToTimed
            if (IsCheckedDspt(checkBoxfqpm))
                war3.patch(0x3B4510, new byte[] { 0xc3, 0x90 });

            //SetUnitAnimation
            if (IsCheckedDspt(checkBoxfmxs))
                war3.patch(0x3C5F70, new byte[] { 0xc3 });

            //SetUserControlForceOff  EnableUserControl(bool)
            if (IsCheckedDspt(checkBoxfsk))
                war3.patch(0x3b3650, new byte[] { 0xc3, 0x90 });//3b43c0  hy:33d2

            //SelectUnit
            if (IsCheckedDspt(checkBoxfxdw))
            {
                war3.patch(0x3c7757, new byte[] { 0xeb, 0x11 });
                war3.patch(0x3C7777, new byte[] { 0xeb, 0x11 });

            }

            //ClearSelection
            if (IsCheckedDspt(checkBoxfqx))
            {
                war3.patch(0x3BB870, 0xc3); //8B0D 3BC5E0  6F3BB870
                //war3.patch(0x2C5AB0, 0xc3);
            }

            //反-AH
            if (IsCheckedDspt(checkBoxgah))
            {
                war3.patch(0x3c616c, 0xb8);
                war3.patch(0x3c6171, 0xeb);
                war3.patch(0x3cb642, 0xeb);
            }


        }

        private unsafe void War3124eMHOff()
        {
            uint oldpro = 0;
            war3.VirtualProtectEx(war3.DllBaseAddress + 0x01000, 0x87E000, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldpro);
            //大地图
            war3.patch(0x39EBBC, new byte[] { 0x74 });
            war3.patch(0x3A2030, new byte[] { 0xeb, 0x09 });
            //war3.patch(0x3A20DB, new byte[] { 0x23, 0xca });
            war3.patch(0x3A20D2, 0x0f, 0xb7, 0x32, 0x0f, 0xb7, 0xce); //2
            //war3.patch(0x87EFDB, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);//2

            //bk
            war3.patch(0x39A4E8, 0x75);

            war3.patch(0x74D1Ba, new byte[] { 0x90, 0x6c });//,0x6c,0x7e,0xab,0x6f}); //大地图迷雾
            //int tmp = 0xAB7E6C + war3.DllBaseAddress;
           
            //byte[] tmpbt = BitConverter.GetBytes(tmp);
            //war3.patch(0x74d1b9, new byte[] { 0x8a, 0x90, tmpbt[0], tmpbt[1], tmpbt[2], tmpbt[3] });

            //大地图显隐
            //			war3.patch(0x362391, new byte[] { 0x85 }); //1
            //			war3.patch(0x362394, new byte[] { 0x84 }); //1
            //			war3.patch(0x39A51B, new byte[] { 0x8b, 0x97, 0x98, 0x01, 0x00, 0x00 }); //1
            //			war3.patch(0x39A52E, new byte[] { 0x0f, 0xb7, 0x00, 0x55, 0x50, 0x56, 0xe8, 0xf7, 0x7b, 0x00, 0x00 }); //1
            war3.patch(0x39a5d8, 0x74);//2 110404+
            war3.patch(0x39A50D, 0x75, 0x0C); //2
            war3.patch(0x39A515, 0x0f, 0x85, 0xb1, 0x0, 0x0, 0x0);//2


            war3.patch(0x285CBC, new byte[] { 0x74, 0x2a });
            war3.patch(0x285CD2, new byte[] { 0x75 });
            war3.patch(0x28357C, new byte[] { 0xc3, 0xcc });
            war3.patch(0x3A201B, new byte[] { 0x75 });
            war3.patch(0x40A864, new byte[] { 0x75, 0x0a });


            war3.patch(0x39BECF, 0x75);
            war3.patch(0x39BEEC, 0x75);

            //小地图
            war3.patch(0x361F7C, 0x01);   //显示单位
            war3.patch(0x361F80, 0xd3, 0xe8);//显示单位
            war3.patch(0x357065, 0x88, 0x01);
            war3.patch(0x43F9A6, 0x85,0xc0,0x0f,0x84);
            war3.patch(0x43F9B9, 0x85,0xc0,0x0f,0x84);
            war3.patch(0x43F9BC, 0x84);
            war3.patch(0x362391, 0x85, 0xC0, 0xF, 0x84); //显隐xi
            

            //数显

            war3.patch(0x339C54, 0x57, 0x8d, 0x4c, 0x24, 0x18);
            war3.patch(0x339DF4, 0x57, 0x8d, 0x4c, 0x24, 0x18);

            war3.patch(0x87EA63, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc);
            war3.patch(0x87EA70, new byte[] { 0xc3, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xc3, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xc3, 0xcc, 0xcc, 0xcc, 0xcc });


            //辅助

            //头像
            patch(0x371701, 0xFB, 0x29);
            patch(0x371708, 0x84);
            patch(0x37170D, 0x8B, 0x85, 0x80, 0x01, 0x00, 0x00);


            war3.patch(0x2031EC, 0x0f, 0x84, 0x5f, 0x01, 0x00, 0x00);
            war3.patch(0x34E8E7, 0x00);
            war3.patch(0x34E8EA, 0x8B, 0x87, 0x68, 0x01);
            war3.patch(0x34E8EF, 0x00);

            war3.patch(0x3345E9, 0x85);
            war3.patch(0x3345EC, 0x84);
            war3.patch(0x34E8E2, 0x8b, 0x87, 0x6c, 0x01);
            war3.patch(0x34FDE8, 0x74, 0x08);
            war3.patch(0x28ECFE, 0x75);
            war3.patch(0x34FE26, 0x85, 0xc0, 0x74, 0x08);
            //防秒开始
            war3.patch(0x5b73da, 0x83, 0xf9, 0x12, 0x0f, 0x87);
            //顶显资源


            war3.patch(0x359AED, 0x85, 0xc0);
            war3.patch(0x35A1DF, 0x85, 0xc0);
            war3.patch(0x35A29F, 0x85, 0xc0);
            war3.patch(0x35A3D0, 0x85, 0xc0);
            war3.patch(0x28EAFA, 0x85, 0xc0);

            //过检测

            //PanCameraToTimed
            war3.patch(0x3B5280, new byte[] { 0x33, 0xd2 });

            //SetUnitAnimation
            war3.patch(0x3c6ce0, new byte[] { 0x8b });

            //SetUserControlForceOff
            war3.patch(0x3b43c0, new byte[] { 0x33, 0xd2 });

            //SelectUnit
            war3.patch(0x3c84C7, new byte[] { 0x6a, 0x00 });
            war3.patch(0x3C84E7, new byte[] { 0x6a, 0x00 });
            //ClearSelection
            //war3.patch(0x2C5AB0, 0x55);
            war3.patch(0x3BC5E0, 0xc3);



        }



        /// <summary>
        /// 魔兽1.24e MH 打开
        /// </summary>
        private unsafe void War3124eMHON()
        {
            uint oldpro = 0;
            war3.VirtualProtectEx(war3.DllBaseAddress + 0x01000, 0x87E000, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldpro);


            //大地图

            if (IsCheckedDspt(checkBoxxsdw))
            {
                ////大地图显示单位
                war3.patch(0x87EFDB, 0x0f, 0xb7, 0x32, 0x0f, 0xb7, 0xce, 0x0f, 0xb7, 0xd0, 0xe9, 0xf4, 0x30, 0xb2, 0xff, 0x90); //2
                war3.patch(0x39EBBC, new byte[] { 0x75 });
                war3.patch(0x3A2030, new byte[] { 0x90, 0x90 });
                //                war3.patch(0x3A20DB, new byte[] { 0x90,0x90 });
                war3.patch(0x3A20D2, 0xE9, 0x04, 0xCF, 0x4D, 0x00, 0x90); //2

            }
            else if (IsCheckedDspt(checkBoxbxdw))
            {
                war3.patch(0x39A4E8, 0xEB);
            }


            if (IsCheckedDspt(checkBoxxsyx))
            {
                //大地图显示隐形
                //war3.patch(0x356FFE, new byte[] { 0x90, 0x90,0x90 });
                //				war3.patch(0x362391, new byte[] { 0x3b });//1
                //				war3.patch(0x362394, new byte[] { 0x85 });//1
                //				war3.patch(0x39A51B, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });//1
                //				war3.patch(0x39A52E, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x33, 0xc0, 0x40 });//1
                war3.patch(0x39a5d8, 0x71);
                war3.patch(0x39A50D, 0x90, 0x90); //2
                war3.patch(0x39A515, 0x75, 0x32, 0x90, 0x90, 0x90, 0x90);//2

            }

            if (IsCheckedDspt(checkBoxqcmw))
            { // 大地图去除迷雾
                war3.patch(0x74D1Ba, new byte[] {  0x15, 0x70 });
                //",0x90,0x8B,0x09"
            }

            if (IsCheckedDspt(checkBoxswdx))
            {
                ////////////////////////////////////////////////视野外点选
                war3.patch(0x285CBC, new byte[] { 0x90, 0x90 });
                war3.patch(0x285CD2, new byte[] { 0xeb });
            }

            if (IsCheckedDspt(checkBoxswxx))
            {
                //视野外显学

            }

            if (IsCheckedDspt(checkBoxfbhx))
            {
                //分辨幻象
                war3.patch(0x28357C, new byte[] { 0x40, 0xc3 });
            }

            if (IsCheckedDspt(checkBoxxssf))
            //显示神符
            {
                war3.patch(0x3A201B, new byte[] { 0xeb });
                war3.patch(0x40A864, new byte[] { 0x90, 0x90 });
            }


            //			if (IsCheckedDspt(checkBoxxswp))
            //			{//显示物品
            //
            //			}

            //显血
            if (IsCheckedDspt(checkBoxyjxx))
                war3.patch(0x39BECF, 0xeb);
            if (IsCheckedDspt(checkBoxdjxx))
                war3.patch(0x39BEEC, 0xeb);


            //小地图

            if (IsCheckedDspt(checkBoxmxsdw))
            {
                //显示单位
                //war3.patch(0x361F7C, 0x00);
                war3.patch(0x361F80, 0x33, 0xc0);//2

            }

            if (IsCheckedDspt(checkBoxmxsyx))
            {
                //显示隐形
                war3.patch(0x362391, 0x33, 0xC0, 0xF, 0x85);//xi
                
            }


            if (IsCheckedDspt(checkBoxmqcmw))
            {
                //去除迷雾
                war3.patch(0x357065, 0x90, 0x90);//",0xEC");
            }

            //辅助

            //敌方头像
            if (IsCheckedDspt(checkBoxdftx))
            {
                patch(0x371700, 0xE8, 0x3B, 0x28, 0x03, 0x00, 0x85, 0xC0, 0x0F, 0x85, 0x8F, 0x02, 0x00, 0x00, 0xEB, 0xC9, 0x90, 0x90, 0x90, 0x90);

            }

            if (IsCheckedDspt(checkBoxyftx))
            {
                patch(0x371700, 0xE8, 0x3B, 0x28, 0x03, 0x00, 0x85, 0xC0, 0x0F, 0x84, 0x8F, 0x02, 0x00, 0x00, 0xEB, 0xC9, 0x90, 0x90, 0x90, 0x90);

            }



            if (IsCheckedDspt(checkBoxdfxh))
            {//敌方信号
                war3.patch(0x43F9A6, 0x3b, 0xc0, 0x0f, 0x85);
                war3.patch(0x43F9B9, 0x3b, 0xc0, 0x0f, 0x85);
            }

            if (IsCheckedDspt(checkBoxwxqx))
            {
                //无限取消
                war3.patch(0x57BA7C, 0xeb);
                war3.patch(0x5B2D77, 0x03);
                war3.patch(0x5B2D8B, 0x03);
            }

            if (IsCheckedDspt(checkBoxyxjy))
            {//允许交易
                war3.patch(0x34E8E2, 0xb8, 0xd0, 0x7, 0x00);
                war3.patch(0x34E8E7, 0x90);
                war3.patch(0x34E8EA, 0xb8, 0x64, 0x00, 0x00);
                war3.patch(0x34E8EF, 0x90);
            }

            if (IsCheckedDspt(checkBoxxsjn))
            {
                //显示技能
                war3.patch(0x2031EC, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90);
                war3.patch(0x34FDE8, 0x90, 0x90);
                //技能CD
                war3.patch(0x28ECFE, 0xeb);
                war3.patch(0x34FE26, 0x90, 0x90, 0x90, 0x90);
            }

            if (IsCheckedDspt(checkBoxtrts))
            {
                //他人提示
                war3.patch(0x3345E9, 0x39, 0xc0, 0x0f, 0x85);
            }

            if (IsCheckedDspt(checkBoxsxys))
            {
                //    //////////////////////////////////////////////////////////数显移速
                war3.patch(0x87EA63, 0x25, 0x30, 0x2e, 0x32, 0x66, 0x7c, 0x52, 0x00);
                war3.patch(0x87EA70, new byte[] { 0x8d, 0x4c, 0x24, 0x18, 0xd9, 0x44, 0x24, 0x60, 0x83, 0xec, 0x08, 0xdd, 0x1c, 0x24, 0x68 });

                int tmp = 0x87EA63 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x87EA7F, bt1);
                war3.patch(0x87EA83, new byte[] { 0x57, 0x51, 0xe8, 0xbc, 0xd2, 0xe6, 0xff, 0x83, 0xc4, 0x14, 0x58, 0x57, 0x8d, 0x4c, 0x24, 0x18, 0xff, 0xe0 });
                war3.patch(0x339C54, 0xe8, 0x17, 0x4e, 0x54, 0x00);
            }

            if (IsCheckedDspt(checkBoxsxgs))
            {
                ////////////////////数显攻速
                war3.patch(0x87EA63, 0x25, 0x30, 0x2e, 0x32, 0x66, 0x7c, 0x52, 0x00);
                war3.patch(0x87EA70, new byte[] { 0x8d, 0x4c, 0x24, 0x18, 0xd9, 0x44, 0x24, 0x60, 0x83, 0xec, 0x08, 0xdd, 0x1c, 0x24, 0x68 });
                int tmp = 0x87EA63 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x87EA7F, bt1);
                war3.patch(0x87EA83, new byte[] { 0x57, 0x51, 0xe8, 0xbc, 0xd2, 0xe6, 0xff, 0x83, 0xc4, 0x14, 0x58, 0x57, 0x8d, 0x4c, 0x24, 0x18, 0xff, 0xe0 });
                war3.patch(0x339DF4, 0xe8, 0x77, 0x4c, 0x54, 0x00);

            }

            //防秒开始
            if (IsCheckedDspt(checkBoxfmks))
            {
                war3.patch(0x87EFBC, 0x83, 0xf9, 0x00, 0x75, 0x03, 0x6a, 0x03, 0x59, 0x83, 0xf9, 0x12, 0x0f, 0x87, 0xa5, 0x85, 0xd3, 0xff, 0xe9, 0x11, 0x84, 0xd3, 0xff);
                war3.patch(0x5b73da, 0xe9, 0xdd, 0x7B, 0x2c, 0x00); //恢复：83 F9 12 F 87
            }

            //顶显资源
            if (IsCheckedDspt(checkBoxdxzy))
            {
                war3.patch(0x359AED, 0xeb, 0x02); //恢复：85 C0
                war3.patch(0x35A1DF, 0xeb, 0x02);
                war3.patch(0x35A29F, 0xeb, 0x02);
                war3.patch(0x35A3D0, 0xeb, 0x02);
                war3.patch(0x28EAFA, 0xeb, 0x02);
            }

            //建筑显资
            if (IsCheckedDspt(checkBoxjzxz))
            {
                war3.patch(0x360584, 0xeb);//恢复： 75
            }


            //特殊
            if (IsCheckedDspt(checkBoxtrxk))
            {//他人选框
                war3.patch(0x28E996, 0xCB, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0xE8, 0xED, 0x6C);
                war3.patch(0x4268BE, 0xeb);
                war3.patch(0x4268CF, 0xeb,0x2);
                war3.patch(0x4268DE, 0xeb,0x2);
            }

            //过检测

            //PanCameraToTimed
            if (IsCheckedDspt(checkBoxfqpm))
                war3.patch(0x3B5280, new byte[] { 0xc3, 0x90 });

            //SetUnitAnimation
            if (IsCheckedDspt(checkBoxfmxs))
                war3.patch(0x3c6ce0, new byte[] { 0xc3 });

            //SetUserControlForceOff
            if (IsCheckedDspt(checkBoxfsk))
                war3.patch(0x3b43c0, new byte[] { 0xc3, 0x90 });

            //SelectUnit
            if (IsCheckedDspt(checkBoxfxdw))
            {
                war3.patch(0x3c84C7, new byte[] { 0xeb, 0x11 });
                war3.patch(0x3C84E7, new byte[] { 0xeb, 0x11 });

            }

            //ClearSelection
            if (IsCheckedDspt(checkBoxfqx))
            {
                war3.patch(0x3BC5E0, 0xc3); //8B0D
                //war3.patch(0x2C5AB0, 0xc3);
            }

            //反-AH
            if (IsCheckedDspt(checkBoxgah))
            {
                war3.patch(0x3C6EDC, new byte[] { 0x3c, 0x4c, 0x74, 0x04, 0xb0, 0xff, 0xeb, 0x04, 0xb0, 0xb0, 0x90, 0x90 });
                war3.patch(0x3CC3B2, 0xe9, 0xb3, 0x00, 0x00, 0x00, 0x90);
            }


        }






        private void War3120eMHOff()
        {
            uint oldpro = 0;
            war3.VirtualProtectEx(war3.DllBaseAddress + 0x1000, 0x704000, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldpro);

            //大地图
            war3.patch(0x2A0930, new byte[] { 0xc9 });
            war3.patch(0x406B53, new byte[] { 0x8b, 0x49, 0x08 });
            war3.patch(0x17d4c2, new byte[] { 0x75, 0x0e });
            war3.patch(0x17D4CC, new byte[] { 0x0f, 0x85, 0xb0 });
            war3.patch(0x17D4d0, 0x00, 0x00);
            war3.patch(0x1bd5a7, new byte[] { 0x74, 0x2e });
            war3.patch(0x1bd5bb, new byte[] { 0x75 });
            war3.patch(0x166e5e, new byte[] { 0x85, 0xc0, 0x0f, 0x84, 0xbb, 0x03, 0x00, 0x00 });
            war3.patch(0x16fe0a, new byte[] { 0x85 });
            war3.patch(0x16fe0c, new byte[] { 0x74, 0x78 });
            war3.patch(0x1acffc, new byte[] { 0x00, 0x00 });
            war3.patch(0x2a07c5, new byte[] { 0x8b, 0x59, 0x14, 0x8b, 0x49, 0x10 });

            //bk
            war3.patch(0x17D496, 0x75);

            //显血
            war3.patch(0x17F133, 0x75);
            war3.patch(0x17F150, 0x75);
            //小地图
            war3.patch(0x1491a8, 0x01);
            war3.patch(0x1494e0, 0x85);
            war3.patch(0x1494e3, 0x84);
            war3.patch(0x147C53, 0xe4);

            //辅助
            //显示技能
            war3.patch(0x12dc1a, 0x74, 0x08);
            war3.patch(0x12dc5a, 0x74, 0x08);
            war3.patch(0x1bfabe, 0x75);
            war3.patch(0x442cc0, 0x0f, 0x84, 0xdc, 0x00, 0x00, 0x00);
            war3.patch(0x443375, 0x74, 0x1e);
            war3.patch(0x45a641, 0x0f, 0x84, 0x60, 0x01, 0x00, 0x00);
            war3.patch(0x45e79e, 0x0f, 0x84, 0x26, 0x01, 0x00, 0x00);
            war3.patch(0x466527, 0x74, 0x0f);
            war3.patch(0x46b258, 0x0f, 0x84, 0x91, 0x00, 0x00, 0x00);

            //
            war3.patch(0x124ddd, 0x85);
            war3.patch(0x124de0, 0x84);

            //防秒
            war3.patch(0x23D20C, 0x83, 0xf9, 0x12, 0xf, 0x87);

            //顶端资源
            war3.patch(0x406B53, 0x8b, 0x49, 0xb);
            war3.patch(0x1509FE, 0x85, 0xc0);
            war3.patch(0x151597, 0x85, 0xc0);
            war3.patch(0x151647, 0x85, 0xc0);
            war3.patch(0x151748, 0x85, 0xc0);
            war3.patch(0x1BED19, 0x85, 0xc0);
            //头像
            patch(0x137BA5, 0xD7, 0x7F);
            patch(0x137BAC, 0x84);
            patch(0x137BB1, 0x8B, 0x83, 0x80, 0x01, 0x00, 0x00);

            //特殊
            //交换物品
            war3.patch(0x16F4E6, 0x74);
            war3.patch(0x16F4E6, 0x74);
            //他人选框
            war3.patch(0x1BEBCE, 0x55, 0xF4, 0x50, 0x8D, 0x4D, 0xF8); //55 F4 50 8D 4D F8
            war3.patch(0x1BEBD5, 0xD7); //D7
            war3.patch(0x30C873, 0x75); //75
            war3.patch(0x30C886, 0x85, 0xC0);//85 C0
            war3.patch(0x30C896, 0x85, 0xC0);//85 C0
            war3.patch(0x30C9B0, 0x75); // 75
            war3.patch(0x30CA03, 0x85, 0xC0); //85 C0
            war3.patch(0x30CA10, 0x85, 0xC0);//85 C0
            war3.patch(0x30CB75, 0x75); //75
            war3.patch(0x30CB87, 0x85, 0xC0); // 85 C0
            war3.patch(0x30CB97, 0x85, 0xC0); //85 C0
            //控制单位
            war3.patch(0x12DC1A, 0x74, 0x08); // 74 08
            war3.patch(0x12DC5A, 0x74, 0x08); // 74 08
            war3.patch(0x1573EC, 0xE8); // E8 13
            war3.patch(0x16F4E6, 0x74); //74 28
            war3.patch(0x1BD5A7, 0x74, 0x2E); // 74 2E
            war3.patch(0x1BD5BB, 0x75); //75
            war3.patch(0x21EAD4, 0x0); // 0
            war3.patch(0x21EAE8, 0x1); //1
            war3.patch(0x4A11A0, 0x74, 0x0F);//74 0F
            war3.patch(0x54C0BF, 0x0F, 0x84, 0xF7, 0x00, 0x00, 0x00);
            war3.patch(0x5573FE, 0x0F, 0x84, 0x61, 0x01, 0x90, 0x90);
            war3.patch(0x55E15C, 0x74, 0x4A);


            //过MH
            war3.patch(0x2c5a7e, new byte[] { 0xeb, 0x0d });
            war3.patch(0x2dd4b0, new byte[] { 0x55, 0x8b, 0xec });
            war3.patch(0x2C5AB0, 0x55);
            war3.patch(0x2c1e10, 0x55);
        }

        private unsafe void War3120eMHON()
        {


            uint oldpro = 0;
            war3.VirtualProtectEx(war3.DllBaseAddress + 0x01000, 0x704000, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldpro);

            //大地图

            if (IsCheckedDspt(checkBoxxsdw))
            {
                //大地图显示单位
                war3.patch(0x2A0930, new byte[] { 0xd2 });//",0xD2");
            }
            else if (IsCheckedDspt(checkBoxbxdw))
            {
                war3.patch(0x17D496, 0xE8);
            }


            if (IsCheckedDspt(checkBoxqcmw))
                // 大地图去除迷雾
                war3.patch(0x406B53, new byte[] { 0x90, 0x8b, 0x09 });
            //",0x90,0x8B,0x09"

            if (IsCheckedDspt(checkBoxxsyx))
            {
                //大地图显示隐形
                war3.patch(0x17d4c2, new byte[] { 0x90, 0x90 });
                war3.patch(0x17D4CC, new byte[] { 0xeb, 0x00, 0xeb, 0x00, 0x75, 0x30 });
            }

            if (IsCheckedDspt(checkBoxswdx))
            {
                ////////////////////////////////////////////////视野外点选
                war3.patch(0x1bd5a7, new byte[] { 0x90, 0x90 });
                war3.patch(0x1bd5bb, new byte[] { 0xeb });
            }

            if (IsCheckedDspt(checkBoxswxx))
            {
                //视野外显学
                war3.patch(0x166e5e, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
                war3.patch(0x16fe0a, new byte[] { 0x33, 0xc0, 0x90, 0x90 });
            }

            if (IsCheckedDspt(checkBoxfbhx))
            {
                //分辨幻想
                war3.patch(0x1acffc, new byte[] { 0x40, 0xc3 });
            }

            if (IsCheckedDspt(checkBoxxssf))
                //显示神符
                war3.patch(0x2a07c5, new byte[] { 0x49, 0x4b, 0x33, 0xdb, 0x33, 0xc9 });

            //显血
            if (IsCheckedDspt(checkBoxyjxx))
                war3.patch(0x17F133, 0xeb);
            if (IsCheckedDspt(checkBoxdjxx))
                war3.patch(0x17F150, 0xeb);

            //小地图

            if (IsCheckedDspt(checkBoxmxsdw))
            {
                //显示单位
                war3.patch(0x1491a8, 0x00);
            }

            if (IsCheckedDspt(checkBoxmxsyx))
            {
                //显示隐形
                war3.patch(0x1494e0, new byte[] { 0x33, 0xc0, 0x0f, 0x85 });
            }


            if (IsCheckedDspt(checkBoxmqcmw))
            {
                //去除迷雾
                war3.patch(0x147C53, 0xec);//",0xEC");
            }

            //辅助

            if (IsCheckedDspt(checkBoxwxqx))
            {
                //无线取消
                war3.patch(0x23d60f, 0xeb);
                war3.patch(0x21ead4, 0x03);
                war3.patch(0x21eae8, 0x03);
            }

            if (IsCheckedDspt(checkBoxyxjy))
            {
                //允许交易
                war3.patch(0x127b3d, 0x40, 0xb8, 0x64);
                war3.patch(0x127B35, 0x40, 0xb8, 0xd0, 0x7, 0x0, 0x0);
            }

            if (IsCheckedDspt(checkBoxxsjn))
            {
                //显示技能
                war3.patch(0x12dc1a, 0x33, 0xc0);
                war3.patch(0x12dc5a, 0x33, 0xc0);
                war3.patch(0x1bfabe, 0xeb);
                war3.patch(0x442cc0, 0x90, 0x40, 0x30, 0xc0, 0x90, 0x90);
                war3.patch(0x443375, 0x30, 0xc0);
                war3.patch(0x45a641, 0x90, 0x90, 0x33, 0xc0, 0x90, 0x90);
                war3.patch(0x45e79e, 0x90, 0x90, 0x33, 0xc0, 0x90, 0x90);
                war3.patch(0x466527, 0x90, 0x90);
                war3.patch(0x46b258, 0x90, 0x33, 0xc0, 0x90, 0x90, 0x90);
            }

            if (IsCheckedDspt(checkBoxtrts))
            {
                //他人提示
                war3.patch(0x124ddd, 0x39, 0xc0, 0x0f, 0x85);
            }

            if (IsCheckedDspt(checkBoxdfxh))
            {//敌方信号
                war3.patch(0x321CC4, 0x39, 0xc0, 0x0f, 0x85);
                war3.patch(0x321CD7, 0x39, 0xc0, 0x75);
            }

            //敌军头像
            if (IsCheckedDspt(checkBoxdftx))
            {
                patch(0x137BA5, 0xE7, 0x7D);
                patch(0x137BAC, 0x85, 0xA3, 0x02, 0x00, 0x00, 0xEB, 0xCE, 0x90, 0x90, 0x90, 0x90);
            }

            if (IsCheckedDspt(checkBoxyftx))
            {
                patch(0x137BA5, 0xE7, 0x7D);
                patch(0x137BB1, 0xEB, 0xCE, 0x90, 0x90, 0x90, 0x90);
            }

            //资源面板
            if (IsCheckedDspt(checkBoxzymb))
            {
                patch(0x13EF03, 0xEB);
                patch(0x150981, 0xeb, 0x02);
                patch(0x1509fe, 0xeb, 0x02);
                patch(0x151597, 0xeb, 0x02);
                patch(0x151647, 0xeb, 0x02);
                patch(0x151748, 0xeb, 0x02);
                patch(0x1BED19, 0xeb, 0x02);
                patch(0x314A9E, 0xeb, 0x02);
                patch(0x21EAD4, 0xEB);
                patch(0x21EAE8, 0x03);
            }

            if (IsCheckedDspt(checkBoxsxys))
            {
                //    //////////////////////////////////////////////////////////数显移速
                war3.patch(0x802e67, 0x32);
                war3.patch(0x13c3f2, new byte[] { 0x90, 0xd9, 0x45, 0x08, 0x83, 0xec, 0x08, 0xdd, 0x1c, 0x24, 0x68 });
                int tmp = 0x802e64 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x13C3FD, bt1);//[3], bt1[2], bt1[1], bt1[0]
                war3.patch(0x13C401, new byte[] { 0x8d, 0x55, 0x98, 0x6a, 0x7f, 0x52, 0xe8, 0x96, 0x74, 0x25, 0x00, 0x83, 0xc4, 0x14, 0x6a, 0x7f, 0x8d, 0x45, 0x98, 0x50 });
            }

            if (IsCheckedDspt(checkBoxsxgs))
            {
                ////////////////////数显攻速
                patch(0x13b400, 0x90, 0x90, 0x90);
                war3.patch(0x802e67, 0x32);
                war3.patch(0x13BA61, new byte[] { 0x90, 0xd9, 0x45, 0x08, 0x83, 0xec, 0x08, 0xdd, 0x1c, 0x24, 0x68 });
                int tmp = 0x802e64 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x13BA6C, bt1);
                war3.patch(0x13BA70, new byte[] { 0x8d, 0x55, 0xa0, 0x6a, 0x7f, 0x52, 0xe8, 0x27, 0x7e, 0x25, 0x00, 0x83, 0xc4, 0x14, 0x6a, 0x7f, 0x8d, 0x45, 0xa0, 0x50 });
            }
            //防秒开始
            if (IsCheckedDspt(checkBoxfmks))
            {
                war3.patch(0x704BB0, 0x83, 0xf9, 0x00, 0x0f, 0x85, 0x3, 0x0, 0x0, 0x0, 0x6a, 0x3, 0x59, 0x83, 0xf9, 0x12, 0xf, 0x87, 0xef, 0x87, 0x83, 0xff, 0xe9, 0x4b, 0x86, 0xb3, 0xff);
                war3.patch(0x23D20C, 0xe9, 0x9f, 0x79, 0x4c, 0x00);
            }

            //顶显资源
            if (IsCheckedDspt(checkBoxdxzy))
            {
                war3.patch(0x406B53, 0x90, 0x8b, 0x9);
                war3.patch(0x1509FE, 0xeb, 0x2);
                war3.patch(0x151597, 0xeb, 0x2);
                war3.patch(0x151647, 0xeb, 0x2);
                war3.patch(0x151748, 0xeb, 0x2);
                war3.patch(0x1BED19, 0xeb, 0x2);

            }

            //建筑显资
            if (IsCheckedDspt(checkBoxjzxz))
            {
                war3.patch(0x13EF03, 0xeb);//恢复： 75
            }

            //特殊

            if (IsCheckedDspt(checkBoxjhwp))
            {//交换物品

                war3.patch(0x704A00, 0x50, 0xA1, 0xFC, 0x49, 0x70, 0x6F, 0x83, 0xF8, 0x00, 0x75, 0x08,
                           0xFF, 0x00, 0xFC, 0xA3, 0x44, 0x0, 0xEB, 0x11, 0x68, 0xAE, 0x20, 0x00, 0x00, 0x8F, 0x45,
                           0xF4, 0x68, 0x6A, 0x2, 0x0, 0x0, 0x8F, 0x45, 0xF8, 0x58, 0xE9, 0xE9, 0xAA, 0xD1, 0x90, 0x0);
                war3.patch(0x7049FC, 0x0, 0x0, 0x0, 0x0);

                war3.patch(0x16F4E6, 0xEB, 0x28);
                war3.patch(0x1573EC, 0xEB);
            }

            if (IsCheckedDspt(checkBoxtrxk))
            {//他人选框
                war3.patch(0x2DA14A, 0x74, 0x1B);
                war3.patch(0x1BEBCE, 0x4D, 0x18, 0x90, 0x90, 0x90, 0x90); //55 F4 50 8D 4D F8
                war3.patch(0x1BEBD5, 0x77); //D7
                war3.patch(0x30C873, 0xEB); //75
                war3.patch(0x30C886, 0xEB, 0x2);//85 C0
                war3.patch(0x30C896, 0xEB, 0x2);//85 C0
                war3.patch(0x30C9B0, 0xEB); // 75
                war3.patch(0x30CA03, 0xEB, 0x2); //85 C0
                war3.patch(0x30CA10, 0xEB, 0x2);//85 C0
                war3.patch(0x30CB75, 0xEB); //75
                war3.patch(0x30CB87, 0xEB, 0x2); // 85 C0
                war3.patch(0x30CB97, 0xEB, 0x2); //85 C0
            }

            if (IsCheckedDspt(checkBoxkzdw))
            {
                war3.patch(0x12DC1A, 0x90, 0x90); // 74 08
                war3.patch(0x12DC5A, 0x90, 0x90); // 74 08
                war3.patch(0x1573EC, 0xEB); // E8 13
                war3.patch(0x16F4E6, 0xEB); //74 28
                war3.patch(0x1BD5A7, 0x90, 0x90); // 74 2E
                war3.patch(0x1BD5BB, 0xEB); //75
                war3.patch(0x21EAD4, 0x3); // 0
                war3.patch(0x21EAE8, 0x3); //1
                war3.patch(0x4A11A0, 0x90, 0x90);//74 0F
                war3.patch(0x54C0BF, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90);
                war3.patch(0x5573FE, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90);
                war3.patch(0x55E15C, 0x90, 0x90);
            }


            //过检测

            //PanCameraToTimed
            if (IsCheckedDspt(checkBoxfqpm))
                war3.patch(0x2dd4b0, new byte[] { 0xc3 });

            //SetUnitAnimation
            if (IsCheckedDspt(checkBoxfmxs))
                war3.patch(0x2c1e10, new byte[] { 0xc3 });

            //SetUserControlForceOff
            if (IsCheckedDspt(checkBoxfsk))
                war3.patch(0x2d3300, new byte[] { 0xc3, 0x90, 0x90 });

            //SelectUnit
            if (IsCheckedDspt(checkBoxfxdw))
                war3.patch(0x2c5a7e, new byte[] { 0x90, 0x90 });

            //ClearSelection
            if (IsCheckedDspt(checkBoxfqx))
            {
                war3.patch(0x2C5AB0, 0xc3);
            }


            //反-AH
            if (IsCheckedDspt(checkBoxgah))
            {
                war3.patch(0x2c240c, new byte[] { 0x3c, 0x4c, 0x74, 0x04, 0xb0, 0xff, 0xeb, 0x04, 0xb0, 0xb0, 0x90, 0x90 });
                war3.patch(0x2d34ed, 0xe9, 0xb3, 0x00, 0x00, 0x00, 0x90);
            }
            //MessageBox.Show("MH开启");
        }

        /// <summary>
        /// 调用war3.patch
        /// </summary>
        /// <param name="baseaddress"></param>
        /// <param name="bt"></param>
        void patch(int baseaddress, params byte[] bt)
        {
            war3.patch(baseaddress, bt);
        }


        private void war3124bMHOff()
        {
            uint oldp = 0;
            war3.VirtualProtectEx(war3.DllBaseAddress + 0x1000, 0x87e000, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldp);


            //大地图显示单位
            war3.patch(0x3A200C, 0x74, 0x4C);

            war3.patch(0x39A428, 0x75);
            //大地图去迷雾
            //patch(0x74D103, 0x88, 0x14);
            //patch(0x74D106, 0x83, 0xC6, 0x01);
            //大地图显示隐形
            patch(0x3622D1, 0x85);
            patch(0x3622D4, 0x84);
            patch(0x39A45B, 0x8B, 0x97, 0x98, 0x01, 0x00, 0x00);
            patch(0x39A46E, 0x0F, 0xB7, 0x00, 0x55, 0x50, 0x56, 0xE8, 0xF7, 0x7B, 0x00, 0x00);
            //视野外点选
            patch(0x285C4C, 0x74, 0x2A);
            patch(0x285C62, 0x75);
            //分辨幻象
            patch(0x28351C, 0xC3, 0xCC);
            //显示神符
            patch(0x4076CA, 0x75, 0x0A);
            patch(0x3A1F5B, 0x75);

            //显血
            war3.patch(0x39BE0F, 0x75);
            war3.patch(0x39BE2C, 0x75);

            //小地图显示单位
            //			patch(0x361EAB, 0x75, 0x0C); //br
            //			patch(0x361EB0, 0x75, 0x07, 0xB9, 0x01);//br
            //			patch(0x361EB8, 0x02);//br
            patch(0x361E98, 0xE9, 0x87, 0x27, 0x4C, 0x00);
            //小地图显示隐形
            patch(0x361EBC, 0x01);
            war3.patch(0x3622D1, 0x85, 0xC0, 0xF, 0x84); 
            //小地图去除迷雾
            patch(0x356FA5, 0x88, 0x01);
            //资源面板
            patch(0x3604CA, 0xEB, 0x08);
            //无限取消
            patch(0x57B9FC, 0x7A);
            patch(0x5B2CC7, 0x00);
            patch(0x5B2CDB, 0x01);
            //允许交易
            patch(0x34E822, 0x8B, 0x87, 0x6C, 0x01);
            patch(0x34E827, 0x00);
            patch(0x34E82A, 0x8B, 0x87, 0x68, 0x01);
            patch(0x34E82F, 0x00);
            //显示技能
            patch(0x28EC8E, 0x75);
            patch(0x20318C, 0x0F, 0x84, 0x5F, 0x01, 0x00, 0x00);
            patch(0x34FD28, 0x74, 0x08);
            patch(0x34FD66, 0x85, 0xC0, 0x74, 0x08);
            //他人提示
            patch(0x334529, 0x85);
            patch(0x33452C, 0x84);
            //敌方信号
            patch(0x43F956, 0x85);
            patch(0x43F959, 0x84);
            patch(0x43F969, 0x85);
            patch(0x43F96C, 0x84);
            //数显
            patch(0x339D34, 0x57, 0x8D);
            patch(0x339D37, 0x24, 0x18);
            patch(0x339B94, 0x57, 0x8D, 0x4C, 0x24, 0x18);
            patch(0x87E9A3, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC);
            patch(0x87E9B0, 0xC3, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xC3, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xC3, 0xCC, 0xCC, 0xCC, 0xCC);
            //防秒
            war3.patch(0x5b732a, 0x83, 0xf9, 0x12, 0x0f, 0x87);
            //顶显资源
            war3.patch(0x359A2D, 0x85, 0xc0);
            war3.patch(0x35A11F, 0x85, 0xc0);
            war3.patch(0x35A1DF, 0x85, 0xc0);
            war3.patch(0x35A310, 0x85, 0xc0);
            war3.patch(0x28EA8A, 0x85, 0xc0);
            //头像
            patch(0x371641, 0xFB, 0x29);
            patch(0x37164D, 0x8B, 0x85, 0x80, 0x01, 0x00, 0x00);


            //过-MH
            patch(0x3C8407, 0x6A, 0x00);
            patch(0x3C8427, 0x6A, 0x00);
            patch(0x3BC520, 0x8B);
            //过-ah
            patch(0x3C6E1C, 0x3D);
            patch(0x3C6E21, 0x76);
            patch(0x3CC2F2, 0x74);
            //
            //patch(0x6F3BC5E0, 0x8b, 0x0d, 0x4c, 0xd4, 0xac, 0x6f);


            war3.patch(0x3b51c0, new byte[] { 0x33, 0xd2 });
            war3.patch(0x3c6c21, new byte[] { 0x8b, 0x44, 0x24, 0x08 });
            war3.patch(0x3b4300, new byte[] { 0x33, 0xd2 });



        }


        private void war3124bMHON()
        {
            uint oldp = 0;
            war3.VirtualProtectEx(war3.DllBaseAddress + 0x1000, 0x87e000, ProcessC.Protection.PAGE_EXECUTE_READWRITE, ref oldp);

            //大地图
            if (IsCheckedDspt(checkBoxxsdw))
            {
                //大地图显示单位
                war3.patch(0x3A201D, new byte[] { 0xEB });//1
                //war3.patch(0x3A201D, 0x0);//
                //87e99d   3a2051
                //war3.patch(0x3A204F, 0x90,0x90);//3
                //3A203A EB     200C EB43   0x3A201B, 0x33
            }
            else if (IsCheckedDspt(checkBoxbxdw))
            {
                war3.patch(0x39A428, 0xEB);
            }

            //if (IsCheckedDspt(checkBoxqcmw))
                // 大地图去除迷雾
                //war3.patch(0x74D103, new byte[] { 0xc6, 0x04, 0x3e, 0x01, 0x90, 0x46 });
            //",0x90,0x8B,0x09"

            if (IsCheckedDspt(checkBoxxsyx))
            {
                //大地图显示隐形
                //				war3.patch(0x3622D1, new byte[] { 0x3b }); //1
                //				war3.patch(0x3622D4, new byte[] { 0x85 }); //1
                //				war3.patch(0x39A45B, new byte[] { 0x90,0x90,0x90,0x90,0x90,0x90 }); //1
                //				war3.patch(0x39A46E, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x33, 0xc0, 0x40 }); //1
                war3.patch(0x39A44D, 0x90, 0x90); //xi
                war3.patch(0x39A455, 0x75, 0x32, 0x90, 0x90, 0x90, 0x90); //xi

            }

            if (IsCheckedDspt(checkBoxswdx))
            {
                ////////////////////////////////////////////////视野外点选
                patch(0x285C4C, 0x90, 0x90);
                patch(0x285C62, 0xEB);
            }

            if (IsCheckedDspt(checkBoxswxx))
            {
                //视野外显血

            }

            if (IsCheckedDspt(checkBoxfbhx))
            {
                //分辨幻象
                war3.patch(0x28351C, new byte[] { 0x40, 0xc3 });
            }

            if (IsCheckedDspt(checkBoxxssf))
            //显示神符
            {
                war3.patch(0x4076CA, new byte[] { 0x90, 0x90 });
                war3.patch(0x3A1F5B, new byte[] { 0xeb });
            }


            //显血
            if (IsCheckedDspt(checkBoxyjxx))
                war3.patch(0x39BE0F, 0xeb);
            if (IsCheckedDspt(checkBoxdjxx))
                war3.patch(0x39BE2C, 0xeb);


            //小地图

            if (IsCheckedDspt(checkBoxmxsdw))
            {
                //显示单位
                //patch(0x361EAB, 0x90,0x90,0x39,0x5E,0x10,0x90,0x90,0xB8,0x00,0x00,0x00,0x00,0xEB,0x07); //br
                //patch(0x361EBC,0x0);//xi

                patch(0x824624, 0x39, 0x5E, 0x14, 0x39, 0x5E, 0x10, 0xE9, 0x93, 0xD8, 0xB3, 0xFF); //3
                patch(0x361E98, 0xE9, 0x87, 0x27, 0x4C, 0x00); //3
            }

            if (IsCheckedDspt(checkBoxmxsyx))
            {
                //显示隐形
                //patch(0x361EBC, 0x00); //1
                patch(0x3622D1, 0x33, 0xC0, 0xF, 0x85); //xi
                
            }


            if (IsCheckedDspt(checkBoxmqcmw))
            {
                //去除迷雾
                patch(0x356FA5, 0x90, 0x90);
            }

            //辅助

            //敌方头像
            if (IsCheckedDspt(checkBoxdftx))
            {
                patch(0x371640, 0xE8, 0x3B, 0x28, 0x03, 0x00, 0x85, 0xC0, 0x0F, 0x85, 0x8F, 0x02, 0x00, 0x00, 0xEB, 0xC9, 0x90, 0x90, 0x90, 0x90);

            }

            if (IsCheckedDspt(checkBoxyftx))
            {
                patch(0x371640, 0xE8, 0x3B, 0x28, 0x03, 0x00, 0x85, 0xC0, 0x0F, 0x84, 0x8F, 0x02, 0x00, 0x00, 0xEB, 0xC9, 0x90, 0x90, 0x90, 0x90);

            }

            //资源面板
            if (IsCheckedDspt(checkBoxzymb))
            {
                patch(0x3604CA, 0x90, 0x90);
                patch(0x150981, 0xEB, 0x02);
                patch(0x1509FE, 0xEB, 0x02);
                patch(0x151597, 0xEB, 0x02);
                patch(0x151647, 0xEB, 0x02);
                patch(0x151748, 0xEB, 0x02);
                patch(0x1BED19, 0xEB, 0x02);
                patch(0x314A9E, 0xEB, 0x02);
                patch(0x21EAD4, 0xEB);
                patch(0x21EAE8, 0x3);

            }

            if (IsCheckedDspt(checkBoxwxqx))
            {
                //无限取消
                patch(0x57B9FC, 0xEB);
                patch(0x5B2CC7, 0x03);
                patch(0x5B2CDB, 0x03);
            }

            if (IsCheckedDspt(checkBoxyxjy))
            //允许交易
            {
                patch(0x34E822, 0xB8, 0xD0, 0x07, 0x00);

                patch(0x34E827, 0x90);

                patch(0x34E82A, 0xB8, 0x64, 0x00, 0x00);
                patch(0x34E82F, 0x90);
            }

            if (IsCheckedDspt(checkBoxxsjn))
            {
                //显示技能
                patch(0x28EC8E, 0xEB);

                patch(0x20318C, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90);
                patch(0x34FD28, 0x90, 0x90);
                patch(0x34FD66, 0x90, 0x90, 0x90, 0x90);
            }

            if (IsCheckedDspt(checkBoxtrts))
            {
                //他人提示
                patch(0x334529, 0x39, 0xC0, 0x0F, 0x85);
            }

            if (IsCheckedDspt(checkBoxdfxh))
            {//敌方信号
                patch(0x43F956, 0x3B);
                patch(0x43F959, 0x85);
                patch(0x43F969, 0x3B);
                patch(0x43F96C, 0x85);
            }

            if (IsCheckedDspt(checkBoxsxys))
            {
                //    //////////////////////////////////////////////////////////数显移速
                patch(0x87E9A3, 0x25, 0x30, 0x2E, 0x32, 0x66, 0x7C, 0x52, 0x00);
                patch(0x87E9B0, 0x8D, 0x4C, 0x24, 0x18, 0xD9, 0x44, 0x24, 0x60, 0x83, 0xEC, 0x08, 0xDD, 0x1C, 0x24, 0x68);
                int tmp = 0x87E9A3 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x87E9BF, bt1);
                patch(0x87E9C3, 0x57, 0x51, 0xE8, 0xBC, 0xD2, 0xE6, 0xFF, 0x83, 0xC4, 0x14, 0x58, 0x57, 0x8D, 0x4C, 0x24, 0x18, 0xFF, 0xE0);
                patch(0x339B94, 0xE8, 0x17, 0x4E, 0x54, 0x00);
            }

            if (IsCheckedDspt(checkBoxsxgs))
            {
                ////////////////////数显攻速
                patch(0x87E9A3, 0x25, 0x30, 0x2E, 0x32, 0x66, 0x7C, 0x52, 0x00);
                patch(0x87E9B0, 0x8D, 0x4C, 0x24, 0x18, 0xD9, 0x44, 0x24, 0x60, 0x83, 0xEC, 0x08, 0xDD, 0x1C, 0x24, 0x68);
                int tmp = 0x87E9A3 + war3.DllBaseAddress;
                byte[] bt1 = BitConverter.GetBytes(tmp);
                war3.patch(0x87E9BF, bt1);
                patch(0x87E9C3, 0x57, 0x51, 0xE8, 0xBC, 0xD2, 0xE6, 0xFF, 0x83, 0xC4, 0x14, 0x58, 0x57, 0x8D, 0x4C, 0x24, 0x18, 0xFF, 0xE0);
                patch(0x339D34, 0xE8, 0x77, 0x4C, 0x54, 0x00);
            }

            //防秒开始
            if (IsCheckedDspt(checkBoxfmks))
            {
                war3.patch(0x87EEFC, 0x83, 0xf9, 0x00, 0x75, 0x03, 0x6a, 0x03, 0x59, 0x83, 0xf9, 0x12, 0x0f, 0x87, 0xb5, 0x85, 0xd3, 0xff, 0xe9, 0x21, 0x84, 0xd3, 0xff);
                war3.patch(0x5b732a, 0xe9, 0xcd, 0x7B, 0x2c, 0x00);
            }

            //顶显资源
            if (IsCheckedDspt(checkBoxdxzy))
            {
                war3.patch(0x359A2D, 0xeb, 0x02);
                war3.patch(0x35A11F, 0xeb, 0x02);
                war3.patch(0x35A1DF, 0xeb, 0x02);
                war3.patch(0x35A310, 0xeb, 0x02);
                war3.patch(0x28EA8A, 0xeb, 0x02);
            }

            //建显资源
            if (IsCheckedDspt(checkBoxjzxz))
            {
                war3.patch(0x3604C4, 0xeb);//恢复： 75
            }


            //特殊

            //过检测

            //PanCameraToTimed
            if (IsCheckedDspt(checkBoxfqpm))
                war3.patch(0x3b51c0, new byte[] { 0xc3, 0x90 });

            //SetUnitAnimation
            if (IsCheckedDspt(checkBoxfmxs))
                war3.patch(0x3c6c21, new byte[] { 0xc3, 0x90, 0x90, 0x90 });

            //SetUserControlForceOff EnableUserControl(false)

            if (IsCheckedDspt(checkBoxfsk))
                war3.patch(0x3b4300, new byte[] { 0xc3, 0x90 });

            //SelectUnit 过-MH
            if (IsCheckedDspt(checkBoxfxdw))
            {
                patch(0x3C8407, 0xEB, 0x11);
                patch(0x3C8427, 0xEB, 0x11);
            }

            //防清选
            if (IsCheckedDspt(checkBoxfqx))
            {
                patch(0x3BC520, 0xc3);
            }

            //反-AH
            if (IsCheckedDspt(checkBoxgah))
            {
                patch(0x3C6E1C, 0xB8, 0xFF, 0x00, 0x00, 0x00, 0xEB);
                patch(0x3CC2F2, 0xEB);
            }


        }



        private void buttonON_Click(object sender, RoutedEventArgs e)
        {
            new Thread(new ThreadStart(delegate()
                                       {
                                           if (findwar3process())
                                           {
                                               lock (war3)
                                               {

                                                   //War3MHon(false);
                                                   War3MHon(true);
                                                   //Dispatcher.BeginInvoke(new ThreadStart(delegate() { War3MHon(true); }), null);
                                               }
                                           }
                                       })).Start();

        }

        private void buttonOFF_Click(object sender, RoutedEventArgs e)
        {

            new Thread(new ThreadStart(delegate()
                                       {
                                           if (findwar3process())
                                           {
                                               lock (war3)
                                               {
                                                   forceOff = true;
                                                   War3MHon(false);
                                               }
                                           }
                                       })).Start();


        }

        #endregion





        void ButtonJiaoyi_Click(object sender, RoutedEventArgs e)
        {

            uint tmp = 100;
            uint.TryParse(TextJYDJ.Text, out tmp);
            if (tmp > 1000000) tmp = 1000000;
            byte[] bt1 = BitConverter.GetBytes(tmp);

            uint tmp2 = 2000;
            uint.TryParse(TextJYCT.Text, out tmp2);
            if (tmp2 > 1000000) tmp2 = 1000000;
            byte[] bt2 = BitConverter.GetBytes(tmp2);


            switch (war3.Version)
            {
                case "1.20.4.6074":
                    {
                        //允许交易
                        war3.patch(0x127b3d, 0x40, 0xb8, bt1[0], bt1[1], bt1[2], bt1[3]);
                        war3.patch(0x127B35, 0x40, 0xb8, bt2[0], bt2[1], bt2[2], bt2[3]);
                        break;
                    }
                case "1.24.1.6374":
                    {
                        //允许交易
                        patch(0x34E822, 0xB8, bt2[0], bt2[1], bt2[2], bt2[3], 0x90);
                        patch(0x34E82A, 0xB8, bt1[0], bt1[1], bt1[2], bt1[3], 0x90);
                        break;
                    }
                case "1.24.4.6387":
                    {
                        //允许交易
                        war3.patch(0x34E8E2, 0xb8, bt2[0], bt2[1], bt2[2], bt2[3], 0x90);
                        war3.patch(0x34E8EA, 0xb8, bt1[0], bt1[1], bt1[2], bt1[3], 0x90);
                        break;
                    }
                case "1.25.1.6397":
                    {
                        war3.patch(0x34db72, 0xb8, bt2[0], bt2[1], bt2[2], bt2[3], 0x90);
                        war3.patch(0x34db7a, 0xb8, bt1[0], bt1[1], bt1[2], bt1[3], 0x90);
                        break;
                    }

            }
        }



    }
}
