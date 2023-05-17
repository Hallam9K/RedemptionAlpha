using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class AncientChainTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileMerge[Type][ModContent.TileType<AncientChainSolidTile>()] = true;
            Main.tileRope[Type] = true;
            DustType = DustID.Lead;
            MinPick = 100;
            MineResist = 7f;
            HitSound = CustomSounds.ChainHit;
            AddMapEntry(new Color(210, 200, 200));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
    public class AncientChainSolidTile : ModTile
    {
        public override string Texture => "Redemption/Tiles/Tiles/AncientChainTile";
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileMerge[Type][ModContent.TileType<AncientChainTile>()] = true;
            DustType = DustID.Lead;
            MinPick = 100;
            MineResist = 7f;
            HitSound = CustomSounds.ChainHit;
            AddMapEntry(new Color(210, 200, 200));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}