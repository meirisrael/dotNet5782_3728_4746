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
		private IBL.IBL bl;
		private IBL.BO.Drone drone;
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
		public droneWindow(IBL.IBL ibl,ListView list)
		{
			bl = ibl;
			listOfDrone = list;
			InitializeComponent();
			add_drone_Grid.Visibility = Visibility.Visible;
			WeightSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.WeightCategories));
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
		public droneWindow(IBL.IBL ibl, IBL.BO.DroneToList d, ListView list)
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
				bl.AddDrone(droneId, ModelBox.Text, (IBL.BO.WeightCategories)WeightSelector.SelectedItem, firstBase);
				MessageBox.Show("Successfuly added", "Successfull");
				listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
				Close();
			}
			catch (IBL.BO.IdExist)
			{
				MessageBox.Show("ID alredy exist", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
			catch (IBL.BO.EmptyValue)
			{
				MessageBox.Show("You must be give an name of model", "ERROR");
				ModelBox.Background = Brushes.Salmon;
			}
			return;


		}///jhbhbn
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
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ERROR");
			}
			
		}
		/// <summary>
		/// if the user press the button  "sent to charge" so assigne the drone to a base station to cahrge
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sentCharge_button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				bl.DroneToCharge(drone.Id);
				MessageBox.Show("Successfuly sent to charge", "Successfull");
				drone = bl.GetDrone(drone.Id);
				Drone_label.Content = drone.ToString();
				charging_button();
				shipping_button();
				listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ERROR");
			}
		}
		/// <summary>
		/// if the user press the button " release drone from charge" so need to release the drone from charge
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void releaseCharge_button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				bl.DroneLeaveCharge(drone.Id);
				MessageBox.Show("Successfuly release frome charge", "Successfull");
				drone = bl.GetDrone(drone.Id);
				Drone_label.Content = drone.ToString();
				charging_button();
				shipping_button();
				listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ERROR");
			}
		}
		/// <summary>
		/// if the user press the button "sent the drone to shipping" need to affect the drone to a parcel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sentShipping_button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				bl.AffectParcelToDrone(drone.Id);
				MessageBox.Show("Successfuly associated to an parcel", "Successfull");
				drone = bl.GetDrone(drone.Id);
				Drone_label.Content = drone.ToString();
				shipping_button();
				listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ERROR");
			}

		}
		/// <summary>
		/// if the user press the button "collect the parcel" so sent the drone collect the parcel from the sender 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void collectParcel_button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				bl.ParcelCollection(drone.Id);
				MessageBox.Show("Successfuly collected", "Successfull");
				drone = bl.GetDrone(drone.Id);
				Drone_label.Content = drone.ToString();
				shipping_button();
				listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ERROR");
			}
		}
		/// <summary>
		/// if the user press the button "deliver the parcel" so drone deliver the parcel to the target customer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void deliverParcel_button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				bl.ParcelDeliverd(drone.Id);
				MessageBox.Show("Successfuly deliverd", "Successfull");
				drone = bl.GetDrone(drone.Id);
				Drone_label.Content = drone.ToString();
				shipping_button();
				charging_button();
				listOfDrone.ItemsSource = bl.GetListOfDrones(d => true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ERROR");
			}

		}

		/// <summary>
		/// choose which button visible to the user
		/// </summary>
		private void charging_button()
		{
			if (drone.Status == IBL.BO.DroneStatuses.free)
			{
				sentCharge_button.Visibility = Visibility.Visible;releaseCharge_button.Visibility = Visibility.Hidden; messege_label_charge.Visibility = Visibility.Hidden; }
			else if (drone.Status == IBL.BO.DroneStatuses.Maintenance)
			{ releaseCharge_button.Visibility = Visibility.Visible; sentCharge_button.Visibility = Visibility.Hidden; messege_label_charge.Visibility = Visibility.Hidden; }
			else
				messege_label_charge.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// choose which button visible to the user
		/// </summary>
		private void shipping_button()
		{
			if (drone.Status == IBL.BO.DroneStatuses.Shipping)
			{
				if (drone.InTransit.Status == false)// if not pick-up 
				{ collectParcel_button.Visibility = Visibility.Visible; deliverParcel_button.Visibility = Visibility.Hidden; }
				else if (drone.InTransit.Status == true)
				{ deliverParcel_button.Visibility = Visibility.Visible; collectParcel_button.Visibility = Visibility.Hidden; }
			}
			else if (drone.Status == IBL.BO.DroneStatuses.free)
			{ sentShipping_button.Visibility = Visibility.Visible; collectParcel_button.Visibility = Visibility.Hidden; deliverParcel_button.Visibility = Visibility.Hidden; messege_label_shipp.Visibility = Visibility.Hidden; }
			else
				messege_label_shipp.Visibility = Visibility.Visible;
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
