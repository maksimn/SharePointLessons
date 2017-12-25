using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace LunchVoting.LunchVotingWebPart {
    [ToolboxItemAttribute(false)]
    public class LunchVotingWebPart : WebPart {
        // Сколько элементов отображать в Web Part
        // Если нужен топ-3, задать равным 3. Если топ-10, то 10.
        [WebBrowsable(true)] // чтобы было показано в SharePoint Web Part Editor
        [WebDisplayName("Result Count")]
        [Description("Specifies the number of results to display in the web part.")]
        [DefaultValue(3)] // Это значение также надо задать в файле .webpart. Так нужно для целей сериализации.
        // Если его не задать в .webpart, оно не будет сериализовано и не получит значения по умолчанию.
        [Category("Behavior")] // только для группировки свойств в редакторе
        [Personalizable(PersonalizationScope.Shared)] // Задает способ хранения значения (общее для всех пользователей)
            // либо каждый пользователь может задать свое значение
        public int ResultCount { get; set; }

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/LunchVoting/LunchVotingWebPart/LunchVotingWebPartUserControl.ascx";

        protected override void CreateChildControls() {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
