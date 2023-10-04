using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Items.Usable;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Redemption.Base;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Items.Materials.HM;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Armor.Vanity;
using ReLogic.Content;
using Redemption.UI.ChatUI;
using Redemption.Globals.NPC;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Textures;
using Terraria.Localization;

namespace Redemption.NPCs.Bosses.KSIII
{
    [AutoloadBossHead]
    public class KS3_Clone : ModNPC
    {
        private static Asset<Texture2D> Glow;
        private static Asset<Texture2D> Arms;
        private static Asset<Texture2D> ArmsGlow;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            Glow = ModContent.Request<Texture2D>(Texture + "_Glow");
            Arms = ModContent.Request<Texture2D>(Texture + "_Arms");
            ArmsGlow = ModContent.Request<Texture2D>(Texture + "_Arms_Glow");
        }
        public override void Unload()
        {
            Glow = null;
            Arms = null;
            ArmsGlow = null;
        }
        public override string Texture => "Redemption/NPCs/Bosses/KSIII/KS3";
        public enum ActionState
        {
            Begin,
            Dialogue,
            GunAttacks,
            SpecialAttacks,
            PhysicalAttacks
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public ref float AttackChoice => ref NPC.ai[3];

        public float[] oldrot = new float[3];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slayer III... ?");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 36500;
            NPC.defense = 30;
            NPC.damage = 84;
            NPC.width = 42;
            NPC.height = 106;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 15, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossSlayer");
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.velocity.Length() >= 13f && AIState is not ActionState.PhysicalAttacks;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 65; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
                for (int i = 0; i < 35; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SlayerBag>()));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<KS3Relic>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KS3Trophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SlayerProjector>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<KingSlayerMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<SlayerGun>(), ModContent.ItemType<Nanoswarmer>(), ModContent.ItemType<SlayerFist>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SlayerController>(), 10));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Holokey>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CyberPlating>(), 1, 14, 18));

            npcLoot.Add(notExpertRule);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A King Slayer III Clone";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedSlayer, -1);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.6f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(chance);
            writer.Write(BodyState);
            writer.Write(gunRot);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            chance = reader.ReadSingle();
            BodyState = reader.ReadInt32();
            gunRot = reader.ReadSingle();
        }

        private float chance = 0.8f;
        public int HeadType;
        public int BodyState;
        public enum BodyAnim
        {
            Idle, Crossed, Gun, GunShoot, GunEnd, RocketFist, Grenade, Charging, Shrug, ShieldOn, ShieldOff, IdlePhysical, WheelkickStart, Wheelkick, WheelkickEnd, ShoulderBash, ShoulderBashEnd, DropkickStart, Dropkick, Pummel1, Pummel2, Jojo
        }

        const float gunRotLimit = (float)Math.PI / 2;
        public float gunRot;
        private Vector2 ShootPos;

        private float TeleGlowTimer;
        private bool TeleGlow;
        private Vector2 TeleVector;

        private static Texture2D Bubble => CommonTextures.TextBubble_Slayer.Value;
        private static readonly SoundStyle voice = CustomSounds.Voice6 with { Pitch = 0.1f };
        public readonly Vector2 modifier = new(0, -260);
        public override void AI()
        {
            Lighting.AddLight(NPC.Center, .3f, .6f, .8f);
            Player player = Main.player[NPC.target];

            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (NPC.DespawnHandler())
                return;
            chance = MathHelper.Clamp(chance, 0, 1);

            player.RedemptionScreen().ScreenFocusPosition = NPC.Center;

            Vector2 GunOrigin = NPC.Center + RedeHelper.PolarVector(54, gunRot) + RedeHelper.PolarVector(13 * NPC.spriteDirection, gunRot - (float)Math.PI / 2);
            int dmgIncrease = NPC.DistanceSQ(player.Center) > 800 * 800 ? 10 : 0;

            if (!player.active || player.dead)
                return;

            switch (AIState)
            {
                case ActionState.Begin:
                    NPC.LookAtEntity(player);
                    BodyState = (int)BodyAnim.Crossed;
                    player.RedemptionScreen().Rumble(5, 5);
                    TeleVector = NPC.Center;
                    TeleGlow = true;
                    if (AITimer++ == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                        for (int i = 0; i < 30; i++)
                        {
                            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, 0f, 0f, 100, default, 3f);
                            Main.dust[dustIndex].velocity *= 6f;
                            Main.dust[dustIndex].noGravity = true;
                        }
                    }
                    if (AITimer > 5)
                    {
                        AITimer = 0;
                        AIState = ActionState.Dialogue;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Dialogue:
                    #region No Dialogue Moment :(
                    NPC.LookAtEntity(player);
                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    AITimer++;
                    if (AITimer == 60)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Clone.Intro.1"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Clone.Intro.2"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, 0, false, null, Bubble, null, modifier))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Clone.Intro.3"), new Color(170, 255, 255), Color.Black, voice, .03f, 1.6f, .16f, true, null, Bubble, null, modifier, 1));
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer == 5001)
                    {
                        ArmsFrameY = 1;
                        ArmsFrameX = 0;
                        BodyState = (int)BodyAnim.Gun;
                    }
                    if (AITimer >= 5060)
                    {
                        ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Shield>(), 0, Vector2.Zero, NPC.whoAmI);
                        AITimer = 0;
                        NPC.dontTakeDamage = false;
                        AIState = ActionState.GunAttacks;
                        NPC.netUpdate = true;
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    #endregion
                    break;
                case ActionState.GunAttacks:
                    NPC.LookAtEntity(player);
                    if (AttackChoice == 0)
                    {
                        AttackChoice = Main.rand.Next(1, 6);
                        chance = Main.rand.NextFloat(0.5f, 1f);
                        NPC.netUpdate = true;
                    }
                    NPC.rotation = NPC.velocity.X * 0.01f;
                    switch (AttackChoice)
                    {
                        case -1:
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 6);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                                if (AITimer == 0)
                                {
                                    if (Main.rand.NextBool(2))
                                        AITimer = 1;
                                    else
                                        AITimer = 2;
                                    NPC.netUpdate = true;
                                }
                                NPC.velocity *= 0.9f;
                                if (AITimer == 1)
                                {
                                    if (BodyState is (int)BodyAnim.Gun)
                                        BodyState = (int)BodyAnim.GunEnd;

                                    if (BodyState is (int)BodyAnim.Idle && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.SpecialAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                    NPC.netUpdate = true;
                                }
                                else if (AITimer == 2)
                                {
                                    if (BodyState is (int)BodyAnim.Gun)
                                        BodyState = (int)BodyAnim.GunEnd;

                                    if (BodyState is (int)BodyAnim.Idle && NPC.velocity.Length() < 1f)
                                    {
                                        gunRot = 0;
                                        BodyState = (int)BodyAnim.IdlePhysical;
                                        AITimer = 0;
                                        AIState = ActionState.PhysicalAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                    NPC.netUpdate = true;
                                }

                            }
                            break;

                        #region Barrage Shot
                        case 1:
                            gunRot.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 60f);
                            SnapGunToFiringArea();
                            if (AITimer++ % 20 == 0)
                                ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));

                            NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) < 100 * 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (AITimer % 20 == 0)
                            {
                                NPC.Shoot(GunOrigin, ModContent.ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot), CustomSounds.Gun1KS);
                                BodyState = (int)BodyAnim.GunShoot;
                                NPC.netUpdate = true;
                            }
                            if (AITimer % 100 == 0)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int rot = 25 * i;
                                    NPC.Shoot(GunOrigin, ProjectileID.MartianTurretBolt, (int)(NPC.damage * .85f), RedeHelper.PolarVector(8 + dmgIncrease, gunRot + MathHelper.ToRadians(rot - 50)), CustomSounds.Gun3KS);
                                }
                                BodyState = (int)BodyAnim.GunShoot;
                            }
                            if (AITimer >= 310)
                            {
                                chance -= Main.rand.NextFloat(0.1f, 0.5f);
                                AITimer = 0;
                                AttackChoice = -1;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Bullet Spray
                        case 2:
                            if (AITimer < 245)
                                gunRot.SlowRotation(NPC.DirectionTo(player.Center + player.velocity * 20f).ToRotation(), (float)Math.PI / 60f);

                            SnapGunToFiringArea();
                            if (AITimer++ == 0)
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_TeleLine1>(), 0, RedeHelper.PolarVector(10, gunRot), ai1: NPC.whoAmI);

                            ShootPos = new Vector2(300 * NPC.RightOfDir(player), 10);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (AITimer < 200)
                            {
                                if (NPC.Distance(ShootPos) < 100 || AITimer > 40)
                                {
                                    AITimer = 200;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 13f, 14f, true);
                                }
                            }
                            else
                            {
                                if (AITimer < 260)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        Vector2 vector;
                                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                        vector.X = (float)(Math.Sin(angle) * 40);
                                        vector.Y = (float)(Math.Cos(angle) * 40);
                                        Dust dust2 = Main.dust[Dust.NewDust(GunOrigin + vector, 2, 2, DustID.Frost, 0f, 0f, 100, default, 2f)];
                                        dust2.noGravity = true;
                                        dust2.velocity = dust2.position.DirectionTo(GunOrigin) * 10f;
                                    }
                                }
                                NPC.velocity *= 0.96f;
                                if (AITimer == 260)
                                {
                                    NPC.velocity.X = -9 * NPC.spriteDirection;
                                    for (int i = 0; i < Main.rand.Next(5, 8); i++)
                                    {
                                        NPC.Shoot(GunOrigin, ModContent.ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(Main.rand.Next(8, 13) + dmgIncrease, gunRot + Main.rand.NextFloat(-0.14f, 0.14f)), CustomSounds.ShotgunBlastKS);
                                    }
                                    BodyState = (int)BodyAnim.GunShoot;
                                    NPC.netUpdate = true;

                                }
                                if (AITimer > 300)
                                {
                                    chance -= Main.rand.NextFloat(0.05f, 0.3f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Rebound Shot
                        case 3:
                            AttackChoice = 4;
                            NPC.netUpdate = true;
                            break;
                        #endregion

                        #region Rebound Shot II
                        case 4:
                            gunRot.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 60f);
                            SnapGunToFiringArea();
                            AITimer++;
                            ShootPos = new Vector2(450 * NPC.RightOfDir(player), -10);
                            NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            int startShot = 41;
                            if (AITimer >= startShot && AITimer % 3 == 0 && AITimer <= startShot + 15)
                            {
                                NPC.Shoot(GunOrigin, ModContent.ProjectileType<ReboundShot>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(15 + dmgIncrease, gunRot), CustomSounds.Gun2KS);
                                BodyState = (int)BodyAnim.GunShoot;
                                NPC.netUpdate = true;
                            }
                            if (AITimer > 67)
                            {
                                chance -= Main.rand.NextFloat(0.05f, 0.1f);
                                AITimer = 0;
                                AttackChoice = -1;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Barrage Shot II
                        case 5:
                            gunRot.SlowRotation(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation(), (float)Math.PI / 60f);
                            SnapGunToFiringArea();
                            if (AITimer++ % 20 == 0)
                                ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));

                            NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (AITimer % 10 == 0)
                            {
                                NPC.Shoot(GunOrigin, ModContent.ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .85f), RedeHelper.PolarVector(7 + dmgIncrease, gunRot), CustomSounds.Gun1KS);
                                BodyState = (int)BodyAnim.GunShoot;
                                NPC.netUpdate = true;
                            }
                            if (AITimer >= 60)
                            {
                                chance -= Main.rand.NextFloat(0.02f, 0.2f);
                                AITimer = 0;
                                AttackChoice = -1;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.SpecialAttacks:
                    NPC.LookAtEntity(player);
                    if (AttackChoice == 0)
                    {
                        AttackChoice = Main.rand.Next(1, 10);
                        chance = Main.rand.NextFloat(0.5f, 1f);
                        NPC.netUpdate = true;
                    }

                    NPC.rotation = NPC.velocity.X * 0.01f;
                    switch ((int)AttackChoice)
                    {
                        case -1:
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 10);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                if (AITimer == 0)
                                {
                                    if (Main.rand.NextBool(2))
                                        AITimer = 1;
                                    else
                                        AITimer = 2;
                                    NPC.netUpdate = true;
                                }
                                NPC.velocity *= 0.9f;
                                if (AITimer == 1)
                                {
                                    if (BodyState is (int)BodyAnim.Idle)
                                    {
                                        gunRot = 0;
                                        BodyState = (int)BodyAnim.IdlePhysical;
                                    }

                                    if (BodyState is (int)BodyAnim.IdlePhysical && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.PhysicalAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                                else if (AITimer == 2)
                                {
                                    if (BodyState is (int)BodyAnim.Idle)
                                    {
                                        ArmsFrameY = 1;
                                        ArmsFrameX = 0;
                                        BodyState = (int)BodyAnim.Gun;
                                    }

                                    if (BodyState is (int)BodyAnim.Gun && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.GunAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;

                        #region Rocket Fist
                        case 1:
                            ShootPos = new Vector2(300 * NPC.RightOfDir(player), -60);
                            AITimer++;
                            if (AITimer < 100)
                            {
                                if (NPC.Distance(ShootPos) < 160 || AITimer > 40)
                                {
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 14f, true);
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;
                                if (AITimer == 105)
                                    BodyState = (int)BodyAnim.RocketFist;

                                if (AITimer == 120)
                                    NPC.Shoot(new Vector2(NPC.Center.X + 15 * NPC.spriteDirection, NPC.Center.Y - 11), ModContent.ProjectileType<KS3_Fist>(), NPC.damage, new Vector2(10 * NPC.spriteDirection, 0), CustomSounds.MissileFire1);

                                if (AITimer > 150)
                                {
                                    chance -= Main.rand.NextFloat(0.03f, 0.1f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Stun Grenade
                        case 2:
                            if (AITimer == 0)
                            {
                                if (Main.rand.NextBool(3))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                ShootPos = new Vector2(200 * NPC.RightOfDir(player), 0);
                                if (AITimer < 100)
                                {
                                    if (NPC.Distance(ShootPos) < 160 || AITimer > 50)
                                    {
                                        AITimer = 100;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 18f, 14f, true);
                                }
                                else
                                {
                                    NPC.velocity *= 0.9f;
                                    if (AITimer == 120)
                                    {
                                        ArmsFrameY = 5;
                                        ArmsCounter = 0;
                                        BodyState = (int)BodyAnim.Grenade;
                                    }
                                    if (AITimer == 140)
                                        NPC.Shoot(new Vector2(NPC.Center.X + 21 * NPC.spriteDirection, NPC.Center.Y - 17), ModContent.ProjectileType<KS3_FlashGrenade>(), (int)(NPC.damage * .9f), new Vector2(10 * NPC.spriteDirection, -6), SoundID.Item1);

                                    if (AITimer > 180)
                                    {
                                        chance -= Main.rand.NextFloat(0.05f, 0.8f);
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Beam Cell
                        case 3:
                            if (AITimer == 0)
                            {
                                if (Main.rand.NextBool(2))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                BodyState = (int)BodyAnim.Charging;
                                ShootPos = new Vector2(320 * NPC.RightOfDir(player), 0);
                                if (AITimer < 100)
                                {
                                    if (NPC.Distance(ShootPos) < 160 || AITimer > 60)
                                    {
                                        AITimer = 100;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        for (int k = 0; k < 3; k++)
                                        {
                                            Vector2 vector;
                                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                            vector.X = (float)(Math.Sin(angle) * 100);
                                            vector.Y = (float)(Math.Cos(angle) * 100);
                                            Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.Frost, 0f, 0f, 100, default, 1f)];
                                            dust2.noGravity = true;
                                            dust2.velocity = -NPC.DirectionTo(dust2.position) * 5f;
                                        }
                                        NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 8f, 14f, true);
                                    }
                                }
                                else
                                {
                                    NPC.Move(ShootPos, 4f, 14f, true);
                                    if (AITimer == 121)
                                        NPC.Shoot(new Vector2(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16), ModContent.ProjectileType<KS3_BeamCell>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation() - MathHelper.ToRadians(35 * NPC.spriteDirection)),
                                            SoundID.Item103, ai0: NPC.whoAmI);

                                    if (AITimer > 240)
                                    {
                                        BodyState = (int)BodyAnim.Idle;
                                        chance -= Main.rand.NextFloat(0.5f, 1f);
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Core Surge
                        case 4:
                            if (AITimer == 0)
                            {
                                if (RedeHelper.Chance(0.75f) && NPC.DistanceSQ(player.Center) < 300 * 300)
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                BodyState = (int)BodyAnim.Charging;
                                ShootPos = new Vector2(80 * NPC.RightOfDir(player), 0);
                                if (AITimer < 200)
                                {
                                    if (NPC.DistanceSQ(ShootPos) < 160 * 160 || AITimer > 120)
                                    {
                                        AITimer = 200;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        for (int k = 0; k < 3; k++)
                                        {
                                            Vector2 vector;
                                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                            vector.X = (float)(Math.Sin(angle) * 100);
                                            vector.Y = (float)(Math.Cos(angle) * 100);
                                            Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.Frost, 0f, 0f, 100, default, 1f)];
                                            dust2.noGravity = true;
                                            dust2.velocity = -NPC.DirectionTo(dust2.position) * 5f;
                                        }
                                        NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 7f, 14f, true);
                                    }
                                }
                                else
                                {
                                    NPC.velocity *= 0.9f;
                                    if (AITimer == 202)
                                    {
                                        NPC.Shoot(new Vector2(NPC.spriteDirection == 1 ? NPC.Center.X + 2 : NPC.Center.X - 2, NPC.Center.Y - 16), ModContent.ProjectileType<KS3_Surge>(), (int)(NPC.damage * .9f), Vector2.Zero, CustomSounds.ElectricNoise, NPC.whoAmI);

                                        for (int i = 0; i < 18; i++)
                                            NPC.Shoot(new Vector2(NPC.Center.X + 2 * NPC.spriteDirection, NPC.Center.Y - 16), ModContent.ProjectileType<KS3_Surge2>(), 0, RedeHelper.PolarVector(14, MathHelper.ToRadians(20) * i));

                                        if (Main.expertMode)
                                        {
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                for (int i = 0; i < 8; i++)
                                                {
                                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(8, MathHelper.ToRadians(45) * i), ProjectileID.MartianTurretBolt, NPCHelper.HostileProjDamage((int)(NPC.damage * .85f)), 0, Main.myPlayer);
                                                    Main.projectile[proj].tileCollide = false;
                                                    Main.projectile[proj].timeLeft = 200;
                                                    Main.projectile[proj].netUpdate = true;
                                                }
                                                for (int i = 0; i < 18; i++)
                                                {
                                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, RedeHelper.PolarVector(7, MathHelper.ToRadians(20) * i), ProjectileID.MartianTurretBolt, NPCHelper.HostileProjDamage((int)(NPC.damage * .85f)), 0, Main.myPlayer);
                                                    Main.projectile[proj].tileCollide = false;
                                                    Main.projectile[proj].timeLeft = 200;
                                                    Main.projectile[proj].netUpdate = true;
                                                }
                                            }
                                        }
                                    }
                                    if (AITimer > 232)
                                    {
                                        BodyState = (int)BodyAnim.Idle;
                                        chance -= Main.rand.NextFloat(0.4f, 0.7f);
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Shrug It Off
                        case 5:
                            if (AITimer == 0)
                            {
                                if (NPC.NPCHasAnyBuff())
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                BodyState = (int)BodyAnim.Shrug;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 21)
                                {
                                    for (int k = 0; k < NPC.buffImmune.Length; k++)
                                    {
                                        if (BuffID.Sets.IsATagBuff[k])
                                            continue;
                                        NPC.BecomeImmuneTo(k);
                                    }
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        NPC.ClearImmuneToBuffs(out _);
                                    NPC.netUpdate = true;
                                }
                                if (AITimer > 31)
                                {
                                    BodyState = (int)BodyAnim.Idle;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Deflect
                        case 6:
                            if (AITimer == 0)
                            {
                                if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_Shield>()) &&
                                    (player.HeldItem.DamageType == DamageClass.Magic || player.HeldItem.DamageType == DamageClass.Ranged) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                if (AITimer++ % 20 == 0)
                                    ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(player), Main.rand.Next(-60, 60));

                                NPC.Move(ShootPos, NPC.Distance(player.Center) < 100 ? 4f : NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f, true);
                                if (AITimer == 16)
                                    NPC.Shoot(new Vector2(NPC.Center.X + 48 * NPC.spriteDirection, NPC.Center.Y - 12), ModContent.ProjectileType<KS3_Reflect>(), 0, Vector2.Zero, NPC.whoAmI);

                                if (AITimer > 231)
                                    BodyState = (int)BodyAnim.ShieldOff;
                                else
                                    BodyState = (int)BodyAnim.ShieldOn;

                                if (AITimer > 261)
                                {
                                    chance -= Main.rand.NextFloat(0.2f, 1f);
                                    BodyState = (int)BodyAnim.Idle;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Missile Drones
                        case 7:
                            if (AITimer == 0)
                            {
                                if (!NPC.AnyNPCs(ModContent.NPCType<KS3_MissileDrone>()) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 16)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2, NPC.whoAmI);
                                    if (!NPC.AnyNPCs(ModContent.NPCType<KS3_MissileDrone>()))
                                    {
                                        for (int i = 0; i < Main.rand.Next(2, 5); i++)
                                        {
                                            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800),
                                                ModContent.NPCType<KS3_MissileDrone>(), NPC.whoAmI);
                                        }
                                    }
                                }
                                if (AITimer > 91)
                                {
                                    chance -= Main.rand.NextFloat(0.2f, 1f);
                                    BodyState = (int)BodyAnim.Idle;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Energy Magnet
                        case 8:
                            if (AITimer == 0)
                            {
                                if (!NPC.AnyNPCs(ModContent.NPCType<KS3_Magnet>()) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 16)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2, NPC.whoAmI);
                                    if (!NPC.AnyNPCs(ModContent.NPCType<KS3_Magnet>()))
                                    {
                                        for (int i = 0; i < 2; i++)
                                        {
                                            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-80, 80), (int)NPC.Center.Y - Main.rand.Next(750, 800),
                                                ModContent.NPCType<KS3_Magnet>(), NPC.whoAmI);
                                        }
                                    }
                                }
                                if (AITimer > 91)
                                {
                                    chance -= Main.rand.NextFloat(0.7f, 1f);
                                    BodyState = (int)BodyAnim.Idle;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Missile Barrage
                        case 9:
                            if (AITimer == 0)
                            {
                                if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_SoSCrosshair>()) && Main.rand.NextBool(4))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 10);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer++;
                                NPC.velocity *= 0.98f;
                                if (AITimer == 16)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2, NPC.whoAmI);
                                    if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_SoSCrosshair>()))
                                        NPC.Shoot(player.Center, ModContent.ProjectileType<KS3_SoSCrosshair>(), (int)(NPC.damage * 1.1f), Vector2.Zero, NPC.whoAmI);
                                }
                                if (AITimer > 91)
                                {
                                    chance -= Main.rand.NextFloat(0.7f, 1f);
                                    BodyState = (int)BodyAnim.Idle;
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.PhysicalAttacks:
                    if (AttackChoice == 0)
                    {
                        AttackChoice = Main.rand.Next(1, 6);
                        chance = Main.rand.NextFloat(0.5f, 1f);
                        NPC.netUpdate = true;
                    }
                    switch ((int)AttackChoice)
                    {
                        case -1:
                            NPC.LookAtEntity(player);
                            if (RedeHelper.Chance(chance) && AITimer == 0)
                            {
                                AttackChoice = Main.rand.Next(1, 6);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                if (AITimer == 0)
                                {
                                    if (Main.rand.NextBool(2))
                                        AITimer = 1;
                                    else
                                        AITimer = 2;
                                    NPC.netUpdate = true;
                                }
                                NPC.velocity *= 0.9f;
                                if (AITimer == 1)
                                {
                                    if (BodyState is (int)BodyAnim.IdlePhysical)
                                        BodyState = (int)BodyAnim.Idle;

                                    if (BodyState is (int)BodyAnim.Idle && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.SpecialAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                                else if (AITimer == 2)
                                {
                                    if (BodyState is (int)BodyAnim.IdlePhysical)
                                    {
                                        ArmsFrameY = 1;
                                        ArmsFrameX = 0;
                                        BodyState = (int)BodyAnim.Gun;
                                    }
                                    if (BodyState is (int)BodyAnim.Gun && NPC.velocity.Length() < 1f)
                                    {
                                        AITimer = 0;
                                        AIState = ActionState.GunAttacks;
                                        AttackChoice = 0;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;

                        #region Guillotine Wheel Kick
                        case 1:
                            NPC.LookAtEntity(player);
                            if (AITimer++ <= 40)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                ShootPos = new Vector2(200 * NPC.RightOfDir(player), -60);
                                NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                            }
                            if (AITimer == 40)
                            {
                                NPC.frame.X = 7 * NPC.frame.Width;
                                NPC.frame.Y = 160;
                                BodyState = (int)BodyAnim.WheelkickStart;
                            }

                            if (AITimer > 40 && AITimer < 100)
                            {
                                if (AITimer % 15 == 0)
                                    SoundEngine.PlaySound(SoundID.Item1, NPC.position);

                                NPC.rotation += NPC.velocity.Y / 30;
                                ShootPos = new Vector2(player.velocity.X * 30, -600);
                                if (NPC.Center.Y < player.Center.Y - 600 || AITimer > 80)
                                {
                                    BodyState = (int)BodyAnim.Wheelkick;
                                    NPC.velocity *= 0.2f;
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 34f : 26f, 3f, true);
                            }
                            else if (AITimer >= 100 && AITimer < 200)
                            {
                                if (AITimer >= 100 && AITimer < 110)
                                {
                                    NPC.rotation = 0;
                                    NPC.velocity.Y -= 0.01f;
                                }
                                if (AITimer == 110)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_Wave>(), NPC.damage, Vector2.Zero, SoundID.Item74, ai0: NPC.whoAmI);
                                    NPC.velocity.Y += 40f;
                                }
                                if (AITimer >= 110)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 44 + (8 * NPC.spriteDirection), (int)NPC.Center.Y - 42 - 5, 88, 84);
                                    for (int p = 0; p < Main.maxPlayers; p++)
                                    {
                                        Player target = Main.player[p];
                                        if (!target.active || target.dead)
                                            continue;

                                        if (!target.Hitbox.Intersects(Hitbox))
                                            continue;

                                        int hitDirection = target.RightOfDir(NPC);
                                        BaseAI.DamagePlayer(target, NPC.damage, 3, hitDirection, NPC);
                                    }
                                }
                                if (AITimer > 130 || NPC.Center.Y > player.Center.Y + 400)
                                {
                                    AITimer = 200;
                                    BodyState = (int)BodyAnim.WheelkickEnd;
                                    NPC.netUpdate = true;
                                }
                            }
                            else if (AITimer >= 200)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                if (AITimer > 220)
                                {
                                    ShootPos = new Vector2(200 * NPC.RightOfDir(player), -60);
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 30f : 22f, 8f, true);
                                }
                                else
                                    NPC.velocity *= 0.8f;
                            }
                            if (AITimer > 280)
                            {
                                NPC.rotation = 0;
                                chance -= Main.rand.NextFloat(0.1f, 0.3f);
                                AITimer = 0;
                                AttackChoice = -1;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Shoulder Bash
                        case 2:
                            AITimer++;
                            if (AITimer == 1)
                                Teleport(false, Vector2.Zero);
                            if (AITimer < 100)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                NPC.LookAtEntity(player);
                                if (NPC.DistanceSQ(ShootPos) < 50 * 50 || AITimer > 70)
                                {
                                    AITimer = 100;

                                    NPC.frame.Y = 160 * 5;
                                    NPC.frame.X = 0;
                                    BodyState = (int)BodyAnim.ShoulderBash;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    ShootPos = new Vector2(100 * NPC.RightOfDir(player), 0);
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                                }
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.rotation = 0;
                                NPC.velocity *= 0.8f;
                                if (AITimer == 101)
                                    NPC.velocity.X = NPC.RightOfDir(player) * 6;

                                if (AITimer == 110)
                                    NPC.Dash(60, false, SoundID.Item74, player.Center);

                                if (AITimer >= 110 && AITimer <= 130)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 28 + (8 * NPC.spriteDirection), (int)NPC.Center.Y - 53 - 5, 56, 106);
                                    for (int p = 0; p < Main.maxPlayers; p++)
                                    {
                                        Player target = Main.player[p];
                                        if (!target.active || target.dead)
                                            continue;

                                        if (!target.Hitbox.Intersects(Hitbox))
                                            continue;

                                        int hitDirection = target.RightOfDir(NPC);
                                        BaseAI.DamagePlayer(target, NPC.damage, 3, hitDirection, NPC);
                                    }
                                }

                                if (AITimer == 130)
                                {
                                    NPC.velocity.X = -15 * NPC.spriteDirection;
                                    BodyState = (int)BodyAnim.ShoulderBashEnd;
                                }
                                if (AITimer > 160)
                                {
                                    NPC.LookAtEntity(player);
                                    chance -= Main.rand.NextFloat(0.05f, 0.2f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion

                        #region Hyperspear Dropkick
                        case 3:
                            if (Main.expertMode)
                            {
                                NPC.LookAtEntity(player);
                                AITimer++;
                                if (AITimer < 100)
                                {
                                    NPC.rotation = 0;
                                    if (NPC.DistanceSQ(ShootPos) < 100 * 100 || AITimer > 50)
                                    {
                                        ShootPos = new Vector2(150 * NPC.RightOfDir(player), 200);
                                        AITimer = 100;
                                        NPC.velocity.X = 0;
                                        NPC.velocity.Y = -25;

                                        NPC.frame.X = 2 * NPC.frame.Width;
                                        NPC.frame.Y = 160;
                                        BodyState = (int)BodyAnim.DropkickStart;

                                        SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_TeleLine2>(), 0, NPC.DirectionTo(player.Center + player.velocity * 20f), ai1: NPC.whoAmI);
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                                }
                                else if (AITimer >= 100 && AITimer < 200)
                                {
                                    NPC.velocity *= 0.97f;
                                    if (NPC.velocity.Length() < 6 || AITimer > 160)
                                    {
                                        AITimer = 200;
                                        NPC.velocity *= 0f;
                                        BodyState = (int)BodyAnim.Dropkick;
                                        NPC.rotation = (player.Center + player.velocity * 20f - NPC.Center).ToRotation() + (float)(-Math.PI / 2);
                                        NPC.netUpdate = true;
                                    }
                                }
                                else if (AITimer >= 200)
                                {
                                    if (AITimer == 204)
                                        NPC.rotation = (player.Center + player.velocity * 20f - NPC.Center).ToRotation() + (float)(-Math.PI / 2);

                                    if (AITimer >= 205)
                                    {
                                        Rectangle Hitbox = new((int)NPC.Center.X - 29, (int)NPC.Center.Y - 59, 58, 118);
                                        for (int p = 0; p < Main.maxPlayers; p++)
                                        {
                                            Player target = Main.player[p];
                                            if (!target.active || target.dead)
                                                continue;

                                            if (!target.Hitbox.Intersects(Hitbox))
                                                continue;

                                            int hitDirection = target.RightOfDir(NPC);
                                            BaseAI.DamagePlayer(target, (int)(NPC.damage * 1.1f), 3, hitDirection, NPC);
                                        }
                                    }

                                    if (AITimer == 205)
                                        NPC.Dash(40, true, SoundID.Item74, player.Center + player.velocity * 20f);

                                    if (AITimer > 260 || NPC.Center.Y > player.Center.Y + 400)
                                    {
                                        NPC.rotation = 0;
                                        NPC.velocity *= 0f;
                                        chance -= Main.rand.NextFloat(0.2f, 0.5f);
                                        NPC.frame.Y = 4 * 80;
                                        if (NPC.frame.X < 4 * NPC.frame.Width)
                                            NPC.frame.X = 4 * NPC.frame.Width;
                                        BodyState = (int)BodyAnim.IdlePhysical;
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                NPC.frame.Y = 4 * 80;
                                if (NPC.frame.X < 4 * NPC.frame.Width)
                                    NPC.frame.X = 4 * NPC.frame.Width;

                                chance -= Main.rand.NextFloat(0.2f, 0.5f);
                                BodyState = (int)BodyAnim.IdlePhysical;
                                AttackChoice = -1;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Iron Pummel
                        case 4:
                            AITimer++;
                            NPC.rotation = NPC.velocity.X * 0.01f;
                            ShootPos = new Vector2(60 * NPC.RightOfDir(player), 20);
                            if (AITimer < 100)
                            {
                                NPC.LookAtEntity(player);
                                if ((NPC.DistanceSQ(player.Center + ShootPos) < 50 * 50 && gunRot != 0) || AITimer > 40)
                                {
                                    AITimer = 100;

                                    NPC.frame.X = 0;
                                    BodyState = Main.rand.NextBool(2) ? (int)BodyAnim.Pummel1 : (int)BodyAnim.Pummel2;
                                    if (BodyState is (int)BodyAnim.Pummel1)
                                    {
                                        NPC.frame.Y = 3 * 80;
                                        NPC.frame.X = 6 * NPC.frame.Width;
                                    }
                                    else
                                    {
                                        NPC.frame.Y = 4 * 80;
                                        NPC.frame.X = 1 * NPC.frame.Width;
                                    }
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.velocity *= 0.9f;
                                if (AITimer == 105)
                                {
                                    gunRot += 1;
                                    NPC.Dash(10, false, CustomSounds.Swoosh1, player.Center);
                                }

                                if (AITimer >= 105 && AITimer <= 115)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 6 + (28 * NPC.spriteDirection), (int)NPC.Center.Y - 6 - 18, 12, 12);
                                    for (int p = 0; p < Main.maxPlayers; p++)
                                    {
                                        Player target = Main.player[p];
                                        if (!target.active || target.dead)
                                            continue;

                                        if (!target.Hitbox.Intersects(Hitbox))
                                            continue;

                                        int hitDirection = target.RightOfDir(NPC);
                                        BaseAI.DamagePlayer(target, (int)(NPC.damage * .9f), 3, hitDirection, NPC);
                                    }
                                }

                                if (AITimer == 125 && RedeHelper.Chance(0.4f))
                                {
                                    AITimer = 140;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer > 125)
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.Move(ShootPos, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 17f, 5f, true);
                                }
                                if (AITimer > 140)
                                {
                                    if (gunRot <= 1 || RedeHelper.Chance(0.35f))
                                    {
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        NPC.LookAtEntity(player);
                                        chance -= Main.rand.NextFloat(0.05f, 0.1f);
                                        AITimer = 0;
                                        AttackChoice = -1;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Hologram Flurry
                        case 5:
                            if (AITimer == 0)
                            {
                                if (Main.rand.NextBool(6))
                                    AITimer = 1;
                                else
                                {
                                    AttackChoice = Main.rand.Next(1, 6);
                                    AITimer = 0;
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.LookAtEntity(player);
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                AITimer++;
                                ShootPos = new Vector2(80 * NPC.RightOfDir(player), 20);

                                if (AITimer == 5)
                                {
                                    NPC.frame.X = 7 * NPC.frame.Width;
                                    NPC.frame.Y = 160 * 2;
                                    BodyState = (int)BodyAnim.Jojo;
                                }

                                NPC.Move(ShootPos, NPC.Distance(player.Center) > 300 ? 20f : 9f, 8f, true);
                                if (AITimer >= 15 && AITimer % 3 == 0)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_JojoFist>(), (int)(NPC.damage * .9f), Vector2.Zero, SoundID.Item60 with { Volume = .3f }, ai0: NPC.whoAmI);
                                }
                                if (AITimer > 240)
                                {
                                    NPC.frame.Y = 4 * 80;
                                    if (NPC.frame.X < 4 * NPC.frame.Width)
                                        NPC.frame.X = 4 * NPC.frame.Width;

                                    BodyState = (int)BodyAnim.IdlePhysical;
                                    chance -= Main.rand.NextFloat(0.05f, 0.2f);
                                    AITimer = 0;
                                    AttackChoice = -1;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                            #endregion
                    }
                    break;
            }
            #region Teleporting
            if (NPC.DistanceSQ(player.Center) >= 1100 * 1100 && NPC.ai[0] > 0 && !player.RedemptionScreen().lockScreen)
            {
                if (AttackChoice == 3 && AIState is ActionState.PhysicalAttacks)
                    return;
                Teleport(false, Vector2.Zero);
                NPC.netUpdate = true;
            }
            #endregion
        }
        public override void PostAI()
        {
            CustomFrames(80);
        }
        public void CustomFrames(int frameHeight)
        {
            if (TeleGlow)
            {
                TeleGlowTimer += 3;
                if (TeleGlowTimer > 60)
                {
                    TeleGlow = false;
                    TeleGlowTimer = 0;
                }
            }

            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            #region Body

            switch (BodyState)
            {
                case (int)BodyAnim.Idle:
                    ArmsFrameY = 0;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX++;
                        if (ArmsFrameX > 3)
                            ArmsFrameX = 0;
                    }
                    break;
                case (int)BodyAnim.Crossed:
                    ArmsFrameY = 0;
                    if (ArmsFrameX < 4)
                        ArmsFrameX = 4;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX++;
                        if (ArmsFrameX > 7)
                            ArmsFrameX = 4;
                    }
                    break;
                case (int)BodyAnim.Gun:
                    ArmsFrameY = 1;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX++;
                        if (ArmsFrameX > 6)
                            ArmsFrameX = 6;
                    }
                    break;
                case (int)BodyAnim.GunShoot:
                    ArmsFrameY = 1;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX++;
                        if (ArmsFrameX > 8)
                        {
                            ArmsFrameX = 6;
                            BodyState = (int)BodyAnim.Gun;
                        }
                    }
                    break;
                case (int)BodyAnim.GunEnd:
                    ArmsFrameY = 1;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX--;
                        if (ArmsFrameX < 0)
                        {
                            ArmsFrameX = 0;
                            BodyState = (int)BodyAnim.Idle;
                        }
                    }
                    break;
                case (int)BodyAnim.RocketFist:
                    ArmsFrameY = 3;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX++;
                        if (ArmsFrameX > 7)
                        {
                            ArmsFrameX = 0;
                            BodyState = (int)BodyAnim.Idle;
                        }
                    }
                    break;
                case (int)BodyAnim.Grenade:
                    ArmsFrameY = 5;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX++;
                        if (ArmsFrameX > 5)
                        {
                            ArmsFrameX = 0;
                            BodyState = (int)BodyAnim.Idle;
                        }
                    }
                    break;
                case (int)BodyAnim.Charging:
                    ArmsFrameY = 2;
                    if (ArmsFrameX < 5)
                        ArmsFrameX = 5;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX++;
                        if (ArmsFrameX > 9)
                            ArmsFrameX = 7;
                    }
                    break;
                case (int)BodyAnim.Shrug:
                    ArmsFrameY = 4;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX++;
                        if (ArmsFrameX > 6)
                        {
                            ArmsFrameX = 0;
                            BodyState = (int)BodyAnim.Idle;
                        }
                    }
                    break;
                case (int)BodyAnim.ShieldOn:
                    ArmsFrameY = 2;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX++;
                        if (ArmsFrameX > 4)
                            ArmsFrameX = 3;
                    }
                    break;
                case (int)BodyAnim.ShieldOff:
                    ArmsFrameY = 2;
                    if (ArmsCounter++ >= 5)
                    {
                        ArmsCounter = 0;
                        ArmsFrameX--;
                        if (ArmsFrameX < 0)
                        {
                            ArmsFrameX = 0;
                            BodyState = (int)BodyAnim.Idle;
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
                        {
                            NPC.frame.Y = 4 * frameHeight;
                            NPC.frame.X = 4 * NPC.frame.Width;
                            BodyState = (int)BodyAnim.IdlePhysical;
                        }
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
                            NPC.frame.Y = 3 * frameHeight;
                        }
                        if (NPC.frame.Y == 3 * frameHeight && NPC.frame.X > 4 * NPC.frame.Width)
                        {
                            NPC.frame.X = 1 * NPC.frame.Width;
                            NPC.frame.Y = 3 * frameHeight;
                        }
                    }
                    break;
                case (int)BodyAnim.Pummel1:
                    NPC.frame.Y = (int)MathHelper.Clamp(NPC.frame.Y, 3 * frameHeight, 4 * frameHeight);
                    if (NPC.frame.Y == 3 * frameHeight && NPC.frame.X < 6 * NPC.frame.Width)
                        NPC.frame.X = 6 * NPC.frame.Width;

                    if (NPC.frameCounter++ >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.X += NPC.frame.Width;
                        if (NPC.frame.X > 7 * NPC.frame.Width)
                        {
                            NPC.frame.X = 0;
                            NPC.frame.Y = 4 * frameHeight;
                        }
                        if (NPC.frame.Y == 4 * frameHeight && NPC.frame.X > 0 * NPC.frame.Width)
                        {
                            NPC.frame.Y = 4 * frameHeight;
                            NPC.frame.X = 4 * NPC.frame.Width;
                            BodyState = (int)BodyAnim.IdlePhysical;
                        }
                    }
                    break;
                case (int)BodyAnim.Pummel2:
                    NPC.frame.Y = 4 * frameHeight;
                    if (NPC.frame.X < 1 * NPC.frame.Width)
                        NPC.frame.X = 1 * NPC.frame.Width;

                    if (NPC.frameCounter++ >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.X += NPC.frame.Width;
                        if (NPC.frame.X > 3 * NPC.frame.Width)
                        {
                            NPC.frame.Y = 4 * frameHeight;
                            NPC.frame.X = 4 * NPC.frame.Width;
                            BodyState = (int)BodyAnim.IdlePhysical;
                        }
                    }
                    break;
                case (int)BodyAnim.IdlePhysical:
                    NPC.frame.Y = 4 * frameHeight;
                    if (NPC.frame.X < 4 * NPC.frame.Width)
                        NPC.frame.X = 4 * NPC.frame.Width;

                    if (NPC.frameCounter++ >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.X += NPC.frame.Width;
                        if (NPC.frame.X > 7 * NPC.frame.Width)
                            NPC.frame.X = 4 * NPC.frame.Width;
                    }
                    break;
                case (int)BodyAnim.ShoulderBash:
                    NPC.frame.Y = 5 * frameHeight;

                    if (NPC.frameCounter++ >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.X += NPC.frame.Width;
                        if (NPC.frame.X > 6 * NPC.frame.Width)
                            NPC.frame.X = 2 * NPC.frame.Width;
                    }
                    break;
                case (int)BodyAnim.ShoulderBashEnd:
                    NPC.frame.Y = 5 * frameHeight;

                    if (NPC.frameCounter++ >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.X -= NPC.frame.Width;
                        if (NPC.frame.X < 0 * NPC.frame.Width)
                        {
                            NPC.frame.Y = 4 * frameHeight;
                            NPC.frame.X = 4 * NPC.frame.Width;
                            BodyState = (int)BodyAnim.IdlePhysical;
                        }
                    }
                    break;
            }
            #endregion
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 5000;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (AIState is ActionState.PhysicalAttacks)
            {
                if (AttackChoice == 2)
                    target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 30);
                if (AttackChoice == 3)
                    target.AddBuff(ModContent.BuffType<StaticStunDebuff>(), 120);
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (AIState is ActionState.PhysicalAttacks)
            {
                if (AttackChoice == 2)
                    modifiers.Knockback += 4;
            }
        }
        #region Methods
        void SnapGunToFiringArea()
        {
            //set bpoundries
            float firingRegionCenter = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
            float minFiringRegion = firingRegionCenter - gunRotLimit / 2f;
            float maxFiringRegion = firingRegionCenter + gunRotLimit / 2f;

            //convert gunRot to equivilent angles within a certian range
            while (gunRot < -(float)Math.PI / 2)
            {
                gunRot += (float)Math.PI * 2f;
            }
            while (gunRot > 3f * (float)Math.PI / 2f)
            {
                gunRot -= (float)Math.PI * 2f;
            }

            //detect if gun points outside the firing area
            if (gunRot > maxFiringRegion || gunRot < minFiringRegion)
            {
                float distFromMin = RedeHelper.AngularDifference(minFiringRegion, gunRot);
                float distFromMax = RedeHelper.AngularDifference(maxFiringRegion, gunRot);

                if (distFromMin < distFromMax)
                {
                    gunRot = minFiringRegion;
                }
                else
                {
                    gunRot = maxFiringRegion;
                }
            }
        }

        public void Teleport(bool specialPos, Vector2 teleportPos)
        {
            TeleGlow = true;
            TeleGlowTimer = 0;
            TeleVector = NPC.Center;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!specialPos)
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            Vector2 newPos = new(Main.rand.Next(-400, -250), Main.rand.Next(-200, 50));
                            NPC.Center = Main.player[NPC.target].Center + newPos;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            Vector2 newPos2 = new(Main.rand.Next(250, 400), Main.rand.Next(-200, 50));
                            NPC.Center = Main.player[NPC.target].Center + newPos2;
                            NPC.netUpdate = true;
                            break;
                    }
                }
                else
                {
                    NPC.Center = Main.player[NPC.target].Center + teleportPos;
                    NPC.netUpdate = true;
                }
            }
            TeleVector = NPC.Center;
            SoundEngine.PlaySound(SoundID.Item74, NPC.position);
            for (int i = 0; i < 30; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 3f);
                Main.dust[dustIndex].velocity *= 6f;
                Main.dust[dustIndex].noGravity = true;
            }
        }
        #endregion

        private int ArmsFrameY;
        private int ArmsFrameX;
        private int ArmsCounter;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 8;

            if (BodyState < (int)BodyAnim.IdlePhysical)
            {
                if (NPC.velocity.Length() < 13f)
                {
                    NPC.frame.Y = 0;
                    if (NPC.frameCounter++ >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.X += NPC.frame.Width;
                        if (NPC.frame.X > 3 * NPC.frame.Width)
                            NPC.frame.X = 0;
                    }
                }
                else
                {
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
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = Arms.Value.Height / 6;
            int width = Arms.Value.Width / 10;
            int y = height * ArmsFrameY;
            int x = width * ArmsFrameX;
            Rectangle ArmsRect = new(x, y, width, height);
            Vector2 ArmsOrigin = new(width / 2f, height / 2f);
            Vector2 ArmsPos = new(NPC.Center.X, NPC.Center.Y - 13);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);
            }
            if (BodyState < (int)BodyAnim.IdlePhysical)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(Arms.Value, oldPos - new Vector2(0, 13) + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(ArmsRect), NPC.GetAlpha(Color.LightCyan) * 0.5f, BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);
                }
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.LightCyan), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);
            spriteBatch.Draw(Glow.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:ScanShader"]?.GetShader().Shader;
            effect.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["alpha"].SetValue(1);
            effect.Parameters["red"].SetValue(new Color(0.1f, 1f, 0.7f, 1).ToVector4());
            effect.Parameters["red2"].SetValue(new Color(0.1f, 1f, 0.7f, 0.9f).ToVector4());

            effect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);

            spriteBatch.End();
            spriteBatch.BeginDefault();

            if (BodyState < (int)BodyAnim.IdlePhysical)
            {
                spriteBatch.Draw(Arms.Value, ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.GetAlpha(Color.LightCyan),
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);

                spriteBatch.Draw(ArmsGlow.Value, ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.GetAlpha(Color.White),
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                effect = Terraria.Graphics.Effects.Filters.Scene["MoR:ScanShader"]?.GetShader().Shader;
                effect.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                effect.Parameters["alpha"].SetValue(1);
                effect.Parameters["red"].SetValue(new Color(0.1f, 1f, 0.7f, 1).ToVector4());
                effect.Parameters["red2"].SetValue(new Color(0.1f, 1f, 0.7f, 0.9f).ToVector4());

                effect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(Arms.Value, ArmsPos - screenPos, new Rectangle?(ArmsRect), NPC.GetAlpha(Color.LightCyan),
                    BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd ? NPC.rotation :
                    gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), ArmsOrigin, NPC.scale, effects, 0);

                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.BeginAdditive();

            Texture2D teleportGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Rectangle rect = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position = TeleVector - screenPos;
            Color colour = Color.Lerp(Color.White, Color.Cyan, 1f / TeleGlowTimer * 10f) * (1f / TeleGlowTimer * 10f);
            if (TeleGlow)
            {
                spriteBatch.Draw(teleportGlow, position, new Rectangle?(rect), colour, NPC.rotation, origin, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(teleportGlow, position, new Rectangle?(rect), colour * 0.4f, NPC.rotation, origin, 2f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.BeginDefault();
        }
    }
}