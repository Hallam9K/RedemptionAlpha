using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Banners;
using Redemption.NPCs.Critters;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Redemption.Tiles.Banners
{
    public class MoonflareBatBannerTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);
            AddMapEntry(Color.LightGoldenrodYellow, Language.GetText("MapObject.Banner"));
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.SceneMetrics.hasBanner = true;
                Main.SceneMetrics.NPCBannerBuff[ModContent.NPCType<MoonflareBat>()] = true;
            }
        }
    }
}