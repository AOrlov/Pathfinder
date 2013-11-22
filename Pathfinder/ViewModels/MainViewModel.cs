using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Automation;
using System.Windows.Input;
using Pathfinder.Annotations;

namespace Pathfinder.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
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

		private AutomationElement.AutomationElementInformation _selectedItem;
		public AutomationElement.AutomationElementInformation SelectedItem
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

		private AutomationElement.AutomationElementInformation[] _elements;
		

		public AutomationElement.AutomationElementInformation[] Elements
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
				Elements = AutomationHelper.LocateElement(AutomationElement.RootElement, Path).Select(a => a.Current).ToArray();
			}
			catch (Exception)
			{
				Elements = new AutomationElement.AutomationElementInformation[0];
			}
		
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
