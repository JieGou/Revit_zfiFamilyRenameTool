namespace zfiFamilyRenameTool.Services
{
    using System.Linq;
    using Abstractions;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    public class FamilyTypeWrapper : IRenameable
    {
        private readonly FamilyType _familyType;
        private readonly Document _doc;

        public FamilyTypeWrapper(FamilyType familyType, Document doc)
        {
            _familyType = familyType;
            _doc = doc;

            // Типоразмер \"{familyType.Name}\"
            Title = string.Format(Language.GetItem(ModPlusConnector.Instance.Name, "p5"), familyType.Name);
            Source = familyType.Name;
        }

        public string Title { get; }

        public string Source { get; }

        public string Destination { get; set; }

        public string GroupCondition => Source;

        public void Rename()
        {
            if (_doc.FamilyManager.Types.Cast<FamilyType>().Any(type => type.Name == Destination))
            {
                return;
            }

            using (var t = new Transaction(_doc, $"Rename {Source} type"))
            {
                t.Start();
                var fm = _doc.FamilyManager;
                fm.CurrentType = _familyType;
                fm.RenameCurrentType(Destination);
                t.Commit();
            }
        }

        public bool CanRename()
        {
            return true;
        }
    }
}
