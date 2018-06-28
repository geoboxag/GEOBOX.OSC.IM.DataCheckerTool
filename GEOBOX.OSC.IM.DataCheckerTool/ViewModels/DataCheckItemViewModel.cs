using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GEOBOX.OSC.IM.DataCheckerTool.WpfExtensions;
using GEOBOX.OSC.IM.DataCheckerTool.Domain;

namespace GEOBOX.OSC.IM.DataCheckerTool.ViewModels
{
    public sealed class DataCheckItemViewModel : TreeViewItemViewModel
    {
        private ICommand sortDataCheckerChildrenDescendingCommand;
        private ICommand sortDataCheckerChildrenAscendingCommand;
        private ICommand sortAllDataCheckerChildrenAscendingCommand;
        private ICommand sortAllDataCheckerChildrenDescendingCommand;
        public DataCheckItem DataCheckItem { get; }

        public DataCheckItemViewModel(DataCheckItem dataCheckItem, DataCheckItemViewModel parent)
            : base(parent, /*lazyLoadChildren*/false)
        {
            DataCheckItem = dataCheckItem;
        }

        public string Name => DataCheckItem.Name;

        public string SqlStatement => DataCheckItem.SqlStatement;

        public string Description => DataCheckItem.Description;

        public bool IsSqlCheckItem => DataCheckItem.IsSqlCheckItem;
        public bool SupportsSortAll => DataCheckItem.SupportsSortAll;

        public static DataCheckItemViewModel Create(IEnumerable<DataCheckItem> dataCheckItems, string rootName)
        {
            var viewModel = new DataCheckItemViewModel(new DataCheckItem(-2, "viewModelRoot", false), null);

            if (dataCheckItems == null)
            {
                return viewModel;
            }

            var groupedDataCheckItems = dataCheckItems.ToLookup(dataCheckItem => dataCheckItem.ParentDataCheckId);
            var parentIds = groupedDataCheckItems.Select(group => group.Key).OrderBy(key => key);
            AddDataCheckItems(viewModel, new DataCheckItem(-1, rootName, /*isSqlCheckItem*/false, /*supportsSortAll*/ true), groupedDataCheckItems, parentIds);

            return viewModel;
        }

        private static void AddDataCheckItems(DataCheckItemViewModel viewModel, DataCheckItem dataCheckItem, ILookup<int, DataCheckItem> groupedDataCheckItems, IOrderedEnumerable<int> parentIds)
        {
            var newViewModel = new DataCheckItemViewModel(dataCheckItem, viewModel);

            if (IsParent(dataCheckItem, parentIds))
            {
                var newParentId = dataCheckItem.Id;
                foreach (var childItem in groupedDataCheckItems[newParentId])
                {   //child assigns parent --> automatically added to parent.Children!
                    AddDataCheckItems(newViewModel, childItem, groupedDataCheckItems, parentIds);
                }
            }
        }

        public static IEnumerable<DataCheckItem> FlattenRenumberedDatacheckItems(DataCheckItemViewModel dataCheckItemViewModel, int newParentId, Func<int> getNextId)
        {
            //return parent
            if (dataCheckItemViewModel == null)
            {
                yield break; //return empty result
            }

            var newId = getNextId();
            if (newId == 0)//it starts with 1 :(!
            {
                newId = getNextId();
            }
            if (dataCheckItemViewModel.DataCheckItem.Id > 0) //root objects are not returned (-2, -1)
            {
                yield return dataCheckItemViewModel.DataCheckItem.CorrectPosition(newId, newParentId);
            }

            newParentId = newId;
            foreach (var childDataViewModel in dataCheckItemViewModel.Children
                                                        .OfType<DataCheckItemViewModel>())
            {
                foreach (var dataCheckItem in FlattenRenumberedDatacheckItems(childDataViewModel, newParentId, getNextId))
                {
                    yield return dataCheckItem;
                }
            }
        }

        public static IEnumerable<DataCheckItem> FlattenDatacheckItemsWithParentCorrection(DataCheckItemViewModel dataCheckItemViewModel, int newParentId)
        {
            //return parent
            if (dataCheckItemViewModel == null)
            {
                yield break; //return empty result
            }

            var newId = dataCheckItemViewModel.DataCheckItem.Id;
            if (dataCheckItemViewModel.DataCheckItem.Id > 0) //root objects are not returned (-2, -1)
            {
                yield return dataCheckItemViewModel.DataCheckItem.CorrectPosition(dataCheckItemViewModel.DataCheckItem.Id, newParentId);
            }

            newParentId = newId;
            foreach (var childDataViewModel in dataCheckItemViewModel.Children
                                                        .OfType<DataCheckItemViewModel>())
            {
                foreach (var dataCheckItem in FlattenDatacheckItemsWithParentCorrection(childDataViewModel, newParentId))
                {
                    yield return dataCheckItem;
                }
            }
        }

        public ICommand SortAllDataCheckerChildrenAscendingCommand
        {
            get
            {
                return sortAllDataCheckerChildrenAscendingCommand
                    ?? (sortAllDataCheckerChildrenAscendingCommand = new SortAllDataCheckerChildrenCommand(this, /*sortDescending*/false));
            }
        }

        public ICommand SortAllDataCheckerChildrenDescendingCommand
        {
            get
            {
                return sortAllDataCheckerChildrenDescendingCommand
                    ?? (sortAllDataCheckerChildrenDescendingCommand = new SortAllDataCheckerChildrenCommand(this, /*sortDescending*/true));
            }
        }

        public ICommand SortDataCheckerChildrenAscendingCommand {
            get
            {
                return sortDataCheckerChildrenAscendingCommand
                    ?? (sortDataCheckerChildrenAscendingCommand = new SortDataCheckerChildrenCommand(this, /*sortDescending*/false));
            }
        }

        public ICommand SortDataCheckerChildrenDescendingCommand
        {
            get
            {
                return sortDataCheckerChildrenDescendingCommand
                    ?? (sortDataCheckerChildrenDescendingCommand = new SortDataCheckerChildrenCommand(this, /*sortDescending*/true));
            }
        }

        public string SortDescendingDetailTooltip => "Sortiert untergeordnete Elemente aufsteigend.";
        public string SortAscendingDetailTooltip => "Sortiert untergeordnete Elemente absteigend.";

        public string SortAllDescendingTooltip => "Sortiert sämtliche Elemente aufsteigend.";
        public string SortAllAscendingTooltip => "Sortiert sämtliche Elemente absteigend.";

        public void Sort(bool descending)
        {
            Children.Sort(viewModel => (viewModel as DataCheckItemViewModel)?.Name, descending);
        }

        private static bool IsParent(DataCheckItem dataCheckItem, IOrderedEnumerable<int> parentIds)
        {
            return parentIds.Contains(dataCheckItem.Id);
        }
    }
}
