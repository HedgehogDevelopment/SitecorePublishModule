using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Sitecore.PublishStatus.Publishing
{
    public static class PublishStatusManager
    {
        public static IEnumerable<PublishStatusInfo> GetPublishStatus(Item contextItem)
        {
            return GetPublishStatus(contextItem, new List<Language>());
        }

        public static IEnumerable<PublishStatusInfo> GetPublishStatus(Item contextItem, List<Language> languageFilter)
        {
            const string publishingTargetsPath = "/sitecore/system/publishing targets";

            List<PublishStatusInfo> publishStatus =  new List<PublishStatusInfo>();

            // Retrieve the publishing targets database names
            Dictionary<string, string> publishingTargetsDatabases = Client
                .GetItemNotNull(publishingTargetsPath)
                .Children
                .ToDictionary(x => x[FieldIDs.PublishingTargetDatabase], x => x.Name);

            // Check publishing targets available
            if (publishingTargetsDatabases.Count == 0)
            {
                publishStatus.Add(PublishStatusInfo.NoPublishingTargets());
            }
            else
            {
                foreach (string databaseName in publishingTargetsDatabases.Keys)
                {
                    if (DatabaseExists(databaseName))
                    {
                        Database database = Configuration.Factory.GetDatabase(databaseName);
                        Item targetItem = database.GetItem(contextItem.ID);

                        string databaseDisplayName = publishingTargetsDatabases[databaseName];

                        PopulatePublishStatus(publishStatus, databaseDisplayName, contextItem, targetItem,
                            languageFilter);
                    }
                }
            }

            return publishStatus;
        }

        private static void PopulatePublishStatus(List<PublishStatusInfo> publishStatus, string database,
            Item contextItem, Item targetItem, List<Language> languageFilter)
        {
            // Check item published to database
            if (targetItem == null)
            {
                publishStatus.Add(PublishStatusInfo.NotPublishedToDatabase(database));
            }
            else
            {
                IEnumerable<Language> languages = contextItem.Languages;

                if (languageFilter.Any())
                {
                    languages = languages.Intersect(languageFilter);
                }

                foreach (Language language in languages)
                {
                    PopulatePublishStatus(publishStatus, database, language, contextItem, targetItem);
                }
            }
        }

        private static void PopulatePublishStatus(List<PublishStatusInfo> publishStatus, string database,
            Language language, Item contextItem, Item targetItem)
        {
            Item contextLanguageVersion = contextItem.Versions.GetLatestVersion(language);

            if (contextLanguageVersion == null || contextLanguageVersion.Versions.Count == 0)
            {
                // No version for this language in the context items so the language isn't used at all
                return;
            }

            string languageIcon = LanguageService.GetIcon(language, contextItem.Database);
            string languageName = language.CultureInfo.DisplayName;

            // Check item published for language
            Item targetLanguageVersion = targetItem.Versions.GetLatestVersion(language);
            if (targetLanguageVersion == null || targetLanguageVersion.Versions.Count == 0)
            {
                publishStatus.Add(PublishStatusInfo.NotPublishedForLanguage(database, languageIcon, languageName));
                return;
            }

            // Check latest version is published
            int contextLanguageVersionNumber = contextLanguageVersion.Version.Number;
            int targetLanguageVersionNumber = targetLanguageVersion.Version.Number;

            if (targetLanguageVersionNumber != contextLanguageVersionNumber)
            {
                publishStatus.Add(PublishStatusInfo.NotLatestVersion(database, languageIcon,
                    languageName, contextLanguageVersionNumber, targetLanguageVersionNumber));

                return;
            }

            // Check latest revision is published
            string contextRevision = contextLanguageVersion.Statistics.Revision;
            string targetRevision = targetLanguageVersion.Statistics.Revision;
            if (contextRevision != targetRevision)
            {
                publishStatus.Add(PublishStatusInfo.NotLatestRevision(database, languageIcon,
                    languageName, contextLanguageVersionNumber));
                return;
            }

            // It seems latest and greatest is published to this database
            publishStatus.Add(PublishStatusInfo.Latest(database, languageIcon, languageName, contextLanguageVersionNumber));
        }

        private static bool DatabaseExists(string key)
        {
            return Configuration.Factory.GetDatabaseNames().Any(d => d.Equals(key));
        }
    }
}