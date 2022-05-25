using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.NPCs.Soulless;
using Redemption.Tiles.Tiles;
using SubworldLibrary;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals
{
    public class SoullessArea : ModSystem
    {
        public static bool Active;
        public static bool[] soullessBools = new bool[4];
        public static int[] soullessInts = new int[3];
        public static Rectangle stalkerZone = new(200 * 16, 925 * 16, 410 * 16, 150 * 16);
        public override void PreUpdateEntities()
        {
            Active = false;
        }
        public override void PreUpdateWorld()
        {
            if (!Active || Main.netMode == NetmodeID.MultiplayerClient)
                return;

            for (int n = 0; n < 255; n++)
            {
                Terraria.Player player = Main.player[n];
                if (!player.active || player.dead)
                    continue;

                Rectangle b1 = new(275 * 16, 832 * 16, 5 * 16, 10 * 16);
                if (!soullessBools[1] && player.Hitbox.Intersects(b1))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.EarthBoom with { Volume = .5f }, b1.Center.ToVector2());

                    Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(b1.Center.ToVector2()) / 64);
                    for (int x = 275; x < 280; x++)
                    {
                        for (int y = 836; y < 841; y++)
                        {
                            if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<ShadestoneTile>())
                                WorldGen.KillTile(x, y, false, false, true);
                        }
                    }
                    soullessBools[1] = true;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                Rectangle b2 = new(347 * 16, 988 * 16, 7 * 16, 7 * 16);
                if (!soullessBools[3] && player.Hitbox.Intersects(b2))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.EarthBoom with { Volume = .5f }, b2.Center.ToVector2());

                    Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(b2.Center.ToVector2()) / 64);
                    for (int x = 347; x < 354; x++)
                    {
                        for (int y = 991; y < 999; y++)
                        {
                            if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<ShadestoneTile>())
                                WorldGen.KillTile(x, y, false, false, true);
                        }
                    }
                    soullessBools[3] = true;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                Rectangle b3 = new(295 * 16, 993 * 16, 45 * 16, 26 * 16);
                if (soullessInts[1] == 0 && player.Hitbox.Intersects(b3))
                {
                    player.AddBuff(ModContent.BuffType<StalkerDebuff>(), 180);
                    soullessInts[1] = 1;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                    break;
                }
            }

            Vector2 LiftPos = new(608 * 16, (822 * 16) + 8);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<ShadestoneLift2>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos.X, (int)LiftPos.Y, ModContent.NPCType<ShadestoneLift2>(), 0, 0, 0, 863, 821);
            Vector2 LiftPos2 = new(334 * 16, (763 * 16) + 8);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<ShadestoneLift>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos2.X, (int)LiftPos2.Y, ModContent.NPCType<ShadestoneLift>(), 0, 0, 0, 787, 762);
            Vector2 LiftPos3 = new(510 * 16, (863 * 16) + 8);
            if (!soullessBools[2] && !Terraria.NPC.AnyNPCs(ModContent.NPCType<ShadestoneLift3>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos3.X, (int)LiftPos3.Y, ModContent.NPCType<ShadestoneLift3>(), 0, 0, 0, 1026, 862);
            Vector2 GMaskPos = new(573 * 16, 1036 * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<RuhRoh>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)GMaskPos.X, (int)GMaskPos.Y, ModContent.NPCType<RuhRoh>(), 0, 10);
            Vector2 StalkerPos = new(316 * 16, 1013 * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<TheStalker>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)StalkerPos.X, (int)StalkerPos.Y, ModContent.NPCType<TheStalker>());
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