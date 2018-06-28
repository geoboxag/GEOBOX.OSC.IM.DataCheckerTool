using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GEOBOX.OSC.IM.DataCheckerTool.ViewModels;
using Microsoft.Win32;

namespace GEOBOX.OSC.IM.DataCheckerTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Point startPoint;
        private StackPanel mouseOverStackPanel;
        private static Thickness topOnlyThickness = new Thickness(0, 0, 0, 2);
        private static Thickness bottomOnlyThickness = new Thickness(0, 2, 0, 0);
        private static Thickness emptyThickness = new Thickness(0, 0, 0, 0);
        private Border dragOver;
        private bool firstOpenDialogCall = true;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = CreateDataCheckerViewModel();

            // Create an Show Infos
            InfoVersionLabel.Content = Settings.SettingsController.GetAssemblyVersionString();

        }

        #region Window Events (Close, Minimized, Drag)
        private void MainWindow_Close(object sender, System.Windows.RoutedEventArgs e)
        {
            App.Current.Shutdown();
            base.OnClosed(e);
        }
        // Minimize Window
        private void MainWindow_Minimize(object sender, System.Windows.RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }
        // Drag Window (relocate)
        private void MainWindow_Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion

        private DataCheckerViewModel CreateDataCheckerViewModel()
        {
            var dataCheckerViewModel = new DataCheckerViewModel(() => GetDataCheckerXmlFile(/*saveMode*/false),
                                             () => GetDataCheckerXmlFile(/*saveMode*/true),
                                             ShowUserInformation
                                             );
            dataCheckerViewModel.CreateEmptyRootViewModel();
            dataCheckerViewModel.PropertyChanged += DataCheckerViewModelOnPropertyChanged;
            return dataCheckerViewModel;
        }

        private void DataCheckerViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var model = (DataCheckerViewModel) sender;
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(model.DataCheckerFileName):
                {
                    break;
                }
                case nameof(model.SystemMessages):
                {
                    UpdateSystemMessages(model.SystemMessages);
                    break;
                }
            }
        }

        private void UpdateSystemMessages(string systemMessages)
        {
            SystemMessagesTextBox.Document.Blocks.Clear();
            SystemMessagesTextBox.AppendText(systemMessages);
            SystemMessagesTextBox.CaretPosition = SystemMessagesTextBox.CaretPosition.DocumentEnd;
        }

        private void ShowUserInformation(UserInformation userInformation)
        {
            var isErrorType = userInformation.UserInformationType == UserInformation.UserInformationTypeError;
            var messageImage = isErrorType ? MessageBoxImage.Error : MessageBoxImage.Information;

            MessageBox.Show(this, userInformation.Message, userInformation.Caption, MessageBoxButton.OK, messageImage);
        }

        private string GetDataCheckerXmlFile(bool saveMode)
        {
            try
            {
                var fileDialog = saveMode
                                            ? new SaveFileDialog()
                                            : new OpenFileDialog() as FileDialog;

                if (saveMode)
                {
                    ((SaveFileDialog) fileDialog).CreatePrompt = true;
                }
                else
                {
                    ((OpenFileDialog) fileDialog).Multiselect = false;
                    ((OpenFileDialog)fileDialog).ShowReadOnly = false;
                }

                fileDialog.Filter = "DataChecker Dateien (*.xml)|*.xml";
                fileDialog.CheckFileExists = !saveMode;
                fileDialog.CheckPathExists = true;
                SetInitialDirectoryOnFirstCall(fileDialog);

                if (fileDialog.ShowDialog() == true)
                {
                    return fileDialog.FileName;
                }
                else
                {
                    return String.Empty;
                }
            }
            catch (Exception unexpectedException)
            {
                MessageBox.Show(this, unexpectedException.Message, "Fehler beim Öffnen der Datei", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return String.Empty;
            }
        }

        private void SetInitialDirectoryOnFirstCall(FileDialog fileDialog)
        {
            if (firstOpenDialogCall)
            {
                firstOpenDialogCall = false;
                var reposDirectory = "C:\\_repos";
                fileDialog.InitialDirectory = Directory.Exists(reposDirectory)
                    ? reposDirectory
                    : Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            }
        }

        void DataChecksTreeView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(null);
                var diff = startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var treeView = sender as TreeView;
                    var treeViewItem = ((DependencyObject)e.OriginalSource).FindAnchestor<TreeViewItem>();

                    if (treeView == null || treeViewItem == null)
                        return;

                    var dataCheckItemViewModel = treeView.SelectedItem as DataCheckItemViewModel;
                    if (dataCheckItemViewModel == null || dataCheckItemViewModel.DataCheckItem.Id <= 0)
                    {
                        return;
                    }


                    var dragData = new DataObject(dataCheckItemViewModel);
                    DragDrop.DoDragDrop(treeViewItem, dragData, DragDropEffects.Move);
                }
            }
        }

        void DataChecksTreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void DataChecksTreeView_Drop(object sender, DragEventArgs e)
        {
            if (dragOver != null)
            {
                dragOver.BorderThickness = emptyThickness;
                dragOver = null;
            }

            if (e.Data.GetDataPresent(typeof(DataCheckItemViewModel)))
            {
                var dataCheckItemViewModel = e.Data.GetData(typeof(DataCheckItemViewModel)) as DataCheckItemViewModel;

                var treeViewItem = ((DependencyObject)e.OriginalSource).FindAnchestor<TreeViewItem>();

                var dropTarget = treeViewItem?.Header as DataCheckItemViewModel;

                if (dropTarget == null || dataCheckItemViewModel == null | dataCheckItemViewModel?.DataCheckItem?.Id <= 0)
                {
                    return;
                }

                if (dropTarget.IsSqlCheckItem ||
                    (!dropTarget.IsSqlCheckItem
                        && IsUpperMouseDropLocationOfTreeViewItem(e, treeViewItem)))
                {
                    var isUpperMouseDrop = IsUpperMouseDropLocationOfTreeViewItem(e, treeViewItem);
                    var isRootDataCheckItem = IsRootDataCheckItem(dropTarget);
                    var dropTargetParent = GetDropParent(isRootDataCheckItem, dropTarget);
                    CorrectCheckItemParent(dataCheckItemViewModel, dropTargetParent);

                    //change order (root parent position cannot be changed!)
                    var parentChildren = dropTargetParent.Children;
                    var indexes = GetDragDropIndexes(parentChildren, dropTarget, dataCheckItemViewModel);
                    var dropTargetIndex = isRootDataCheckItem ? 0 : indexes.First(model => model.Model.Equals(dropTarget)).Index;
                    if (dropTarget.IsSqlCheckItem
                        && !isRootDataCheckItem
                        && isUpperMouseDrop
                        //&& (parentChildren.Count - 1 > dropTargetIndex)
                        && (dropTargetIndex > 0)
                        )
                    {
                        dropTargetIndex--;
                    }
                    var draggedIndex = indexes.First(model => model.Model.Equals(dataCheckItemViewModel)).Index;

                    parentChildren.Move(draggedIndex, dropTargetIndex);
                }
                else
                {
                    if (dataCheckItemViewModel.Parent != dropTarget)
                    {
                        //change parent
                        dataCheckItemViewModel.Parent = dropTarget;
                        dropTarget.Children.Move(dropTarget.Children.Count - 1, 0);
                    }
                    else
                    {
                        //change location
                        var indexes = GetDragDropIndexes(dropTarget.Children, dropTarget, dataCheckItemViewModel);
                        var draggedIndex = indexes.First(model => model.Model.Equals(dataCheckItemViewModel)).Index;
                        dropTarget.Children.Move(draggedIndex, 0);
                    }
                }
            }
        }

        private static ModelIndex[] GetDragDropIndexes(ObservableCollection<TreeViewItemViewModel> parentChildren, DataCheckItemViewModel dropTarget,
            DataCheckItemViewModel dataCheckItemViewModel)
        {
            var indexes = parentChildren
                .Select((model, index) => new ModelIndex {Model = model, Index = index})
                .Where(
                    treeViewItemModel =>
                        treeViewItemModel.Model.Equals(dropTarget) ||
                        treeViewItemModel.Model.Equals(dataCheckItemViewModel))
                .ToArray();
            return indexes;
        }

        private static TreeViewItemViewModel GetDropParent(bool isRootDataCheckItem, DataCheckItemViewModel dropTarget)
        {
            return isRootDataCheckItem ? dropTarget : dropTarget.Parent;
        }

        private bool IsUpperMouseDropLocationOfTreeViewItem(DragEventArgs dragEventArgs, TreeViewItem treeViewItem)
        {
            if (mouseOverStackPanel == null)
            {
                return false;
            }
            var currentPosition = dragEventArgs.GetPosition(treeViewItem);
            return mouseOverStackPanel.ActualHeight/2 >= currentPosition.Y;
        }

        private static void CorrectCheckItemParent(DataCheckItemViewModel dataCheckItemViewModel, TreeViewItemViewModel dropTargetParent)
        {
            if (dataCheckItemViewModel.Parent != dropTargetParent)
            {
                dataCheckItemViewModel.Parent = dropTargetParent;
            }
        }

        private static bool IsRootDataCheckItem(DataCheckItemViewModel dropTarget)
        {
            return dropTarget.DataCheckItem.Id == -1;
        }


        private void DataChecksTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(null);
                var diff = startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var treeView = sender as TreeView;
                    var treeViewItem = ((DependencyObject)e.OriginalSource).FindAnchestor<TreeViewItem>();

                    if (treeView == null || treeViewItem == null)
                    {
                        return;
                    }

                    var treeviewItemViewModel = treeView.SelectedItem as TreeViewItemViewModel;

                    if (treeviewItemViewModel == null)
                    {
                        return;
                    }


                    var dragData = new DataObject(treeviewItemViewModel);

                    DragDrop.DoDragDrop(treeViewItem, dragData, DragDropEffects.Move);

                }

            }
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!ReferenceEquals(mouseOverStackPanel, (StackPanel) sender))
            {
                mouseOverStackPanel = (StackPanel)sender;
            }

        }

        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            dragOver = ((Border) sender);
            var currentPosition = e.GetPosition(dragOver);

            if (dragOver.ActualHeight/2 <= currentPosition.Y)
            {
                ((Border) sender).BorderThickness = topOnlyThickness;
            }
            else
            {
                ((Border)sender).BorderThickness = bottomOnlyThickness;
            }
        }

        private void TreeViewItem_DragLeave(object sender, DragEventArgs e)
        {
            ((Border)sender).BorderThickness = emptyThickness;
        }

        private sealed class ModelIndex
        {
            public TreeViewItemViewModel Model { get; set; }
            public int Index { get; set; }
        }

        private void DataChecksTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var dataCheckerViewModel = (DataContext as DataCheckerViewModel);
            if (dataCheckerViewModel == null)
            {
                return;
            }

            dataCheckerViewModel.SelectedCheckItemViewModel = DataChecksTreeView.SelectedItem as DataCheckItemViewModel;
        }

    }
}
