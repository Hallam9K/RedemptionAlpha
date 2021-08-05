using Terraria.ModLoader;

namespace Redemption.Tags
{
    public sealed class ItemTags : TagGroup
    {
        public override int TypeCount => ItemLoader.ItemCount;
    }
}