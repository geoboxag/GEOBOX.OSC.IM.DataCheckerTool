using System;
using System.Windows.Input;
using GEOBOX.OSC.IM.DataCheckerTool.WpfExtensions;

namespace GEOBOX.OSC.IM.DataCheckerTool.ViewModels
{
    public sealed class SortDataCheckerChildrenCommand : ICommand
    {
        private readonly DataCheckItemViewModel dataCheckItemViewModel;
        private readonly bool sortDescending;

        public SortDataCheckerChildrenCommand(DataCheckItemViewModel dataCheckItemViewModel, bool sortDescending)
        {
            this.dataCheckItemViewModel = dataCheckItemViewModel;
            this.sortDescending = sortDescending;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            dataCheckItemViewModel.Children?.Sort(model => (model as DataCheckItemViewModel)?.Name, sortDescending);
        }

        public event EventHandler CanExecuteChanged;
    }
}
