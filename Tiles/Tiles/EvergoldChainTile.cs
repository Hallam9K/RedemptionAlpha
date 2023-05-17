using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Terraria.Audio;

namespace Redemption.Tiles.Tiles
{
    public class EvergoldChainTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
            Main.tileMerge[Type][ModContent.TileType<EvergoldChainSolidTile>()] = true;
            Main.tileRope[Type] = true;
            DustType = DustID.GoldCoin;
            MinPick = 100;
            MineResist = 7f;
            HitSound = CustomSounds.ChainHit;
            ItemDrop = ModContent.ItemType<EvergoldChain>();
            AddMapEntry(new Color(230, 230, 50));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
    public class EvergoldChainSolidTile : ModTile
    {
        public override string Texture => "Redemption/Tiles/Tiles/EvergoldChainTile";
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileMerge[Type][ModContent.TileType<EvergoldChainTile>()] = true;
            DustType = DustID.GoldCoin;
            MinPick = 100;
            MineResist = 7f;
            HitSound = CustomSounds.ChainHit;
            ItemDrop = ModContent.ItemType<EvergoldChain>();
            AddMapEntry(new Color(230, 230, 50));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}