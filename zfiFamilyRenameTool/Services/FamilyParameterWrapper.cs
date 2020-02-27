namespace zfiFamilyRenameTool.Services
{
    using Abstractions;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    public class FamilyParameterWrapper : IRenameable
    {
        private readonly FamilyParameter _parameter;
        private readonly Document _doc;

        public FamilyParameterWrapper(FamilyParameter parameter, Document doc)
        {
            _parameter = parameter;
            _doc = doc;

            // Параметр \"{_parameter.Definition.Name}\"
            Title = string.Format(Language.GetItem(ModPlusConnector.Instance.Name, "p5"), parameter.Definition.Name);
            Source = _parameter.Definition.Name;
            Destination = string.Empty;
        }

        public string Title { get; }

        public string Source { get; }

        public string Destination { get; private set; }

        public string GroupCondition => Source;

        public void SetNewDestination(string value)
        {
            Destination = value;
        }

        public void Rename()
        {
            using (var t = new Transaction(_doc, $"Rename {Source} parameter"))
            {
                t.Start();
                var fm = _doc.FamilyManager;
                fm.RenameParameter(_parameter, Destination);
                t.Commit();
            }
        }

        public bool CanRename()
        {
            return true;
        }
    }
}