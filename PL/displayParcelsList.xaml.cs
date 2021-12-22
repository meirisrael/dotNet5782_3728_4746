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
	/// Interaction logic for displayParcelsList.xaml
	/// </summary>
	public partial class displayParcelsList : Window
	{
		private BlApi.IBL bl;
		private IEnumerable<BO.ParcelToList> parcels;
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
		public displayParcelsList(BlApi.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
			StatusSelector.ItemsSource = Enum.GetValues(typeof(BO.ParcelStatues));
			ParcelListView.Visibility = Visibility.Visible;
			parcels = bl.GetListOfParcels(p => true);
			ParcelListView.ItemsSource = parcels;
		}

		private void Close_Click(object sender, RoutedEventArgs e) => Close();

		private void groupButton_Click(object sender, RoutedEventArgs e)
		{
			if (groupButton.Content.ToString() == "Group the List")
			{
				ParcelListViewGrouping.ItemsSource = parcels;
				CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelListViewGrouping.ItemsSource);
				PropertyGroupDescription groupDescription = new PropertyGroupDescription("NameSender");
				view.GroupDescriptions.Add(groupDescription);
				ParcelListViewGrouping.Visibility = Visibility.Visible;
				ParcelListView.Visibility = Visibility.Hidden;
				groupButton.Content = "Default display";
				StatusSelector.IsEnabled = false;
			}
			else if (groupButton.Content.ToString() == "Default display")
			{
				ParcelListViewGrouping.Visibility = Visibility.Hidden;
				ParcelListView.Visibility = Visibility.Visible;
				groupButton.Content = "Group the List";
				parcels = bl.GetListOfParcels(d => true);
				ParcelListView.ItemsSource = parcels;
				StatusSelector.IsEnabled = true;
			}
		}

		private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{filterByStatus();}
		/// <summary>
		/// filter the list view by the status of parcel
		/// </summary>
		private void filterByStatus()
		{
			if (StatusSelector.SelectedItem == null)
				return;
			else if (StatusSelector.SelectedItem != null)
			{
				parcels = bl.GetListOfParcels(p => true);
				parcels = parcels.ToList().FindAll(p => p.Status == (BO.ParcelStatues)StatusSelector.SelectedItem);
			}
			else
				parcels = bl.GetListOfParcels(d => true);
			ParcelListView.ItemsSource = parcels;
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			StatusSelector.SelectedItem = null;
			parcels = bl.GetListOfParcels(p => true);
			ParcelListView.ItemsSource = parcels;
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			new parcelWindow(bl,ParcelListView).ShowDialog();
		}
	}
}
