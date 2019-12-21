namespace zfiFamilyRenameTool.Services
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Autodesk.Revit.DB;

    public class FamilyParameterValuesProvider : IRenameableProvider
    {
        public string Name => "Значения параметров";

        public IEnumerable<IRenameable> GetRenameables(Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                throw new ArgumentException($"Документ {doc.Title} не является семейством");
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
