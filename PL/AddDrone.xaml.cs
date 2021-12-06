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
	/// Interaction logic for AddDrone.xaml
	/// </summary>
	public partial class AddDrone : Window
	{
		private IBL.IBL bl;
		public AddDrone(IBL.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
			WeightSelector.ItemsSource = Enum.GetValues(typeof(IBL.BO.WeightCategories));
			BaseListView.ItemsSource = bl.GetListOfBaseStations(b => b.ChargeSlots > 0);
			for (int i = 0; i < bl.GetListOfBaseStations(b => b.ChargeSlots > 0).Count(); ++i)
			{
				ComboBoxItem newItem = new ComboBoxItem();
				newItem.Content = bl.GetListOfBaseStations(b => b.ChargeSlots > 0).ToList()[i].Id;
				BaseSelectore.Items.Add(newItem);
			}
		}
		private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}
		private void BaseSelectore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}
		private void Add_Click(object sender, RoutedEventArgs e)
		{
			string id = IdBox.Text;
			int droneId, firstBase;
			firstBase = int.Parse(BaseSelectore.Text.ToString());
			if (!int.TryParse(id, out droneId))
			{
				MessageBox.Show("Drone ID most be an intenger", "ERROR");
				return;
			}
			else
				droneId = int.Parse(id);
			try
			{
				bl.AddDrone(droneId, ModelBox.Text, (IBL.BO.WeightCategories)WeightSelector.SelectedItem, firstBase);
				MessageBox.Show("Successfuly added", "Successfull");
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ERROR");
				IdBox.Background = Brushes.OrangeRed;
				
			}
			return;

		}
		private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

		private void IdBox_TextChanged(object sender, TextChangedEventArgs e)
		{
				IdBox.Background = null;
		}
	}
}
