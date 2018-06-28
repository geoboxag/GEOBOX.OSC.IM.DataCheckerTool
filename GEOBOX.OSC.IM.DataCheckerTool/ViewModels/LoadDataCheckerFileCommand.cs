using System;
using System.Windows.Input;

namespace GEOBOX.OSC.IM.DataCheckerTool.ViewModels
{
    public class LoadDataCheckerFileCommand : ICommand
    {
        private readonly DataCheckerViewModel dataCheckerViewModel;

        public LoadDataCheckerFileCommand(DataCheckerViewModel dataCheckerViewModel)
        {
            this.dataCheckerViewModel = dataCheckerViewModel;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var toLoadFile = dataCheckerViewModel.GetLoadFile();
            if (String.IsNullOrEmpty(toLoadFile))
            {
                return;
            }

            if (dataCheckerViewModel.Load(toLoadFile))
            {
                dataCheckerViewModel.DataCheckerFileName = toLoadFile;
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}