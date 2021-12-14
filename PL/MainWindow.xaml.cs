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
using System.Windows.Threading;

namespace PL
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		BlApi.IBL ibl = BL.BlFactory.GetBl();
		public MainWindow()
		{
			InitializeComponent();
		}
		/// <summary>
		/// if the user press the button "drone"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DronesButton_Click(object sender, RoutedEventArgs e)
		{
			new displayListOfDrones(ibl).Show();
		}
		/// <summary>
		/// when the window was in loaded set the box time
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, (object s, EventArgs ev) =>
			{
				this.myDateTime.Text = DateTime.Now.ToString("  hh:mm:ss\ndd/MM/yyyy ");
			}, this.Dispatcher);
			timer.Start();
		}
	}
}
