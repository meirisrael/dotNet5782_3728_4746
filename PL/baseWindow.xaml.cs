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
	/// Interaction logic for baseWindow.xaml
	/// </summary>
	public partial class baseWindow : Window
	{
		private BlApi.IBL bl;
		private ListView baseList;
		private BO.BaseStation BaseStation;
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
		/// if the user want to open the action on base grid
		/// </summary>
		/// <param name="ibl"></param>
		/// <param name="b"> the base station that the user choose</param>
		/// <param name="list"> the list view </param>
		public baseWindow(BlApi.IBL ibl, BO.BaseToList b,ListView list)
		{
			InitializeComponent();
			bl = ibl;
			baseList = list;
			BaseStation = bl.GetBaseStation(b.Id);
			action_base_Grid.Visibility = Visibility.Visible;
			if (BaseStation.DroneInCharge.Count == 0)
				delete_base.IsEnabled = true;
			Base_details.Content =BaseStation.ToString();
			droneListView.ItemsSource = BaseStation.DroneInCharge;
		}
		/// <summary>
		/// uf the user want to add a new base station
		/// </summary>
		/// <param name="ibl"></param>
		/// <param name="list"> the list view </param>
		public baseWindow(BlApi.IBL ibl, ListView list)
		{
			InitializeComponent();
			bl = ibl;
			baseList = list;
			add_base_Grid.Visibility = Visibility.Visible;
		}

		private void chargeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ChargeBox.Background = null;
			if (LatitudeBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "" && ChargeBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void latitudeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			LatitudeBox.Background = null;
			if (ChargeBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "" && LatitudeBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void longitudeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			LongitudeBox.Background = null;
			if (LatitudeBox.Text != "" && ChargeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "" && LongitudeBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			NameBox.Background = null;
			if (LatitudeBox.Text != "" && LongitudeBox.Text != "" && ChargeBox.Text != "" && IdBox.Text != "" && NameBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void idBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IdBox.Background = null;
			if (LatitudeBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && ChargeBox.Text != "" && IdBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void add_button_Click(object sender, RoutedEventArgs e)
		{
			string id = IdBox.Text;
			string name = NameBox.Text;
			string charge = ChargeBox.Text;
			string longi = LongitudeBox.Text;
			string lati = LatitudeBox.Text;
			int baseId, baseName, chargeSlots;
			double longitude, latitude;

			if (!int.TryParse(id, out baseId))
			{
				MessageBox.Show("Base-Station ID most be an intenger", "ERROR");
				IdBox.Background = Brushes.Salmon;
				return;
			}
			else baseId = int.Parse(id);

			if (!int.TryParse(name, out baseName))
			{
				MessageBox.Show("Base-Station Name most be an intenger", "ERROR");
				NameBox.Background = Brushes.Salmon;
				return;
			}
			else baseName = int.Parse(name);

			if (!int.TryParse(charge, out chargeSlots))
			{
				MessageBox.Show("Base-Station charge-slots most be an intenger", "ERROR");
				ChargeBox.Background = Brushes.Salmon;
				return;
			}
			else chargeSlots = int.Parse(charge); 
			
			if (!double.TryParse(longi, out longitude))
			{
				MessageBox.Show("Base-Station longitude most be an intenger", "ERROR");
				LongitudeBox.Background = Brushes.Salmon;
				return;
			}
			else longitude = double.Parse(longi); 
			
			if (!double.TryParse(lati, out latitude))
			{
				MessageBox.Show("Base-Station latitude most be an intenger", "ERROR");
				LatitudeBox.Background = Brushes.Salmon;
				return;
			}
			else latitude = double.Parse(lati);
			BO.Location loc = new BO.Location { Longitude = longitude, Latitude = latitude };
			try
			{
				bl.AddBaseStation(baseId, baseName, chargeSlots, loc);
				MessageBox.Show("Successfuly added", "Successfull");
				baseList.ItemsSource = bl.GetListOfBaseStations(b => true);
				Close();
			}
			catch (BO.InvalidId)//drone id
			{
				MessageBox.Show("ID need to be bigger than zero", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
			catch (BO.InvalidChargeSlot)
			{
				MessageBox.Show("Charge-slots need to be bigger than zero", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
			catch (BO.InvalidLoc ex)
			{
				if (ex.type == "LATITUDE")
				{
					MessageBox.Show($"Latitude need to be between {ex.range}", "ERROR");
					LatitudeBox.Background = Brushes.Salmon;
				}
				else
				{
					MessageBox.Show($"Longitude need to be between {ex.range}", "ERROR");
					LongitudeBox.Background = Brushes.Salmon;
				}
			}
			catch (BO.IdExist)
			{
				MessageBox.Show("ID alredy exist", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
		}

		private void name_base_TextChanged(object sender, TextChangedEventArgs e)
		{
			name_base.Background = null;
			if (name_base.Text != "") update_button.IsEnabled = true;
			else update_button.IsEnabled = false;
		}

		private void chargeSlots_base_TextChanged(object sender, TextChangedEventArgs e)
		{
			chargeSlots_base.Background = null;
			if (chargeSlots_base.Text != "") update_button.IsEnabled = true;
			else update_button.IsEnabled = false;
		}

		private void update_Click(object sender, RoutedEventArgs e)
		{
			string name = name_base.Text;
			string charge = chargeSlots_base.Text;
			int baseName, chargeSlots;
			if (!int.TryParse(name, out baseName) && name != "")
			{
				MessageBox.Show("Base-Station Name most be an intenger", "ERROR");
				name_base.Background = Brushes.Salmon;
				return;
			}

			if (!int.TryParse(charge, out chargeSlots) && charge != "")
			{
				MessageBox.Show("Base-Station charge-slots most be an intenger", "ERROR");
				chargeSlots_base.Background = Brushes.Salmon;
				return;
			}

			try
			{
				bl.UpdateBaseStation(BaseStation.Id, name, charge);
				MessageBox.Show("Successfuly Update", "Successfull");
				Base_details.Content = bl.GetBaseStation(BaseStation.Id);
				baseList.ItemsSource = bl.GetListOfBaseStations(d => true);
				name_base.Text = "";
				chargeSlots_base.Text = "";
			}
			catch (Exception)
			{ }
		}

		private void close_Click(object sender, RoutedEventArgs e) => Close();

		private void delete_Click(object sender, RoutedEventArgs e)
		{
			//bl.GetBaseStation(BaseStation.Id)
		}

		private void droneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				BO.DroneToList d = new BO.DroneToList();
				d.Id = BaseStation.DroneInCharge[droneListView.SelectedIndex].DroneId;
				new droneWindow(bl, d, droneListView).ShowDialog();
				BaseStation = bl.GetBaseStation(BaseStation.Id);
				droneListView.ItemsSource = BaseStation.DroneInCharge;
				Base_details.Content = BaseStation.ToString();
				if (BaseStation.DroneInCharge.Count == 0)
					delete_base.IsEnabled = true;
			}
			catch (Exception)
			{ MessageBox.Show("Choose a drone !!", "ERROR"); }
		}
	}
}