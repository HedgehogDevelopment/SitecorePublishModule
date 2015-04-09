
namespace Sitecore.PublishStatus.Publishing
{
    public class PublishStatusInfo
    {
        public PublishStatusType Type { get; private set; }
        public string Database { get; private set; }
        public string LanguageIcon { get; private set; }
        public string LanguageName { get; private set; }
        public int VersionLatest { get; private set; }
        public int VersionPublished { get; private set; }
                
        private PublishStatusInfo()
        {
            // Only utility static methods are used to obtain an instance of this class
        }
        
        public static PublishStatusInfo NoPublishingTargets()
        {
            return new PublishStatusInfo
            {
                Type = PublishStatusType.NoPublishingTargets
            };
        }

        public static PublishStatusInfo NotPublishedToDatabase(string database)
        {
            return new PublishStatusInfo
            {
                Type = PublishStatusType.NotPublishedToDatabase,
                Database = database
            };
        }

        public static PublishStatusInfo NotPublishedForLanguage(string database, string languageIcon, string languageName)
        {
            return new PublishStatusInfo
            {
                Type = PublishStatusType.NotPublishedForLanguage,
                Database = database,
                LanguageIcon = languageIcon,
                LanguageName = languageName
            };
        }

        public static PublishStatusInfo NotLatestVersion(string database, string languageIcon, string languageName, int versionLatest, int versionPublished)
        {
            return new PublishStatusInfo
            {
                Type = PublishStatusType.NotLatestVersion,
                Database = database,
                LanguageIcon = languageIcon,
                LanguageName = languageName,
                VersionLatest = versionLatest,
                VersionPublished = versionPublished
            };
        }

        public static PublishStatusInfo NotLatestRevision(string database, string languageIcon, string languageName, int versionLatest)
        {
            return new PublishStatusInfo
            {
                Type = PublishStatusType.NotLatestRevision,
                Database = database,
                LanguageIcon = languageIcon,
                LanguageName = languageName,
                VersionLatest = versionLatest
            };
        }

        public static PublishStatusInfo Latest(string database, string languageIcon, string languageName, int versionLatest)
        {
            return new PublishStatusInfo
            {
                Type = PublishStatusType.Latest,
                Database = database,
                LanguageIcon = languageIcon,
                LanguageName = languageName,
                VersionLatest = versionLatest
            };
        }
    }
}