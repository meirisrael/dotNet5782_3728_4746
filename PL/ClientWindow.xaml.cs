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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL
{
	/// <summary>
	/// Interaction logic for ClientWindow.xaml
	/// </summary>
	public partial class ClientWindow : Window
	{
		private BlApi.IBL bl;
		private BO.CustomerToList customer;
		public ClientWindow(BlApi.IBL ibl, BO.CustomerToList c)
		{
			InitializeComponent();
			bl = ibl;
			customer = c;
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

		private void Customer_Click(object sender, RoutedEventArgs e)
		{
			new CustomerWindowClient(bl, customer).ShowDialog();
		}

		private void Parcels_Click(object sender, RoutedEventArgs e)
		{
			new ParcelWindowClient(bl).ShowDialog();
		}
	}
}
