using System.Windows;
using System.Windows.Controls;

namespace Pathfinder.Views
{
	/// <summary>
	/// Interaction logic for TreeView.xaml
	/// </summary>
	public partial class TreeView
	{
		public TreeView()
		{
			InitializeComponent();
		}

		private void TreeViewItemRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
		{
			e.Handled = true;
		}
	}
}
