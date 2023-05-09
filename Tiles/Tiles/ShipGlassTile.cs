using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Redemption.Items.Tools.PostML;

namespace Redemption.Tiles.Tiles
{
    public class ShipGlassTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = DustID.Glass;
            MinPick = 1000;
            MineResist = 7f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(193, 255, 219));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<NanoAxe2>())
                return true;
            return RedeBossDowned.downedSlayer;
        }
        public override bool Slope(int i, int j) => true;
        public override bool CanExplode(int i, int j) => false;
    }
}