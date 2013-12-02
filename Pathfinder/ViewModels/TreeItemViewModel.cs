using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Automation;

namespace Pathfinder.ViewModels
{
	public class TreeItemViewModel : BaseViewModel
	{
		private readonly TreeItemViewModel _dummyChild = new TreeItemViewModel();
		private readonly AutomationElement _innerAutomationElement;

		private AutomationElementViewModel _innerElementViewModel;
		public AutomationElementViewModel InnerElementViewModel
		{
			get
			{
				return _innerElementViewModel;
			}
			private set
			{
				_innerElementViewModel = value;
				OnPropertyChanged();
			}
		}

		public TreeItemViewModel(AutomationElement element)
		{
			Children = new ObservableCollection<TreeItemViewModel> { _dummyChild };
			_innerAutomationElement = element;
			InnerElementViewModel = AutomationElementViewModel.FromAutomationElement(element);
		}

		private TreeItemViewModel()
		{
			Children = new ObservableCollection<TreeItemViewModel> { _dummyChild };
		}

		private bool _isSelected;
		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}

		private ObservableCollection<TreeItemViewModel> _children;
		public ObservableCollection<TreeItemViewModel> Children
		{
			get { return _children; }
			private set
			{
				_children = value;
				OnPropertyChanged();
			}
		}

		public bool HasDummyChild
		{
			get { return Children.Count == 1 && Children[0] == _dummyChild; }
		}

		private bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if (value != _isExpanded)
				{
					_isExpanded = value;
					OnPropertyChanged();
				}

				if (HasDummyChild)
				{
					Children.Remove(_dummyChild);
					LoadChildren();
				}
			}
		}

		private void LoadChildren()
		{
			Children =
				new ObservableCollection<TreeItemViewModel>(
					_innerAutomationElement.GetChildren()
						.Select(c => new TreeItemViewModel(c)));
		}
	}
}
