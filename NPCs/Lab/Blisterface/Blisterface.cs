using Microsoft.Xna.Framework.Graphics;
using MonoMod.Core.Platforms;
using Redemption.Base;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using Redemption.Helpers;
using Redemption.Items.Usable;
using Redemption.WorldGeneration;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Blisterface
{
    [AutoloadBossHead]
    public class Blisterface : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Velocity = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCWater[Type] = true;
            ElementID.NPCPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 96;
            NPC.height = 64;
            NPC.friendly = false;
            NPC.damage = 60;
            NPC.defense = 30;
            NPC.lifeMax = 24000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.knockBackResist = 0.0f;
            NPC.SpawnWithHigherTime(30);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.BossBar = GetInstance<NoHeadHealthBar>();
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic");
            SpawnModBiomes = new int[1] { GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.Bestiary.Blisterface1")),
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.Bestiary.Blisterface2"))
            });
        }
        public override bool CheckActive()
        {
            return !Main.LocalPlayer.InModBiome<LabBiome>();
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            Point water = NPC.Center.ToTileCoordinates();
            if (Main.tile[water.X, water.Y].LiquidType == LiquidID.Water && Main.tile[water.X, water.Y].LiquidAmount > 0)
                modifiers.FinalDamage /= 10;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, Scale: 1.5f);
                    Main.dust[dustIndex].velocity *= 2f;
                }
            }
        }
        public override bool PreKill()
        {
            if (!RedeBossDowned.downedBlisterface)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<Blisterface_GirusTalk>(), 0, 0, Main.myPlayer);
            return true;
        }
        public override void OnKill()
        {
            Player player = Main.LocalPlayer;
            if (!LabArea.labAccess[2])
                Item.NewItem(NPC.GetSource_Loot(), (int)player.position.X, (int)player.position.Y, player.width, player.height, ItemType<ZoneAccessPanel3>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedBlisterface, -1);
        }
        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AITimer[0]);
            writer.Write(AITimer[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AITimer[0] = reader.ReadInt32();
            AITimer[1] = reader.ReadInt32();
        }

        static float ArenaLeft => (RedeGen.LabVector.X + 189) * 16;
        static float ArenaRight => (RedeGen.LabVector.X + 223) * 16;
        static float ArenaTop => (RedeGen.LabVector.Y + 158) * 16;
        static float ArenaWidth => ArenaRight - ArenaLeft;
        static float ArenaCenterX => ArenaLeft + (ArenaWidth / 2);
        static float ArenaHigh => (RedeGen.LabVector.Y + 170) * 16;
        static float ArenaWaterY => (RedeGen.LabVector.Y + 190) * 16;
        static Vector2 ArenaPos => new(ArenaLeft, ArenaTop);

        Vector2 oldPos;

        private readonly int[] AITimer = new int[2];
        public override void PostAI()
        {
            NPC.LookByVelocity();
            if (AITimer[0] < 1)
            {
                if (NPC.Center.Y < (RedeGen.LabVector.Y + 186) * 16)
                    NPC.velocity.Y += 0.1f;
                if (NPC.Center.Y > (RedeGen.LabVector.Y + 191) * 16)
                    NPC.velocity.Y -= 0.1f;
            }
            if (NPC.Center.X < ArenaLeft)
                NPC.velocity.X += 1f;
            if (NPC.Center.X > ArenaRight)
                NPC.velocity.X -= 1f;

            if (NPC.Center.Y > (RedeGen.LabVector.Y + 196) * 16)
            {
                if (AITimer[0] > 0)
                    AITimer[0] = 0;
                NPC.velocity.Y = -3f;
            }
            switch (AITimer[0])
            {
                case 0:
                    if (NPC.DespawnHandler(1, 5))
                        return;

                    AITimer[1]++;
                    int jump = NPC.life > NPC.lifeMax / 2 ? 320 : 170;
                    if (AITimer[1] == jump - 40)
                        GlowActive = true;
                    if (AITimer[1] >= jump)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Main.rand.NextBool(3))
                            {
                                NPC.localAI[0] = NPC.rotation;
                                AITimer[0] = 2;
                                NPC.ai[0] = 0;
                                NPC.ai[1] = 0;
                            }
                            else
                            {
                                AITimer[0] = Main.rand.NextBool() ? 5 : 1;
                            }
                        }
                        AITimer[1] = 0;
                        NPC.netUpdate = true;
                    }
                    NPC.noTileCollide = false;
                    if (Main.rand.NextBool(20))
                    {
                        NPC.Shoot(new Vector2(NPC.position.X + Main.rand.Next(0, NPC.width), NPC.position.Y + Main.rand.Next(0, NPC.height)), ProjectileType<Blisterface_Bubble>(), NPC.damage, Vector2.Zero, SoundID.Item111);
                    }
                    if (NPC.CountNPCS(NPCType<BlisteredFish2>()) <= 3)
                    {
                        if (Main.rand.NextBool(250))
                            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<BlisteredFish2>());
                    }
                    break;
                case 1:
                    NPC.noTileCollide = true;
                    NPC.velocity.X = 0;
                    if (AITimer[1]++ == 0)
                    {
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = 15 * -1;
                    }
                    if (AITimer[1] >= 50 && AITimer[1] < 70)
                    {
                        if (AITimer[1] % 2 == 0)
                        {
                            NPC.Shoot(new Vector2(NPC.Center.X + 12f * NPC.spriteDirection, NPC.Center.Y), ProjectileType<Blisterface_Bubble>(), NPC.damage, new Vector2(Main.rand.Next(6, 13) * NPC.spriteDirection, Main.rand.Next(-2, 3)), SoundID.NPCDeath13, 0, 1);
                        }
                    }
                    if (AITimer[1] >= 68)
                    {
                        NPC.velocity.Y += 0.15f;
                    }
                    Point water = NPC.Center.ToTileCoordinates();
                    if (AITimer[1] >= 180 && Main.tile[water.X, water.Y].LiquidType == LiquidID.Water && Main.tile[water.X, water.Y].LiquidAmount > 0)
                    {
                        AITimer[1] = 0;
                        AITimer[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                // Pipes Slam
                case 2:
                    if (NPC.Center.X < ArenaCenterX)
                        NPC.direction = 1;
                    else
                        NPC.direction = -1;
                    NPC.spriteDirection = NPC.direction;

                    NPC.rotation = MathHelper.Lerp(NPC.localAI[0], (-MathHelper.PiOver2 - 0.1f) * NPC.spriteDirection, NPC.ai[0] / 90f);
                    NPC.ai[0]++;
                    NPC.velocity *= 0.98f;

                    if (NPC.ai[0] == 50)
                        GlowActive = true;
                    if (NPC.ai[0] >= 90)
                    {
                        NPC.noTileCollide = true;
                        NPC.velocity.Y = -16;
                        AITimer[0] = 3;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case 3:
                    if (NPC.Center.Y < ArenaHigh)
                    {
                        if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        {
                            NPC.velocity *= 0;
                            if (!Main.dedServ)
                            {
                                SoundEngine.PlaySound(CustomSounds.MetalHit, NPC.position);
                                SoundEngine.PlaySound(SoundID.Shatter.WithVolumeScale(2f).WithPitchOffset(-1f), NPC.position);
                                SoundEngine.PlaySound(CustomSounds.Quake.WithPitchOffset(-0.5f), NPC.position);
                            }

                            PunchCameraModifier camPunch = new(NPC.Center, new Vector2(0, -1f * RedeConfigClient.Instance.ShakeIntensity), 20f, 3f, 80, 1600f, "Blisterface");
                            Main.instance.CameraModifiers.Add(camPunch);

                            for (int i = 0; i < 20; i++)
                            {
                                int d = Dust.NewDust(NPC.position, NPC.width, 2, DustID.Glass, 0, -NPC.oldVelocity.Y * .7f, Scale: 1);
                                Main.dust[d].fadeIn = 120;
                            }

                            for (int i = 0; i < 100; i++)
                            {
                                int d = Dust.NewDust(ArenaPos, (int)ArenaWidth, 9 * 16, DustID.Glass, 0, 0, Scale: Main.rand.NextFloat(1, 1.2f));
                                Main.dust[d].velocity.X = Main.rand.NextFloat(-1, 1);
                                Main.dust[d].velocity.Y = Main.rand.NextFloat(0, 4);
                                Main.dust[d].fadeIn = 120;
                                d = Dust.NewDust(ArenaPos, (int)ArenaWidth, 9 * 16, DustType<SludgeDust>(), 0, 0, Scale: Main.rand.NextFloat(1, 2));
                                Main.dust[d].velocity.X = Main.rand.NextFloat(-1, 1);
                                Main.dust[d].velocity.Y = Main.rand.NextFloat(0, 4);
                                Main.dust[d].fadeIn = 120;
                            }

                            for (int i = 0; i < 16; i++)
                            {
                                Vector2 pos = ArenaPos + new Vector2(Main.rand.Next(12, (int)ArenaWidth - 12), 4 * 16);
                                NPC.Shoot(pos, ProjectileType<Blisterface_Drip_Proj>(), NPC.damage, Vector2.Zero, 0, Main.rand.Next(80, 180), ai2: NPC.whoAmI);
                            }

                            if (NPC.Center.X < ArenaCenterX)
                                NPC.direction = 1;
                            else
                                NPC.direction = -1;
                            NPC.spriteDirection = NPC.direction;

                            oldPos = NPC.position;
                            AITimer[0] = 4;
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                        }
                    }
                    else
                    {
                        if (NPC.ai[1] == 0 && !Collision.WetCollision(NPC.position, NPC.width, NPC.height))
                        {
                            for (int i = 0; i < 50; i++)
                            {
                                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustType<WastelandWaterSplash>(),
                                    NPC.velocity.X * .6f, -10, Scale: Main.rand.NextFloat(1f, 2f));
                                Main.dust[d].fadeIn = 300;
                            }

                            SoundEngine.PlaySound(SoundID.Splash.WithVolumeScale(2f).WithPitchOffset(-0.2f), NPC.position);
                            NPC.ai[1] = 1;
                        }

                        if (MathHelper.Distance(NPC.Center.X, ArenaCenterX) > 40)
                            NPC.velocity.X = NPC.Center.X < ArenaCenterX ? 1 : -1;
                        NPC.velocity.Y = -16;
                    }
                    break;
                case 4:
                    if (oldPos == Vector2.Zero)
                    {
                        oldPos = NPC.position;
                        break;
                    }

                    NPC.ai[0]++;

                    NPC.rotation += 0.2f * NPC.spriteDirection;
                    NPC.position.X = MathHelper.Lerp(oldPos.X, ArenaCenterX, EaseFunction.EaseCubicOut.Ease(NPC.ai[0] / 80f));
                    if (NPC.ai[0] <= 30)
                        NPC.position.Y = MathHelper.Lerp(oldPos.Y, oldPos.Y - 150, EaseFunction.EaseCubicOut.Ease(NPC.ai[0] / 30f));
                    else
                        NPC.position.Y = MathHelper.Lerp(oldPos.Y - 150, ArenaWaterY - 32, EaseFunction.EaseQuadIn.Ease((NPC.ai[0] - 30) / 50f));

                    if (NPC.ai[1] == 0 && Collision.WetCollision(NPC.position, NPC.width, NPC.height))
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustType<WastelandWaterSplash>(),
                                NPC.velocity.X * .6f, -10, Scale: Main.rand.NextFloat(1f, 2f));
                            Main.dust[d].fadeIn = 300;
                        }

                        SoundEngine.PlaySound(SoundID.Splash.WithVolumeScale(2f).WithPitchOffset(-0.5f), NPC.position);
                        NPC.ai[1] = 1;
                    }

                    if (NPC.ai[0] >= 80)
                    {
                        AITimer[1] = 0;
                        AITimer[0] = 0;
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                // Flop Edition
                case 5:
                    NPC.noTileCollide = true;
                    NPC.velocity.X = 0;
                    if (AITimer[1]++ == 0)
                    {
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = 15 * -1;
                    }
                    if (AITimer[1] >= 40)
                    {
                        NPC.noTileCollide = false;
                        NPC.velocity.Y += 0.15f;
                    }
                    if (AITimer[1] >= 120 || NPC.collideY)
                    {
                        AITimer[1] = 0;
                        AITimer[0] = 6;
                        NPC.netUpdate = true;
                    }
                    break;
                case 6:
                    if (AITimer[1]++ < 200)
                    {
                        if (AITimer[1] % 2 == 0 && Main.rand.NextBool(4))
                        {
                            NPC.Shoot(new Vector2(NPC.Center.X + 12f * NPC.spriteDirection, NPC.Center.Y), ProjectileType<Blisterface_Bubble>(), NPC.damage, new Vector2(Main.rand.Next(6, 13) * NPC.spriteDirection, Main.rand.Next(-2, 3)), SoundID.NPCDeath13, 0, 1);
                        }
                    }
                    if (AITimer[1] >= 240)
                    {
                        NPC.noTileCollide = true;
                        NPC.velocity.Y += 0.25f;
                    }
                    water = NPC.Center.ToTileCoordinates();
                    if (AITimer[1] >= 250 && Main.tile[water.X, water.Y].LiquidType == LiquidID.Water && Main.tile[water.X, water.Y].LiquidAmount > 0)
                    {
                        AITimer[1] = 0;
                        AITimer[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
            }
            if (AITimer[0] < 2 || AITimer[0] >= 5)
                BaseAI.AIFish(NPC, ref NPC.ai, true);
            if (GlowActive)
            {
                if (GlowTimer++ > 60)
                {
                    GlowActive = false;
                    GlowTimer = 0;
                }
            }
        }
        private bool GlowActive;
        private int GlowTimer;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 7 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        float drawTimer;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!GlowActive)
                return true;

            Asset<Texture2D> texture = TextureAssets.Npc[Type];
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color colour = Color.Lerp(Color.White, Color.White, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            float opacity = MathHelper.Lerp(0, 1, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            RedeDraw.DrawTreasureBagEffect(spriteBatch, texture.Value, ref drawTimer, NPC.Center - screenPos, NPC.frame, colour, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, NPC.Opacity * opacity);
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!GlowActive)
                return;

            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color colour = Color.Lerp(Color.White, Color.White, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, colour, NPC.rotation, NPC.frame.Size() / 2, 1f, effects, 0);
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(BuffType<GreenRashesDebuff>(), Main.rand.Next(800, 1600));
        }
    }
}
