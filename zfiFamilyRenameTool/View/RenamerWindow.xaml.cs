namespace zfiFamilyRenameTool.View
{
    using Abstractions;

    public partial class RenamerWindow : ICloseable
    {
        public RenamerWindow()
        {
            InitializeComponent();

            Title = ModPlusAPI.Language.GetFunctionLocalName(ModPlusConnector.Instance);
        }
    }
}