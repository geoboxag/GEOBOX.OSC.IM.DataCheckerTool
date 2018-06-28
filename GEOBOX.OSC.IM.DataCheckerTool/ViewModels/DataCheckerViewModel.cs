using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Xml.Linq;
using GEOBOX.OSC.IM.DataCheckerTool.Domain;
using GEOBOX.OSC.IM.DataCheckerTool.IO;

namespace GEOBOX.OSC.IM.DataCheckerTool.ViewModels
{
    public sealed class DataCheckerViewModel : INotifyPropertyChanged
    {
        public Action<UserInformation> InformUser { get; }
        public Func<string> GetLoadFile { get; }
        public Func<string> GetSaveFile { get; }
        private DataCheckItemViewModel rootViewModel;
        private string dataCheckerFileName;
        private XDocument currentXDocument;
        private DataCheckItemViewModel selectedCheckItemViewModel;
        private string systemMessages;
        public IDataCheckerContext DataCheckerContext { get; }

        public XDocument CurrentXDocument
        {
            get { return currentXDocument; }
            set
            {
                PropertyChanged.ChangeValue(
                    () => currentXDocument,
                    newValue => currentXDocument = newValue,
                    value, this);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DataCheckerViewModel(Func<string> getLoadFile, Func<string> getSaveFile,
            Action<UserInformation> informUser)
        {
            SaveDataCheckerFileCommand = new SaveDataCheckerFileCommand(this);
            LoadDataCheckerFileCommand = new LoadDataCheckerFileCommand(this);

            GetLoadFile = getLoadFile ?? (() => String.Empty);
            GetSaveFile = getSaveFile ?? (() => String.Empty);
            InformUser = informUser ?? (info => { });
            DataCheckerContext = new DataCheckerContext(AppendSystemMessage);

        }

        private void AppendSystemMessage(string message)
        {
            SystemMessages = systemMessages + message;
        }

        public ICommand SaveDataCheckerFileCommand { get; }

        public DataCheckItemViewModel RootViewModel
        {
            get { return rootViewModel; }
            set
            {
                PropertyChanged.ChangeValue(() => RootViewModel, newValue => rootViewModel = newValue,
                    value, this);
            }
        }

        public DataCheckItemViewModel SelectedCheckItemViewModel
        {
            get { return selectedCheckItemViewModel; }
            set
            {
                PropertyChanged.ChangeValue(() => SelectedCheckItemViewModel, newValue => selectedCheckItemViewModel = newValue,
                    value, this);
            }
        }

        public string SystemMessages
        {
            get { return systemMessages; }
            set
            {
                PropertyChanged.ChangeValue(() => SystemMessages, newValue => systemMessages = newValue,
                    value, this);
            }
        }

        public bool HasValidSelectedCheckItemViewModel()
        {
            return selectedCheckItemViewModel != null;
        }

        public void CreateEmptyRootViewModel()
        {
            RootViewModel = DataCheckItemViewModel.Create( /*dataCheckItems*/null, String.Empty);
        }

        public string DataCheckerFileName
        {
            get { return dataCheckerFileName; }
            set
            {
                DataCheckerContext.LogMessage($"Datei wurde ausgewählt: [{value}]", TraceLevel.Info);
                PropertyChanged.ChangeValue(() => DataCheckerFileName, newValue => dataCheckerFileName = newValue, value,
                    this);
            }
        }

        public ICommand LoadDataCheckerFileCommand { get; }

        public bool Load(string toLoadDataCheckerFile)
        {
            if (!IsValidAndExistingFile(toLoadDataCheckerFile))
            {
                return false;
            }

            var readerResult = new XmlDataCheckItemsReader().Read(toLoadDataCheckerFile, DataCheckerContext);

            if (!readerResult.HasErrorMessage())
            {
                CurrentXDocument = readerResult.DataChecksDocument;

                RootViewModel =
                    DataCheckItemViewModel.Create(readerResult.DataCheckItems, System.IO.Path.GetFileName(toLoadDataCheckerFile));
            }
            return !readerResult.HasErrorMessage();
        }

        private bool IsValidAndExistingFile(string fileNameToCheck)
        {
            return !string.IsNullOrEmpty(fileNameToCheck)
                && System.IO.File.Exists(fileNameToCheck);
        }
    }
}
