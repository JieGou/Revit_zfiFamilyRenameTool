namespace zfiFamilyRenameTool.View
{
    using System.Windows;
    using System.Windows.Controls;
    using Abstractions;
    using ModPlusAPI.Windows.Helpers;
    using ViewModel;

    public class TabItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TabViewModel tabViewModel)
            {
                var userControl = container.TryFindParent<UserControl>();
                switch (tabViewModel.TabItemType)
                {
                    case TabItemType.SourceAndDestination:
                        return userControl.FindResource("SourceAndDestination") as DataTemplate;
                    case TabItemType.ParameterNameAndSourceAndDestination:
                        return userControl.FindResource("ParameterNameAndSourceAndDestination") as DataTemplate;
                    case TabItemType.ParameterNameAndTypeNameAndSourceAndDestination:
                        return userControl.FindResource("ParameterNameAndTypeNameAndSourceAndDestination") as DataTemplate;
                    case TabItemType.ParameterNameAndSourceAndDestinationAndFormula:
                        return userControl.FindResource("ParameterNameAndSourceAndDestinationAndFormula") as DataTemplate;
                }
            }

            return null;
        }
    }
}
