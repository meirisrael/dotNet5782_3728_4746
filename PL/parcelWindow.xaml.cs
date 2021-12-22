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
		private BO.Drone parcel;
		private ListView listOfParcels;
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
		public parcelWindow(BlApi.IBL ibl,ListView list)
		{
			InitializeComponent();
			bl = ibl;
			weightSelector.ItemsSource = Enum.GetValues(typeof(BO.WeightCategories));
			prioritySelector.ItemsSource = Enum.GetValues(typeof(BO.Priorities));
			listOfParcels = list;
		}

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
				listOfParcels.ItemsSource = bl.GetListOfParcels(p => true);
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

		private void idBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			idBox.Background = Brushes.LightGreen;
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}

		private void senderBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			senderBox.Background = Brushes.LightGreen;
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}

		private void targetBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			targetBox.Background = Brushes.LightGreen;
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}

		private void weightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}

		private void prioritySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (idBox.Text != "" && senderBox.Text != "" && targetBox.Text != "" && weightSelector.SelectedItem != null && prioritySelector.SelectedItem != null)
				add_button.IsEnabled = true;
			else add_button.IsEnabled = false;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
	}
}
