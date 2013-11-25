using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
			set
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
			set
			{
				_className = value;
				OnPropertyChanged();
			}
		}
		[Category("Identification")]
		public string AutomationId
		{
			get { return _automationId; }
			set
			{
				_automationId = value;
				OnPropertyChanged();
			}
		}
		[Category("Identification")]
		public ControlType ControlType
		{
			get { return _controlType; }
			set
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

				BoubdingRectangle = current.BoundingRectangle,
				ClicablePoint = current.IsOffscreen,

				ControlPatterns = element.GetSupportedPatterns().Select(AutomationPatternViewModel.FromAutomationPattern).ToArray()
			};
		}

		[ExpandableObject]
		[Category("ControlPatterns")]
		[DisplayName("Supported Patterns")]
		public AutomationPatternViewModel[] ControlPatterns { get; set; }

		[Category("Visibility")]
		public bool ClicablePoint { get; set; }
		[Category("Visibility")]
		public Rect BoubdingRectangle { get; set; }

		[Category("State")]
		public bool HasKeyboardFocus { get; set; }
		[Category("State")]
		public bool IsEnabled { get; set; }

		[Category("Identification")]
		public bool IsControlElement { get; set; }
		[Category("Identification")]
		public bool IsContentElement { get; set; }
		[Category("Identification")]
		public bool IsPassword { get; set; }
		[Category("Identification")]
		public string FrameworkId { get; set; }
		[Category("Identification")]
		public int ProcessId { get; set; }
		[Category("Identification")]
		public string LocalizedControlType { get; set; }
	}
}
