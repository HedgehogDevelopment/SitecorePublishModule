using System.Collections.Generic;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.PublishStatus.Publishing;
using Sitecore.Resources;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;

namespace Sitecore.PublishStatus.sitecore.shell.applications.Panels
{
    public class PublishStatusPanel : RibbonPanel
    {
        public override void Render(HtmlTextWriter output, Ribbon ribbon, Item button, CommandContext context)
        {
            Assert.ArgumentNotNull(output, "output");
            Assert.ArgumentNotNull(ribbon, "ribbon");
            Assert.ArgumentNotNull(button, "button");
            Assert.ArgumentNotNull(context, "context");

            if (context.Items.Length != 1)
            {
                return;
            }

            Item contextItem = context.Items[0];
            Assert.IsNotNull(contextItem, "item");

            RenderStatus(output, contextItem);
        }

        private static void RenderStatus(HtmlTextWriter output, Item contextItem)
        {
            output.AddAttribute(HtmlTextWriterAttribute.Class, "scRibbonToolbarText");
            output.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "80%");

            // setting float, display and vertical-align fixes the default left align of scRibbonToolbarText
            output.AddStyleAttribute("float", "left");
            output.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline-block");
            output.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");

            output.RenderBeginTag(HtmlTextWriterTag.Div);

            // we want the panel to display only English
            IEnumerable<PublishStatusInfo> statuses = PublishStatusManager.GetPublishStatus(contextItem,
                new List<Language> { LanguageManager.GetLanguage("en") });

            foreach (PublishStatusInfo status in statuses)
            {
                output.RenderBeginTag(HtmlTextWriterTag.Div);

                RenderSingleStatus(output, status);

                output.RenderEndTag();
            }

            output.RenderEndTag();
        }

        private static void RenderSingleStatus(HtmlTextWriter output, PublishStatusInfo status)
        {
            string languageString = string.Empty;
            if (!string.IsNullOrEmpty(status.LanguageIcon) && !string.IsNullOrEmpty(status.LanguageName))
            {
                languageString = Images.GetImage(status.LanguageIcon, 16, 16, "absmiddle", "0 4px 0 0") + status.LanguageName;
            }

            switch (status.Type)
            {
                case PublishStatusType.NoPublishingTargets:
                    output.Write("No publishing targets.");
                    break;
                case PublishStatusType.NotPublishedToDatabase:
                    output.Write("{0}: <strong>Not published to database.</strong>", status.Database);
                    break;
                case PublishStatusType.NotPublishedForLanguage:
                    output.Write("{0}, {1}: <strong>Not published for language.</strong>", status.Database, languageString);
                    break;
                case PublishStatusType.NotLatestVersion:
                    output.Write("{0}, {1}, v{2}: <strong>Not latest version.</strong> Latest is v{3}.",
                        status.Database, languageString, status.VersionPublished, status.VersionLatest);
                    break;
                case PublishStatusType.NotLatestRevision:
                    output.Write("{0}, {1}, v{2}: <strong>Not latest revision.</strong>", status.Database, languageString, status.VersionLatest);
                    break;
                case PublishStatusType.Latest:
                    output.Write("{0}, {1}, v{2}: Latest is published.", status.Database, languageString, status.VersionLatest);
                    break;
            }
        }
    }
}