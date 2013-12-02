using System;
using System.Windows.Input;

namespace Pathfinder
{
	internal class RelayCommand<T> : ICommand
	{
		private readonly Action<T> _action;

		public RelayCommand(Action<T> action)
		{
			_action = action;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			if(CanExecute(parameter))
				_action((T)parameter);
		}
		public event EventHandler CanExecuteChanged;
	}

	internal class RelayCommand : ICommand
	{
		private readonly Action _action;

		public RelayCommand(Action action)
		{
			_action = action;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
				_action();
		}
		public event EventHandler CanExecuteChanged;
	}
}