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
	/// Interaction logic for droneDetails.xaml
	/// </summary>
	public partial class droneWindow : Window
	{
		private BlApi.IBL bl;
		private BO.Drone drone;
		private ListView listOfDrone;
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
		/// if the ctor get only one param so need do open the add window
		/// </summary>
		/// <param name="ibl"></param>
		public droneWindow(BlApi.IBL ibl,ListView list)
		{
			bl = ibl;
			listOfDrone = list;
			InitializeComponent();
			add_drone_Grid.Visibility = Visibility.Visible;
			WeightSelector.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
			BaseListView.ItemsSource = bl.GetListOfBaseStations(b => b.ChargeSlots > 0);
			for (int i = 0; i < bl.GetListOfBaseStations(b => b.ChargeSlots > 0).Count(); ++i)
			{
				ComboBoxItem newItem = new ComboBoxItem();
				newItem.Content = bl.GetListOfBaseStations(b => b.ChargeSlots > 0).ToList()[i].Id;
				BaseSelectore.Items.Add(newItem);
			}
		}
		/// <summary>
		/// if the ctor get two param so need to open the details window
		/// </summary>
		/// <param name="ibl"></param>
		/// <param name="d"></param>
		public droneWindow(BlApi.IBL ibl, BO.DroneToList d, ListView list)
		{
			bl = ibl;
			listOfDrone = list;
			InitializeComponent();
			action_drone_Grid.Visibility = Visibility.Visible;
			drone = bl.GetDrone(d.Id);
			Drone_label.Content = drone.ToString();
			charging_button();
			shipping_button();
		}
		//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// when the user selcte a object in the selector
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (BaseSelectore.SelectedItem != null && IdBox.Text != "" && ModelBox.Text != "")
				add_button.IsEnabled = true;
		}
		/// <summary>
		/// when the user selcte a object in the selector
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BaseSelectore_SelectionChanged(object sender, SelectionChangedEventArgs e) 
		{
			if(WeightSelector.SelectedItem != null && IdBox.Text != "" && ModelBox.Text != "")
				add_button.IsEnabled = true;
		}
		
		/// <summary>
		/// if the user press the button "add" so add a new drone to the data base
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Add_Click(object sender, RoutedEventArgs e)
		{
			string id = IdBox.Text;
			int droneId, firstBase;
			firstBase = int.Parse(BaseSelectore.Text.ToString());
			if (!int.TryParse(id, out droneId))
			{
				MessageBox.Show("Drone ID most be an intenger", "ERROR");
				IdBox.Background = Brushes.Salmon;
				return;
			}
			else
				droneId = int.Parse(id);
			try
			{
				bl.AddDrone(droneId, ModelBox.Text, (BO.WeightCategories)WeightSelector.SelectedItem, firstBase);
				MessageBox.Show("Successfuly added", "Successfull");
				listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
				Close();
			}
			catch (BO.InvalidId)//drone id
			{
				MessageBox.Show("ID need to be bigger than zero", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
			catch (BO.IdExist)
			{
				MessageBox.Show("ID alredy exist", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
			return;


		}
		/// <summary>
		/// if the user press the button "update" so update the model of the drone
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Update_button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				bl.UpdateDrone(drone.Id, UpdateModelBox.Text);
				MessageBox.Show("Successfuly update", "Successfull");
				drone = bl.GetDrone(drone.Id);
				Drone_label.Content = drone.ToString();
				listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
				UpdateModelBox.Text = "";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ERROR");
			}
			
		}
		/// <summary>
		/// if the user press the button for charge so assigne the drone to a base station to charge or release him 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void charge_button_Click(object sender, RoutedEventArgs e)
		{
			if (charge_button.Content.ToString() == "Sent to charge")
			{
				try
				{
					bl.DroneToCharge(drone.Id);
					MessageBox.Show("Successfuly sent to charge", "Successfull");
				}
				catch (Exception)
				{ MessageBox.Show("Drone can't assigne to charge beacause is not free", "ERROR"); }
			}
			else if (charge_button.Content.ToString() == "Release drone from charge")
			{
				try
				{
					bl.DroneLeaveCharge(drone.Id);
					MessageBox.Show("Successfuly release frome charge", "Successfull");
				}
				catch (Exception)
				{ MessageBox.Show("Drone can't release from charge because is not in charge", "ERROR"); }
			}
			else { }
			drone = bl.GetDrone(drone.Id);
			Drone_label.Content = drone.ToString();
			charging_button();
			shipping_button();
			listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
		}
		/// <summary>
		/// if the user press the button for shipping need to affect/collect/delive the parcel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void parcel_button_Click(object sender, RoutedEventArgs e)
		{
			if (parcel_button.Content.ToString() == "Sent to shipping")
			{
				try
				{
					bl.AffectParcelToDrone(drone.Id);
					MessageBox.Show("Successfuly associated to an parcel", "Successfull");
				}
				catch (Exception)
				{ MessageBox.Show("Drone can't be affected to a parcel", "ERROR"); }
			}
			else if (parcel_button.Content.ToString() == "Collecte parcel")
			{
				try
				{
					bl.ParcelCollection(drone.Id);
					MessageBox.Show("Successfuly collected", "Successfull");
				}
				catch (Exception)
				{ MessageBox.Show("Parcel can't be collected", "ERROR"); }
			}
			else if (parcel_button.Content.ToString() == "Deliverd parcel")
			{
				try
				{
					bl.ParcelDeliverd(drone.Id);
					MessageBox.Show("Successfuly deliverd", "Successfull");
					charging_button();
				}
				catch (Exception)
				{ MessageBox.Show("Parcel can't be deliverd", "ERROR"); }
			}
			else { }
			drone = bl.GetDrone(drone.Id);
			Drone_label.Content = drone.ToString();
			shipping_button();
			listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
		}

		/// <summary>
		/// choose which button visible to the user
		/// </summary>
		private void charging_button()
		{
			if (drone.Status == BO.DroneStatuses.free)
				charge_button.Content = "Sent to charge";
			else if (drone.Status == BO.DroneStatuses.Maintenance)
				charge_button.Content = "Release drone from charge";
			else
			{ charge_button.Content = "Can't charging now"; charge_button.FontWeight = new FontWeight(); }
		}
		/// <summary>
		/// choose which button visible to the user
		/// </summary>
		private void shipping_button()
		{
			if (drone.Status == BO.DroneStatuses.Shipping)
			{
				if (drone.InTransit.Status == false)// if not pick-up 
					parcel_button.Content = "Collecte parcel";
				else if (drone.InTransit.Status == true)
					parcel_button.Content = "Deliverd parcel";
			}
			else if (drone.Status == BO.DroneStatuses.free)
				parcel_button.Content = "Sent to shipping";
			else
			{ parcel_button.Content = "Can't do a shipping now"; parcel_button.FontWeight = new FontWeight(); }
		}

		/// <summary>
		/// get the name of model to update 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpdateModel_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (UpdateModelBox.Text != null)
				Update_button.IsEnabled = true;
			else
				Update_button.IsEnabled = false;
		}
		/// <summary>
		/// get the id from the box for add a new drone
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void IdBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IdBox.Background = null;
			if (BaseSelectore.SelectedItem != null && WeightSelector.SelectedItem != null && ModelBox.Text != "" && IdBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}
		/// <summary>
		/// get the model of drone from the box for add a new drone
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ModelBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ModelBox.Background = null;
			if (BaseSelectore.SelectedItem != null && WeightSelector.SelectedItem != null && IdBox.Text != "" && ModelBox.Text != "")
				add_button.IsEnabled = true;
			else
				add_button.IsEnabled = false;
		}

		/// <summary>
		/// if the user press the button "close" so close the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Close_Click(object sender, RoutedEventArgs e) => Close();
		/// <summary>
		/// if the user press the button "cancel" so close the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
	}
}
