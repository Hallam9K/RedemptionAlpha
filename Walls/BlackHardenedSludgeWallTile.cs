using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class BlackHardenedSludgeWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            HitSound = SoundID.NPCHit13;
            DustType = ModContent.DustType<SludgeDust>();
            AddMapEntry(new Color(12, 16, 19));
        }
        public override bool CanExplode(int i, int j) => false;
        public override void KillWall(int i, int j, ref bool fail) => fail = true;
    }
    public class BlackHardenedSludgeWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/BlackHardenedSludgeWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = ModContent.DustType<SludgeDust>();
            AddMapEntry(new Color(14, 49, 15));
            HitSound = SoundID.NPCHit13;
        }
    }
}