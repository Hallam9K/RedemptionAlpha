using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Redemption.NPCs.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class InfestedStoneTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileStone[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            ItemDrop = ItemID.StoneBlock;
            DustType = DustID.Stone;
            SoundType = SoundID.Tink;
            MinPick = 0;
            MineResist = 1.5f;
            AddMapEntry(new Color(128, 128, 128));
        }
        public bool breakCheck;
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail && !WorldGen.gen)
            {
                if (Main.rand.NextBool(2) && Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.NewNPC(i * 16 + 8, j * 16, ModContent.NPCType<SpiderSwarmer>());

                if (breakCheck)
                    return;
                breakCheck = true;
                for (int k = i - 4; k <= i + 4; k++)
                {
                    for (int l = j - 4; l <= j + 4; l++)
                    {
                        if ((k != i || l != j) && Main.tile[k, l].IsActive && Main.tile[k, l].type == ModContent.TileType<InfestedStoneTile>() && !Main.rand.NextBool(3))
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
    }
}