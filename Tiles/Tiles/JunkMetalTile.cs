using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Tiles.Tiles
{
    public class JunkMetalTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 650;
            DustType = DustID.Electric;
            MinPick = 200;
            MineResist = 4f;
            HitSound = CustomSounds.MetalHit;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Cyberscrap");
            AddMapEntry(new Color(189, 191, 200), name);
		}
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override bool CanExplode(int i, int j) => false;
    }
}