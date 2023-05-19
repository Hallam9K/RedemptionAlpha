using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class LivingSilverwoodTwistedTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<SilverwoodLeafTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<LivingSilverwoodTile>()] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            DustType = DustID.t_PearlWood;
            AddMapEntry(new Color(228, 213, 173));
            MineResist = 2.5f;
            RegisterItemDrop(ModContent.ItemType<Silverwood>(), 0);
        }
    }
}

