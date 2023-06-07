using Microsoft.Xna.Framework;
using Redemption.NPCs.Lab.Blisterface;
using System.Linq;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Redemption.Globals.RedeNet;
using Redemption.NPCs.Friendly;
using Terraria.DataStructures;
using Redemption.Dusts;

namespace Redemption.Tiles.Furniture.Lab
{
    public class SewerHoleTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = false;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = DustID.Tungsten;
            MinPick = 1000;
            MineResist = 3f;
            AddMapEntry(new Color(51, 61, 54));
        }
        public override bool CanExplode(int i, int j) => false;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX != 0 || Main.tile[i, j].TileFrameY != 0)
                return;
            Player player = Main.LocalPlayer;
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 1f, j + 1f));
            if (dist <= 20f && dist > 5f)
            {
                int spawnOdds = 250 * ((Main.netMode == NetmodeID.SinglePlayer) ? 1 : Main.player.Where(x => x.active && !x.dead && x.DistanceSQ(new Vector2(i, j).ToWorldCoordinates()) < 1500 * 1500).Count());

                if (Main.rand.NextBool(Math.Max(1, spawnOdds)) && NPC.CountNPCS(ModContent.NPCType<BlisteredFish>()) < 7)
                {
                    Vector2 pos = new Vector2(i + 1, j + 1).ToWorldCoordinates();
                    static int GetNPCIndex() => ModContent.NPCType<BlisteredFish>();

                    if (Main.netMode == NetmodeID.SinglePlayer)
                        NPC.NewNPC(new EntitySource_TileUpdate(i, j), (int)pos.X, (int)pos.Y, GetNPCIndex());
                    else
                    {
                        ModPacket packet = Redemption.Instance.GetPacket(ModMessageType.SpawnNPCFromClient, 3);
                        packet.Write(GetNPCIndex());
                        packet.Write((int)pos.X);
                        packet.Write((int)pos.Y);
                        packet.Send();
                    }
                    for (int k = 0; k < 8; k++)
                        Dust.NewDust(pos - new Vector2(24, 16), 16, 16, ModContent.DustType<XenoWaterDust>(), Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-4f, -2f));
                }
            }
        }
    }
}