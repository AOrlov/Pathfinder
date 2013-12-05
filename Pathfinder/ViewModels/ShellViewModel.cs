using System.Windows.Input;

namespace Pathfinder.ViewModels
{
	public class ShellViewModel: BaseViewModel
	{
		private BaseViewModel _contenth;
		public ICommand SwitchToListViewCommand { get; set; }
		public ICommand SwitchToTreeViewCommand { get; set; }

		public ShellViewModel()
		{
			SwitchToListViewCommand = new RelayCommand(SwitchToListView);
			SwitchToTreeViewCommand = new RelayCommand(SwitchToTreeView);
			Contenth = new ListViewModel();
		}

		public BaseViewModel Contenth
		{
			get
			{
				return _contenth;
			}
			set
			{
				_contenth = value;
				OnPropertyChanged();
			}
		}

		private void SwitchToListView()
		{
			Contenth = new ListViewModel();
		}
		
		private void SwitchToTreeView()
		{
			Contenth = new TreeViewModel();
		}
	}
}
