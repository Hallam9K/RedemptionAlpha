using Microsoft.Xna.Framework;
using Redemption.NPCs.Critters;
using Terraria;
using Terraria.DataStructures;
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
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            RegisterItemDrop(ItemID.StoneBlock, 0);
            DustType = DustID.Stone;
            HitSound = SoundID.Tink;
            MinPick = 0;
            MineResist = 1.5f;
            AddMapEntry(new Color(128, 128, 128));
        }
        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public bool breakCheck;
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail && !WorldGen.gen)
            {
                if (Main.rand.NextBool(2) && Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16 + 8, j * 16, ModContent.NPCType<SpiderSwarmer>());

                if (breakCheck)
                    return;
                breakCheck = true;
                for (int k = i - 3; k <= i + 3; k++)
                {
                    for (int l = j - 3; l <= j + 3; l++)
                    {
                        if ((k != i || l != j) && Main.tile[k, l].HasTile && Main.tile[k, l].TileType == ModContent.TileType<InfestedStoneTile>() && !Main.rand.NextBool(3))
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