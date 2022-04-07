using Microsoft.Xna.Framework;
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
        public static bool Active;
        public static bool[] labAccess = new bool[6];
        public override void PreUpdateEntities()
        {
            Active = false;
        }
        public override void PreUpdateWorld()
        {
            if (!Active || RedeGen.LabVector.X == -1 || RedeGen.LabVector.Y == -1 || Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 ToasterPos = new(((RedeGen.LabVector.X + 84) * 16) + 14, (RedeGen.LabVector.Y + 42) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<JustANormalToaster>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)ToasterPos.X, (int)ToasterPos.Y, ModContent.NPCType<JustANormalToaster>());

            Vector2 JanitorPos = new((RedeGen.LabVector.X + 173) * 16, (RedeGen.LabVector.Y + 22) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<JanitorBot_Cleaning>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<JanitorBot>())
                && !RedeBossDowned.downedJanitor)
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)JanitorPos.X, (int)JanitorPos.Y, ModContent.NPCType<JanitorBot_Cleaning>());

            Vector2 JanitorNPCPos = new((RedeGen.LabVector.X + 181) * 16, (RedeGen.LabVector.Y + 102) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<JanitorBot_NPC>()) && RedeBossDowned.downedJanitor)
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)JanitorNPCPos.X, (int)JanitorNPCPos.Y, ModContent.NPCType<JanitorBot_NPC>());

            Vector2 BehemothPos = new(((RedeGen.LabVector.X + 214) * 16) - 4, (RedeGen.LabVector.Y + 45) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<IrradiatedBehemoth_Inactive>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<IrradiatedBehemoth>()) && !RedeBossDowned.downedBehemoth)
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)BehemothPos.X, (int)BehemothPos.Y, ModContent.NPCType<IrradiatedBehemoth_Inactive>());

            Vector2 BlisterfacePos = new(((RedeGen.LabVector.X + 209) * 16) - 4, (RedeGen.LabVector.Y + 191) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<Blisterface_Inactive>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<Blisterface>()) && !RedeBossDowned.downedBlisterface)
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)BlisterfacePos.X, (int)BlisterfacePos.Y, ModContent.NPCType<Blisterface_Inactive>());

            Vector2 VoltPos = new((RedeGen.LabVector.X + 49) * 16, (RedeGen.LabVector.Y + 122) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<ProtectorVolt>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<ProtectorVolt_Start>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<ProtectorVolt_NPC>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)VoltPos.X, (int)VoltPos.Y, ModContent.NPCType<ProtectorVolt_Start>());

            Vector2 CraneOperatorPos = new(((RedeGen.LabVector.X + 107) * 16) + 8, (RedeGen.LabVector.Y + 157) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<CraneOperator>()) && !RedeBossDowned.downedMACE)
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)CraneOperatorPos.X, (int)CraneOperatorPos.Y, ModContent.NPCType<CraneOperator>());

            Vector2 MacePos = new(((RedeGen.LabVector.X + 74) * 16) - 8, (RedeGen.LabVector.Y + 169) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<MACEProject_Off>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<MACEProject>()) && !RedeBossDowned.downedMACE)
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)MacePos.X, (int)MacePos.Y, ModContent.NPCType<MACEProject_Off>());

            Vector2 KariPos = new((RedeGen.LabVector.X + 144) * 16, ((RedeGen.LabVector.Y + 193) * 16) + 1);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<PZ>()) && !Terraria.NPC.AnyNPCs(ModContent.NPCType<PZ_Inactive>()) && !RedeBossDowned.downedPZ)
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)KariPos.X, (int)KariPos.Y, ModContent.NPCType<PZ_Inactive>());
        }
        public override void OnWorldLoad()
        {
            for (int k = 0; k < labAccess.Length; k++)
                labAccess[k] = false;
        }
        public override void OnWorldUnload()
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