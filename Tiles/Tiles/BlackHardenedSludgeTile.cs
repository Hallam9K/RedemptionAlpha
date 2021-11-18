using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Dusts;
using Redemption.Buffs.Debuffs;
using Terraria.ID;
using Redemption.NPCs.Lab;

namespace Redemption.Tiles.Tiles
{
    public class BlackHardenedSludgeTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
            Main.tileBouncy[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            DustType = ModContent.DustType<SludgeDust>();
            MinPick = 300;
            MineResist = 8f;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Black Hardened Sludge");
            AddMapEntry(new Color(31, 30, 46));
		}
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.rand.NextBool(4) && !fail && Main.netMode != NetmodeID.MultiplayerClient)
                NPC.NewNPC(i * 16 + 8, j * 16 + 8, ModContent.NPCType<SludgeBlob>());
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}