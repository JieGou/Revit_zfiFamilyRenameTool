namespace zfiFamilyRenameTool.Services
{
    using System.Globalization;
    using Abstractions;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    public class FamilyParameterValueWrapper : IRenameable
    {
        private readonly FamilyParameter _parameter;
        private readonly FamilyType _familyType;
        private readonly Document _doc;

        public FamilyParameterValueWrapper(FamilyParameter parameter, FamilyType familyType, Document doc)
        {
            _parameter = parameter;
            _familyType = familyType;
            _doc = doc;

            // Значение параметра \"{parameter.Definition.Name}\"
            Title = string.Format(Language.GetItem(ModPlusConnector.Instance.Name, "p4"), parameter.Definition.Name);
            ParameterName = parameter.Definition.Name;
            FamilyTypeName = familyType.Name;
            switch (parameter.StorageType)
            {
                case StorageType.String:
                    Source = familyType.AsString(parameter);
                    break;
                case StorageType.Integer:
                    Source = familyType.AsInteger(parameter).ToString();
                    break;
                case StorageType.Double:
                    var d = familyType.AsDouble(parameter);
                    if (d.HasValue)
                    {
                        Source = UnitUtils.ConvertFromInternalUnits(d.Value, parameter.DisplayUnitType)
                            .ToString(CultureInfo.InvariantCulture);
                    }

                    break;
            }
        }

        public string Title { get; }

        public string ParameterName { get; }

        public string FamilyTypeName { get; }

        public string Source { get; }

        public string Destination { get; set; }

        public string GroupCondition => $"{ParameterName}.{FamilyTypeName}.{Source}";

        public void Rename()
        {
            using (var t = new Transaction(_doc, $"Rename {ParameterName} parameter value"))
            {
                t.Start();
                var fm = _doc.FamilyManager;
                fm.CurrentType = _familyType;
                switch (_parameter.StorageType)
                {
                    case StorageType.Double:
                        if (double.TryParse(Destination.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
                            fm.Set(_parameter, d);
                        break;
                    case StorageType.String:
                        fm.Set(_parameter, Destination);
                        break;
                    case StorageType.Integer:
                        if (int.TryParse(Destination, out var i))
                            fm.Set(_parameter, i);
                        break;
                }

                t.Commit();
            }
        }

        public bool CanRename()
        {
            return true;
        }
    }
}
