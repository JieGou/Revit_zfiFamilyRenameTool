// source - https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/

namespace zfiFamilyRenameTool.View
{
    using System.Windows;

    public class BindingProxy : Freezable
    {
        // Overrides of Freezable
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
 
        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
 
        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}
