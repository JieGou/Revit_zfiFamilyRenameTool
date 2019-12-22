namespace zfiFamilyRenameTool.Services
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    public class FamilyTypesProvider : IRenameableProvider
    {
        // Имена типоразмеров
        public string Name => Language.GetItem(ModPlusConnector.Instance.Name, "p3");

        public IEnumerable<IRenameable> GetRenameables(Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                throw new ArgumentException(string.Format(Language.GetItem(ModPlusConnector.Instance.Name, "err1"), doc.Title));
            }

            var fm = doc.FamilyManager;

            foreach (FamilyType fmType in fm.Types)
            {
                if (string.IsNullOrWhiteSpace(fmType.Name))
                    continue;

                yield return new FamilyTypeWrapper(fmType, doc);
            }
        }
    }
}
