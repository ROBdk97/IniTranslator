using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace IniTranslator.Helpers
{
    public class TreeViewSelectedItemBehavior : Behavior<TreeView>
    {
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(TreeViewSelectedItemBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectedItemChanged -= OnSelectedItemChanged;
            base.OnDetaching();
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }
    }
}
