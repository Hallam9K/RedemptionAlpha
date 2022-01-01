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
            Main.tileLighted[Type] = true;
            Main.tileOreFinderPriority[Type] = 320;
            TileID.Sets.Ore[Type] = true;
            DustType = DustID.Torch;
			ItemDrop = ModContent.ItemType<DragonLeadOre>();
            MinPick = 100;
            MineResist = 1.4f;
            SoundType = SoundID.Tink;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Dragon-Lead Ore");
            AddMapEntry(new Color(177, 142, 142), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.05f;
            b = 0f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (dist <= 2)
                player.AddBuff(BuffID.OnFire, 20);
        }
        public bool breakCheck;
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail && !WorldGen.gen)
            {
                if (breakCheck)
                    return;
                breakCheck = true;
                for (int k = i - 2; k <= i + 2; k++)
                {
                    for (int l = j - 2; l <= j + 2; l++)
                    {
                        if ((k != i || l != j) && Main.tile[k, l].IsActive && Main.tile[k, l].type == ModContent.TileType<DragonLeadOreTile>() && !Main.rand.NextBool(2))
                        {
                            WorldGen.KillTile(k, l, noItem: true);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, k, l);
                        }
                    }
                }
                breakCheck = false;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override bool CanExplode(int i, int j) => false;
    }
}