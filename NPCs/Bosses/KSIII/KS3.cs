using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.DataStructures;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Redemption.Items.Usable;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;

namespace Redemption.NPCs.Bosses.KSIII
{
    [AutoloadBossHead]
    public class KS3 : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            GunAttacks,
            SpecialAttacks,
            PhysicalAttacks,
            PhaseChange,
            Death,
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public float[] oldrot = new float[3];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("King Slayer III");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 36),
                PortraitPositionYOverride = 8
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 42000;
            NPC.defense = 35;
            NPC.damage = 120;
            NPC.width = 42;
            NPC.height = 106;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossSlayer");
            //BossBag = ModContent.ItemType<SlayerBag>();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.velocity.Length() >= 13f && NPC.ai[0] != 5;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("")
            });
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ModContent.ProjectileType<KS3_Exit>(), 0, Vector2.Zero, false, SoundID.Item1.WithVolume(0));

            if (!RedeBossDowned.downedSlayer)
            {
                RedeWorld.alignment -= NPC.ai[0] == 12 ? 0 : 1;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, NPC.ai[0] == 12 ? "+0" : "-1", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                    {
                        if (NPC.ai[0] == 12)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Good thing you left him be...", 240, 30, 0, Color.DarkGoldenrod);
                        else
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Oh dear, he seems to have a very short temper, and you winning probably made it worse. I hope he doesn't do anything stupid.", 240, 30, 0, Color.DarkGoldenrod);
                    }

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedSlayer, -1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (phase >= 5)
                damage *= 0.6;
            else
                damage *= 0.75;
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(chance);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                chance = (float)reader.ReadDouble();
            }
        }

        private float chance = 0.8f;
        private int phase;
        public int HeadType;
        public int BodyState;
        public enum BodyAnim
        {
            Idle, Charge, DropkickStart, Dropkick, WheelkickStart, Wheelkick, WheelkickEnd, Jojo, Pummel, IdlePhysical, ShoulderBash
        }
        public int ArmState;
        private enum ArmsAnim
        {
            Idle, ArmCross, Gun, GunShoot, Shield, Charging, RocketFist, Shrug, Grenade
        }
        public override void AI()
        {

        }

        private int ArmsFrameY;
        private int ArmsFrameX;
        private int HeadFrame;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                {
                    oldrot[k] = oldrot[k - 1];
                }
                oldrot[0] = NPC.rotation;

                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 8;
                if (AIState is not ActionState.PhysicalAttacks)
                {
                    if (NPC.velocity.Length() < 13f)
                        BodyState = (int)BodyAnim.Idle;
                    else
                        BodyState = (int)BodyAnim.Charge;
                }

                switch (BodyState)
                {
                    case (int)BodyAnim.Idle:
                        NPC.frame.Y = 0;
                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 3 * NPC.frame.Width)
                                NPC.frame.X = 0;
                        }
                        break;
                    case (int)BodyAnim.Charge:
                        NPC.frame.Y = (int)MathHelper.Clamp(NPC.frame.Y, 0, frameHeight);
                        if (NPC.frame.Y is 0 && NPC.frame.X < 4 * NPC.frame.Width)
                            NPC.frame.X = 4 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 7 * NPC.frame.Width)
                            {
                                NPC.frame.X = 0;
                                NPC.frame.Y = frameHeight;
                            }
                            if (NPC.frame.Y == frameHeight && NPC.frame.X > 1 * NPC.frame.Width)
                            {
                                NPC.frame.X = 6 * NPC.frame.Width;
                                NPC.frame.Y = 0;
                            }
                        }
                        break;
                    case (int)BodyAnim.DropkickStart:
                        NPC.frame.Y = frameHeight;
                        if (NPC.frame.X < 2 * NPC.frame.Width)
                            NPC.frame.X = 2 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 5 * NPC.frame.Width)
                                NPC.frame.X = 2 * NPC.frame.Width;
                        }
                        break;
                    case (int)BodyAnim.Dropkick:
                        NPC.frame.Y = frameHeight;
                        NPC.frame.X = 6 * NPC.frame.Width;
                        NPC.frameCounter = 0;
                        Vector2 position = NPC.Center + (Vector2.Normalize(NPC.velocity) * 30f);
                        Dust dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 2f)];
                        dust.position = position;
                        dust.velocity = (NPC.velocity.RotatedBy(1.5708) * 0.33f) + (NPC.velocity / 4f);
                        dust.position += NPC.velocity.RotatedBy(1.5708);
                        dust.fadeIn = 0.5f;
                        dust.noGravity = true;
                        dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 2f)];
                        dust.position = position;
                        dust.velocity = (NPC.velocity.RotatedBy(-1.5708) * 0.33f) + (NPC.velocity / 4f);
                        dust.position += NPC.velocity.RotatedBy(-1.5708);
                        dust.fadeIn = 0.5f;
                        dust.noGravity = true;
                        break;
                    case (int)BodyAnim.WheelkickStart:
                        NPC.frame.Y = (int)MathHelper.Clamp(NPC.frame.Y, frameHeight, 2 * frameHeight);
                        if (NPC.frame.Y == frameHeight && NPC.frame.X < 7 * NPC.frame.Width)
                            NPC.frame.X = 7 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 7 * NPC.frame.Width)
                            {
                                NPC.frame.X = 0;
                                NPC.frame.Y = 2 * frameHeight;
                            }
                            if (NPC.frame.Y == 2 * frameHeight && NPC.frame.X > 3 * NPC.frame.Width)
                            {
                                NPC.frame.X = 7 * NPC.frame.Width;
                                NPC.frame.Y = frameHeight;
                            }
                        }
                        break;
                    case (int)BodyAnim.Wheelkick:
                        NPC.frame.Y = 2 * frameHeight;
                        NPC.frame.X = 4 * NPC.frame.Width;
                        NPC.frameCounter = 0;
                        break;
                    case (int)BodyAnim.WheelkickEnd:
                        NPC.frame.Y = 2 * frameHeight;
                        if (NPC.frame.X < 5 * NPC.frame.Width)
                            NPC.frame.X = 5 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 6 * NPC.frame.Width)
                                BodyState = (int)BodyAnim.IdlePhysical;
                        }
                        break;
                    case (int)BodyAnim.Jojo:
                        NPC.frame.Y = (int)MathHelper.Clamp(NPC.frame.Y, 2 * frameHeight, 3 * frameHeight);
                        if (NPC.frame.Y == 2 * frameHeight && NPC.frame.X < 7 * NPC.frame.Width)
                            NPC.frame.X = 7 * NPC.frame.Width;

                        if (NPC.frameCounter++ >= 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.X += NPC.frame.Width;
                            if (NPC.frame.X > 7 * NPC.frame.Width)
                            {
                                NPC.frame.X = 0;
                                NPC.frame.Y = 2 * frameHeight;
                            }
                            if (NPC.frame.Y == 2 * frameHeight && NPC.frame.X > 3 * NPC.frame.Width)
                            {
                                NPC.frame.X = 7 * NPC.frame.Width;
                                NPC.frame.Y = frameHeight;
                            }
                        }
                        break;
                }

                switch (HeadType)
                {
                    case 0: // Normal
                        HeadFrame = NPC.frame.X / NPC.frame.Width;
                        break;
                    case 1: // Bored
                        HeadFrame = (NPC.frame.X / NPC.frame.Width) + 4;
                        break;
                    case 2: // Angry
                        HeadFrame = (NPC.frame.X / NPC.frame.Width) + 8;
                        break;
                    case 3: // Suspicious
                        HeadFrame = (NPC.frame.X / NPC.frame.Width) + 12;
                        break;
                    case 4: // Confused
                        HeadFrame = (NPC.frame.X / NPC.frame.Width) + 16;
                        break;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D Glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D Arms = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arms").Value;
            Texture2D ArmsGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arms_Glow").Value;
            Texture2D Head = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Heads").Value;
            Texture2D HeadGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Heads_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!NPC.IsABestiaryIconDummy)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(RedeColor.SlayerColour) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }
            }
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Glow, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (BodyState is (int)BodyAnim.Idle)
            {
                Vector2 HeadPos = new(NPC.Center.X - 2 * NPC.spriteDirection, NPC.Center.Y - 35);
                int HeadHeight = Head.Height / 20;
                int yHead = HeadHeight * HeadFrame;
                Rectangle HeadRect = new(0, yHead, Head.Width, HeadHeight);
                spriteBatch.Draw(Head, HeadPos - screenPos, new Rectangle?(HeadRect), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(Head.Width / 2f, HeadHeight / 2f), NPC.scale, effects, 0f);
                spriteBatch.Draw(HeadGlow, HeadPos - screenPos, new Rectangle?(HeadRect), NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(Head.Width / 2f, HeadHeight / 2f), NPC.scale, effects, 0f);
            }

            if (BodyState is (int)BodyAnim.Idle or (int)BodyAnim.Charge)
            {
                int height = Arms.Height / 10;
                int width = Arms.Height / 5;
                int y = height * ArmsFrameY;
                int x = width * ArmsFrameX;
                Rectangle ArmsRect = new(x, y, width, height);
                Vector2 ArmsOrigin = new(width / 2f, height / 2f);
                Vector2 ArmsPos = new(NPC.Center.X - 2 * NPC.spriteDirection, NPC.Center.Y - 13);

                spriteBatch.Draw(Arms, ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.GetAlpha(drawColor), NPC.rotation, ArmsOrigin, NPC.scale, effects, 0);

                spriteBatch.Draw(ArmsGlow, ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.GetAlpha(Color.White), NPC.rotation, ArmsOrigin, NPC.scale, effects, 0);
            }
            return false;
        }
    }
}