using Plugin.Iconize;
using Xamarin.Forms;

namespace TimeIt.Controls
{
    public class BindableToolbarItem : IconToolbarItem
    {
        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = bindable as BindableToolbarItem;

            if (item == null || item.Parent == null || Device.RuntimePlatform != Device.Android)
                return;
            
            var toolbarItems = ((ContentPage)item.Parent).ToolbarItems;

            if ((bool)newvalue && !toolbarItems.Contains(item))
            {
                toolbarItems.Add(item);
                Device.BeginInvokeOnMainThread(() => { toolbarItems.Add(item); });
            }
            else if (!(bool)newvalue && toolbarItems.Contains(item))
            {
                toolbarItems.Remove(item);
                Device.BeginInvokeOnMainThread(() => { toolbarItems.Remove(item); });
            }
        }
    }
}
