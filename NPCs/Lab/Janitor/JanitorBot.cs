using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Redemption.Items.Usable;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using ReLogic.Content;

namespace Redemption.NPCs.Lab.Janitor
{
    [AutoloadBossHead]
    public class JanitorBot : ModNPC
    {
        private static Asset<Texture2D> SlipAni;
        private static Asset<Texture2D> YeetAni;
        public override void Load()
        {
            SlipAni = ModContent.Request<Texture2D>(Texture + "_Slip");
            YeetAni = ModContent.Request<Texture2D>(Texture + "_Yeet");
        }
        public override void Unload()
        {
            SlipAni = null;
            YeetAni = null;
        }
        public enum ActionState
        {
            Begin,
            Jump,
            Yeet,
            Toss,
            Stunned,
            Slip
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Janitor");
            Main.npcFrameCount[NPC.type] = 19;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 40;
            NPC.friendly = false;
            NPC.damage = 55;
            NPC.defense = 20;
            NPC.lifeMax = 10500;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.value = 5200f;
            NPC.knockBackResist = 0.0f;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.netAlways = true;
            NPC.RedemptionGuard().GuardPoints = NPC.lifeMax;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic");
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LabBiome>().Type };
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Amidst all the ailments that germs can conjure, all the grime mankind can produce. We'll send unto them, only you. Scrape and Clean until it is done.")
            });
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            bool vDmg = false;
            if (AIState is not ActionState.Slip && NPC.RedemptionGuard().GuardPoints >= 0)
            {
                NPC.RedemptionGuard().GuardHit(NPC, ref vDmg, ref damage, ref knockback, SoundID.NPCHit4);
                if (NPC.RedemptionGuard().GuardPoints >= 0)
                    return vDmg;
            }
            NPC.RedemptionGuard().GuardBreakCheck(NPC, DustID.Electric, CustomSounds.GuardBreak, 10, 1, 1000);

            if (AIState is ActionState.Slip)
                damage /= 4;
            damage *= 2;
            return true;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LabPlatingDust>());
                    Main.dust[dustIndex].velocity *= 2;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<LabPlatingDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[0])
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel1>());

            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<JanitorBot_Defeated>());
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedJanitor, -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            DespawnHandler();
            NPC.LookAtEntity(player);

            if (!player.active || player.dead)
                return;

            switch (AIState)
            {
                case ActionState.Begin:
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("The Janitor", 60, 90, 0.8f, 0, Color.Yellow, "A Janitor");

                    AIState = ActionState.Jump;
                    NPC.netUpdate = true;
                    break;

                case ActionState.Jump:
                    if (AITimer++ == 0)
                    {
                        int tileRight = BaseWorldGen.GetFirstTileSide((int)NPC.Center.X / 16, (int)NPC.position.Y / 16, false);
                        int distRight = (tileRight * 16) - (int)NPC.Center.X;
                        int tileLeft = BaseWorldGen.GetFirstTileSide((int)NPC.Center.X / 16, (int)NPC.position.Y / 16, true);
                        int distLeft = (int)NPC.Center.X - (tileLeft * 16);
                        if (distRight <= 12 * 16)
                            NPC.velocity.X -= Main.rand.Next(3, 7);
                        else if (distLeft <= 12 * 16)
                            NPC.velocity.X += Main.rand.Next(3, 7);
                        else
                            NPC.velocity.X += Main.rand.Next(3, 7) * NPC.spriteDirection;
                        NPC.velocity.Y = -7;
                    }
                    if (NPC.collideX)
                        NPC.velocity.X *= -0.7f;

                    if (AITimer > 5 && (NPC.collideY || NPC.velocity.Y == 0))
                    {
                        NPC.velocity.X = 0;
                        AITimer = 0;
                        if (NPC.HasBuff(BuffID.Wet) && Main.rand.NextBool(3))
                        {
                            AIState = ActionState.Slip;
                            NPC.netUpdate = true;
                            break;
                        }
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                AIState = ActionState.Toss;
                                NPC.netUpdate = true;
                                break;
                            default:
                                AIState = ActionState.Yeet;
                                NPC.netUpdate = true;
                                break;
                        }
                    }
                    break;
                case ActionState.Yeet:
                    if (AITimer++ == 30)
                        NPC.frameCounter = 0;
                    if (AITimer >= 80)
                    {
                        AniFrameY = 0;
                        AITimer = 0;
                        AIState = ActionState.Jump;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Toss:
                    if (AITimer++ == 40)
                        NPC.frameCounter = 0;
                    if (AITimer >= 90)
                    {
                        AniFrameY = 0;
                        AITimer = 0;
                        AIState = ActionState.Jump;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Stunned:
                    if (AITimer++ == 0)
                    {
                        NPC.velocity.X = 0;
                        string ouch = "Oof!";
                        switch (Main.rand.Next(6))
                        {
                            case 1:
                                ouch = "Owch!";
                                break;
                            case 2:
                                ouch = "Yowch!";
                                break;
                            case 3:
                                ouch = "Ow!";
                                break;
                            case 4:
                                ouch = "Arg!";
                                break;
                            case 5:
                                ouch = "Damn it!";
                                break;
                        }
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, ouch, false, false);
                    }
                    if (AITimer >= 260)
                    {
                        AniFrameY = 0;
                        NPC.RedemptionGuard().GuardPoints = NPC.lifeMax;
                        NPC.RedemptionGuard().GuardBroken = false;
                        AITimer = 0;
                        AIState = ActionState.Jump;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Slip:
                    if (AITimer++ == 0)
                    {
                        string ouch = "Ah!";
                        switch (Main.rand.Next(6))
                        {
                            case 1:
                                ouch = "D'oh!";
                                break;
                            case 2:
                                ouch = "Oops!";
                                break;
                            case 3:
                                ouch = "Whoops!";
                                break;
                            case 4:
                                ouch = "Not again!";
                                break;
                            case 5:
                                ouch = "Damn it!";
                                break;
                        }
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, ouch, false, false);
                    }
                    if (AITimer >= 160)
                    {
                        AniFrameY = 0;
                        AITimer = 0;
                        AIState = ActionState.Jump;
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }
        private int AniFrameY;
        public override void FindFrame(int frameHeight)
        {
            Player player = Main.player[NPC.target];
            if (AIState is ActionState.Yeet && AITimer >= 30 && AITimer <= 60)
            {
                NPC.rotation = 0;
                NPC.frameCounter++;
                if (NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;
                    AniFrameY++;
                    if (AniFrameY == 4)
                    {
                        if (!Main.rand.NextBool(3))
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<JanitorMop_Proj>(), NPC.damage, RedeHelper.PolarVector(12, (player.Center - NPC.Center).ToRotation()), true, SoundID.Item19);
                        else
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<JanitorMop_Proj>(), NPC.damage, RedeHelper.PolarVector(8, (player.Center - NPC.Center).ToRotation()), true, SoundID.Item19, 1);
                    }
                    if (AniFrameY > 5)
                        AniFrameY = 5;
                }
                return;
            }
            if (AIState is ActionState.Toss && AITimer >= 40 && AITimer <= 70)
            {
                if (NPC.frame.Y < 5 * frameHeight)
                    NPC.frame.Y = 5 * frameHeight;

                NPC.rotation = 0;
                NPC.frameCounter++;
                if (NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y == 8 * frameHeight)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<JanitorBucket_Proj>(), NPC.damage, RedeHelper.PolarVector(12, (player.Center - NPC.Center).ToRotation()), true, SoundID.Item19);

                    if (NPC.frame.Y > 10 * frameHeight)
                        NPC.frame.Y = 10 * frameHeight;
                }
                return;
            }
            if (AIState is ActionState.Stunned)
            {
                if (NPC.frame.Y < 11 * frameHeight)
                    NPC.frame.Y = 11 * frameHeight;

                NPC.rotation = 0;
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (AITimer >= 240)
                    {
                        if (NPC.frame.Y > 18 * frameHeight)
                            NPC.frame.Y = 18 * frameHeight;
                    }
                    else
                    {
                        if (NPC.frame.Y > 17 * frameHeight)
                            NPC.frame.Y = 16 * frameHeight;
                    }
                }
                return;
            }
            if (AIState is ActionState.Slip)
            {
                NPC.rotation = 0;
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    AniFrameY++;
                    if (AITimer >= 140)
                    {
                        if (AniFrameY > 6)
                            AniFrameY = 6;
                    }
                    else
                    {
                        if (AniFrameY > 5)
                            AniFrameY = 4;
                    }
                }
                return;
            }
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                NPC.frameCounter++;
                if (NPC.frameCounter >= 4)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 4 * frameHeight;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (AIState is ActionState.Slip)
            {
                int Height = SlipAni.Value.Height / 7;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, SlipAni.Value.Width, Height);
                Vector2 origin = new(SlipAni.Value.Width / 2f, Height / 2f);
                spriteBatch.Draw(SlipAni.Value, NPC.Center - screenPos - new Vector2(0, 3), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else if (AIState is ActionState.Yeet && AITimer >= 30 && AITimer <= 60)
            {
                int Height = YeetAni.Value.Height / 6;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, YeetAni.Value.Width, Height);
                Vector2 origin = new(YeetAni.Value.Width / 2f, Height / 2f);
                spriteBatch.Draw(YeetAni.Value, NPC.Center - screenPos - new Vector2(0, 6), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else
                spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
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