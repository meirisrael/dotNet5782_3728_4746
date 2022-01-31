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
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PL
{
	/// <summary>
	/// Interaction logic for displayListOfDrones.xaml
	/// </summary>
	public partial class displayListOfDrones : Window
	{
		private BlApi.IBL bl;
		IEnumerable<BO.DroneToList> drones = new List<BO.DroneToList>();
		public ObservableCollection<BO.DroneToList> dronesList { set; get; }
		bool flag = false;
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
		/// CTOR 
		/// </summary>
		/// <param name="ibl"></param>
		public displayListOfDrones(BlApi.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
			drones = bl.GetListOfDrones(d => true);
			//DroneListView.ItemsSource = drones;
			dronesList = new ObservableCollection<BO.DroneToList>(drones);
			dronesList.CollectionChanged += dronesList_CollectionChanged;
			DroneListView.ItemsSource = dronesList;
			this.DataContext = this;

			StatusSelector.ItemsSource = Enum.GetValues(typeof(BO.DroneStatuses));
			WeightSelector.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
		}

		/// <summary>
		/// if the user select a status to filter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{ filterByStatus(); }
		/// <summary>
		/// if the user select a weight to filter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{ filterByWeight(); }

		/// <summary>
		/// if the user press the button clear "status selector"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClearStatus_Click(object sender, RoutedEventArgs e)
		{
			StatusSelector.SelectedItem = null;
			if (WeightSelector.SelectedItem == null)
			{
				dronesList.Clear();
				foreach (var item in bl.GetListOfDrones(d => true))
				{
					dronesList.Add(item);
				}
			}
			else
			{
				dronesList.Clear();
				foreach (var item in bl.GetListOfDrones(d => true))
				{
					dronesList.Add(item);
				}
			}
		}
		/// <summary>
		/// if the user press the button clear "weight selector"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClearWeight_Click(object sender, RoutedEventArgs e)
		{
			WeightSelector.SelectedItem = null;
			if (StatusSelector.SelectedItem == null)
			{
				dronesList.Clear();
				foreach (var item in bl.GetListOfDrones(d => true))
				{
					dronesList.Add(item);
				}
			}
			else
			{
				dronesList.Clear();
				foreach (var item in bl.GetListOfDrones(d => d.Status == (BO.DroneStatuses)StatusSelector.SelectedItem))
				{
					dronesList.Add(item);
				}
			}
		}
		/// <summary>
		/// groop all drone by her status
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void group_Click(object sender, RoutedEventArgs e)
		{
			if (groupButton.Content.ToString() == "Group the List")
			{
				if (!flag)
				{
					DroneListViewGrouping.ItemsSource = dronesList;
					CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DroneListViewGrouping.ItemsSource);
					PropertyGroupDescription groupDescription = new PropertyGroupDescription("Status");
					view.GroupDescriptions.Add(groupDescription);
					flag = true;
				}
				DroneListViewGrouping.Visibility = Visibility.Visible;
				DroneListView.Visibility = Visibility.Hidden;
				groupButton.Content = "Default display";
				StatusSelector.IsEnabled = false;
				WeightSelector.IsEnabled = false;
			}
			else if (groupButton.Content.ToString() == "Default display")
			{
				DroneListViewGrouping.Visibility = Visibility.Hidden;
				DroneListView.Visibility = Visibility.Visible;
				groupButton.Content = "Group the List";
				StatusSelector.IsEnabled = true;
				WeightSelector.IsEnabled = true;
			}
		}


		/// <summary>
		/// if the user press the button "add drone" so open a new window for add
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Add_Click(object sender, RoutedEventArgs e)
		{
			new droneWindow(bl,this).ShowDialog();
			filterByStatus();
			filterByWeight();
		}
		/// <summary>
		/// if the user do a double click on the drone list view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DroneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if ((BO.DroneToList)DroneListView.SelectedItem == null)
				MessageBox.Show("Choose a drone !!", "ERROR");
			else
			{
				new droneWindow(bl, (BO.DroneToList)DroneListView.SelectedItem, this).Show();
				filterByStatus();
				filterByWeight();
			}
		}
		private void DroneListViewGrouping_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if ((BO.DroneToList)DroneListViewGrouping.SelectedItem == null)
				MessageBox.Show("Choose a drone !!", "ERROR");
			else
			{
				new droneWindow(bl, (BO.DroneToList)DroneListViewGrouping.SelectedItem,this).Show();
				
				filterByStatus();
				filterByWeight();
			}
		}

		/// <summary>
		/// filter the list view by the status of drone
		/// </summary>
		private void filterByStatus()
		{
			if (StatusSelector.SelectedItem == null && WeightSelector.ItemsSource == null)
				return;
			else if (StatusSelector.SelectedItem != null && WeightSelector.SelectedItem == null)
				drones = bl.GetListOfDrones(d => d.Status == (BO.DroneStatuses)StatusSelector.SelectedItem);
			else if (StatusSelector.SelectedItem != null && WeightSelector.SelectedItem != null)
				drones = bl.GetListOfDrones(d => d.Status == (BO.DroneStatuses)StatusSelector.SelectedItem && d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem);
			else
				drones = bl.GetListOfDrones(d => true);

			DroneListView.ItemsSource = drones;
		}
		/// <summary>
		/// filter the list view by the weight of drone
		/// </summary>
		private void filterByWeight()
		{
			if (WeightSelector.SelectedItem == null && StatusSelector.SelectedItem == null)
				return;
			else if (WeightSelector.SelectedItem != null && StatusSelector.SelectedItem == null)
				drones = bl.GetListOfDrones(d => d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem);
			else if (WeightSelector.SelectedItem != null && StatusSelector.SelectedItem != null)
				drones = bl.GetListOfDrones(d => d.Status == (BO.DroneStatuses)StatusSelector.SelectedItem && d.MaxWeight == (BO.WeightCategories)WeightSelector.SelectedItem);
			else
				drones = bl.GetListOfDrones(d => d.Status == (BO.DroneStatuses)StatusSelector.SelectedItem);

			DroneListView.ItemsSource = drones;
		}

		/// <summary>
		/// if the user press the button close so close the window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Close_Click(object sender, RoutedEventArgs e) => Close();

		public void dronesList_CollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
			DroneListView.ItemsSource = dronesList;
		}
	}
}
