using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Tiles.Ores
{
    public class DragonLeadOreTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileBlockLight[Type] = true;
            //Main.tileValue[Type] = 320;
            DustType = DustID.Torch;
			ItemDrop = ModContent.ItemType<DragonLeadOre>();
            MinPick = 100;
            MineResist = 1.4f;
            SoundType = SoundID.Tink;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Dragon-Lead Ore");
            AddMapEntry(new Color(177, 142, 142), name);
		}
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (dist <= 2)
                player.AddBuff(BuffID.OnFire, 20);
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override bool CanExplode(int i, int j) => false;
    }
}