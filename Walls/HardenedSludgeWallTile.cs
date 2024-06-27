using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class HardenedSludgeWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = ModContent.DustType<SludgeDust>();
            AddMapEntry(new Color(14, 49, 15));
            HitSound = SoundID.NPCHit13;
        }
        public override bool CanExplode(int i, int j) => false;
        public override void KillWall(int i, int j, ref bool fail) => fail = true;
    }
    public class HardenedSludgeWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/HardenedSludgeWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = ModContent.DustType<SludgeDust>();
            AddMapEntry(new Color(14, 49, 15));
            HitSound = SoundID.NPCHit13;
        }
    }
}