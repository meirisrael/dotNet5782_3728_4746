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
	/// Interaction logic for AdminWindow.xaml
	/// </summary>
	public partial class AdminWindow : Window
	{
		private BlApi.IBL bl;
		public AdminWindow(BlApi.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
		}
		/// <summary>
		/// if the user press the button "drone"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DronesButton_Click(object sender, RoutedEventArgs e)
		{
			new displayListOfDrones(bl).ShowDialog();
		}
		/// <summary>
		/// if the user ant to open the base station window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> press button</param>
		private void BaseButton_Click(object sender, RoutedEventArgs e)
		{
			new displayBaseList(bl).ShowDialog();
		}
		/// <summary>
		/// if the user want to see all customers and to add or upsate one
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Customers_Click(object sender, RoutedEventArgs e)
		{
			new displayCustomersList(bl).ShowDialog();
		}
		/// <summary>
		/// if the user want to see all parcels and to add or update one
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Parcels_Click(object sender, RoutedEventArgs e)
		{
			new displayParcelsList(bl).ShowDialog();
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
