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
    public partial class customerWindow : Window
    {
		private BlApi.IBL bl;
		private BO.Customer customer;
		private ListView listOfCustomers;
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
		public customerWindow(BlApi.IBL ibl, ListView list)
        {
			bl = ibl;
			listOfCustomers = list;
			InitializeComponent();
			addCustomer_Grid.Visibility = Visibility.Visible;
		}
		public customerWindow(BlApi.IBL ibl,BO.CustomerToList c, ListView list)
		{
			bl = ibl;
			listOfCustomers = list;
			customer = bl.GetCustomer(c.Id);
			InitializeComponent();
			update_Grid.Visibility = Visibility.Visible;
			customerLabel.Content = customer.ToString();
			parcelListView.ItemsSource = bl.GetListOfParcels(p => (p.SenderId == c.Id || p.TargetId == c.Id));
		}
		//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		private void add_Click(object sender, RoutedEventArgs e)
        {
			string id = IdBox.Text;
			string lon = LongitudeBox.Text;
			string lat=LatitudeBox.Text;
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
			if (!double.TryParse(lon, out longitude)|| longitude> 180 || longitude<-180)
			{
				MessageBox.Show("Longitude ID most be a double between -180 and 180", "ERROR");
				LongitudeBox.Background = Brushes.Salmon;
				return;
			}
			else
				longitude = double.Parse(lon);
			if (!double.TryParse(lat, out latitude)||latitude>90||latitude<-90)
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
				listOfCustomers.ItemsSource = bl.GetListOfCustomers();
				Close();
			}
			catch(BO.IdExist)
            {
				MessageBox.Show("ID alredy exist", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}	
        }

		private void LatitudeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			LatitudeBox.Background = null;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		private void IdBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IdBox.Background = null;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
			NameBox.Background = null;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			PhoneBox.Background = null;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		private void LongitudeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			LongitudeBox.Background = null;
			if (LatitudeBox.Text != "" && PhoneBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void update_button_Click(object sender, RoutedEventArgs e)
        {
			bl.UpdateCustomer(customer.Id, upNameBox.Text, upPhoneBox.Text);
			MessageBox.Show("Successfuly updated");
			listOfCustomers.ItemsSource = bl.GetListOfCustomers();
			Close();
		}

        private void upNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
			upNameBox.Background = null;
			if (upNameBox.Text != "" || upPhoneBox.Text != "")
				update_button.IsEnabled = true;
			else 
				update_button.IsEnabled = false;
        }

        private void upPhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
			upPhoneBox.Background = null;
			if (upNameBox.Text != "" || upPhoneBox.Text != "")
				update_button.IsEnabled = true;
			else
				update_button.IsEnabled = false;
		}

        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
			MessageBoxResult result = MessageBox.Show("Are you sure to delete", "Deleting Customer", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				//do something
			}
			else if (result == MessageBoxResult.No)
			{
				//do something else
			}
		}

        private void parcelListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
			if ((BO.ParcelToList)parcelListView.SelectedItem == null)
				MessageBox.Show("Choose a parcel !!", "ERROR");
			else
			{
				//new parcelWindow(bl, (BO.ParcelToList)parcelListView.SelectedItem, parcelListView).ShowDialog();
			}
		}
    }
}
