using System.Collections.ObjectModel;
using System.Windows.Automation;
using System.Windows.Input;

namespace Pathfinder.ViewModels
{
	public class TreeViewModel : BaseViewModel
	{
		private ObservableCollection<TreeItemViewModel> _items;

		public ObservableCollection<TreeItemViewModel> Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		public TreeViewModel()
		{
			Items = new ObservableCollection<TreeItemViewModel>{new TreeItemViewModel(AutomationElement.RootElement)};
			SetSelectedItemCommand = new RelayCommand<TreeItemViewModel>(selected => { SelectedItem = selected.InnerElementViewModel; });
		}

		public ICommand SetSelectedItemCommand { get; set; }

		private AutomationElementViewModel _selectedItem;
		public AutomationElementViewModel SelectedItem
		{
			get { return _selectedItem; }
			private set
			{
				if (_selectedItem != value)
				{
					_selectedItem = value;
					OnPropertyChanged();
				}
			}
		}
	}
}
