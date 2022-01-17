using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
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
		private ListView droneList_;
		displayListOfDrones mySender = new displayListOfDrones();
		private System.ComponentModel.BackgroundWorker backgroundWorker1 = new BackgroundWorker();
		private bool flag;
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
			this.DataContext = drone;
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
		public droneWindow(BlApi.IBL ibl, displayListOfDrones win)
		{
			InitializeComponent();
			bl = ibl;
			mySender = win;
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
		public droneWindow(BlApi.IBL ibl, BO.DroneToList d, displayListOfDrones win)
		{
			InitializeComponent();
			InitializeBackgroundWorker();//////
			bl = ibl;
			mySender = win;
			action_drone_Grid.Visibility = Visibility.Visible;
			drone = bl.GetDrone(d.Id);
			Drone_label.Content = drone.ToString();
			charging_button();
			shipping_button();
			if (drone.InTransit.Id != 0)
				parcelDetails_Button.Visibility = Visibility.Visible;
		}
		private void InitializeBackgroundWorker()//////
		{
			backgroundWorker1.DoWork +=
				new DoWorkEventHandler(backgroundWorker1_DoWork);
			backgroundWorker1.RunWorkerCompleted +=
				new RunWorkerCompletedEventHandler(
			backgroundWorker1_RunWorkerCompleted);
			backgroundWorker1.ProgressChanged +=
				new ProgressChangedEventHandler(
			backgroundWorker1_ProgressChanged);
			backgroundWorker1.WorkerSupportsCancellation = true;
			backgroundWorker1.WorkerReportsProgress = true;
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
				mySender.dronesList.Clear();
				foreach (var item in bl.GetListOfDrones(d => true))
				{
					mySender.dronesList.Add(item);
				}
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
				mySender.dronesList.Clear();
				foreach (var item in bl.GetListOfDrones(d => true))
				{
					mySender.dronesList.Add(item);
				}
				Drone_label.Content = drone.ToString();
				UpdateModelBox.Text = drone.Model;
				Update_button.IsEnabled = false; 
				UpdateModelBox.Background = Brushes.White;
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
			drone = bl.GetDrone(drone.Id);
			mySender.dronesList.Clear();
			foreach (var item in bl.GetListOfDrones(d => true))
			{
				mySender.dronesList.Add(item);
			}
			Drone_label.Content = drone.ToString();
			charging_button();
			shipping_button();
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
					parcelDetails_Button.Visibility = Visibility.Hidden;
				}
				catch (Exception)
				{ MessageBox.Show("Parcel can't be deliverd", "ERROR"); }
			}
			else { }
			drone = bl.GetDrone(drone.Id);
			mySender.dronesList.Clear();
			foreach (var item in bl.GetListOfDrones(d => true))
			{
				mySender.dronesList.Add(item);
			}
			Drone_label.Content = drone.ToString();
			charging_button();
			shipping_button();
		}
		/// <summary>
		/// if the user want to see or update parcel details
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void parcelDetails_Click(object sender, RoutedEventArgs e)
		{
			BO.ParcelToList p = new BO.ParcelToList { Id = drone.InTransit.Id };
			new ParcelWindowAdmin(bl,p).ShowDialog();
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
				charge_button.Content = "Can't charging now";
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
			if (UpdateModelBox.Text != drone.Model && UpdateModelBox.Text != "")
			{ Update_button.IsEnabled = true; UpdateModelBox.Background = Brushes.LightGreen; }
			else
			{ Update_button.IsEnabled = false; UpdateModelBox.Background = Brushes.White; }
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
		/// if the user press the button "close" or "cancel" so close the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void Close_Click(object sender, RoutedEventArgs e)
		{
			if (auto_button.Content.ToString() == "Manual")
			{
				Cursor = Cursors.Wait;
				auto_button.IsEnabled = false;
				controlContainer.IsEnabled = false;
				close_button.IsEnabled = false;
				this.backgroundWorker1.CancelAsync();
				flag = true;
			}
			else
			{
				Close();
			}
		}

		private void Automatic_Button_Click(object sender, RoutedEventArgs e)
		{
			
			if (auto_button.Content.ToString() == "Manual")
			{
				Cursor = Cursors.Wait;
				auto_button.IsEnabled = false;
				close_button.IsEnabled = false;
				this.backgroundWorker1.CancelAsync();
			}
			else
			{
				auto_button.Content = "Manual";
				auto_button.Background = Brushes.Orange;
				controlContainer.Visibility = Visibility.Hidden;
				this.backgroundWorker1.RunWorkerAsync();
			}
		}
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			//BackgroundWorker worker = sender as BackgroundWorker;
			Action<BO.Drone> a =(drone)=> ReportProgressSimulator(drone);
			bl.Simulator(drone.Id,a, Cancellation);
		}
		private void ReportProgressSimulator(BO.Drone drone)
		{
			//drone = bl.GetDrone(drone.Id);
			this.backgroundWorker1.ReportProgress(0, drone);
		}
		private bool Cancellation()
        {
			return this.backgroundWorker1.CancellationPending;
		}
		private void backgroundWorker1_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(e.Error.Message);
			}
            else
            {
				auto_button.Content = "Automatic";
				auto_button.IsEnabled = true;
				close_button.IsEnabled = true;
				controlContainer.IsEnabled = true;
				auto_button.Background = Brushes.LightGreen;
				charging_button();
				shipping_button();
				Cursor = Cursors.Arrow;
				controlContainer.Visibility = Visibility.Visible;
				shipping_button();
				charging_button();
				if (flag) Close();
            }
		}
		private void backgroundWorker1_ProgressChanged(object sender,ProgressChangedEventArgs e)
		{
			Drone_label.Content = e.UserState.ToString();
			mySender.dronesList.Clear();
			foreach (var item in bl.GetListOfDrones(d => true))
			{
				mySender.dronesList.Add(item);
			}
		}
	}
}
