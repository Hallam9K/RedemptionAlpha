using System.Collections;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals
{
    public class RedeBossDowned : ModSystem
    {
        public static bool downedThorn;
        public static bool downedKeeper;
        public static bool downedSkullDigger;
        public static bool downedSeed;
        public static bool keeperSaved;
        public static bool skullDiggerSaved;
        public static bool downedSkeletonInvasion;
        public static bool downedEaglecrestGolem;
        public static bool foundNewb;
        public static bool downedSlayer;
        public static bool downedVlitch1;
        public static bool downedVlitch2;
        public static bool downedVlitch3;
        public static bool downedErhan;
        public static int erhanDeath;
        public static int slayerDeath;
        public static bool nukeDropped;

        public override void OnWorldLoad()
        {
            downedThorn = false;
            downedKeeper = false;
            downedSkullDigger = false;
            downedSeed = false;
            keeperSaved = false;
            skullDiggerSaved = false;
            downedSkeletonInvasion = false;
            downedEaglecrestGolem = false;
            foundNewb = false;
            downedSlayer = false;
            downedVlitch1 = false;
            downedVlitch2 = false;
            downedVlitch3 = false;
            downedErhan = false;
            erhanDeath = 0;
            slayerDeath = 0;
            nukeDropped = false;
            //downedOtherBoss = false;
        }

        public override void OnWorldUnload()
        {
            downedThorn = false;
            downedKeeper = false;
            downedSkullDigger = false;
            downedSeed = false;
            keeperSaved = false;
            skullDiggerSaved = false;
            downedSkeletonInvasion = false;
            downedEaglecrestGolem = false;
            foundNewb = false;
            downedSlayer = false;
            downedVlitch1 = false;
            downedVlitch2 = false;
            downedVlitch3 = false;
            downedErhan = false;
            erhanDeath = 0;
            slayerDeath = 0;
            nukeDropped = false;
            //downedOtherBoss = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            var downed = new List<string>();

            if (downedThorn)
                downed.Add("downedThorn");
            if (downedKeeper)
                downed.Add("downedKeeper");
            if (downedSkullDigger)
                downed.Add("downedSkullDigger");
            if (downedSeed)
                downed.Add("downedSeed");
            if (keeperSaved)
                downed.Add("keeperSaved");
            if (skullDiggerSaved)
                downed.Add("skullDiggerSaved");
            if (downedSkeletonInvasion)
                downed.Add("downedSkeletonInvasion");
            if (downedEaglecrestGolem)
                downed.Add("downedEaglecrestGolem");
            if (foundNewb)
                downed.Add("foundNewb");
            if (downedSlayer)
                downed.Add("downedSlayer");
            if (downedVlitch1)
                downed.Add("downedVlitch1");
            if (downedVlitch2)
                downed.Add("downedVlitch2");
            if (downedVlitch3)
                downed.Add("downedVlitch3");
            if (downedErhan)
                downed.Add("downedErhan");
            if (nukeDropped)
                downed.Add("nukeDropped");

            tag["downed"] = downed;
            tag["erhanDeath"] = erhanDeath;
            tag["slayerDeath"] = slayerDeath;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");

            downedThorn = downed.Contains("downedThorn");
            downedKeeper = downed.Contains("downedKeeper");
            downedSkullDigger = downed.Contains("downedSkullDigger");
            downedSeed = downed.Contains("downedSeed");
            keeperSaved = downed.Contains("keeperSaved");
            skullDiggerSaved = downed.Contains("skullDiggerSaved");
            downedSkeletonInvasion = downed.Contains("downedSkeletonInvasion");
            downedEaglecrestGolem = downed.Contains("downedEaglecrestGolem");
            foundNewb = downed.Contains("foundNewb");
            downedSlayer = downed.Contains("downedSlayer");
            downedVlitch3 = downed.Contains("downedVlitch1");
            downedVlitch3 = downed.Contains("downedVlitch2");
            downedVlitch3 = downed.Contains("downedVlitch3");
            downedErhan = downed.Contains("downedErhan");
            erhanDeath = tag.GetInt("erhanDeath");
            slayerDeath = tag.GetInt("slayerDeath");
            nukeDropped = downed.Contains("nukeDropped");
            //downedOtherBoss = downed.Contains("downedOtherBoss");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedThorn;
            flags[1] = downedKeeper;
            flags[2] = downedSkullDigger;
            flags[3] = downedSeed;
            flags[4] = keeperSaved;
            flags[5] = skullDiggerSaved;
            flags[6] = downedSkeletonInvasion;
            flags[7] = downedEaglecrestGolem;
            writer.Write(flags);
            var flags2 = new BitsByte();
            flags[0] = foundNewb;
            flags[1] = downedSlayer;
            flags[2] = downedVlitch1;
            flags[3] = downedVlitch2;
            flags[4] = downedVlitch3;
            flags[5] = downedErhan;
            flags[6] = nukeDropped;
            writer.Write(flags2);

            writer.Write(erhanDeath);
            writer.Write(slayerDeath);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedThorn = flags[0];
            downedKeeper = flags[1];
            downedSkullDigger = flags[2];
            downedSeed = flags[3];
            keeperSaved = flags[4];
            skullDiggerSaved = flags[5];
            downedSkeletonInvasion = flags[6];
            downedEaglecrestGolem = flags[7];
            BitsByte flags2 = reader.ReadByte();
            foundNewb = flags2[0];
            downedSlayer = flags2[1];
            downedVlitch1 = flags2[2];
            downedVlitch2 = flags2[3];
            downedVlitch3 = flags2[4];
            downedErhan = flags2[5];
            nukeDropped = flags2[6];

            erhanDeath = reader.ReadInt32();
            slayerDeath = reader.ReadInt32();
        }
    }
}