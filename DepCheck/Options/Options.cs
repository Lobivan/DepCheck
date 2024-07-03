using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DepCheck
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.OptionsOptions), "DepCheck", "Options", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class OptionsOptions : BaseOptionPage<Options> { }
    }
    public enum TerminalState
    {
        doNotShow,
        showAndHideWhenDone,
        showAndKeepWhenDone,
    }
    public class Options : BaseOptionModel<Options>
    {
        [Category("Customization")]
        [DisplayName("Show Cmd")]
        [Description("Настройка отображения окна коммандной строки во время работы расширения")]
        [DefaultValue(TerminalState.showAndHideWhenDone)]
        public TerminalState ShowTrminal { get; set; } = TerminalState.showAndHideWhenDone;

        [Category("Customization")]
        [DisplayName("Open Report Automaticly")]
        [Description("Настройка автоматического открытия отчёта по завершении сканирования")]
        [DefaultValue(true)]
        public bool AutoOpenReport { get; set; } = true;

        [Category("Paths")]
        [DisplayName("Path to DC")]
        [Description("Указание пути до исполняемого файла dependency check")]
        [DefaultValue("")]
        public string DCPath { get; set; } = "";

        [Category("Paths")]
        [DisplayName("Path to DC Report")]
        [Description("Указание директории, в которую будет помещён отчёт.\nОставьте пустым для размещения в директории проекта")]
        [DefaultValue("")]
        public string ReportPath { get; set; } = "";
    }
}
