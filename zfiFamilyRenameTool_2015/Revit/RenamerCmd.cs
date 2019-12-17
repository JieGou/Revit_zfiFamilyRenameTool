namespace zfiFamilyRenameTool.Revit
{
    using System;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Exceptions;
    using Services;
    using View;
    using ViewModel;

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RenamerCmd : IExternalCommand
    {
        /// <inheritdoc/>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var app = commandData.Application.Application;
                var revitEvent = new RevitEvent();
                var revitService = new RevitService(app, revitEvent);
                var mainVm = new MainVm(revitService);
                var renamerWindow = new RenamerWindow()
                {
                    DataContext = mainVm
                };
                renamerWindow.Show();
            }
            catch (PluginException e)
            {
                message = e.Message;
                if (e.Elements.Count > 0)
                {
                    foreach (var element in e.Elements)
                    {
                        elements.Insert(element);
                    }
                }

                return Result.Failed;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}