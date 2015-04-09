namespace Sitecore.PublishStatus.Publishing
{
    public enum PublishStatusType
    {
        NoPublishingTargets,
        NotPublishedToDatabase,
        NotPublishedForLanguage,
        NotLatestVersion,
        NotLatestRevision,
        Latest
    }
}