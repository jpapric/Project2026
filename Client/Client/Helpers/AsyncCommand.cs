using System;
<<<<<<< HEAD
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Helpers
=======
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1.Helpers
>>>>>>> FrontendBackendWorkFlow721
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private bool _isExecuting;

        public AsyncCommand(Func<Task> execute)
        {
            _execute = execute;
<<<<<<< HEAD
=======
            _isExecuting = false;
>>>>>>> FrontendBackendWorkFlow721
        }

        public event EventHandler CanExecuteChanged;

<<<<<<< HEAD
        public bool CanExecute(object parameter) => !_isExecuting;

        public async void Execute(object parameter)
        {
            if (_isExecuting) return;
=======
        public bool CanExecute(object parameter)
        {
            return !_isExecuting;
        }

        public async void Execute(object parameter)
        {
>>>>>>> FrontendBackendWorkFlow721
            _isExecuting = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            try
            {
                await _execute();
            }
<<<<<<< HEAD
            finally
            {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public class AsyncCommand<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private bool _isExecuting;

        public AsyncCommand(Func<T, Task> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => !_isExecuting;

        public async void Execute(object parameter)
        {
            if (_isExecuting) return;
            if (!(parameter is T typedParam)) return;
            _isExecuting = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            try
            {
                await _execute(typedParam);
=======
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
>>>>>>> FrontendBackendWorkFlow721
            }
            finally
            {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}