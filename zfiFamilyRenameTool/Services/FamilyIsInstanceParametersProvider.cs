namespace zfiFamilyRenameTool.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    public class FamilyIsInstanceParametersProvider : IRenameableProvider
    {
        /// <inheritdoc />
        public string Name => Language.GetItem(ModPlusConnector.Instance.Name, "h24");

        /// <inheritdoc />
        public TabItemType TabItemType => TabItemType.ParameterNameAndSourceAndDestinationAndFormula;

        public IEnumerable<IRenameable> GetRenameables(Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                // Документ \"{doc.Title}\" не является семейством
                throw new ArgumentException(string.Format(Language.GetItem(ModPlusConnector.Instance.Name, "err1"), doc.Title));
            }

            var fm = doc.FamilyManager;
            
            foreach (FamilyParameter p in fm.Parameters)
            {
                if (p.Id.IntegerValue < 0)
                {
                    continue;
                }
                
                yield return new FamilyIsInstanceParameterWrapper(p, doc);
            }
        }
    }
}
