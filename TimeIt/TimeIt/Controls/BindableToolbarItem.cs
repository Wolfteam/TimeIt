using Xamarin.Forms;

namespace TimeIt.Controls
{
    public class BindableToolbarItem : ToolbarItem
    {
        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(
            nameof(IsVisible),
            typeof(bool),
            typeof(BindableToolbarItem),
            true,
            BindingMode.TwoWay,
            propertyChanged: OnIsVisibleChanged);

        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = bindable as BindableToolbarItem;

            if (item == null || item.Parent == null)
                return;

            var toolbarItems = ((ContentPage)item.Parent).ToolbarItems;
            bool isVisible = (bool)newvalue;

            if (isVisible && !toolbarItems.Contains(item))
            {
                toolbarItems.Add(item);
            }
            else if (!isVisible && toolbarItems.Contains(item))
            {
                toolbarItems.Remove(item);
            }
        }
    }
}
