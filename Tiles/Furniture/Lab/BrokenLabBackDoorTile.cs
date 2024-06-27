using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using System.Linq;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Redemption.Globals.RedeNet;
using Redemption.NPCs.Lab;
using Terraria.Utilities;

namespace Redemption.Tiles.Furniture.Lab
{
    public class BrokenLabBackDoorTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 4;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 5000;
            MineResist = 13f;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Broken Laboratory Door");
            AddMapEntry(new Color(189, 191, 200), name);
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if ((Main.tile[i, j].TileFrameX != 216 && Main.tile[i, j].TileFrameX != 144) || Main.tile[i, j].TileFrameY != 0)
                return;
            Player player = Main.LocalPlayer;
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 2f, j + 2f));
            if (dist <= 30f && dist > 4f)
            {
                int spawnOdds = 300 * ((Main.netMode == NetmodeID.SinglePlayer) ? 1 : Main.player.Where(x => x.active && !x.dead && x.DistanceSQ(new Vector2(i, j).ToWorldCoordinates()) < 1500 * 1500).Count());

                if (Main.rand.NextBool(Math.Max(1, spawnOdds)))
                {
                    WeightedRandom<int> NPCType = new(Main.rand);
                    NPCType.Add(ModContent.NPCType<BlisteredScientist>());
                    NPCType.Add(ModContent.NPCType<OozingScientist>());
                    NPCType.Add(ModContent.NPCType<BloatedScientist>(), .3);

                    int choice = NPCType;
                    static int GetNPCIndex(int choice) => choice;
                    if (NPC.CountNPCS(choice) >= 20)
                        return;
                    Vector2 pos = new Vector2(i + (Main.tile[i, j].TileFrameX == 144 ? 2.5f : .5f), j + 3.5f).ToWorldCoordinates();

                    if (Main.netMode == NetmodeID.SinglePlayer)
                        NPC.NewNPC(new EntitySource_TileUpdate(i, j), (int)pos.X, (int)pos.Y, GetNPCIndex(choice));
                    else
                    {
                        ModPacket packet = Redemption.Instance.GetPacket(ModMessageType.SpawnNPCFromClient, 3);
                        packet.Write(GetNPCIndex(choice));
                        packet.Write((int)pos.X);
                        packet.Write((int)pos.Y);
                        packet.Send();
                    }
                }
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class BrokenLabBackDoor2Tile : ModTile
    {
        public override string Texture => "Redemption/Tiles/Furniture/Lab/BrokenLabBackDoorTile";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 200;
            MineResist = 13f;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Broken Laboratory Door");
            AddMapEntry(new Color(189, 191, 200), name);
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class BrokenLabBackDoor : PlaceholderTile
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Lab/BrokenLabBackDoor2";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<BrokenLabBackDoorTile>();
        }
    }
}