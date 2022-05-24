using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class ShadesteelChainTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
            Main.tileRope[Type] = true;
            DustType = ModContent.DustType<ShadesteelDust>();
            MinPick = 100;
            MineResist = 7f;
            HitSound = SoundID.Tink;
            ItemDrop = ModContent.ItemType<ShadesteelChain>();
            AddMapEntry(new Color(83, 87, 123));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}