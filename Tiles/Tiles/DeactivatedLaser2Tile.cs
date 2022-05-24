using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Tiles.Tiles
{
    public class DeactivatedLaser2Tile : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = false;
            DustType = DustID.Electric;
            MinPick = 500;
            MineResist = 3f;
            HitSound = SoundID.Tink;
        }
        public override bool KillSound(int i, int j) => false;
        public override bool CreateDust(int i, int j, ref int type) => false;
        public override bool CanExplode(int i, int j) => false;
    }
}