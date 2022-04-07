using Microsoft.Xna.Framework;
using Redemption.NPCs.Soulless;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals
{
    public class SoullessArea : ModSystem
    {
        public static bool Active;
        public static bool[] soullessBools = new bool[1];
        public static int[] soullessInts = new int[1];
        public override void PreUpdateEntities()
        {
            Active = false;
        }
        public override void PreUpdateWorld()
        {
            if (!Active || Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 LiftPos = new(608 * 16, (822 * 16) + 8);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<ShadestoneLift>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos.X, (int)LiftPos.Y, ModContent.NPCType<ShadestoneLift>());
        }
        public override void OnWorldLoad()
        {
            for (int k = 0; k < soullessBools.Length; k++)
                soullessBools[k] = false;
            for (int k = 0; k < soullessInts.Length; k++)
                soullessInts[k] = 0;
        }
        public override void OnWorldUnload()
        {
            for (int k = 0; k < soullessBools.Length; k++)
                soullessBools[k] = false;
            for (int k = 0; k < soullessInts.Length; k++)
                soullessInts[k] = 0;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            var lists = new List<string>();

            for (int k = 0; k < soullessBools.Length; k++)
            {
                if (soullessBools[k])
                    lists.Add("SBool" + k);
            }
            tag["lists"] = lists;
            for (int k = 0; k < soullessInts.Length; k++)
                tag["SInt" + k] = soullessInts[k];
        }
        public override void LoadWorldData(TagCompound tag)
        {
            var lists = tag.GetList<string>("lists");
            for (int k = 0; k < soullessBools.Length; k++)
                soullessBools[k] = lists.Contains("SBool" + k);
            for (int k = 0; k < soullessInts.Length; k++)
                soullessInts[k] = tag.GetInt("SInt" + k);
        }
        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            for (int k = 0; k < soullessBools.Length; k++)
                flags[k] = soullessBools[k];
            writer.Write(flags);

            for (int k = 0; k < soullessInts.Length; k++)
                writer.Write(soullessInts[k]);
        }
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            for (int k = 0; k < soullessBools.Length; k++)
                soullessBools[k] = flags[k];

            for (int k = 0; k < soullessInts.Length; k++)
                soullessInts[k] = reader.ReadInt32();
        }
    }
}