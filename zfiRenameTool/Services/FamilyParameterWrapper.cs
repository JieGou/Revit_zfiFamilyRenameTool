namespace zfiRenameTool.Services
{
    using Abstractions;
    using Autodesk.Revit.DB;

    public class FamilyParameterWrapper : IRenameable
    {
        private readonly FamilyParameter _parameter;
        private readonly Document _doc;

        public FamilyParameterWrapper(FamilyParameter parameter, Document doc)
        {
            _parameter = parameter;
            _doc = doc;
        }

        public string Title => "Параметр";

        public string Source => _parameter.Definition.Name;

        public string Destination { get; set; } = string.Empty;

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
    }
}