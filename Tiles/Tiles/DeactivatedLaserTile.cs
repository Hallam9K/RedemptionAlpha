using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class DeactivatedLaserTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = false;
            TileID.Sets.CanPlaceNextToNonSolidTile[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = DustID.Electric;
            MinPick = 5000;
            MineResist = 3f;
            HitSound = SoundID.Tink;
        }
        public override bool KillSound(int i, int j, bool fail) => false;
        public override bool CreateDust(int i, int j, ref int type) => false;
        public override bool CanExplode(int i, int j) => false;
    }
}