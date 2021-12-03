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

namespace PL
{
	/// <summary>
	/// Interaction logic for displayListOfDrones.xaml
	/// </summary>
	public partial class displayListOfDrones : Window
	{
		private IBL.IBL bl;
		IEnumerable<IBL.BO.DroneToList> drones = new List<IBL.BO.DroneToList>();
		public displayListOfDrones(IBL.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
			drones = bl.GetListOfDrones(d => true);
			DroneListView.ItemsSource = drones;
			StatusSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.DroneStatuses));
			WeightSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.WeightCategories));
		}
		private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (StatusSelector.SelectedItem == null && WeightSelector.ItemsSource == null)
				drones = drones;
			else if (StatusSelector.SelectedItem != null && WeightSelector.SelectedItem == null)
				drones = bl.GetListOfDrones(d => d.Status == (IBL.BO.DroneStatuses)StatusSelector.SelectedItem);
			else if (StatusSelector.SelectedItem != null && WeightSelector.SelectedItem != null)
				drones = bl.GetListOfDrones(d => d.Status == (IBL.BO.DroneStatuses)StatusSelector.SelectedItem && d.MaxWeight == (IBL.BO.WeightCategories)WeightSelector.SelectedItem);
			else
				drones = bl.GetListOfDrones(d => true);

			DroneListView.ItemsSource = drones;
		}
		private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (WeightSelector.SelectedItem == null && StatusSelector.SelectedItem == null)
				drones = drones;
			else if (WeightSelector.SelectedItem != null && StatusSelector.SelectedItem == null)
				drones = bl.GetListOfDrones(d => d.MaxWeight == (IBL.BO.WeightCategories)WeightSelector.SelectedItem);
			else if (WeightSelector.SelectedItem != null && StatusSelector.SelectedItem != null)
				drones = bl.GetListOfDrones(d => d.Status == (IBL.BO.DroneStatuses)StatusSelector.SelectedItem && d.MaxWeight == (IBL.BO.WeightCategories)WeightSelector.SelectedItem);
			else
				drones = bl.GetListOfDrones(d => d.Status == (IBL.BO.DroneStatuses)StatusSelector.SelectedItem);

			DroneListView.ItemsSource = drones;
		}
		private void ClearStatus_Click(object sender, RoutedEventArgs e)
		{
			StatusSelector.SelectedItem = null;
			if (WeightSelector.SelectedItem == null)
				drones = bl.GetListOfDrones(d => true);
			else
				drones = bl.GetListOfDrones(d => d.MaxWeight == (IBL.BO.WeightCategories)WeightSelector.SelectedItem);
			DroneListView.ItemsSource = drones;
		}
		private void ClearWeight_Click(object sender, RoutedEventArgs e)
		{
			WeightSelector.SelectedItem = null;
			if(StatusSelector.SelectedItem == null)
				drones = bl.GetListOfDrones(d => true);
			else
				drones = bl.GetListOfDrones(d => d.Status == (IBL.BO.DroneStatuses)StatusSelector.SelectedItem);
			DroneListView.ItemsSource = drones;
		}
		private void Add_Click(object sender, RoutedEventArgs e)
		{
			new AddDrone(bl).Show();
		}
		private void Close_Click(object sender, RoutedEventArgs e) => Close();
		
		
	}
}
