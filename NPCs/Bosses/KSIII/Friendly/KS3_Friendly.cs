using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using Redemption.Projectiles.Melee;
using Redemption.Projectiles.Ranged;
using Redemption.Textures;
using Redemption.UI;
using Redemption.UI.ChatUI;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII.Friendly
{
    public class KS3_Friendly : KS3
    {
        public override string Texture => "Redemption/NPCs/Bosses/KSIII/KS3";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slayer III");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.UsesNewTargetting[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.damage *= 4;
            NPC.npcSlots = 0f;
            NPC.friendly = true;
            NPC.value = 0;
            NPC.boss = false;
            NPC.alpha = 255;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Psychic] *= 1.25f;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) { }

        public override void OnKill()
        {
            NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ProjectileType<KS3_Exit>(), 0, Vector2.Zero);
        }

        private static Texture2D Bubble => !Main.dedServ ? CommonTextures.TextBubble_Slayer.Value : null;
        private static readonly SoundStyle voice = CustomSounds.Voice6 with { Pitch = 0.2f };
        public override bool CheckActive()
        {
            return false;
        }

        bool playerDeadLine;
        public override bool PreAI()
        {
            Lighting.AddLight(NPC.Center, .3f, .6f, .8f);

            SetPlayerTarget();
            Player player = GetPlayerTarget();
            int gotNPC = NPC.FindFirstNPC(NPCID.MoonLordCore);
            if (gotNPC == -1 || (gotNPC >= 0 && Main.npc[gotNPC].dontTakeDamage))
                gotNPC = NPC.FindFirstNPC(NPCID.MoonLordHead);
            if (gotNPC == -1 || (gotNPC >= 0 && Main.npc[gotNPC].dontTakeDamage))
                gotNPC = NPC.FindFirstNPC(NPCID.MoonLordHand);
            if (gotNPC == -1 || (gotNPC >= 0 && Main.npc[gotNPC].dontTakeDamage))
                gotNPC = FindSecondNPC(NPCID.MoonLordHand);
            if (gotNPC != -1 && !Main.npc[gotNPC].dontTakeDamage)
            {
                NPC.target = Main.npc[gotNPC].WhoAmIToTargettingIndex;
            }
            else
                NPC.target = player.whoAmI;
            Entity attacker = Attacker();

            chance = MathHelper.Clamp(chance, 0, 1);

            Vector2 GunOrigin = NPC.Center + RedeHelper.PolarVector(54, gunRot) + RedeHelper.PolarVector(13 * NPC.spriteDirection, gunRot - (float)Math.PI / 2);
            HeadRotation = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

            if (NPC.downedMoonlord && AIState is not ActionState.FriendlyWin && !NPC.AnyNPCs(NPCID.MoonLordCore))
            {
                playerDeadLine = false;
                NPC.velocity *= 0;
                Teleport(false, Vector2.Zero);
                AITimer = 0;
                AIState = ActionState.FriendlyWin;
                NPC.netUpdate = true;
                return false;
            }
            if (NPC.MoonLordCountdown < 0 && !NPC.AnyNPCs(NPCID.MoonLordCore) || AIState is ActionState.GunAttacks or ActionState.SpecialAttacks or ActionState.PhysicalAttacks && !NPC.AnyNPCs(NPCID.MoonLordCore))
            {
                NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ProjectileType<KS3_Exit>(), 0, Vector2.Zero);
                NPC.active = false;
            }

            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                        DustHelper.DrawDustImage(NPC.Center, 92, 0.2f, "Redemption/Effects/DustImages/WarpShape", 3, true, 0);
                        for (int i = 0; i < 30; i++)
                        {
                            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, 0f, 0f, 100, default, 3f);
                            Main.dust[dustIndex].velocity *= 6f;
                            Main.dust[dustIndex].noGravity = true;
                        }
                    }
                    NPC.LookAtEntity(player);
                    BodyState = (int)BodyAnim.Crossed;
                    player.RedemptionScreen().Rumble(5, 5);
                    TeleVector = NPC.Center;
                    TeleGlow = true;
                    if (AITimer > 5)
                    {
                        NPC.alpha = 0;
                        player.Redemption().yesChoice = false;
                        player.Redemption().noChoice = false;

                        AITimer = 0;
                        AIState = ActionState.Dialogue;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Dialogue:
                    #region Dialogue Moment
                    if (NPC.DistanceSQ(player.Center) >= 500 * 500)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;

                    NPC.LookAtEntity(player);
                    AITimer++;
                    if (AITimer == 30 && !Main.dedServ)
                    {
                        string line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.1");
                        if (player.dead)
                        {
                            playerDeadLine = true;
                            line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.1Dead");
                        }

                        DialogueChain chain = new();
                        chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 4f, .5f, true, null, Bubble, null, modifier, 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer > 60 && player.dead && !playerDeadLine)
                    {
                        ChatUI.Visible = false;
                        ChatUI.Clear();

                        string line = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.1DiedAfter");
                        DialogueChain chain = new();
                        chain.Add(new(NPC, line, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                        playerDeadLine = true;
                    }
                    if (AITimer > 60 && !player.dead && playerDeadLine)
                    {
                        ChatUI.Visible = false;
                        ChatUI.Clear();

                        string line = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.1");
                        DialogueChain chain = new();
                        chain.Add(new(NPC, line, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                        playerDeadLine = false;
                    }
                    if (AITimer >= 120 && NPC.AnyNPCs(NPCID.MoonLordCore))
                    {
                        ChatUI.Visible = false;
                        ChatUI.Clear();

                        if (player.dead)
                        {
                            NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ProjectileType<KS3_Exit>(), 0, Vector2.Zero);
                            NPC.active = false;
                            break;
                        }

                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.TooLate"), new Color(170, 255, 255), Color.Black, voice, .01f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);

                        ArmsFrameY = 1;
                        ArmsFrameX = 0;
                        BodyState = (int)BodyAnim.Gun;

                        HeadType = 0;
                        AITimer = 0;
                        AIState = ActionState.GunAttacks;
                        NPC.netUpdate = true;
                    }
                    if (AITimer > 4000)
                    {
                        AITimer = 0;
                        AIState = ActionState.PhaseChange;
                        NPC.netUpdate = true;
                    }
                    #endregion
                    break;
                case ActionState.PhaseChange:
                    if (NPC.DistanceSQ(player.Center) >= 500 * 500)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;

                    NPC.LookAtEntity(player);
                    YesNoUI.DisplayYesNoButtons(player, Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.Accept"), Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.Decline"), new Vector2(0, 28), new Vector2(0, 28), .6f, .6f);
                    if (player.Redemption().yesChoice)
                    {
                        ChatUI.Visible = false;
                        ChatUI.Clear();

                        AITimer = 0;
                        AIState = ActionState.FriendlyAccept;
                        NPC.netUpdate = true;
                    }
                    else if (player.Redemption().noChoice)
                    {
                        ChatUI.Visible = false;
                        ChatUI.Clear();

                        AITimer = 0;
                        AIState = ActionState.FriendlyDecline;
                        NPC.netUpdate = true;
                    }
                    if (AITimer++ >= 120 && NPC.AnyNPCs(NPCID.MoonLordCore))
                    {
                        YesNoUI.Visible = false;
                        player.Redemption().yesChoice = false;
                        player.Redemption().noChoice = false;

                        if (player.dead)
                        {
                            NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ProjectileType<KS3_Exit>(), 0, Vector2.Zero);
                            NPC.active = false;
                            break;
                        }

                        ChatUI.Visible = false;
                        ChatUI.Clear();

                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.TooLate2"), new Color(170, 255, 255), Color.Black, voice, .01f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);

                        ArmsFrameY = 1;
                        ArmsFrameX = 0;
                        BodyState = (int)BodyAnim.Gun;

                        HeadType = 0;
                        AITimer = 0;
                        AIState = ActionState.GunAttacks;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.FriendlyDecline:
                    NPC.LookAtEntity(player);
                    if (AITimer++ == 5)
                    {
                        ChatUI.Visible = false;
                        ChatUI.Clear();

                        string line = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.Decline");
                        DialogueChain chain = new();
                        chain.Add(new(NPC, line, new Color(170, 255, 255), Color.Black, voice, .02f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 245)
                    {
                        NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ProjectileType<KS3_Exit>(), 0, Vector2.Zero);
                        NPC.active = false;
                    }
                    break;
                case ActionState.FriendlyAccept:
                    if (NPC.DistanceSQ(player.Center) >= 500 * 500)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;

                    NPC.LookAtEntity(player);
                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

                    if (AITimer++ == 5)
                    {
                        ChatUI.Visible = false;
                        ChatUI.Clear();

                        string line = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.Accept");
                        DialogueChain chain = new();
                        chain.Add(new(NPC, line, new Color(170, 255, 255), Color.Black, voice, .02f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (NPC.AnyNPCs(NPCID.MoonLordCore))
                    {
                        ArmsFrameY = 1;
                        ArmsFrameX = 0;
                        BodyState = (int)BodyAnim.Gun;

                        HeadType = 0;
                        AITimer = 0;
                        AIState = ActionState.GunAttacks;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.GunAttacks:
                    if (AttackChoice != 2 || AITimer <= 200)
                        NPC.LookAtEntity(attacker);

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
                                    if (Main.rand.NextBool())
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
                            if (NPC.HasPlayerTarget)
                                gunRot.SlowRotation(MathHelper.PiOver4, (float)Math.PI / 60f);
                            else
                                gunRot.SlowRotation(NPC.DirectionTo(attacker.Center).ToRotation(), (float)Math.PI / 60f);

                            SnapGunToFiringArea();
                            if (AITimer++ % 20 == 0)
                                ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(attacker), Main.rand.Next(-60, 60));

                            NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) < 100 * 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 12f, 14f);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (AITimer % 10 == 0 && !NPC.HasPlayerTarget)
                            {
                                int proj = Shoot(GunOrigin, ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(8, gunRot), CustomSounds.Gun1KS);
                                Main.projectile[proj].damage *= NPCHelper.HostileProjDamageMultiplier();
                                Main.projectile[proj].hostile = false;
                                Main.projectile[proj].friendly = true;
                                Main.projectile[proj].Redemption().friendlyHostile = false;
                                Main.projectile[proj].tileCollide = true;
                                Main.projectile[proj].extraUpdates = 1;
                                Main.projectile[proj].netUpdate = true;

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
                            NPC.Redemption().ignoreNewTargeting = true;
                            if (AITimer < 145)
                            {
                                if (NPC.HasPlayerTarget)
                                    gunRot.SlowRotation(MathHelper.PiOver4, (float)Math.PI / 60f);
                                else
                                    gunRot.SlowRotation(NPC.DirectionTo(attacker.Center + attacker.velocity * 20f).ToRotation(), (float)Math.PI / 30f);
                            }

                            SnapGunToFiringArea();
                            AITimer++;

                            ShootPos = new Vector2(300 * NPC.RightOfDir(attacker), 10);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (AITimer < 100)
                            {
                                if (NPC.Distance(ShootPos) < 100 || AITimer > 40)
                                {
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 13f, 14f);
                                }
                            }
                            else
                            {
                                if (AITimer < 145)
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
                                if (AITimer == 145 && !NPC.HasPlayerTarget)
                                {
                                    NPC.velocity.X = -9 * NPC.spriteDirection;
                                    for (int i = 0; i < Main.rand.Next(5, 8); i++)
                                    {
                                        int proj = Shoot(GunOrigin, ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(Main.rand.Next(8, 13), gunRot + Main.rand.NextFloat(-0.14f, 0.14f)), CustomSounds.ShotgunBlastKS);
                                        Main.projectile[proj].damage *= NPCHelper.HostileProjDamageMultiplier();
                                        Main.projectile[proj].hostile = false;
                                        Main.projectile[proj].friendly = true;
                                        Main.projectile[proj].Redemption().friendlyHostile = false;
                                        Main.projectile[proj].extraUpdates = 1;
                                        Main.projectile[proj].tileCollide = true;
                                        Main.projectile[proj].netUpdate = true;
                                    }
                                    BodyState = (int)BodyAnim.GunShoot;

                                }
                                if (AITimer > 185)
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
                            if (NPC.HasPlayerTarget)
                                gunRot.SlowRotation(MathHelper.PiOver4, (float)Math.PI / 60f);
                            else
                                gunRot.SlowRotation(NPC.DirectionTo(attacker.Center).ToRotation(), (float)Math.PI / 60f);

                            SnapGunToFiringArea();
                            AITimer++;
                            ShootPos = new Vector2(450 * NPC.RightOfDir(attacker), -10);
                            NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 12f, 14f);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            int startShot = 41;
                            if (AITimer >= startShot && AITimer % 3 == 0 && AITimer <= startShot + 15 && !NPC.HasPlayerTarget)
                            {
                                int proj = Shoot(GunOrigin, ProjectileType<ReboundShot>(), (int)(NPC.damage * .9f) * NPCHelper.HostileProjDamageMultiplier(), RedeHelper.PolarVector(15, gunRot), CustomSounds.Gun2KS);
                                Main.projectile[proj].hostile = false;
                                Main.projectile[proj].friendly = true;
                                Main.projectile[proj].Redemption().friendlyHostile = false;
                                Main.projectile[proj].extraUpdates = 1;
                                Main.projectile[proj].tileCollide = true;
                                Main.projectile[proj].netUpdate = true;

                                BodyState = (int)BodyAnim.GunShoot;
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
                            if (NPC.HasPlayerTarget)
                                gunRot.SlowRotation(MathHelper.PiOver4, (float)Math.PI / 60f);
                            else
                                gunRot.SlowRotation(NPC.DirectionTo(attacker.Center).ToRotation(), (float)Math.PI / 60f);

                            SnapGunToFiringArea();
                            if (AITimer++ % 20 == 0)
                                ShootPos = new Vector2(Main.rand.Next(300, 400) * NPC.RightOfDir(attacker), Main.rand.Next(-60, 60));

                            NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 12f, 14f);

                            if (BodyState < (int)BodyAnim.Gun || BodyState > (int)BodyAnim.GunEnd)
                            {
                                ArmsFrameY = 1;
                                ArmsFrameX = 0;
                                BodyState = (int)BodyAnim.Gun;
                            }

                            if (AITimer % 4 == 0 && !NPC.HasPlayerTarget)
                            {
                                int proj = Shoot(GunOrigin, ProjectileType<KS3_EnergyBolt>(), (int)(NPC.damage * .9f), RedeHelper.PolarVector(7, gunRot), CustomSounds.Gun1KS);
                                Main.projectile[proj].damage *= NPCHelper.HostileProjDamageMultiplier();
                                Main.projectile[proj].hostile = false;
                                Main.projectile[proj].friendly = true;
                                Main.projectile[proj].Redemption().friendlyHostile = false;
                                Main.projectile[proj].extraUpdates = 1;
                                Main.projectile[proj].tileCollide = true;
                                Main.projectile[proj].netUpdate = true;

                                BodyState = (int)BodyAnim.GunShoot;
                            }
                            if (AITimer >= 61)
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
                    if (AttackChoice != 3 || AITimer <= 120)
                        NPC.LookAtEntity(attacker);

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
                                    if (Main.rand.NextBool())
                                        AITimer = 2;
                                    else
                                        AITimer = 1;
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
                                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

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
                            ShootPos = new Vector2(300 * NPC.RightOfDir(attacker), -60);
                            AITimer++;
                            if (AITimer < 100)
                            {
                                if (NPC.Distance(ShootPos) < 160 || AITimer > 40)
                                {
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) < 100 ? 4f : NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 14f);
                            }
                            else
                            {
                                NPC.velocity *= 0.96f;

                                if (AITimer < 105 && NPC.HasPlayerTarget)
                                    AITimer = 150;

                                if (AITimer == 105)
                                    BodyState = (int)BodyAnim.RocketFist;

                                if (AITimer == 120)
                                    Shoot(new Vector2(NPC.Center.X + 15 * NPC.spriteDirection, NPC.Center.Y - 11), ProjectileType<KS3_FistF>(), NPC.damage * 2 * NPCHelper.HostileProjDamageMultiplier(), new Vector2(10 * NPC.spriteDirection, 0), CustomSounds.MissileFire1);

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
                            AttackChoice = Main.rand.Next(1, 10);
                            AITimer = 0;
                            NPC.netUpdate = true;
                            break;
                        #endregion

                        #region Beam Cell
                        case 3:
                            AttackChoice = Main.rand.Next(1, 10);
                            AITimer = 0;
                            NPC.netUpdate = true;
                            break;
                        #endregion

                        #region Core Surge
                        case 4:
                            AttackChoice = Main.rand.Next(1, 10);
                            AITimer = 0;
                            NPC.netUpdate = true;
                            break;
                        #endregion

                        #region Shrug It Off
                        case 5:
                            AttackChoice = Main.rand.Next(1, 10);
                            AITimer = 0;
                            NPC.netUpdate = true;
                            break;
                        #endregion

                        #region Deflect
                        case 6 or 7 or 8:
                            AttackChoice = Main.rand.Next(1, 10);
                            AITimer = 0;
                            NPC.netUpdate = true;
                            break;
                        #endregion

                        #region Missile Barrage
                        case 9:
                            if (AITimer == 0)
                            {
                                if (!RedeHelper.AnyProjectiles(ProjectileType<Hardlight_SoSCrosshair>()) && Main.rand.NextBool(4) && !NPC.HasPlayerTarget)
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
                                    Shoot(NPC.Center, ProjectileType<KS3_Call>(), 0, Vector2.Zero, CustomSounds.Alarm2);
                                    if (!RedeHelper.AnyProjectiles(ProjectileType<Hardlight_SoSCrosshair>()))
                                        Shoot(attacker.Center, ProjectileType<Hardlight_SoSCrosshair>(), (int)(NPC.damage * 1.8f) * NPCHelper.HostileProjDamageMultiplier(), Vector2.Zero);
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
                    if (NPC.HasPlayerTarget && AttackChoice != -1)
                    {
                        BodyState = (int)BodyAnim.IdlePhysical;

                        NPC.rotation = 0;
                        chance = 0;
                        AITimer = 0;
                        AttackChoice = -1;
                        NPC.netUpdate = true;
                    }
                    switch ((int)AttackChoice)
                    {
                        case -1:
                            NPC.LookAtEntity(attacker);
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
                                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;

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
                            NPC.Redemption().ignoreNewTargeting = true;
                            NPC.LookAtEntity(attacker);
                            if (AITimer++ <= 40)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                ShootPos = new Vector2(200 * NPC.RightOfDir(attacker), -60);
                                NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
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
                                ShootPos = new Vector2(attacker.velocity.X * 30, -600);
                                if (NPC.Center.Y < attacker.Center.Y - 600 || AITimer > 80)
                                {
                                    BodyState = (int)BodyAnim.Wheelkick;
                                    NPC.velocity *= 0.2f;
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                                else
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 34f : 26f, 3f);
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
                                    int proj = Shoot(NPC.Center, ProjectileType<KS3_Wave>(), NPC.damage * NPCHelper.HostileProjDamageMultiplier(), Vector2.Zero, SoundID.Item74);
                                    Main.projectile[proj].hostile = false;
                                    Main.projectile[proj].friendly = true;
                                    Main.projectile[proj].Redemption().friendlyHostile = false;
                                    Main.projectile[proj].netUpdate = true;

                                    NPC.velocity.Y += 40f;
                                }
                                if (AITimer >= 110)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 44 + 8 * NPC.spriteDirection, (int)NPC.Center.Y - 42 - 5, 88, 84);
                                    DamageInHitbox(Hitbox, (int)(NPC.damage * 1.5f), 4.5f, hitAnyPlayer: false);
                                }
                                if (AITimer > 130 || NPC.Center.Y > attacker.Center.Y + 400)
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
                                    ShootPos = new Vector2(200 * NPC.RightOfDir(attacker), -60);
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 30f : 22f, 8f);
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
                            NPC.Redemption().ignoreNewTargeting = true;
                            AITimer++;
                            if (AITimer == 1)
                                Teleport(false, Vector2.Zero);
                            if (AITimer < 100)
                            {
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                NPC.LookAtEntity(attacker);
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
                                    ShootPos = new Vector2(100 * NPC.RightOfDir(attacker), 0);
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
                                }
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.rotation = 0;
                                NPC.velocity *= 0.8f;
                                if (AITimer == 101)
                                    NPC.velocity.X = NPC.RightOfDir(attacker) * 6;

                                if (AITimer == 110)
                                    NPC.Dash(60, false, SoundID.Item74, attacker.Center);

                                if (AITimer >= 110 && AITimer <= 130)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 28 + 8 * NPC.spriteDirection, (int)NPC.Center.Y - 53 - 5, 56, 106);
                                    DamageInHitbox(Hitbox, NPC.damage * 2, 20f, hitAnyPlayer: false);
                                }

                                if (AITimer == 130)
                                {
                                    NPC.velocity.X = -15 * NPC.spriteDirection;
                                    BodyState = (int)BodyAnim.ShoulderBashEnd;
                                }
                                if (AITimer > 160)
                                {
                                    NPC.LookAtEntity(attacker);
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
                            NPC.Redemption().ignoreNewTargeting = true;
                            if (Main.expertMode)
                            {
                                NPC.LookAtEntity(attacker);
                                AITimer++;
                                if (AITimer < 100)
                                {
                                    NPC.rotation = 0;
                                    if (NPC.DistanceSQ(ShootPos) < 100 * 100 || AITimer > 50)
                                    {
                                        ShootPos = new Vector2(150 * NPC.RightOfDir(attacker), 200);
                                        AITimer = 100;
                                        NPC.velocity.X = 0;
                                        NPC.velocity.Y = -25;

                                        NPC.frame.X = 2 * NPC.frame.Width;
                                        NPC.frame.Y = 160;
                                        BodyState = (int)BodyAnim.DropkickStart;

                                        SoundEngine.PlaySound(SoundID.Item74, NPC.position);
                                        NPC.netUpdate = true;
                                    }
                                    else
                                        NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
                                }
                                else if (AITimer >= 100 && AITimer < 200)
                                {
                                    NPC.velocity *= 0.97f;
                                    if (NPC.velocity.Length() < 6 || AITimer > 160)
                                    {
                                        AITimer = 200;
                                        NPC.velocity *= 0f;
                                        BodyState = (int)BodyAnim.Dropkick;
                                        NPC.rotation = (attacker.Center + attacker.velocity * 20f - NPC.Center).ToRotation() + (float)(-Math.PI / 2);
                                        NPC.netUpdate = true;
                                    }
                                }
                                else if (AITimer >= 200)
                                {
                                    if (AITimer == 204)
                                        NPC.rotation = (attacker.Center + attacker.velocity * 20f - NPC.Center).ToRotation() + (float)(-Math.PI / 2);

                                    if (AITimer >= 205)
                                    {
                                        Rectangle Hitbox = new((int)NPC.Center.X - 29, (int)NPC.Center.Y - 59, 58, 118);
                                        DamageInHitbox(Hitbox, (int)(NPC.damage * 3f), hitAnyPlayer: false);
                                    }

                                    if (AITimer == 205)
                                        NPC.Dash(40, true, SoundID.Item74, attacker.Center + attacker.velocity * 20f);

                                    if (AITimer > 260 || NPC.Center.Y > attacker.Center.Y + 400)
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
                                chance -= Main.rand.NextFloat(0.2f, 0.5f);
                                NPC.frame.Y = 4 * 80;
                                if (NPC.frame.X < 4 * NPC.frame.Width)
                                    NPC.frame.X = 4 * NPC.frame.Width;
                                BodyState = (int)BodyAnim.IdlePhysical;
                                AttackChoice = -1;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Iron Pummel
                        case 4:
                            NPC.Redemption().ignoreNewTargeting = true;
                            AITimer++;
                            if (AITimer == 1 && NPC.DistanceSQ(attacker.Center) > 300 * 300)
                                Teleport(false, Vector2.Zero);
                            NPC.rotation = NPC.velocity.X * 0.01f;
                            ShootPos = new Vector2(60 * NPC.RightOfDir(attacker), 20);
                            if (AITimer < 100)
                            {
                                NPC.LookAtEntity(attacker);
                                if (NPC.DistanceSQ(attacker.Center + ShootPos) < 50 * 50 && gunRot != 0 || AITimer > 40)
                                {
                                    AITimer = 100;

                                    NPC.frameCounter = 0;
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
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
                            }
                            else if (AITimer >= 100)
                            {
                                NPC.velocity *= 0.9f;
                                if (AITimer == 105)
                                {
                                    gunRot += 1;
                                    NPC.Dash(10, false, CustomSounds.Swoosh1, attacker.Center);
                                }

                                if (AITimer >= 105 && AITimer <= 115)
                                {
                                    Rectangle Hitbox = new((int)NPC.Center.X - 6 + 28 * NPC.spriteDirection, (int)NPC.Center.Y - 6 - 18, 12, 12);
                                    DamageInHitbox(Hitbox, (int)(NPC.damage * 1.2f), hitAnyPlayer: false);
                                }

                                if (AITimer == 125 && RedeHelper.Chance(0.4f))
                                {
                                    AITimer = 140;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer > 125)
                                {
                                    NPC.LookAtEntity(attacker);
                                    NPC.Move(attacker.Center + ShootPos, NPC.DistanceSQ(attacker.Center) > 800 * 800 ? 20f : 17f, 5f);
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
                                        NPC.LookAtEntity(attacker);
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
                            NPC.Redemption().ignoreNewTargeting = true;
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
                                NPC.LookAtEntity(attacker);
                                NPC.rotation = NPC.velocity.X * 0.01f;
                                AITimer++;
                                ShootPos = new Vector2(80 * NPC.RightOfDir(attacker), 20);

                                if (AITimer == 5)
                                {
                                    NPC.frame.X = 7 * NPC.frame.Width;
                                    NPC.frame.Y = 160 * 2;
                                    BodyState = (int)BodyAnim.Jojo;
                                }

                                NPC.Move(attacker.Center + ShootPos, NPC.Distance(attacker.Center) > 300 ? 20f : 9f, 8f);
                                if (AITimer >= 15 && AITimer % 3 == 0)
                                {
                                    int proj = Shoot(NPC.Center, ProjectileType<KS3_JojoFist>(), NPC.damage * NPCHelper.HostileProjDamageMultiplier(), Vector2.Zero, SoundID.Item60 with { Volume = .3f });
                                    Main.projectile[proj].hostile = false;
                                    Main.projectile[proj].friendly = true;
                                    Main.projectile[proj].Redemption().friendlyHostile = false;
                                    Main.projectile[proj].netUpdate = true;
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
                case ActionState.FriendlyWin:
                    gunRot = NPC.spriteDirection == 1 ? 0f : (float)Math.PI;
                    BodyState = (int)BodyAnim.Idle;
                    if (NPC.DistanceSQ(player.Center) >= 300 * 300)
                        NPC.Move(player.Center, NPC.DistanceSQ(player.Center) > 800 * 800 ? 20f : 12f, 14f);
                    else
                        NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.01f;

                    NPC.LookAtEntity(player);
                    AITimer++;
                    if (AITimer == 80 && !Main.dedServ)
                    {
                        string line1 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.Win1");
                        string line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.Win2");
                        string line3 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.Win3");
                        if (RedeQuest.slayerRep >= 4)
                        {
                            line2 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.Win2Alt");
                            line3 = Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.Win3Alt");
                        }

                        DialogueChain chain = new();
                        chain.Add(new(NPC, line1, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier))
                             .Add(new(NPC, line2, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier))
                             .Add(new(NPC, line3, new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer > 100 && player.dead && !playerDeadLine && !Main.dedServ)
                    {
                        ChatUI.Visible = false;
                        ChatUI.Clear();

                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3.MLHelp.WinDied"), new Color(170, 255, 255), Color.Black, voice, .03f, 2f, .5f, true, null, Bubble, null, modifier, 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                        playerDeadLine = true;
                    }
                    if (AITimer > 5000)
                    {
                        RedeWorld.slayerMessageGiven = true;
                        RedeWorld.SyncData();

                        NPC.Shoot(new Vector2(NPC.Center.X - 60, NPC.Center.Y), ProjectileType<KS3_Exit>(), 0, Vector2.Zero);
                        NPC.active = false;
                    }
                    break;
            }
            #region Teleporting
            if (NPC.DistanceSQ(attacker.Center) >= 1100 * 1100 && NPC.ai[0] > 0 || NPC.DistanceSQ(player.Center) >= 1100 * 1100 && NPC.ai[0] is 1 or 5 or 14 or 15 or 16)
            {
                if (AttackChoice == 3 && AIState is ActionState.PhysicalAttacks)
                    return false;
                TeleportCount++;
                Teleport(false, Vector2.Zero);
                NPC.netUpdate = true;
            }
            #endregion
            return false;
        }
        public static int FindSecondNPC(int Type)
        {
            int count = 0;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == Type)
                {
                    if (count++ == 0)
                        continue;
                    return i;
                }
            }

            return -1;
        }
    }
}