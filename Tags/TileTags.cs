using Terraria.ModLoader;

namespace Redemption.Tags
{
    public sealed class TileTags : TagGroup
    {
        public override int TypeCount => TileLoader.TileCount;
    }
}