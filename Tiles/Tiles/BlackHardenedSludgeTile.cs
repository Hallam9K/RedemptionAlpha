using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Redemption.Dusts;
using Terraria.ID;
using Redemption.NPCs.Lab;
using Terraria.DataStructures;

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
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe>()] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe2>()] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile2>()] = true;
            DustType = ModContent.DustType<SludgeDust>();
            MinPick = 300;
            MineResist = 8f;
            HitSound = SoundID.NPCHit13;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Black Hardened Sludge");
            AddMapEntry(new Color(31, 30, 46));
        }
        public override void FloorVisuals(Player player)
        {
            if (player.velocity.X != 0f && Main.rand.NextBool(4))
                Dust.NewDustDirect(player.BottomLeft, player.width, 0, DustType, -player.velocity.X / 4, -Main.rand.NextFloat(1f));
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.rand.NextBool(4) && !fail && Main.netMode != NetmodeID.MultiplayerClient)
                NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16 + 8, j * 16 + 8, ModContent.NPCType<OozeBlob>());
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}