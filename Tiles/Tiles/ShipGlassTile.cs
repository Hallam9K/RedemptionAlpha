using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;

namespace Redemption.Tiles.Tiles
{
    public class ShipGlassTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            DustType = DustID.Glass;
            MinPick = 500;
            MineResist = 7f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(193, 255, 219));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => RedeBossDowned.downedSlayer;
        public override bool CanExplode(int i, int j) => false;
    }
}