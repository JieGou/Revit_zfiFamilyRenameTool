namespace zfiRenameTool.Services
{
    using System.Collections.Generic;
    using Abstractions;
    using Autodesk.Revit.ApplicationServices;
    using Revit;

    public class RevitService
    {
        private readonly Application _app;
        private readonly RevitEvent _event;
        private List<IRenameableProvider> _providers;

        public RevitService(Application app, RevitEvent @event)
        {
            _app = app;
            _event = @event;

            SetupProvides();
        }

        public List<IRenameableProvider> Providers => _providers;

        private void SetupProvides()
        {
            _providers = new List<IRenameableProvider>();
            Providers.Add(new FamilyParametersProvider());
        }
    }
}