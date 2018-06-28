using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GEOBOX.OSC.IM.DataCheckerTool.ViewModels
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        private readonly ObservableCollection<TreeViewItemViewModel> children;

        private bool isExpanded = true;
        private bool isSelected;
        private TreeViewItemViewModel parent;

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            Parent = parent;

            children = new ObservableCollection<TreeViewItemViewModel>();
            

            if (lazyLoadChildren)
            {
                children.Add(DummyChild);
            }
        }

        // This is used to create the DummyChild instance.
        private TreeViewItemViewModel()
        {
        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children => children;

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild => (Children.Count == 1) && (Children[0] == DummyChild);

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (value != isExpanded)
                {
                    isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }

                // Expand all the way up to the root.
                if (isExpanded && Parent != null)
                {
                    Parent.IsExpanded = true;
                }

                // Lazy load the child items, if necessary.
                if (HasDummyChild)
                {
                    Children.Remove(DummyChild);
                    LoadChildren();
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
        }

        public TreeViewItemViewModel Parent
        {
            get { return parent; }
            set
            {
                if (value != parent)
                {
                    var oldParent = parent;
                    parent = value;

                    oldParent?.Children.Remove(this);
                    parent.Children.Add(this);
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}