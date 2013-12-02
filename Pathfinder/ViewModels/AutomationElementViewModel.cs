using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Pathfinder.ViewModels
{
	public class AutomationElementViewModel : BaseViewModel
	{
		private string _name;
		private string _className;
		private string _automationId;
		private ControlType _controlType;
		[Category("Identification")]
		public string Name
		{
			get { return _name; }
			private set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		public override string ToString()
		{
			return ControlType.ToString();
		}
		[Category("Identification")]
		public string ClassName
		{
			get { return _className; }
			private set
			{
				_className = value;
				OnPropertyChanged();
			}
		}
		[Category("Identification")]
		public string AutomationId
		{
			get { return _automationId; }
			private set
			{
				_automationId = value;
				OnPropertyChanged();
			}
		}
		[Category("Identification")]
		public ControlType ControlType
		{
			get { return _controlType; }
			private set
			{
				_controlType = value;
				OnPropertyChanged();
			}
		}

		public static AutomationElementViewModel FromAutomationElement(AutomationElement element)
		{
			var current = element.Current;
			return new AutomationElementViewModel
			{
				Name = current.Name,
				ClassName = current.ClassName,
				AutomationId = current.AutomationId,
				ControlType = current.ControlType,
				LocalizedControlType = current.LocalizedControlType,
				ProcessId = current.ProcessId,
				FrameworkId = current.FrameworkId,
				IsPassword = current.IsPassword,
				IsContentElement = current.IsContentElement,
				IsControlElement = current.IsControlElement,

				IsEnabled = current.IsEnabled,
				HasKeyboardFocus = current.HasKeyboardFocus,

				BoundingRectangle = current.BoundingRectangle,
				ClicablePoint = current.IsOffscreen,

				ControlPatterns = element.GetSupportedPatterns().Select(AutomationPatternViewModel.FromAutomationPattern).ToArray()
			};
		}

		[ExpandableObject]
		[Category("ControlPatterns")]
		[DisplayName("Supported Patterns")]
		public AutomationPatternViewModel[] ControlPatterns { get; private set; }

		[Category("Visibility")]
		public bool ClicablePoint { get; private set; }
		[Category("Visibility")]
		public Rect BoundingRectangle { get; private set; }

		[Category("State")]
		public bool HasKeyboardFocus { get; private set; }
		[Category("State")]
		public bool IsEnabled { get; private set; }

		[Category("Identification")]
		public bool IsControlElement { get; private set; }
		[Category("Identification")]
		public bool IsContentElement { get; private set; }
		[Category("Identification")]
		public bool IsPassword { get; private set; }
		[Category("Identification")]
		public string FrameworkId { get; private set; }
		[Category("Identification")]
		public int ProcessId { get; private set; }
		[Category("Identification")]
		public string LocalizedControlType { get; private set; }
	}
}
