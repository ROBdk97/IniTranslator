using System.Collections.ObjectModel;

namespace ROBdk97.Unp4k.P4kModels
{
    public abstract class P4KItem
    {
        public abstract string Name { get; }
        public abstract string FullPath { get; }
        public abstract bool IsDirectory { get; }
        public virtual ObservableCollection<P4KItem> Children { get; } = new ObservableCollection<P4KItem>();

        public List<P4KItem>? FindName(string name)
        {
            return Children.Where(x => x.Name.Contains(name)).ToList();
        }
    }
}
