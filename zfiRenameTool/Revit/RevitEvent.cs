namespace zfiRenameTool.Revit
{
    using System;
    using Autodesk.Revit.UI;

    public class RevitEvent : IExternalEventHandler
    {
        private Action<UIApplication> _action;
        private ExternalEvent _externalEvent;

        public RevitEvent()
        {
            _externalEvent = ExternalEvent.Create(this);
        }

        public void Run(Action<UIApplication> action)
        {
            _action = action;
            _externalEvent.Raise();
        }

        /// <inheritdoc/>
        public void Execute(UIApplication app)
        {
            try
            {
                _action?.Invoke(app);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <inheritdoc/>
        public string GetName()
        {
            return "RevitEvent";
        }
    }
}