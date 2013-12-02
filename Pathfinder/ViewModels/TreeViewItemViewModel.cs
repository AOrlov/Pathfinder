using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using Condition = System.Windows.Automation.Condition;

namespace Pathfinder.ViewModels
{
	public class TreeViewItemViewModel : BaseViewModel
	{
		private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel() { Name = "Dummy"};
		//public IList<AutomationElementViewModel> Elements { get; set; }

		private AutomationElement _thisAutomationElement;

		private AutomationElementViewModel _innerElement;
		public AutomationElementViewModel InnerElement
		{
			get
			{
				return _innerElement;
			}
			private set
			{
				_innerElement = value;
				OnPropertyChanged();
			}
		}

		private string _name;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		

		public TreeViewItemViewModel(AutomationElement element)
		{
			Children = new ObservableCollection<TreeViewItemViewModel>() { DummyChild };
		//	Name = element.ToString();
			_thisAutomationElement = element;
			InnerElement = AutomationElementViewModel.FromAutomationElement(element);
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

		public TreeViewItemViewModel()
		{
			Children = new ObservableCollection<TreeViewItemViewModel> { new TreeViewItemViewModel(AutomationElement.RootElement) };
			Name = "root";
			//LoadChildren();
		}

		private ObservableCollection<TreeViewItemViewModel> _children;
		public ObservableCollection<TreeViewItemViewModel> Children
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
			get { return Children.Count == 1 && Children[0] == DummyChild; }
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

				// Expand all the way up to the root.
				//if (_isExpanded && _parent != null)
				//	_parent.IsExpanded = true;

				// Lazy load the child items, if necessary.
				if (this.HasDummyChild)
				{
					this.Children.Remove(DummyChild);
					this.LoadChildren();
				}
			}
		}

		private void LoadChildren()
		{
			Children =
				new ObservableCollection<TreeViewItemViewModel>(
					_thisAutomationElement.FindAll(TreeScope.Children, Condition.TrueCondition)
						.OfType<AutomationElement>()
						.Select(e => new TreeViewItemViewModel(e)));
		}
	}

}
