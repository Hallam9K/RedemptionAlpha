using Microsoft.Xna.Framework;
using Redemption.NPCs.FowlMorning;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Redemption.Globals.RedeNet;

namespace Redemption.Globals.World
{
    public class FowlMorningWorld : ModSystem
    {
        public static int ChickPoints = 0;
        public static int ChickWave = 0;
        public static bool FowlMorningActive;

        public override void ClearWorld()
        {
            FowlMorningActive = false;
            ChickPoints = 0;
            ChickWave = 0;
        }
        public static ModPacket CreateProgressPacket()
        {
            ModPacket packet = ModContent.GetInstance<Redemption>().GetPacket();
            packet.Write((byte)ModMessageType.FowlMorningData);
            packet.Write(ChickPoints);
            packet.Write(ChickWave);
            packet.Write(FowlMorningActive);

            return packet;
        }

        public static void SendInfoPacket()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            CreateProgressPacket().Send();
        }

        public static void HandlePacket(BinaryReader reader)
        {
            ChickPoints = reader.ReadInt32();
            ChickWave = reader.ReadInt32();
            FowlMorningActive = reader.ReadBoolean();

            if (Main.netMode == NetmodeID.Server)
                SendInfoPacket(); // Forward packet to rest of clients
        }
        public static void ChickArmyStart()
        {
            SendInfoPacket();
        }
        public static void ChickArmyEnd()
        {
            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Event.ChickRetreat");
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(250, 170, 50));
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), new Color(250, 170, 50));

            FowlMorningActive = false;
            ChickPoints = 0;
            ChickWave = 0;

            RedeBossDowned.downedFowlMorning = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            SendInfoPacket();
        }
        public override void PostUpdateWorld()
        {
            if (!FowlMorningActive)
            {
                ChickPoints = 0;
                ChickWave = 0;
            }
            if (!Main.dayTime)
            {
                ChickPoints = 0;
                ChickWave = 0;
                FowlMorningActive = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }
    }
    public class FowlMorningNPC : GlobalNPC
    {
        public static int maxPoints = 15;
        public override void OnKill(Terraria.NPC npc)
        {
            if (FowlMorningWorld.FowlMorningActive)
            {
                maxPoints = FowlMorningWorld.ChickWave switch
                {
                    1 => 30,
                    2 => 40,
                    3 => 50,
                    4 => 80,
                    5 => 100,
                    6 => 150,
                    7 => 500,
                    _ => 15,
                };
                if (FowlMorningWorld.ChickPoints >= maxPoints)
                {
                    if (FowlMorningWorld.ChickWave >= 7)
                        FowlMorningWorld.ChickArmyEnd();
                    else
                    {
                        FowlMorningWorld.ChickPoints = 0;
                        FowlMorningWorld.ChickWave++;

                        string waveText = GetWaveChatText(FowlMorningWorld.ChickWave);
                        Color color = new(175, 75, 255);
                        if (Main.netMode == NetmodeID.Server)
                            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(waveText), color);
                        else if (Main.netMode == NetmodeID.SinglePlayer)
                            Main.NewText(Language.GetTextValue(waveText), color);
                    }
                }
                FowlMorningWorld.SendInfoPacket();
            }
        }
        public static string GetWaveChatText(int wave)
        {
            string wavetext = "Wave: " + (wave + 1) + ": ";
            IDictionary<int, float> spawnpool = SpawnPool.ElementAt(wave);
            wavetext += Lang.GetNPCName(spawnpool.First().Key);
            foreach (KeyValuePair<int, float> key in spawnpool.Skip(1))
            {
                wavetext += ", " + Lang.GetNPCName(key.Key);
            }
            return wavetext;
        }
        public static List<IDictionary<int, float>> SpawnPool
        {
            get => new()
            {
                new Dictionary<int, float> { {ModContent.NPCType<ChickenScratcher>(), 1f} }, // 1
                new Dictionary<int, float> { // 2
                    {ModContent.NPCType<ChickenScratcher>(), 1f},
                    {ModContent.NPCType<ChickenBomber>(), Terraria.NPC.CountNPCS(ModContent.NPCType<ChickenBomber>()) < 2 ? .4f : 0f },
                },
                new Dictionary<int, float> { // 3
                    {ModContent.NPCType<ChickenScratcher>(), 1f},
                    {ModContent.NPCType<ChickenBomber>(), Terraria.NPC.CountNPCS(ModContent.NPCType<ChickenBomber>()) < 2 ? .5f : 0f },
                    {ModContent.NPCType<RoosterBooster>(), Terraria.NPC.CountNPCS(ModContent.NPCType<RoosterBooster>()) < 2 ? .6f : 0f },
                },
                new Dictionary<int, float> { // 4
                    {ModContent.NPCType<ChickenScratcher>(), 1f},
                    {ModContent.NPCType<Haymaker>(), Terraria.NPC.CountNPCS(ModContent.NPCType<Haymaker>()) < 2 ? .5f : 0f },
                    {ModContent.NPCType<RoosterBooster>(), Terraria.NPC.CountNPCS(ModContent.NPCType<RoosterBooster>()) < 3 ? .6f : 0f },
                },
                new Dictionary<int, float> { // 5
                    {ModContent.NPCType<ChickenScratcher>(), 1f},
                    {ModContent.NPCType<ChickenBomber>(), Terraria.NPC.CountNPCS(ModContent.NPCType<ChickenBomber>()) < 3 ? .7f : 0f },
                    {ModContent.NPCType<Haymaker>(), Terraria.NPC.CountNPCS(ModContent.NPCType<Haymaker>()) < 2 ? .5f : 0f },
                    {ModContent.NPCType<HeadlessChicken>(), .8f}
                },
                new Dictionary<int, float> { // 6
                    {ModContent.NPCType<ChickenBomber>(), Terraria.NPC.CountNPCS(ModContent.NPCType<ChickenBomber>()) < 3 ? .7f : 0f },
                    {ModContent.NPCType<RoosterBooster>(), Terraria.NPC.CountNPCS(ModContent.NPCType<RoosterBooster>()) < 3 ? .6f : 0f },
                    {ModContent.NPCType<Haymaker>(), Terraria.NPC.CountNPCS(ModContent.NPCType<Haymaker>()) < 3 ? .5f : 0f },
                    {ModContent.NPCType<Cockatrice>(), Terraria.NPC.CountNPCS(ModContent.NPCType<Cockatrice>()) < 1 ? .3f : 0f },
                    {ModContent.NPCType<HeadlessChicken>(), .5f}
                },
                new Dictionary<int, float> { // 7
                    {ModContent.NPCType<RoosterBooster>(), Terraria.NPC.CountNPCS(ModContent.NPCType<RoosterBooster>()) < 4 ? .6f : 0f },
                    {ModContent.NPCType<Haymaker>(), Terraria.NPC.CountNPCS(ModContent.NPCType<Haymaker>()) < 3 ? .4f : 0f },
                    {ModContent.NPCType<HeadlessChicken>(), .6f},
                    {ModContent.NPCType<Cockatrice>(), Terraria.NPC.CountNPCS(ModContent.NPCType<Cockatrice>()) < 2 ? .6f : 0f },
                },
                new Dictionary<int, float> { // 8
                    {ModContent.NPCType<RoosterBooster>(), Terraria.NPC.CountNPCS(ModContent.NPCType<RoosterBooster>()) < 4 ? .6f : 0f },
                    {ModContent.NPCType<Cockatrice>(), Terraria.NPC.CountNPCS(ModContent.NPCType<Cockatrice>()) < 2 ? .2f : 0f },
                    {ModContent.NPCType<Basan>(), Terraria.NPC.CountNPCS(ModContent.NPCType<Basan>()) < 1 ? 10f : 0f },
                },
            };
        }
    }
}
