using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.DataStructures;
using Redemption.Globals.NPC;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Redemption.Items.Usable;
using Redemption.Items.Lore;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using System;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACEProject : ModNPC
    {
        public enum ActionState
        {
            Begin,
            JawPhase1
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];
        public ref float TimerRand2 => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("MACE Project");
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom
                }
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 92;
            NPC.height = 164;
            NPC.damage = 100;
            NPC.lifeMax = 125000;
            NPC.knockBackResist = 0;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 25, 0, 0);
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.lavaImmune = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.GetGlobalNPC<GuardNPC>().GuardPoints = NPC.lifeMax / 4;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusicMP");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 20, default, 2f);
                    Main.dust[dustIndex].velocity *= 8f;
                }
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreMetalPart").Type);
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreMetalScrap").Type);
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGorePaintScrap").Type);
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(Main.rand.Next(0, NPC.height))), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreWinglet").Type);
                Gore.NewGore(new Vector2(NPC.Center.X, NPC.Center.Y - 16), NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreForeheadGem").Type);
                Gore.NewGore(NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Redemption/MACEGoreHead").Type);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<LabPlatingDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[4])
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel5>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedMACE, -1);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk6>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloppyDisk6_1>()));
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }

        private Vector2 JawCenter;

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            DespawnHandler();

            if (!player.active || player.dead)
                return;

            Vector2 MouthOrigin = new(NPC.Center.X, NPC.Center.Y + 62);
            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        if (!Main.dedServ)
                        {
                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle("MACE Project", 60, 90, 0.8f, 0, Color.Yellow, "Incomplete War Machine"); SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/SpookyNoise"), NPC.position);
                        }
                        GlowActive = true;
                    }
                    if (AITimer >= 80)
                    {
                        AITimer = 0;
                        JawOpen = true;
                        AIState = ActionState.JawPhase1;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.JawPhase1:
                    switch (TimerRand2)
                    {
                        case 0:
                            AITimer++;
                            if (!Main.dedServ)
                            {
                                if (AITimer == 1)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/DistortedRoar").WithVolume(.5f), NPC.position);
                                if (AITimer < 60)
                                {
                                    TimerRand += (float)Math.PI / 120;
                                    if (TimerRand >= Math.PI) TimerRand = 0;
                                    float timer = TimerRand;
                                    Terraria.Graphics.Effects.Filters.Scene.Activate("MoR:Shockwave", NPC.Center)?.GetShader().UseProgress(timer).UseOpacity(100f * (1 - timer / 2f)).UseColor(1, 1, 6).UseTargetPosition(MouthOrigin);
                                }
                                else
                                    Terraria.Graphics.Effects.Filters.Scene["MoR:Shockwave"].Deactivate();
                            }
                            if (AITimer == 35)
                            {
                                for (int i = 0; i < 20; i++)
                                {
                                    int dustIndex = Dust.NewDust(MouthOrigin, 2, 2, DustID.OrangeTorch, Scale: 2f);
                                    Main.dust[dustIndex].velocity *= 10f;
                                }
                                for (int i = 0; i < 5; i++)
                                    NPC.Shoot(MouthOrigin, ModContent.ProjectileType<MACE_Miniblast>(), NPC.damage, RedeHelper.PolarVector(8, MathHelper.ToRadians(72) * i), false, SoundID.Item73);
                            }
                            if (AITimer == 40)
                            {
                                for (int i = 0; i < 20; i++)
                                {
                                    int dustIndex = Dust.NewDust(MouthOrigin, 2, 2, DustID.OrangeTorch, Scale: 2f);
                                    Main.dust[dustIndex].velocity *= 10f;
                                }
                                for (int i = 0; i < 10; i++)
                                    NPC.Shoot(MouthOrigin, ModContent.ProjectileType<MACE_Miniblast>(), NPC.damage, RedeHelper.PolarVector(8, MathHelper.ToRadians(36) * i), false, SoundID.Item73);
                            }
                            if (AITimer == 80)
                                JawOpen = false;
                            if (AITimer > 160)
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            if (AITimer++ == 0)
                                JawOpen = true;
                            if (AITimer == 50 || AITimer == 70 || AITimer == 90)
                            {
                                NPC.Shoot(MouthOrigin, ModContent.ProjectileType<BigElectronade>(), NPC.damage, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), false, SoundID.Item61);
                            }
                            if (AITimer == 100)
                                JawOpen = false;
                            if (AITimer > 160)
                            {
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2++;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
            }
        }
        private bool GlowActive;
        private int GlowTimer;
        private bool JawOpen;
        public override void FindFrame(int frameHeight)
        {
            if (GlowActive)
            {
                if (GlowTimer++ > 60)
                {
                    GlowActive = false;
                    GlowTimer = 0;
                }
            }
            if (JawOpen)
            {
                if (JawCenter.Y < 12)
                    JawCenter.Y += 1f;
            }
            else
            {
                if (JawCenter.Y >= 0)
                    JawCenter.Y -= 1f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D trolleyTex = ModContent.Request<Texture2D>("Redemption/NPCs/Lab/MACE/CraneTrolley").Value;
            Texture2D jawTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Jaw").Value;
            Vector2 drawCenter = new(NPC.Center.X, NPC.Center.Y - 18);

            Vector2 drawCenterTrolley = new(drawCenter.X, drawCenter.Y + 10);
            Rectangle rect = new(0, 0, trolleyTex.Width, trolleyTex.Height);
            Main.spriteBatch.Draw(trolleyTex, drawCenterTrolley - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(trolleyTex.Width / 2f, trolleyTex.Height / 2f), NPC.scale, SpriteEffects.None, 0);

            Vector2 drawCenterJaw = new(drawCenter.X - 1, drawCenter.Y + 60);
            Rectangle rect2 = new(0, 0, jawTex.Width, jawTex.Height);
            Main.spriteBatch.Draw(jawTex, drawCenterJaw + JawCenter - screenPos, new Rectangle?(rect2), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(jawTex.Width / 2f, jawTex.Height / 2f), NPC.scale, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, drawCenter - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(glowTex, drawCenter - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/MACEEyeFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = NPC.Center - screenPos + new Vector2(-22, 4);
            Color colour = Color.Lerp(Color.DarkGreen, Color.White, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            if (GlowActive)
            {
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour, NPC.rotation, origin, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour * 0.4f, NPC.rotation, origin, 2f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.alpha += 5;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
    }
}