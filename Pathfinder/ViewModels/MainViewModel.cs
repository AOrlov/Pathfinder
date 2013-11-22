using System;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;

namespace Pathfinder.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public ICommand CheckCommand { get; set; }
		private string _path;
		
		public string Path
		{
			get { return _path; }
			set
			{
				_path = value;
				OnPropertyChanged();
			}
		}

		private int _selectedIndex;
		public int SelectedIndex
		{
			get { return _selectedIndex; }
			set
			{
				_selectedIndex = value;
				OnPropertyChanged();
				SelectedItem = Elements[_selectedIndex];
			}
		}

		private AutomationElementViewModel _selectedItem;
		public AutomationElementViewModel SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				_selectedItem = value;
				OnPropertyChanged();
			}
		}

		private AutomationElementViewModel[] _elements;


		public AutomationElementViewModel[] Elements
		{
			get { return _elements; }
			set
			{
				_elements = value;
				OnPropertyChanged();
			}
		}

		public MainViewModel()
		{
			CheckCommand = new RelayCommand(OnCheckCommand);
		}

		public void OnCheckCommand()
		{
			try
			{
				Elements = AutomationHelper.LocateElement(AutomationElement.RootElement, Path).Select(AutomationElementViewModel.FromAutomationElement).ToArray();
			}
			catch (Exception)
			{
				Elements = new AutomationElementViewModel[0];
			}
		
		}
	}
}
