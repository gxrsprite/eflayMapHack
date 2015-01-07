using System;
using System.Xml;
using System.Collections;
using System.IO;
using WPFCTR = System.Windows.Controls;

namespace eflayMH_WPF
{
    public partial class Window1 
    {

        # region 配置


        /// <summary>
        /// 为Panel内的CheckBox读取配置
        /// </summary>
        /// <param name="xnl2"></param>
        /// <param name="panel"></param>
        public static void rlReadConfigforPanel(XmlNode xnl2, WPFCTR.Panel panel)
        {
            IEnumerator xnls = xnl2.GetEnumerator();
            foreach (var cb in panel.Children)
            {
                if (IsInherit(cb.GetType(), typeof(WPFCTR.Primitives.ToggleButton)) && !((WPFCTR.Primitives.ToggleButton)cb).Content.ToString().Contains("."))
                {

                    if (xnls.MoveNext())
                    {
                        ((WPFCTR.Primitives.ToggleButton)cb).IsChecked = ((XmlNode)(xnls.Current)).InnerText == "true" ? true : false;
                    }
                }
            }
        }


        /// <summary>
        /// 载入配置
        /// </summary>
        private  void LoadConfig()
        {

            XmlDocument xd = new XmlDocument();
            try
            {
                xd.Load("Config.xml");
                XmlNode xnl = xd.SelectSingleNode("eflayMH");


                rlReadConfigforPanel(xnl.SelectSingleNode("大地图"), stackPanel1);

                //XmlNode xnl2 = xnl.SelectSingleNode("基本配置");

                rlReadConfigforPanel(xnl.SelectSingleNode("基本配置"), canvas1);
                rlReadConfigforPanel(xnl.SelectSingleNode("小地图"), stackPanel2);
                rlReadConfigforPanel(xnl.SelectSingleNode("辅助"), stackPanel3);
                rlReadConfigforPanel(xnl.SelectSingleNode("过MH检测"), wrapPanel1);
                rlReadConfigforPanel(xnl.SelectSingleNode("特殊"), war120esp);
                XmlNode fuzhund = xnl.SelectSingleNode("辅助2");
                CheckBoxDesjoyAA.IsChecked =  fuzhund.SelectSingleNode("终结AA").InnerText == "True" ? true : false;
                ComboBoxVersion.SelectedIndex = int.Parse( fuzhund.SelectSingleNode("war3v").InnerText);
                forceOff = checkBoxzdkq.IsChecked.Value ? false : true;
            }
            catch (Exception)
            { }

        }

        /// <summary>
        /// 判断   BaseType   是不是   type   的基类 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        public static bool IsInherit(System.Type type, System.Type BaseType)
        {
            if (type.BaseType == null) return false;
            if (type.BaseType == BaseType) return true;
            return IsInherit(type.BaseType, BaseType);
        }

        /// <summary>
        /// 保存配置，Panel内的ToggleButton
        /// </summary>
        /// <param name="xw"></param>
        /// <param name="panel"></param>
        /// <param name="name"></param>
        static void rlXmlWriteConfigOfTogglesInPanel(XmlWriter xw, WPFCTR.Panel panel, string name)
        {
            xw.WriteStartElement(name);
            foreach (object cb in panel.Children)
            {
                if (IsInherit(cb.GetType(), typeof(WPFCTR.Primitives.ToggleButton)))
                {
                    string valuename = ((WPFCTR.Primitives.ToggleButton)cb).Content.ToString();
                    valuename = valuename.Replace("(", "").Replace(")", "");
                    if (valuename.Contains("."))
                    { continue; }

                    xw.WriteStartElement(valuename);
                    try
                    {
                        xw.WriteValue(((WPFCTR.Primitives.ToggleButton)cb).IsChecked);
                    }
                    catch (ArgumentException)
                    {

                    }
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private  void SaveConfig()
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            FileStream filestream = new FileStream("Config.xml",FileMode.Create,FileAccess.ReadWrite);

            XmlWriter xw = XmlWriter.Create(filestream, xws);
            xw.WriteStartDocument();
            xw.WriteStartElement("eflayMH");

            rlXmlWriteConfigOfTogglesInPanel(xw, canvas1, "基本配置");
            rlXmlWriteConfigOfTogglesInPanel(xw, stackPanel1, "大地图");
            rlXmlWriteConfigOfTogglesInPanel(xw, stackPanel2, "小地图");
            rlXmlWriteConfigOfTogglesInPanel(xw, stackPanel3, "辅助");
            rlXmlWriteConfigOfTogglesInPanel(xw, wrapPanel1, "过MH检测");
            rlXmlWriteConfigOfTogglesInPanel(xw, war120esp, "特殊");
            
            xw.WriteStartElement("辅助2");
            //xw.WriteValue(CheckBoxDesjoyAA.IsChecked.Value);
            xw.WriteElementString("终结AA",CheckBoxDesjoyAA.IsChecked.ToString());
            xw.WriteElementString("war3v",ComboBoxVersion.SelectedIndex.ToString());
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Flush();
            xw.Close();

        }

        #endregion
    }
}
