using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Nitro_Stream.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class NitroStreamMain: Window
    {
        public NitroStreamMain()
        {
            InitializeComponent();
            this.DataContext = this.DataContext as ViewModel.NitroStreamViewModel;
			Closing += NitroStreamMain_Closing;
            
        }

		private void NitroStreamMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ViewModel.NitroStreamViewModel vm = this.DataContext as ViewModel.NitroStreamViewModel;
			if (vm != null)
			{
				vm.Disconnect();
				if (vm.NtrViewerProcess != null)
					vm.NtrViewerProcess.Kill();
			}
		}

		private void ConfigureViewerPath(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.DefaultExt = "exe";
            if(fd.ShowDialog() == true)
            {
                ViewModel.NitroStreamViewModel vm = this.DataContext as ViewModel.NitroStreamViewModel;
                if (vm != null)
                    vm.ViewSettings.ViewerPath = fd.FileName;
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NitroStreamViewModel vm = this.DataContext as ViewModel.NitroStreamViewModel;
            //if (vm != null)
                //vm.Updater.GetUpdate();
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NitroStreamViewModel vm = this.DataContext as ViewModel.NitroStreamViewModel;
            if (vm != null)
                vm.Donate();
        }

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			ViewModel.NitroStreamViewModel vm = this.DataContext as ViewModel.NitroStreamViewModel;
			if (vm != null)
				vm.UpdateKeyboardState(e.Key, true);
			e.Handled = true;
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			ViewModel.NitroStreamViewModel vm = this.DataContext as ViewModel.NitroStreamViewModel;
			if (vm != null)
				vm.UpdateKeyboardState(e.Key, false);
			e.Handled = true;
		}
	}
}
