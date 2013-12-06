using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Input;

namespace Pathfinder.ViewModels
{
	public class ListViewModel : BaseViewModel, IDisposable
	{
		private string _path;
		private CancellationTokenSource _cts;
		private Task _task;

		private ICommand _checkCommand;
		public ICommand CheckCommand
		{
			get
			{
				return _checkCommand;
			}
			private set
			{
				_checkCommand = value;
				OnPropertyChanged();
			}
		}

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
				if(_elements!= null && _elements.Length != 0)
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
				SelectedIndex = 0;
				IsInProgress = false;
			}
		}

		public ListViewModel()
		{
			SetSearchState();
			CheckCommand = new RelayCommand(OnCheckCommand);
		}

		public void OnCheckCommand()
		{
			_cts = new CancellationTokenSource();
			var token = _cts.Token;
			token.ThrowIfCancellationRequested();
			_task = new Task(() =>
			{
				SetCancelState();
				SelectedItem = null;
				Elements =
					AutomationHelper.LocateElement(AutomationElement.RootElement, Path)
						.Select(AutomationElementViewModel.FromAutomationElement)
						.ToArray();
			}, token, TaskCreationOptions.AttachedToParent);
			_task.ContinueWith(t => SetSearchState(), token);
			_task.Start();
		}

		private void SetCancelState()
		{
			IsInProgress = true;
			ButtonName = "Cancel";
			CheckCommand = new RelayCommand(OnCancellCommand);
		}

		public void OnCancellCommand()
		{
			_cts.Cancel();
			SetSearchState();
		}

		private void SetSearchState()
		{
			CheckCommand = new RelayCommand(OnCheckCommand);
			ButtonName = "Search";
			IsInProgress = false;
			
		}

		private bool _isInProgress;
		public bool IsInProgress
		{
			get { return _isInProgress; }
			set
			{
				_isInProgress = value;
				OnPropertyChanged();
			}
		}

		private string _buttonName;
		

		public string ButtonName
		{
			get { return _buttonName; }
			set
			{
				_buttonName = value;
				OnPropertyChanged();
			}
		}

		public void Dispose()
		{
			_cts.Cancel();
		}
	}
}
