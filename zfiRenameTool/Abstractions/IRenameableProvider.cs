namespace zfiFamilyRenameTool.Abstractions
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;

    public interface IRenameableProvider
    {
        string Name { get; }

        IEnumerable<IRenameable> GetRenameables(Document doc);
    }
}