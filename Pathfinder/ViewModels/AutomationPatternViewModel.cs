using System.Windows.Automation;

namespace Pathfinder.ViewModels
{
	public class AutomationPatternViewModel
	{
		public string Name { get; set; }

		private AutomationPatternViewModel(){}

		public static AutomationPatternViewModel FromAutomationPattern(AutomationPattern pattern)
		{
			return new AutomationPatternViewModel
			{
				Name = pattern.ProgrammaticName
			};
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
