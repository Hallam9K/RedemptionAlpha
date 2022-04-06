using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class AncientAlloyPipeTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
            DustType = ModContent.DustType<SlateDust>();
            MinPick = 350;
            MineResist = 11f;
            SoundType = SoundID.Tink;
            AddMapEntry(new Color(105, 97, 102));
            ItemDrop = ModContent.ItemType<AncientAlloyPipe>();
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}