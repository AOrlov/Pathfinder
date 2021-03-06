﻿using System;
using System.Windows.Input;

namespace Pathfinder
{
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
			if(CanExecute(parameter))
				_action();
		}
		public event EventHandler CanExecuteChanged;
	}
}