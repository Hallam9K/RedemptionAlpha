using Terraria.ModLoader;

namespace Redemption.Tags
{
    public static class ContentTags
    {
        /// <summary>
        ///     Returns a TagData instance, which can be used to modify and check for tags. <b />
        ///     Be sure to cache the return value whenever possible!
        /// </summary>
        /// <typeparam name="TTagGroup">The tag group that the tag comes from.</typeparam>
        /// <param name="tagName">The name of the tag.</param>
        public static TagData Get<TTagGroup>(string tagName) where TTagGroup : TagGroup =>
            GetGroup<TTagGroup>().GetTag(tagName);

        /// <summary> Returns an instance of the specified TagGroup. This is just a shorthand for ModContent.GetInstance. </summary>
        /// <typeparam name="TTagGroup"> The tag group that the tag comes from. </typeparam>
        public static TTagGroup GetGroup<TTagGroup>() where TTagGroup : TagGroup
            => ModContent.GetInstance<TTagGroup>();
    }
}