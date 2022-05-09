using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
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
        public static bool[] soullessBools = new bool[3];
        public static int[] soullessInts = new int[3];
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
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/EarthBoom2").WithVolume(0.5f), b1.Center.ToVector2());

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