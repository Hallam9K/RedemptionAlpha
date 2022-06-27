using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.HM;

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
			ItemDrop = ModContent.ItemType<Cyberscrap>();
            MinPick = 200;
            MineResist = 4f;
            HitSound = SoundID.Tink;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cyberscrap");
            AddMapEntry(new Color(189, 191, 200), name);
		}
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override bool CanExplode(int i, int j) => false;
    }
}