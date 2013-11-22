using System.Windows.Automation;

namespace Pathfinder.ViewModels
{
	public class AutomationElementViewModel : BaseViewModel
	{
		private string _name;
		private string _className;
		private string _automationId;
		private string _controlType;

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
			return ControlType;
		}

		public string ClassName
		{
			get { return _className; }
			set
			{
				_className = value;
				OnPropertyChanged();
			}
		}

		public string AutomationId
		{
			get { return _automationId; }
			set
			{
				_automationId = value;
				OnPropertyChanged();
			}
		}

		public string ControlType
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
				ControlType = current.LocalizedControlType
			};
		}
	}
}
