using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.NPCs.Soulless;
using Redemption.Tiles.Tiles;
using Redemption.WorldGeneration;
using ReLogic.Content;
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
        public static bool[] soullessBools = new bool[5];
        public static int[] soullessInts = new int[3];
        public static Rectangle stalkerZone = new(200 * 16, 925 * 16, 377 * 16, 212 * 16);
        public static int keyEventTimer;
        public override void PreUpdateWorld()
        {
            bool active = Main.LocalPlayer.InModBiome<SoullessBiome>();
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Terraria.Player player = Main.player[i];
                    if (!player.active)
                        continue;

                    if (player.InModBiome<SoullessBiome>())
                        active = true;
                }
            }
            if (!active || Main.netMode == NetmodeID.MultiplayerClient)
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
                Rectangle b3 = new(300 * 16, 993 * 16, 40 * 16, 26 * 16);
                if (soullessInts[1] < 4 && player.Hitbox.Intersects(b3))
                {
                    if (soullessInts[1] == 0)
                    {
                        player.AddBuff(ModContent.BuffType<StalkerDebuff>(), 180);
                        soullessInts[1] = 1;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }
                    else if (soullessInts[1] == 3)
                    {
                        SoundStyle s = CustomSounds.SpookyNoise with { Pitch = -.2f };
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(s);

                        player.AddBuff(ModContent.BuffType<StalkerDebuff>(), 1200);
                        soullessInts[1] = 4;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }
                    break;
                }
                if (soullessInts[1] == 4)
                {
                    if (player.position.Y < 970 * 16)
                    {
                        player.velocity.Y = MathHelper.Max(player.velocity.Y, 0);
                        player.velocity.Y += 2;
                    }
                    if (player.Hitbox.Intersects(stalkerZone))
                        player.AddBuff(ModContent.BuffType<StalkerDebuff>(), 30);

                    Rectangle b4 = new(400 * 16, 1093 * 16, 4 * 16, 4 * 16);
                    if (!soullessBools[4] && player.Hitbox.Intersects(b4))
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.EarthBoom with { Volume = .5f }, b4.Center.ToVector2());

                        RedeHelper.SpawnNPC(new EntitySource_WorldGen(), 396 * 16, 1136 * 16, ModContent.NPCType<LostLight>(), 1);
                        RedeHelper.SpawnNPC(new EntitySource_WorldGen(), 309 * 16, 1084 * 16, ModContent.NPCType<LostLight>(), 2);

                        Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(b4.Center.ToVector2()) / 64);
                        for (int x = 400; x < 404; x++)
                        {
                            for (int y = 1097; y < 1104; y++)
                            {
                                if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<ShadestoneTile>())
                                    WorldGen.KillTile(x, y, false, false, true);
                            }
                        }
                        for (int x = 385; x < 395; x++)
                        {
                            for (int y = 1052; y < 1055; y++)
                            {
                                if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<ShadestoneTile>())
                                    WorldGen.KillTile(x, y, false, false, true);
                            }
                        }
                        soullessBools[4] = true;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        break;
                    }
                }
            }
            if (soullessInts[1] == 2)
            {
                if (keyEventTimer++ >= 120)
                {
                    soullessInts[1] = 3;

                    Dictionary<Color, int> colorToTile = new()
                    {
                        [new Color(0, 255, 0)] = ModContent.TileType<ShadestoneTile>(),
                        [new Color(150, 150, 150)] = -2,
                        [Color.Black] = -1
                    };

                    Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/Soulless/StalkerDestroy", AssetRequestMode.ImmediateLoad).Value;
                    Point origin = new(286, 995);
                    GenUtils.InvokeOnMainThread(() =>
                    {
                        TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                        gen.Generate(origin.X, origin.Y, true, true);
                    });
                    Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<StalkerDebuff>(), 180);
                    Main.BlackFadeIn = 100;
                    Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity += 12;
                    SoundStyle s = CustomSounds.ElevatorImpact with { Volume = 0.3f, Pitch = -.5f };
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(s);

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
            }

            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<ShadestoneLift>()))
            {
                Vector2 LiftPos = new(608 * 16, (822 * 16) + 8);
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos.X, (int)LiftPos.Y, ModContent.NPCType<ShadestoneLift>(), 0, 0, 0, 863, 821);
                Vector2 LiftPos2 = new(334 * 16, (763 * 16) + 8);
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos2.X, (int)LiftPos2.Y, ModContent.NPCType<ShadestoneLift>(), 0, 0, 0, 787, 762);
            }
            if (!soullessBools[2] && !Terraria.NPC.AnyNPCs(ModContent.NPCType<ShadestoneLift2>()))
            {
                Vector2 LiftPos3 = new(510 * 16, (863 * 16) + 8);
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos3.X, (int)LiftPos3.Y, ModContent.NPCType<ShadestoneLift2>(), 0, 0, 0, 1026, 862);
            }
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<RuhRoh>()))
            {
                Vector2 GMaskPos = new(573 * 16, 1036 * 16);
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)GMaskPos.X, (int)GMaskPos.Y, ModContent.NPCType<RuhRoh>(), 0, 10);
            }
            if (soullessInts[1] <= 1 && !Terraria.NPC.AnyNPCs(ModContent.NPCType<TheStalker>()))
            {
                Vector2 StalkerPos = new(316 * 16, 1013 * 16);
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)StalkerPos.X, (int)StalkerPos.Y, ModContent.NPCType<TheStalker>());
            }
            if (soullessInts[1] == 4 && !Terraria.NPC.AnyNPCs(ModContent.NPCType<TheStalker>()))
            {
                Vector2 StalkerPos = new(466 * 16, 1102 * 16);
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)StalkerPos.X, (int)StalkerPos.Y, ModContent.NPCType<TheStalker>(), 0, 1);
            }
            if (soullessInts[1] <= 1 && !Terraria.NPC.AnyNPCs(ModContent.NPCType<SpookyEyes2>()))
            {
                for (int i = 0; i < 30; i++)
                {
                    Vector2 eyesPos = RedeHelper.RandomPosition(new Vector2(312, 1074), new Vector2(332, 1083)) * 16;
                    Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)eyesPos.X, (int)eyesPos.Y, ModContent.NPCType<SpookyEyes2>());
                }
            }
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