using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GEOBOX.OSC.IM.DataCheckerTool.Domain;
using GEOBOX.OSC.IM.DataCheckerTool.IO;

namespace GEOBOX.OSC.IM.DataCheckerTool.ViewModels
{
    public sealed class SaveDataCheckerFileCommand : ICommand
    {
        private readonly DataCheckerViewModel dataCheckerViewModel;
        private bool canExecute;

        public SaveDataCheckerFileCommand(DataCheckerViewModel dataCheckerViewModel)
        {
            this.dataCheckerViewModel = dataCheckerViewModel;
            dataCheckerViewModel.PropertyChanged += DataCheckerViewModelOnPropertyChanged;
        }

        private void DataCheckerViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(dataCheckerViewModel.CurrentXDocument))
            {
                if (canExecute != (dataCheckerViewModel.CurrentXDocument != null))
                {
                    canExecute = true;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public void Execute(object parameter)
        {
            var fileName = dataCheckerViewModel.GetSaveFile();
            if (String.IsNullOrEmpty(fileName)
                || !Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                dataCheckerViewModel.InformUser(new UserInformation("Speichervorgang abgebrochen", "Keine gültige Datei für die Speicherung gewählt.", UserInformation.UserInformationTypeError));
                return;
            }
            try
            {
                int idCounter = -2;
                Func<int> nextId = () => idCounter++;
                var dataItems = DataCheckItemViewModel.FlattenRenumberedDatacheckItems(dataCheckerViewModel.RootViewModel, -2, nextId)
                                    .Where(IsSavableDataItem);
                using (var destinationStream = new FileStream(fileName, FileMode.Create))
                {
                    new XmlDataCheckItemsWriter().Write(dataItems, dataCheckerViewModel.CurrentXDocument, destinationStream, dataCheckerViewModel.DataCheckerContext);
                }
                dataCheckerViewModel.InformUser(new UserInformation("Datei speichern", "Datei wurde erfolgreich erstellt.", UserInformation.UserInformationTypeInformation));

            }
            catch (Exception exception)
            {
                dataCheckerViewModel.InformUser(new UserInformation("Fehler beim Speichern", exception.Message, UserInformation.UserInformationTypeError));
                throw;
            }
        }

        private static bool IsSavableDataItem(DataCheckItem item)
        {
            return item.Id > 0; //below 1 are treeview root elements that are not saved
        }

        public event EventHandler CanExecuteChanged;
    }
}