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
        public static bool downedOmega1;
        public static bool downedOmega2;
        public static bool downedOmega3;
        public static bool downedErhan;
        public static int erhanDeath;
        public static int slayerDeath;
        public static int oblitDeath;
        public static int nebDeath;
        public static int ADDDeath;
        public static bool nukeDropped;
        public static bool downedJanitor;
        public static bool downedBehemoth;
        public static bool downedBlisterface;
        public static bool downedVolt;
        public static bool downedMACE;
        public static bool voltBegin;
        public static bool downedPZ;
        public static bool downedNebuleus;
        public static bool downedADD;
        public static int downedGGBossFirst;
        public static bool downedFowlEmperor;
        public static bool downedFowlMorning;
        public static bool downedTreebark;
        public static bool downedCalavia;

        public override void ClearWorld()
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
            downedOmega1 = false;
            downedOmega2 = false;
            downedOmega3 = false;
            downedErhan = false;
            erhanDeath = 0;
            slayerDeath = 0;
            oblitDeath = 0;
            nebDeath = 0;
            ADDDeath = 0;
            nukeDropped = false;
            downedJanitor = false;
            downedBehemoth = false;
            downedBlisterface = false;
            downedVolt = false;
            downedMACE = false;
            voltBegin = false;
            downedPZ = false;
            downedNebuleus = false;
            downedADD = false;
            downedGGBossFirst = 0;
            downedFowlEmperor = false;
            downedFowlMorning = false;
            downedTreebark = false;
            downedCalavia = false;
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
            if (downedOmega1)
                downed.Add("downedOmega1");
            if (downedOmega2)
                downed.Add("downedOmega2");
            if (downedOmega3)
                downed.Add("downedOmega3");
            if (downedErhan)
                downed.Add("downedErhan");
            if (nukeDropped)
                downed.Add("nukeDropped");
            if (downedJanitor)
                downed.Add("downedJanitor");
            if (downedBehemoth)
                downed.Add("downedBehemoth");
            if (downedBlisterface)
                downed.Add("downedBlisterface");
            if (downedVolt)
                downed.Add("downedVolt");
            if (downedMACE)
                downed.Add("downedMACE");
            if (voltBegin)
                downed.Add("voltBegin");
            if (downedPZ)
                downed.Add("downedPZ");
            if (downedNebuleus)
                downed.Add("downedNebuleus");
            if (downedADD)
                downed.Add("downedADD");
            if (downedFowlEmperor)
                downed.Add("downedFowlEmperor");
            if (downedFowlMorning)
                downed.Add("downedFowlMorning");
            if (downedTreebark)
                downed.Add("downedTreebark");
            if (downedCalavia)
                downed.Add("downedCalavia");

            tag["downed"] = downed;
            tag["erhanDeath"] = erhanDeath;
            tag["slayerDeath"] = slayerDeath;
            tag["oblitDeath"] = oblitDeath;
            tag["nebDeath"] = nebDeath;
            tag["ADDDeath"] = ADDDeath;
            tag["downedGGBossFirst"] = downedGGBossFirst;
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
            downedOmega1 = downed.Contains("downedOmega1");
            downedOmega2 = downed.Contains("downedOmega2");
            downedOmega3 = downed.Contains("downedOmega3");
            downedErhan = downed.Contains("downedErhan");
            erhanDeath = tag.GetInt("erhanDeath");
            slayerDeath = tag.GetInt("slayerDeath");
            oblitDeath = tag.GetInt("oblitDeath");
            nebDeath = tag.GetInt("nebDeath");
            ADDDeath = tag.GetInt("ADDDeath");
            nukeDropped = downed.Contains("nukeDropped");
            downedJanitor = downed.Contains("downedJanitor");
            downedBehemoth = downed.Contains("downedBehemoth");
            downedBlisterface = downed.Contains("downedBlisterface");
            downedVolt = downed.Contains("downedVolt");
            downedMACE = downed.Contains("downedMACE");
            voltBegin = downed.Contains("voltBegin");
            downedPZ = downed.Contains("downedPZ");
            downedNebuleus = downed.Contains("downedNebuleus");
            downedADD = downed.Contains("downedADD");
            downedGGBossFirst = tag.GetInt("downedGGBossFirst");
            downedFowlEmperor = downed.Contains("downedFowlEmperor");
            downedFowlMorning = downed.Contains("downedFowlMorning");
            downedTreebark = downed.Contains("downedTreebark");
            downedCalavia = downed.Contains("downedCalavia");
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
            flags2[0] = foundNewb;
            flags2[1] = downedSlayer;
            flags2[2] = downedOmega1;
            flags2[3] = downedOmega2;
            flags2[4] = downedOmega3;
            flags2[5] = downedErhan;
            flags2[6] = nukeDropped;
            flags2[7] = downedJanitor;
            writer.Write(flags2);
            var flags3 = new BitsByte();
            flags3[0] = downedBehemoth;
            flags3[1] = downedBlisterface;
            flags3[2] = downedVolt;
            flags3[3] = downedMACE;
            flags3[4] = voltBegin;
            flags3[5] = downedPZ;
            flags3[6] = downedNebuleus;
            flags3[7] = downedADD;
            writer.Write(flags3);
            var flags4 = new BitsByte();
            flags4[0] = downedFowlEmperor;
            flags4[1] = downedFowlMorning;
            flags4[2] = downedTreebark;
            flags4[3] = downedCalavia;
            writer.Write(flags4);

            writer.Write(erhanDeath);
            writer.Write(slayerDeath);
            writer.Write(oblitDeath);
            writer.Write(nebDeath);
            writer.Write(ADDDeath);
            writer.Write(downedGGBossFirst);
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
            downedOmega1 = flags2[2];
            downedOmega2 = flags2[3];
            downedOmega3 = flags2[4];
            downedErhan = flags2[5];
            nukeDropped = flags2[6];
            downedJanitor = flags2[7];
            BitsByte flags3 = reader.ReadByte();
            downedBehemoth = flags3[0];
            downedBlisterface = flags3[1];
            downedVolt = flags3[2];
            downedMACE = flags3[3];
            voltBegin = flags3[4];
            downedPZ = flags3[5];
            downedNebuleus = flags3[6];
            downedADD = flags3[7];
            BitsByte flags4 = reader.ReadByte();
            downedFowlEmperor = flags4[0];
            downedFowlMorning = flags4[1];
            downedTreebark = flags4[2];
            downedCalavia = flags4[3];

            erhanDeath = reader.ReadInt32();
            slayerDeath = reader.ReadInt32();
            oblitDeath = reader.ReadInt32();
            nebDeath = reader.ReadInt32();
            ADDDeath = reader.ReadInt32();
            downedGGBossFirst = reader.ReadInt32();
        }
    }
}
