namespace zfiFamilyRenameTool.Services
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Autodesk.Revit.DB;

    public class FamilyTypesProvider : IRenameableProvider
    {
        public string Name => "Имена типоразмеров";

        public IEnumerable<IRenameable> GetRenameables(Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                throw new ArgumentException($"Документ {doc.Title} не является семейством");
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
