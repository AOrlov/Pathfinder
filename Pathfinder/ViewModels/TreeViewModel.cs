using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Input;

namespace Pathfinder.ViewModels
{
	public class TreeViewModel : BaseViewModel
	{
		private ObservableCollection<TreeViewItemViewModel> _items;

		public ObservableCollection<TreeViewItemViewModel> Items
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
			_selectedItem = null;
			Items = new ObservableCollection<TreeViewItemViewModel>{new TreeViewItemViewModel(AutomationElement.RootElement)};
			SetSelectedItemCommand = new RelayCommand<TreeViewItemViewModel>(selected => { SelectedItem = selected.InnerElement; });
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
