using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.NPCs.Bosses.PatientZero;
using Redemption.NPCs.Lab;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.MACE;
using Redemption.NPCs.Lab.Volt;
using Redemption.WorldGeneration;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals
{
    public class LabArea : ModSystem
    {
        public static bool[] labAccess = new bool[6];
        public override void PreUpdateWorld()
        {
            bool active = Main.LocalPlayer.InModBiome<LabBiome>();
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Terraria.Player player = Main.player[i];
                    if (!player.active)
                        continue;

                    if (player.InModBiome<LabBiome>())
                        active = true;
                }
            }
            if (!active || RedeGen.LabVector.X == -1)
                return;

            Vector2 ToasterPos = new(((RedeGen.LabVector.X + 84) * 16) + 14, (RedeGen.LabVector.Y + 42) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<JustANormalToaster>()))
                SpawnNPCInWorld(ToasterPos, ModContent.NPCType<JustANormalToaster>());

            Vector2 JanitorPos = new((RedeGen.LabVector.X + 173) * 16, (RedeGen.LabVector.Y + 22) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<JanitorBot_Cleaning>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<JanitorBot>())
                && !RedeBossDowned.downedJanitor)
                SpawnNPCInWorld(JanitorPos, ModContent.NPCType<JanitorBot_Cleaning>());

            Vector2 JanitorNPCPos = new((RedeGen.LabVector.X + 181) * 16, (RedeGen.LabVector.Y + 102) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<JanitorBot_NPC>()) && RedeBossDowned.downedJanitor)
                SpawnNPCInWorld(JanitorNPCPos, ModContent.NPCType<JanitorBot_NPC>());

            Vector2 BehemothPos = new(((RedeGen.LabVector.X + 214) * 16) - 4, (RedeGen.LabVector.Y + 45) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<IrradiatedBehemoth_Inactive>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<IrradiatedBehemoth>()) && !RedeBossDowned.downedBehemoth)
                SpawnNPCInWorld(BehemothPos, ModContent.NPCType<IrradiatedBehemoth_Inactive>());

            Vector2 BlisterfacePos = new(((RedeGen.LabVector.X + 209) * 16) - 4, (RedeGen.LabVector.Y + 191) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<Blisterface_Inactive>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<Blisterface>()) && !RedeBossDowned.downedBlisterface)
                SpawnNPCInWorld(BlisterfacePos, ModContent.NPCType<Blisterface_Inactive>());

            Vector2 VoltPos = new((RedeGen.LabVector.X + 49) * 16, (RedeGen.LabVector.Y + 122) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<ProtectorVolt>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<ProtectorVolt_Start>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<ProtectorVolt_NPC>()))
                SpawnNPCInWorld(VoltPos, ModContent.NPCType<ProtectorVolt_Start>());

            Vector2 CraneOperatorPos = new(((RedeGen.LabVector.X + 107) * 16) + 8, (RedeGen.LabVector.Y + 157) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<CraneOperator>()) && !RedeBossDowned.downedMACE)
                SpawnNPCInWorld(CraneOperatorPos, ModContent.NPCType<CraneOperator>());

            Vector2 MacePos = new(((RedeGen.LabVector.X + 74) * 16) - 8, (RedeGen.LabVector.Y + 169) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<MACEProject_Off>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<MACEProject>()) && !RedeBossDowned.downedMACE)
                SpawnNPCInWorld(MacePos, ModContent.NPCType<MACEProject_Off>());

            Vector2 KariPos = new((RedeGen.LabVector.X + 144) * 16, ((RedeGen.LabVector.Y + 193) * 16) + 1);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<PZ>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<PZ_Inactive>()) && !RedeBossDowned.downedPZ)
                SpawnNPCInWorld(KariPos, ModContent.NPCType<PZ_Inactive>());
        }
        public static void SpawnNPCInWorld(Vector2 pos, int npcType, int ai0 = 0, int ai1 = 0, int ai2 = 0, int ai3 = 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X, (int)pos.Y, npcType, ai0, ai1, ai2, ai3);
            //else if (Main.netMode != NetmodeID.SinglePlayer)
            //    Redemption.WriteToPacket(Redemption.Instance.GetPacket(), (byte)ModMessageType.NPCSpawnFromClient, npcType, pos).Send(-1);
        }
        public override void ClearWorld()
        {
            for (int k = 0; k < labAccess.Length; k++)
                labAccess[k] = false;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            var lists = new List<string>();

            for (int k = 0; k < labAccess.Length; k++)
            {
                if (labAccess[k])
                    lists.Add("LB" + k);
            }
            tag["lists"] = lists;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var lists = tag.GetList<string>("lists");
            for (int k = 0; k < labAccess.Length; k++)
                labAccess[k] = lists.Contains("LB" + k);
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            for (int k = 0; k < labAccess.Length; k++)
                flags[k] = labAccess[k];
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            for (int k = 0; k < labAccess.Length; k++)
                labAccess[k] = flags[k];
        }
    }
}