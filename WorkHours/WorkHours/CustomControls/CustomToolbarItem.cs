using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace WorkHours.CustomControls
{
    public class CustomToolbarItem:ToolbarItem
    {
        public CustomToolbarItem()
        {
            InitVisibility();
        }

        private void InitVisibility()
        {
            OnIsVisibleChanged(this, false, IsVisible);
        }

        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create("CustomToolbarItem", typeof(bool), typeof(ToolbarItem), false, BindingMode.TwoWay, propertyChanged: OnIsVisibleChanged);

        private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var oldvalue = (bool)oldValue;
            var newvalue = (bool)newValue;
            var item = (CustomToolbarItem)bindable;

            if (item.Parent == null)
                return;

            var items = ((ContentPage)item.Parent).ToolbarItems;

            if (item.IsVisible && !items.Contains(item))
                items.Add(item);

            if (!item.IsVisible && items.Contains(item))
                items.Remove(item);
        }


    }
}
