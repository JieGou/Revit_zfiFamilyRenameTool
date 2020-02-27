namespace zfiFamilyRenameTool.Services
{
    using Abstractions;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    public class FamilyIsInstanceParameterWrapper : IRenameable
    {
        private readonly FamilyParameter _parameter;
        private readonly Document _doc;
        private readonly bool _isInstance;

        public FamilyIsInstanceParameterWrapper(FamilyParameter parameter, Document doc)
        {
            _parameter = parameter;
            _doc = doc;

            // Параметр \"{_parameter.Definition.Name}\"
            Title = string.Format(Language.GetItem(ModPlusConnector.Instance.Name, "p5"), parameter.Definition.Name);
            ParameterName = parameter.Definition.Name;
            ParameterFormula = parameter.Formula ?? string.Empty;
            _isInstance = parameter.IsInstance;
            Destination = string.Empty;
        }

        /// <inheritdoc />
        public string Title { get; }

        public string ParameterName { get; }

        /// <summary>
        /// Formula
        /// </summary>
        public string ParameterFormula { get; }

        /// <inheritdoc />
        public string Source => GetIsInstanceDisplay(_isInstance);

        /// <inheritdoc />
        public string Destination { get; private set; }

        /// <inheritdoc />
        public string GroupCondition => $"{ParameterName}.{ParameterFormula}";

        /// <inheritdoc />
        public void SetNewDestination(string value)
        {
            Destination = !string.IsNullOrEmpty(value) ? GetIsInstanceDisplay(!_isInstance) : string.Empty;
        }

        /// <inheritdoc />
        public void Rename()
        {
            using (var t = new Transaction(_doc, "Change IsInstance property"))
            {
                t.Start();
                var fm = _doc.FamilyManager;
                if (!string.IsNullOrEmpty(Destination))
                {
                    if (_isInstance)
                        fm.MakeType(_parameter);
                    else 
                        fm.MakeInstance(_parameter);
                }

                t.Commit();
            }
        }

        /// <inheritdoc />
        public bool CanRename()
        {
            return true;
        }

        private static string GetIsInstanceDisplay(bool isInstance)
        {
            return isInstance ? 
                Language.GetItem(ModPlusConnector.Instance.Name, "h25") : // instance parameter
                Language.GetItem(ModPlusConnector.Instance.Name, "h26"); // type parameter
        }
    }
}
