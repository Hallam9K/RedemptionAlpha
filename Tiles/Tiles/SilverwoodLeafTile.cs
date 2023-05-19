using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class SilverwoodLeafTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LivingSilverwoodTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<LivingSilverwoodTwistedTile>()] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            AddMapEntry(new Color(228, 213, 173));
            TileID.Sets.Leaves[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(207, 193, 27));
            MineResist = 1f;
            DustType = DustID.GoldCoin;
            HitSound = SoundID.Grass;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .4f;
            g = .3f;
            b = .3f;
        }
        public override void RandomUpdate(int i, int j)
        {
            if (Main.netMode != NetmodeID.Server && Framing.GetTileSafely(i, j).HasTile && Framing.GetTileSafely(i, j + 1).LiquidAmount == 0)
                Gore.NewGore(new EntitySource_TileUpdate(i, j), new Vector2((i + .5f) * 16, (j + 1) * 16), Vector2.Zero, ModContent.Find<ModGore>("Redemption/SilverwoodLeafFX").Type);
        }
    }
}

