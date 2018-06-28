using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GEOBOX.OSC.IM.DataCheckerTool.WpfExtensions;

namespace GEOBOX.OSC.IM.DataCheckerTool.ViewModels
{
    public sealed class SortAllDataCheckerChildrenCommand : ICommand
    {
        private readonly DataCheckItemViewModel dataCheckItemViewModel;
        private readonly bool sortDescending;

        public SortAllDataCheckerChildrenCommand(DataCheckItemViewModel dataCheckItemViewModel, bool sortDescending)
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
            SortChildren(dataCheckItemViewModel?.Children);
        }

        private void SortChildren(ObservableCollection<TreeViewItemViewModel> sortableChildren)
        {
            if (sortableChildren == null ||
                !sortableChildren.Any())
            {
                return;
            }

            sortableChildren.Sort(model => (model as DataCheckItemViewModel)?.Name, sortDescending);
            foreach (var child in sortableChildren)
            {
                SortChildren(child?.Children);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
