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
	/// Interaction logic for parcelWindow.xaml
	/// </summary>
	public partial class parcelWindow : Window
	{
		private BlApi.IBL bl;
		private BO.Parcel parcel;
		//-------------------------------------------------------------- FUNC AND CONST VARIABL -------------------------------------------------------------------------------------------------
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
		//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// ctor add new parcel
		/// </summary>
		/// <param name="ibl"></param>
		public parcelWindow(BlApi.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
			addParcel_grid.Visibility = Visibility.Visible;
			weightSelector.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
			prioritySelector.ItemsSource = Enum.GetValues(typeof(BO.Priorities));
		}
		/// <summary>
		/// ctor update data of parcel and see details 
		/// </summary>
		/// <param name="ibl"></param>
		/// <param name="p"></param>
		public parcelWindow(BlApi.IBL ibl, BO.ParcelToList p,string status)
		{
			InitializeComponent();
			actionPrcel_grid.Visibility = Visibility.Visible;
			bl = ibl;
			parcel = bl.GetParcel(p.Id);
			parcelDetails.Content = parcel.ToString();
			shippingButton();
			if (parcel.Delivered != null || parcel.Scheduled == null)
				drone_Button.Visibility = Visibility.Hidden;
		}

		/// <summary>
		/// after the user enter all data for a new parcel can click to add the parcel to the data base
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void add_button_Click(object sender, RoutedEventArgs e)
		{
			string id = idBox.Text;
			string sender_ = senderBox.Text;
			string target = targetBox.Text;
			int parcelId, senderId, targetId;
			if (!int.TryParse(id, out parcelId))
			{
				MessageBox.Show("Parcel ID most be an intenger", "ERROR");
				idBox.Background = Brushes.Salmon;
				return;
			}
			else
				parcelId = int.Parse(id);
			if (!int.TryParse(id, out senderId))
			{
				MessageBox.Show("Sender ID most be an intenger", "ERROR");
				senderBox.Background = Brushes.Salmon;
				return;
			}
			else
				senderId = int.Parse(sender_);
			if (!int.TryParse(id, out targetId))
			{
				MessageBox.Show("Target ID most be an intenger", "ERROR");
				targetBox.Background = Brushes.Salmon;
				return;
			}
			else
				targetId = int.Parse(target);
			try
			{
				bl.AddParcel(parcelId, senderId, targetId, (BO.WeightCategories)weightSelector.SelectedItem, (BO.Priorities)prioritySelector.SelectedItem);
				MessageBox.Show("Successfuly added", "Successfull");
				Close();
			}
			catch (BO.InvalidId ex)
			{
				MessageBox.Show($"{ex.type} ID need to be bigger than zero", "ERROR");
				if (ex.type == "PARCEL")
					idBox.Background = Brushes.Salmon;
				else if (ex.type == "SENDER")
					senderBox.Background = Brushes.Salmon;
				else
					targetBox.Background = Brushes.Salmon;
			}
			catch (BO.IdExist ex)
			{
				MessageBox.Show($"{ex.type} ID aleredy exist", "ERROR");
				idBox.Background = Brushes.Salmon;
			}
			catch (BO.IdNotExist ex)
			{
				MessageBox.Show($"{ex.type} ID not exist", "ERROR");
				if (ex.type == "SENDER")
					senderBox.Background = Brushes.Salmon;
				else
					targetBox.Background = Brushes.Salmon;
			}
			catch (BO.SenderTargetIdEqual)
			{
				MessageBox.Show("The target and the sender is the same paerson", "ERROR");
				senderBox.Background = Brushes.Salmon;
				targetBox.Background = Brushes.Salmon;
			}
		}
		/// <summary>
		/// if the customer(sender) deliver the parcel to the drone and if the customer(target)recive the parcel they can confirm 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void parcelShip_Button_Click(object sender, RoutedEventArgs e)
		{
			if (parcelShip_Button.Content == "Collection confirmation")
			{
				bl.ParcelCollection(parcel.Drone.Id);
				parcel = bl.GetParcel(parcel.Id);
				parcelDetails.Content = parcel.ToString();
				shippingButton();
			}
			else
			{
				bl.ParcelDeliverd(parcel.Drone.Id);
				parcel = bl.GetParcel(parcel.Id);
				parcelDetails.Content = parcel.ToString();
				shippingButton();
				drone_Button.Visibility = Visibility.Hidden;
			}
		}
		/// <summary>
		/// if the user want to see custome(sender) details
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void senderDetails_Click(object sender, RoutedEventArgs e)
		{
			BO.CustomerToList customer = new BO.CustomerToList { Id = parcel.Sender.Id };
			new CustomerWindow(bl, customer,"admin").ShowDialog();
			parcel = bl.GetParcel(parcel.Id);
			parcelDetails.Content = parcel.ToString();
		}
		/// <summary>
		/// if the user want to see custome(target) details
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void targetDetails_Click(object sender, RoutedEventArgs e)
		{
			BO.CustomerToList customer = new BO.CustomerToList { Id = parcel.Target.Id };
			new CustomerWindow(bl, customer,"admin").ShowDialog();
			parcel = bl.GetParcel(parcel.Id);
			parcelDetails.Content = parcel.ToString();
		}
		/// <summary>
		/// if the user want to see drone details or update data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void droneDetails_Click(object sender, RoutedEventArgs e)
		{
			BO.DroneToList drone = new BO.DroneToList { Id = parcel.Drone.Id };
			new droneWindow(bl, drone).ShowDialog();
			parcel = bl.GetParcel(parcel.Id);
			parcelDetails.Content = parcel.ToString();
		}

		/// <summary>
		/// set which functionality the button has according to the parcel data
		/// </summary>
		private void shippingButton()
		{
			if (parcel.Scheduled != null)
				parcelShip_Button.Content = "Collection confirmation";
			if (parcel.PickedUp != null)
				parcelShip_Button.Content = "Confirmation of delivery";
			if (parcel.Delivered != null)
			{
				parcelShip_Button.Visibility = Visibility.Hidden;
				close_.HorizontalAlignment = HorizontalAlignment.Center;
				close_.Margin = new(0,0,0,50);
			}
		}


		/// <summary>
		/// enter data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> warp </param>
		private void idBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			idBox.Background = Brushes.LightGreen;
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}
		/// <summary>
		/// enter data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> warp </param>
		private void senderBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			senderBox.Background = Brushes.LightGreen;
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}
		/// <summary>
		/// enter data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> warp </param>
		private void targetBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			targetBox.Background = Brushes.LightGreen;
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}
		/// <summary>
		/// choose option
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void weightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}
		/// <summary>
		/// choose option
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void prioritySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}

		/// <summary>
		/// if the user want to cancel or close the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void Cancel_Click(object sender, RoutedEventArgs e) => Close();


		private void adminOption(BO.ParcelToList p)
		{

		}
		private void clientOption(BO.ParcelToList p)
		{

		}
	}
}
