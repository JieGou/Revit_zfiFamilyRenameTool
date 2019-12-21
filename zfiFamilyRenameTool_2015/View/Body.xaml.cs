namespace zfiFamilyRenameTool.View
{
    public partial class Body 
    {
        public Body()
        {
            InitializeComponent();

            // change theme
            ModPlusAPI.Windows.Helpers.WindowHelpers.ChangeStyleForResourceDictionary(Resources);

            // change lang
            ModPlusAPI.Language.SetLanguageProviderForResourceDictionary(Resources);
        }
    }
}