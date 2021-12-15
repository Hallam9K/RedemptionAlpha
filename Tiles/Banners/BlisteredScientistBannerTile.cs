using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Banners;
using Redemption.NPCs.Lab;
using Redemption.NPCs.PreHM;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Banners
{
    public class BlisteredScientistBannerTile : ModTile
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
            AddMapEntry(Color.DarkGreen);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<BlisteredScientistBanner>());
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.SceneMetrics.hasBanner = true;
                Main.SceneMetrics.NPCBannerBuff[ModContent.NPCType<BlisteredScientist>()] = true;
            }
        }
    }
}