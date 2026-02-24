using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class JunkMetalWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustID.Electric;
            HitSound = CustomSounds.MetalHit;
            RegisterItemDrop(ModContent.ItemType<CyberscrapWall>());
            AddMapEntry(new Color(113, 115, 120));
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class JunkMetalWallSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/JunkMetalWall";
        public override void SetStaticDefaults()
        {
            DustType = DustID.Electric;
            HitSound = CustomSounds.MetalHit;
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(113, 115, 120));
        }
        public override bool CanExplode(int i, int j) => false;
    }
}