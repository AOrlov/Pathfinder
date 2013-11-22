using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Forms;

namespace Pathfinder
{
	public static class AutomationElementExtensions
	{
		public static AutomationElement GetElement(this AutomationElement window, string locator)
		{
			var elements = GetElements(window, locator).Take(2).ToArray();
			if (elements.Length > 1)
			{
				Assert.Fail("Found more than one element matching \"{0}\"", locator);
			}
			if (elements.Length == 0)
			{
				Assert.Fail("No elements matching \"{0}\"", locator);
			}
			return elements[0];
		}

		public static bool TryGetElement(this AutomationElement window, string locator, out AutomationElement element)
		{
			IEnumerable<AutomationElement> foundedElements = GetElements(window, locator);
			return (element = foundedElements.SingleOrDefault()) != null;
		}

		private static TPattern GetElementAs<TPattern>(AutomationElement window, string locator)
			where TPattern : BasePattern
		{
			var element = string.IsNullOrEmpty(locator) ? window : GetElement(window, locator);

			// Get AutomationPattern instance
			var patternField = typeof(TPattern).GetField("Pattern", BindingFlags.Public | BindingFlags.Static);
			if (patternField == null)
			{
				Assert.Fail("Invalid pattern: {0}", typeof(TPattern));
			}
			var pattern = patternField.GetValue(null) as AutomationPattern;
			if (pattern == null)
			{
				Assert.Fail("Invalid pattern: {0}", typeof(TPattern));
			}

			// Apply pattern
			object result;
			if (!element.TryGetCurrentPattern(pattern, out result))
			{
				Assert.Fail("Element \"{0}\" found, but does not implement pattern {1}", locator, typeof(TPattern));
			}
			return (TPattern)result;
		}

		public static IEnumerable<AutomationElement> GetElements(this AutomationElement window, string locator)
		{
			if (window == null)
				throw new ArgumentNullException("window");
			if (string.IsNullOrEmpty(locator))
			{
				return new[] { window };
			}

			return AutomationHelper.LocateElement(window, locator);
		}

		public static void ClickElement(this AutomationElement parent, string locator = "")
		{
			GetElementAs<InvokePattern>(parent, locator).Invoke();
		}

		public static void RightClickElement(this AutomationElement parent, string locator = "")
		{
			var bounds = GetElement(parent, locator).Current.BoundingRectangle;

			new InputSimulator().Mouse
				.MoveMouseTo(ConvertXToAbsolute(bounds.Left + (bounds.Right - bounds.Left) / 2), 
					ConvertYToAbsolute(bounds.Top + (bounds.Bottom - bounds.Top) / 2))
				.Sleep(500)
				.RightButtonClick()
				.Sleep(1000);
		}

		public static void MoveMouseOverElement(this AutomationElement parent, string locator = "")
		{
			var bounds = GetElement(parent, locator).Current.BoundingRectangle;

			new InputSimulator().Mouse
				.MoveMouseTo(ConvertXToAbsolute(bounds.Left + (bounds.Right - bounds.Left) / 2),
					ConvertYToAbsolute(bounds.Top + (bounds.Bottom - bounds.Top) / 2))
				.Sleep(1000);
		}

		public static void LeftClickElement(this AutomationElement parent, string locator = "")
		{
			var bounds = GetElement(parent, locator).Current.BoundingRectangle;

			new InputSimulator().Mouse
				.MoveMouseTo(ConvertXToAbsolute(bounds.Left + (bounds.Right - bounds.Left) / 2),
					ConvertYToAbsolute(bounds.Top + (bounds.Bottom - bounds.Top) / 2))
				.Sleep(500)
				.LeftButtonClick()
				.Sleep(1000);
		}

		private static double ConvertXToAbsolute(double x)
		{
			return 65535 * x / Screen.PrimaryScreen.Bounds.Width;
		}

		private static double ConvertYToAbsolute(double y)
		{
			return  65535 * y / Screen.PrimaryScreen.Bounds.Height;
		}

		public static void SetText(this AutomationElement parent, string locator, string text)
		{
			GetElementAs<ValuePattern>(parent, locator).SetValue(text);
		}

		public static string GetText(this AutomationElement parent, string locator = "")
		{
			if (!string.IsNullOrEmpty(locator))
			{
				parent = parent.GetElement(locator);
				locator = string.Empty;
			}
				
			if (parent.Current.ControlType.Equals(ControlType.Edit))
				return GetElementAs<TextPattern>(parent, locator).DocumentRange.GetText(-1);
			if(parent.Current.ControlType.Equals(ControlType.Text))
				return GetElementAs<ValuePattern>(parent, locator).Current.Value;

			Assert.Fail("Cannot retrieve text for {0} control", parent.Current.ControlType.LocalizedControlType);
			return null;
		}

		public static IEnumerable<AutomationElement> GetSelection(this AutomationElement parent, string locator = "")
		{
			return GetElementAs<SelectionPattern>(parent, locator).Current.GetSelection();
		}

		public static void ToggleControl(this AutomationElement parent, string locator = "")
		{
			GetElementAs<TogglePattern>(parent, locator).Toggle();
		}

		public static void SetToggleState(this AutomationElement parent, string locator, ToggleState newState)
		{
			var toggleState = parent.GetToggleState(locator);
			if (newState != toggleState)
			{
				GetElementAs<TogglePattern>(parent, locator).Toggle();
			}
		}

		public static ToggleState GetToggleState(this AutomationElement parent, string locator = "")
		{
			return GetElementAs<TogglePattern>(parent, locator).Current.ToggleState;
		}

		public static void SelectElement(this AutomationElement parent, string locator = "")
		{
			GetElementAs<SelectionItemPattern>(parent, locator).Select();
		}

		public static void SelectComboByTitle(this AutomationElement window, string locator, string itemName)
		{
			var control = GetElement(window, locator);

			var pattern = control.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
			if (pattern != null)
			{
				var res = control.FindFirst(TreeScope.Descendants,
													 new PropertyCondition(AutomationElement.NameProperty, itemName));
				Assert.That(res, Is.Not.Null, "Combo box does not contain item {0}", itemName);
				var selectpattern = res.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
				if (selectpattern != null)
				{
					selectpattern.Select();
				}
			}
		}

		public static void SelectTabByTitle(this AutomationElement window, string tabTitle)
		{
			var tab = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, tabTitle));
			var pattern = tab.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
			if (pattern != null)
				pattern.Select();	
		}

		public static AutomationElement FindCell(this AutomationElement window, string locator, int column)
		{
			return GetElements(window, locator)
				.SingleOrDefault(el => ((GridItemPattern)el.GetCurrentPattern(GridItemPatternIdentifiers.Pattern)).Current.Column == column);
		}

		public static AutomationElement GetModalDialog(this AutomationElement root, string locator)
		{
			AutomationElement window;
			if (TryGetElement(root, locator, out window))
			{
				AutomationElementCollection childWindows = window
					.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
					
				if (childWindows != null && childWindows.Count > 0)
					foreach (var childWindow in childWindows.Cast<AutomationElement>())
					{
						WindowPattern pattern;
						try
						{
							pattern = childWindow.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
						}
						catch (InvalidOperationException)
						{
							continue;
						}
						if (pattern != null && pattern.Current.IsModal)
						{
							return childWindow;
						}
					}
			}
			return null;
		}

		public static void SelectControl(this AutomationElement window, string locator)
		{
			GetElementAs<SelectionItemPattern>(window, locator).Select();
		}

		public static void ExpandControl(this AutomationElement window, string locator = "")
		{
			GetElementAs<ExpandCollapsePattern>(window, locator).Expand();
		}

		public static void CollapseControl(this AutomationElement window, string locator = "")
		{
			GetElementAs<ExpandCollapsePattern>(window, locator).Collapse();
		}

		public static bool GetExpanded(this AutomationElement window, string locator = "")
		{
			return GetElementAs<ExpandCollapsePattern>(window, locator).Current.ExpandCollapseState == ExpandCollapseState.Expanded;
		}

		public static bool IsElementExists(this AutomationElement window, string locator)
		{
			return AutomationHelper.LocateElement(window, locator).Any();
		}

		public static bool IsElementEnabled(this AutomationElement window, string locator = "")
		{
			return (bool) GetElements(window, locator).Single().GetCurrentPropertyValue(AutomationElement.IsEnabledProperty);
		}

		public static bool IsElementSelected(this AutomationElement window, string locator = "")
		{
			return GetElementAs<SelectionItemPattern>(window, locator).Current.IsSelected;
		}

		public static bool AreEqual(this Table first, Table second)
		{
			if (first == second)
				return true;
			if (first.RowCount != second.RowCount)
				return false;
			for (int i = 0; i < first.RowCount; i++)
			{
				TableRow rowA = first.Rows[i];
				TableRow rowB = second.Rows[i];
				if (!rowA.AreEqual(rowB))
					return false;
			}
			return true;
		}
		public static bool AreEqual(this TableRow first, TableRow second)
		{
			if (first == second)
				return true;
			if (first.Keys.Count != second.Keys.Count)
				return false;
			foreach (var column in first.Keys)
			{
				string value;
				if (!second.TryGetValue(column, out value) || first[column] != value)
					return false;
			}
			return true;
		}

		public static Table GetVisibleDataGridContent(this AutomationElement grid)
		{
			var headers = GetElementAs<TablePattern>(grid, "").Current.GetColumnHeaders().Select(h => h.Current.Name).ToArray();
			var rowContent = new Dictionary<string, string>();
			Table table = new Table(headers);
			var rows = grid.GetElements(">*DataGridRow");
			foreach (var row in rows)
			{
				var cells = row.GetElements(">*DataGridCell");
				foreach (var cell in cells)
				{
					var header = GetElementAs<TableItemPattern>(cell, "").Current.GetColumnHeaderItems().First().Current.Name;
					var textElement = cell.GetElements(">*TextBlock").FirstOrDefault();
					var text = textElement != null ? textElement.Current.Name: string.Empty;
					rowContent.Add(header, text);
				}
				table.AddRow(rowContent);
				rowContent.Clear();
			}
			return table;
		}

		public static void SetWindowVisualState(this AutomationElement window, WindowVisualState state, string locator="")
		{
			GetElementAs<WindowPattern>(window, locator).SetWindowVisualState(state);
		}

		public static void Close(this AutomationElement window, string locator = "")
		{
			GetElementAs<WindowPattern>(window, locator).Close();
		}
	}
}
