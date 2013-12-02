using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Automation;

namespace Pathfinder
{
	public static class AutomationHelper
	{
		public static IEnumerable<AutomationElement> LocateElement(AutomationElement parent, string controlLocator)
		{
			if (parent == null || string.IsNullOrWhiteSpace(controlLocator)) return Enumerable.Empty<AutomationElement>();
			var controlDescriptions = controlLocator.Trim('/').Split('/'); //TODO What if locator itself has '/' symbol?
			IEnumerable<AutomationElement> currentControl = new []{ parent };
			foreach (string control in controlDescriptions)
			{
				if (IsParentLocator(control))
				{
					currentControl = currentControl.Select(a => TreeWalker.RawViewWalker.GetParent(a));
				}
				else if (IsNextSiblingLocator(control))
				{
					currentControl = currentControl.Select(a => TreeWalker.RawViewWalker.GetNextSibling(a));
				}
				else if (IsPreviousSiblingLocator(control))
				{
					currentControl = currentControl.Select(a => TreeWalker.RawViewWalker.GetPreviousSibling(a));
				}
				else
				{
					TreeScope treeScope = TreeScope.Descendants;
					var controlTitle = control;
					int? controlIndex = null;

					if (control.Length > 0 && control[0] == '>')
					{
						treeScope = TreeScope.Children;
						controlTitle = control.Substring(1);
					}
					if (Regex.IsMatch(controlTitle, @"^.*\[[0-9]+\]$"))
					{
						controlIndex = Convert.ToInt32(Regex.Replace(controlTitle, @"^.*\[([0-9]+)\]$", "$1"));
						controlTitle = Regex.Replace(controlTitle, @"^(.*)\[[0-9]+\]$", "$1");
					}

					Condition condition = GetPropertyCondition(controlTitle);
					if (condition == null)
						return null;
					var res = currentControl.SelectMany(a => a.FindAll(treeScope, condition).OfType<AutomationElement>());
					currentControl = controlIndex.HasValue ? res.Skip(controlIndex.Value).Take(1) : res;
				}
			}
			return currentControl;
		}

		private static Condition GetPropertyCondition(string locator)
		{
			if (string.IsNullOrEmpty(locator))
			{
				return null;
			}

			List<Condition> detectedConditions = new List<Condition>();
			var properties = locator.Split('&');
			foreach (string property in properties)
			{
				char selector = property[0];
				string id = property.Substring(1);

				switch (selector)
				{
					case '=':
						detectedConditions.Add(new PropertyCondition(AutomationElement.NameProperty, id));
						break;
					case '*':
						detectedConditions.Add(new PropertyCondition(AutomationElement.ClassNameProperty, id));
						break;
					case '#':
						detectedConditions.Add(new PropertyCondition(AutomationElement.AutomationIdProperty, id));
						break;
					default:
						detectedConditions.Add(new PropertyCondition(AutomationElement.NameProperty, property));
						break;
				}
			}
			return detectedConditions.Count > 1 ? new AndCondition(detectedConditions.ToArray()) : detectedConditions[0];
		}

		private static bool IsPreviousSiblingLocator(string control)
		{
			return control.StartsWith("<-");
		}

		private static bool IsNextSiblingLocator(string control)
		{
			return control.StartsWith("->");
		}

		private static bool IsParentLocator(string control)
		{
			return control.StartsWith("..");
		}

		public static IEnumerable<AutomationElement> GetChildren(this AutomationElement parent)
		{
			return parent.FindAll(TreeScope.Children, Condition.TrueCondition).OfType<AutomationElement>();
		} 
	}
}