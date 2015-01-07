using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Text;
using System.Collections.ObjectModel;

namespace eflayMH_WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
      public static Window1 windows11;

        //private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();

        //    stringBuilder.AppendFormat("应用程序出现了未捕获的异常，{0}\n", e.Exception.Message);

        //    if (e.Exception.InnerException != null)
        //    {

        //        stringBuilder.AppendFormat("\n {0}", e.Exception.InnerException.Message);

        //    }

        //    stringBuilder.AppendFormat("\n {0}", e.Exception.StackTrace);

        //    MessageBox.Show(stringBuilder.ToString());

        //    e.Handled = true;   

        //}  
   protected override void OnStartup(StartupEventArgs e){
      //let the base class have a crack
      base.OnStartup(e);
      //
//      windows11 = new Window1();
//      windows11.Show();
    }
        
        
        public void ApplySkin(Uri skinDictionaryUri)
		{
			// Load the ResourceDictionary into memory.
			ResourceDictionary skinDict = Application.LoadComponent(skinDictionaryUri) as ResourceDictionary;

			Collection<ResourceDictionary> mergedDicts = base.Resources.MergedDictionaries;

			// Remove the existing skin dictionary, if one exists.
			// NOTE: In a real application, this logic might need
			// to be more complex, because there might be dictionaries
			// which should not be removed.
			if (mergedDicts.Count > 0)
				mergedDicts.Clear();

			// Apply the selected skin so that all elements in the
			// application will honor the new look and feel.
			mergedDicts.Add(skinDict);
		}


    }
}
