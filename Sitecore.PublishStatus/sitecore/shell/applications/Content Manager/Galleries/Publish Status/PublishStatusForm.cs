using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.PublishStatus.Publishing;
using Sitecore.Resources;
using Sitecore.Shell.Applications.ContentManager.Galleries;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.PublishStatus.sitecore.shell.applications.Galleries
{
    public class PublishStatusForm : GalleryForm
    {
        protected Border Result;
        
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnLoad(e);

            if (Context.ClientPage.IsEvent)
                return;

            Item contextItem = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
            Assert.IsNotNull(contextItem, "item");

            StringBuilder result = new StringBuilder();

            IEnumerable<PublishStatusInfo> statuses = PublishStatusManager.GetPublishStatus(contextItem);
            foreach (var statusForDatabase in statuses.GroupBy(x => x.Database))
            {
                result.Append("<div style='font-weight: bold; padding: 2px 0 0 5px'>");
                result.Append(statusForDatabase.Key);
                result.Append("</div>");

                foreach (PublishStatusInfo status in statusForDatabase)
                {
                    result.Append("<div style='padding: 2px 0 0 10px'>");
                    result.Append(RenderSingleStatus(status));
                    result.Append("</div>");
                }
            }
            
            Result.Controls.Add(new LiteralControl(result.ToString()));
        }

        private static string RenderSingleStatus(PublishStatusInfo status)
        {
            string languageString = string.Empty;
            if (!string.IsNullOrEmpty(status.LanguageIcon) && !string.IsNullOrEmpty(status.LanguageName))
            {
                languageString = Images.GetImage(status.LanguageIcon, 16, 16, "absmiddle", "0 4px 0 0") + status.LanguageName;
            }

            switch (status.Type)
            {
                case PublishStatusType.NoPublishingTargets:
                    return "No publishing targets.";
                case PublishStatusType.NotPublishedToDatabase:
                    return "<strong>Not published to database.</strong>";
                case PublishStatusType.NotPublishedForLanguage:
                    return string.Format("{0}: <strong>Not published for language.</strong>", languageString);
                case PublishStatusType.NotLatestVersion:
                    return string.Format("{0}, v{1}: <strong>Not latest version.</strong> Latest is v{2}.",
                        languageString, status.VersionPublished, status.VersionLatest);
                case PublishStatusType.NotLatestRevision:
                    return string.Format("{0}, v{1}: <strong>Not latest revision.</strong>", languageString, status.VersionLatest);
                case PublishStatusType.Latest:
                    return string.Format("{0}, v{1}: Latest is published.", languageString, status.VersionLatest);
            }

            return string.Empty;
        }
    }
}