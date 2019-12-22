namespace zfiFamilyRenameTool.Services
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    public class FamilyParameterValuesProvider : IRenameableProvider
    {
        // Значения параметров
        public string Name => Language.GetItem(ModPlusConnector.Instance.Name, "p2");

        public IEnumerable<IRenameable> GetRenameables(Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                throw new ArgumentException(string.Format(Language.GetItem(ModPlusConnector.Instance.Name, "err1"), doc.Title));
            }

            var fm = doc.FamilyManager;

            foreach (FamilyParameter p in fm.Parameters)
            {
                if (p.IsShared || p.IsReadOnly || p.Id.IntegerValue < 0 ||
                    p.StorageType == StorageType.ElementId ||
                    p.StorageType == StorageType.None)
                {
                    continue;
                }

                foreach (FamilyType fmType in fm.Types)
                {
                    if (string.IsNullOrWhiteSpace(fmType.Name))
                        continue;

                    if (!fmType.HasValue(p))
                        continue;
                    
                    yield return new FamilyParameterValueWrapper(p, fmType, doc);
                }
            }
        }
    }
}
