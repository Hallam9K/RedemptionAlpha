using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class ShinkiteBrickTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            DustType = ModContent.DustType<ShinkiteDust>();
            ItemDrop = ModContent.ItemType<ShinkiteBrick>();
            MinPick = 200;
            MineResist = 4f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(93, 62, 65));
		}
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}