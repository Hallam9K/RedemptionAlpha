using Microsoft.Xna.Framework;
using Redemption.NPCs.Wasteland;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Banners
{
    public class RadioactiveJellyBannerTile : ModTile
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
            AddMapEntry(Color.DarkGreen, Language.GetText("MapObject.Banner"));
        }
        public override bool CreateDust(int i, int j, ref int type) => false;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.SceneMetrics.hasBanner = true;
                Main.SceneMetrics.NPCBannerBuff[ModContent.NPCType<RadioactiveJelly>()] = true;
            }
        }
    }
}