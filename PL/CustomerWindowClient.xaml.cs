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
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace PL
{
	/// <summary>
	/// Interaction logic for customerWindow.xaml
	/// </summary>
	public partial class CustomerWindowClient : Window
	{
		private BlApi.IBL bl;
		private BO.Customer customer;
		//------------------------------------------------------------------ FUNC AND CONST VARIABL --------------------------------------------------------------------------------------------------
		private const Int32 GWL_STYLE = -16;
		private const uint MF_BYCOMMAND = 0x00000000;
		private const uint MF_BYPOSITION = 0x00000400;
		private const uint SC_CLOSE = 0xF060;

		[DllImport("user32.dll")]
		static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("user32.dll")]
		static extern uint RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);

		protected override void OnSourceInitialized(EventArgs e)
			{
				base.OnSourceInitialized(e);

				WindowInteropHelper wih = new WindowInteropHelper(this);
				IntPtr hWnd = wih.Handle;
				IntPtr hMenu = GetSystemMenu(hWnd, false);

				// CloseButton disabled
				RemoveMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
			}

		//-------------------------------------------------------------------------- CTOR ------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// ctor for registration of a new customer
		/// </summary>
		/// <param name="ibl"></param>
		public CustomerWindowClient(BlApi.IBL ibl)
		{
			bl = ibl;
			InitializeComponent();
			addCustomer_Grid.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// ctor to view and modify the profile
		/// </summary>
		/// <param name="ibl"></param>
		/// <param name="c"></param>
		public CustomerWindowClient(BlApi.IBL ibl, BO.Customer c)
		{
			bl = ibl;
			customer = c;
			InitializeComponent();
			update_Grid.Visibility = Visibility.Visible;
			customerLabel.Content = customer.ToString();
			if (customer.ParcelFromCustomer.Count != 0)
			{
				parcelListView.Visibility = Visibility.Visible;
				parcelListView.ItemsSource = bl.GetListOfParcels(p => p.SenderId == c.Id);
			}
		}
		//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// button "ADD" become available if all fives Textbox are filled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LatitudeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			LatitudeBox.Background = Brushes.LightGreen;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		/// <summary>
		/// button "ADD" become available if all fives Textbox are filled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void IdBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IdBox.Background = Brushes.LightGreen;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
			}
		/// <summary>
		/// button "ADD" become available if all fives Textbox are filled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			NameBox.Background = Brushes.LightGreen;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		/// <summary>
		/// button "ADD" become available if all fives Textbox are filled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			PhoneBox.Background = Brushes.LightGreen;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		/// <summary>
		/// button "ADD" become available if all fives Textbox are filled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LongitudeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			LongitudeBox.Background = Brushes.LightGreen;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		/// <summary>
		/// button "Update" is available if one of the fields "Name" or "Phone" is filled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void upNameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (upNameBox.Text != customer.Name || upPhoneBox.Text != customer.Phone)
			{ update_button.IsEnabled = true; upNameBox.Background = Brushes.LightGreen; }
			else
			{ update_button.IsEnabled = false; upNameBox.Background = Brushes.White; }
		}
		/// <summary>
		/// button "Update" is available if one of the fields "Name" or "Phone" is filled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void upPhoneBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (upNameBox.Text != customer.Name || upPhoneBox.Text != customer.Phone)
			{ update_button.IsEnabled = true; upPhoneBox.Background = Brushes.LightGreen; }
			else
			{ update_button.IsEnabled = false; upPhoneBox.Background = Brushes.White; }
		}

		/// <summary>
		/// click on button "add" to add a new customer with the details from the five Textbox above
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>


		private void add_Click(object sender, RoutedEventArgs e)
		{
			string id = IdBox.Text;
			string lon = LongitudeBox.Text;
			string lat = LatitudeBox.Text;
			int customerId;
			double longitude, latitude;
			BO.Location location = new();
			if (!int.TryParse(id, out customerId))
			{
				MessageBox.Show("Customer ID most be an intenger", "ERROR");
				IdBox.Background = Brushes.Salmon;
				return;
			}
			else
				customerId = int.Parse(id);
			if (!double.TryParse(lon, out longitude) || longitude > 180 || longitude < -180)
			{
				MessageBox.Show("Longitude ID most be a double between -180 and 180", "ERROR");
				LongitudeBox.Background = Brushes.Salmon;
				return;
			}
			else
				longitude = double.Parse(lon);
			if (!double.TryParse(lat, out latitude) || latitude > 90 || latitude < -90)
			{
				MessageBox.Show("Latitude ID most be a double between -90 and 90", "ERROR");
				LatitudeBox.Background = Brushes.Salmon;
				return;
			}
			else
				latitude = double.Parse(lat);
			location.Longitude = longitude;
			location.Latitude = latitude;
			try
			{
				bl.AddCustomer(customerId, NameBox.Text, PhoneBox.Text, location);
				MessageBox.Show("Successfuly added", "Successfull");
				MessageBox.Show($"Your userId is- {customerId}\nYour code is- {NameBox.Text + customerId}", "Login information");
				Close();
			}
			catch (BO.IdExist)
			{
				MessageBox.Show("ID alredy exist", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
		}
		/// <summary>
		/// "Update" button to update the name and/or the phone number of a customer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void update_button_Click(object sender, RoutedEventArgs e)
		{
			bl.UpdateCustomer(customer.Id, upNameBox.Text, upPhoneBox.Text);
			MessageBox.Show("Successfuly updated");
			customer = bl.GetCustomer(customer.Id);
			customerLabel.Content = customer.ToString();
			upPhoneBox.Text = customer.Phone;
			upNameBox.Text = customer.Name;
			upPhoneBox.Background = Brushes.White;
			upNameBox.Background = Brushes.White;
		}
		/// <summary>
		///  double click on a parcel from the list to view his details
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void parcelListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if ((BO.ParcelToList)parcelListView.SelectedItem == null)
				MessageBox.Show("Choose a parcel !!", "ERROR");
			else
			{
				new ParcelWindowClient(bl, (BO.ParcelToList)parcelListView.SelectedItem).ShowDialog();
				parcelListView.ItemsSource = bl.GetListOfParcels(p => p.SenderId == customer.Id);
				customer = bl.GetCustomer(customer.Id);
				customerLabel.Content = customer.ToString();
			}
		}
	
		//Close button to quit the page
		private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
	}
}
