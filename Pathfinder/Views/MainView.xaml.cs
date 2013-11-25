using System.Windows.Input;

namespace Pathfinder.Views
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView
	{
		public MainView()
		{
			InitializeComponent();
		}

		private void InputPath_OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				Keyboard.Focus(SeachButton);
		}
	}
}
