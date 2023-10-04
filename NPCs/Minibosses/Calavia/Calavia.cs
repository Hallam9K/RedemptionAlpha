using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Items.Weapons.PreHM.Magic;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Redemption.WorldGeneration;
using Redemption.Base;
using System;
using Redemption.UI.ChatUI;
using ReLogic.Content;
using Terraria.Utilities;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Weapons.PreHM.Melee;
using Terraria.Localization;
using Redemption.Globals.NPC;
using Redemption.Textures;

namespace Redemption.NPCs.Minibosses.Calavia
{
    [AutoloadBossHead]
    public class Calavia : ModNPC
    {
        public static Asset<Texture2D> ShieldTex;
        public static Asset<Texture2D> CloakTex;
        public static Asset<Texture2D> LegsTex;
        public static Asset<Texture2D> ArmTex;
        public static Asset<Texture2D> ArmTex2;
        public static Asset<Texture2D> ShoulderTex;
        public static Asset<Texture2D> AltTex;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            ShieldTex = ModContent.Request<Texture2D>(Texture + "_Shield");
            CloakTex = ModContent.Request<Texture2D>(Texture + "_Cloak");
            LegsTex = ModContent.Request<Texture2D>(Texture + "_Legs");
            ArmTex = ModContent.Request<Texture2D>(Texture + "_Arm");
            ArmTex2 = ModContent.Request<Texture2D>(Texture + "_Arm2");
            ShoulderTex = ModContent.Request<Texture2D>(Texture + "_Shoulder");
            AltTex = ModContent.Request<Texture2D>(Texture + "2");
        }
        public override void Unload()
        {
            ShieldTex = null;
            CloakTex = null;
            LegsTex = null;
            ArmTex = null;
            ArmTex2 = null;
            ShoulderTex = null;
            AltTex = null;
        }
        public enum ActionState
        {
            Begin,
            JumpToOrigin,
            Walk,
            Slash,
            Bored,
            Stab,
            SpinSlash,
            Icefall,
            DrinkRecall,
            DrinkRandom,
            Defeat
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public ref float TimerRand2 => ref NPC.ai[3];
        public float[] oldrot = new float[6];
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Calavia");
            Main.npcFrameCount[Type] = 20;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Hot);
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Cold);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1 };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 48;
            NPC.damage = 40;
            NPC.defense = 23;
            NPC.lifeMax = 3000;
            NPC.knockBackResist = 0.2f;
            NPC.SpawnWithHigherTime(30);
            NPC.HitSound = SoundID.FemaleHit with { Pitch = .3f, Volume = .5f };
            NPC.DeathSound = SoundID.PlayerKilled with { Pitch = .1f };
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.noGravity = false;
            NPC.RedemptionGuard().GuardPoints = NPC.lifeMax / 10;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1");
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Fire] *= .8f;
        }
        private Rectangle ShieldHitbox;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Calavia"))
            });
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (hit.Damage > 1)
            {
                if (AIState is ActionState.Defeat && TimerRand > 0)
                    AITimer = 0;
                SoundEngine.PlaySound(SoundID.NPCHit4);
            }
            if (NPC.life <= 0 && AIState is ActionState.Defeat)
            {
                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Scale: 2);
                    Main.dust[dustIndex].velocity *= 2;
                }
                if (Main.netMode == NetmodeID.Server)
                    return;
                for (int i = 0; i < 5; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/CalaviaGore" + (i + 1)).Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Iron, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnKill()
        {
            if (!RedeBossDowned.downedCalavia)
            {
                RedeWorld.alignment -= 2;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, "-2", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.CalaviaKilled"), 180, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedCalavia, -1);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (RedeQuest.calaviaVar < 30)
            {
                RedeQuest.calaviaVar = 30;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BladeOfTheMountain>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Icefall>()));
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        private bool blocked;
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (potionCooldown[1] > 0)
                modifiers.Defense.Base += 8;
            if (AIState is ActionState.Defeat && TimerRand is 0)
                modifiers.FinalDamage /= 50;
            if (blocked && NPC.RedemptionGuard().GuardPoints >= 0)
            {
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.Tink, 0.1f, true, DustID.Iron, default, 10, 1, 200);
            }
            blocked = false;
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (NPC.RedemptionGuard().GuardBroken)
                return;

            if (NPC.RightOf(player) && NPC.spriteDirection == 1 || (player.RightOf(NPC) && NPC.spriteDirection == -1))
                return;

            if (item.noMelee || item.damage <= 0)
                return;

            if (player.Redemption().meleeHitbox.Intersects(ShieldHitbox))
            {
                if (NPC.RedemptionGuard().GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.Tink, 0.1f, true, DustID.Iron, default, 10, 1, 200);
                    blocked = false;
                }
            }
        }
        private readonly List<int> projBlocked = new();
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (NPC.RedemptionGuard().GuardBroken)
                return;

            Vector2 projImpact = projectile.Center - projectile.velocity;
            if (NPC.Center.RightOf(projImpact) && NPC.spriteDirection == 1 || (projImpact.RightOf(NPC.Center) && NPC.spriteDirection == -1))
                return;

            if (!projBlocked.Contains(projectile.whoAmI) && (!projectile.active || !projectile.friendly))
                return;

            Rectangle projectileHitbox = projectile.Hitbox;
            if (projectile.Redemption().swordHitbox != default)
                projectileHitbox = projectile.Redemption().swordHitbox;
            if (projectile.Colliding(projectileHitbox, ShieldHitbox))
            {
                projBlocked.Remove(projectile.whoAmI);
                if (!projectile.ProjBlockBlacklist() && projectile.penetrate > 1)
                    projectile.timeLeft = Math.Min(projectile.timeLeft, 2);
                if (NPC.RedemptionGuard().GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.Tink, 0.1f, true, DustID.Iron, default, 10, 1, 200);
                    blocked = false;
                }
            }
        }
        private static Texture2D Bubble => CommonTextures.TextBubble_Epidotra.Value;
        private static readonly SoundStyle voice = CustomSounds.Voice1 with { Pitch = 0.6f };
        private Vector2 origin;
        private int dodgeCooldown;
        private int boredomTimer;
        public float[] doorVars = new float[3];
        private readonly int[] potionCooldown = new int[3];
        private readonly int[] potionAmount = new int[3] { 1, 2, 2 };
        private bool Defeat;
        public override void AI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            for (int i = 0; i < 3; i++)
            {
                if (potionCooldown[i] > 0)
                    potionCooldown[i]--;
            }
            dodgeCooldown--;
            dodgeCooldown = (int)MathHelper.Max(0, dodgeCooldown);
            RegenCheck();

            ShieldHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 18 : NPC.Center.X), (int)(NPC.Center.Y - 2), 18, 24);
            if (!NPC.RedemptionGuard().GuardBroken && !NPC.dontTakeDamage)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (!projectile.active || !projectile.friendly || projectile.ProjBlockBlacklist())
                        continue;

                    Vector2 projImpact = projectile.Center - projectile.velocity;
                    if ((NPC.Center.RightOf(projImpact) && NPC.spriteDirection == -1 || projImpact.RightOf(NPC.Center) && NPC.spriteDirection == 1) && projectile.Colliding(projectile.Hitbox, ShieldHitbox))
                    {
                        if (!projectile.ProjBlockBlacklist() && projectile.penetrate != -1)
                        {
                            blocked = true;
                            NPC.SimpleStrikeNPC(projectile.damage, 1);
                            projectile.Kill();
                        }
                        if (!projBlocked.Contains(projectile.whoAmI))
                            projBlocked.Add(projectile.whoAmI);
                    }
                }
            }

            Player player = Main.player[NPC.target];
            if (NPC.DespawnHandler(1, 5))
                return;
            if (AIState < ActionState.Slash)
                NPC.LookAtEntity(player);

            if (!player.active || player.dead)
                return;

            if (NPC.life <= 300 && !Defeat)
            {
                Defeated();
                return;
            }

            float swiftness = potionCooldown[2] > 0 ? 1.25f : 1;
            switch (AIState)
            {
                case ActionState.Begin:
                    origin = NPC.Center;
                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<Calavia_IcefallArena>(), 0, Vector2.Zero, NPC.whoAmI);
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Calavia.Name"), 60, 90, 0.8f, 0, Color.LightCyan, Language.GetTextValue("Mods.Redemption.TitleCard.Calavia.Modifier"));
                    AIState = ActionState.JumpToOrigin;
                    NPC.netUpdate = true;
                    break;
                case ActionState.JumpToOrigin:
                    Vector2 landPos = new((RedeGen.gathicPortalVector.X + Main.rand.Next(38, 64)) * 16, (RedeGen.gathicPortalVector.Y + 21) * 16);
                    if (AITimer++ == 0)
                    {
                        if (landPos.DistanceSQ(NPC.Center) < 120 * 120 || !BaseAI.HitTileOnSide(NPC, 3))
                        {
                            TimerRand = Main.rand.Next(240, 300);
                            AITimer = 0;
                            AIState = ActionState.Walk;
                            break;
                        }
                        TimerRand = MathHelper.Distance(NPC.Center.X, landPos.X) / 16;
                        TimerRand = MathHelper.Clamp(TimerRand, -20, 20);
                        NPC.velocity = new Vector2(TimerRand / 3 * landPos.RightOfDir(NPC.Center), -TimerRand / 3);
                    }
                    if (AITimer < 4)
                    {
                        NPC.velocity.X = TimerRand / 4 * landPos.RightOfDir(NPC.Center);
                    }
                    if (BaseAI.HitTileOnSide(NPC, 3))
                    {
                        TimerRand2 = 0;
                        TimerRand = Main.rand.Next(240, 300);
                        AITimer = 0;
                        AIState = Main.rand.NextBool(4) ? ActionState.DrinkRandom : ActionState.Walk;
                    }
                    break;
                case ActionState.Walk:
                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform);
                    NPCHelper.HorizontallyMove(NPC, player.Center, 0.18f, 3 * swiftness, 18, 18, NPC.Center.Y > player.Center.Y, player);

                    if (NPC.RedemptionGuard().GuardBroken && dodgeCooldown <= 0 && BaseAI.HitTileOnSide(NPC, 3))
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile proj = Main.projectile[i];
                            if (!proj.active || !proj.friendly || proj.damage <= 0 || proj.sentry || proj.minion || proj.velocity.Length() == 0)
                                continue;

                            if (!NPC.Sight(proj, 100 + (proj.velocity.Length() * 5), true, true))
                                continue;

                            for (int l = 0; l < 10; l++)
                            {
                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke);
                                Main.dust[dust].velocity *= 0.2f;
                                Main.dust[dust].noGravity = true;
                            }
                            if (Main.rand.NextBool())
                                NPC.velocity.X *= -1;
                            NPC.velocity.X *= 2f;
                            NPC.velocity.Y -= Main.rand.NextFloat(1, 3);
                            dodgeCooldown = 30;
                            break;
                        }
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, 10, interactDoorStyle: 2);
                    Vector2 gathicPortalPos = new((RedeGen.gathicPortalVector.X + 47) * 16, (RedeGen.gathicPortalVector.Y + 20) * 16 + 8);
                    Rectangle tooHighCheck = new((int)NPC.Center.X - 60, (int)NPC.Center.Y - 300, 120, 300);
                    if (NPC.DistanceSQ(player.Center) < 180 * 180 || player.Hitbox.Intersects(tooHighCheck) || (AITimer >= 60 && !NPC.Sight(player, -1, false, true) && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, gathicPortalPos - new Vector2(8, 8), 16, 16)))
                    {
                        WeightedRandom<ActionState> attacks = new(Main.rand);
                        attacks.Add(ActionState.Slash);
                        attacks.Add(ActionState.Stab, NPC.DistanceSQ(player.Center) < 80 * 80 ? 0 : 0.5);
                        attacks.Add(ActionState.SpinSlash, 0.2);
                        attacks.Add(ActionState.Icefall, 0.3);
                        ActionState choice = attacks;
                        if (!NPC.Sight(player, -1, false, true, true))
                            choice = ActionState.Stab;
                        if (BaseAI.HitTileOnSide(NPC, 3))
                        {
                            AIState = choice;
                            AITimer = 0;
                            TimerRand = 0;
                            TimerRand2 = 0;
                            if (choice == ActionState.Slash && player.Center.Y + 60 < NPC.Center.Y)
                            {
                                AITimer = -60;
                                TimerRand = 1;
                            }
                        }
                        break;
                    }
                    if (!Collision.CanHitLine(NPC.position, NPC.width, NPC.height, gathicPortalPos - new Vector2(8, 8), 16, 16))
                        TimerRand2++;
                    if (TimerRand2 >= 200)
                    {
                        HoldPotionType = ItemID.RecallPotion;
                        potion = ModContent.Request<Texture2D>("Terraria/Images/Item_" + HoldPotionType, AssetRequestMode.ImmediateLoad).Value;
                        AITimer = 0;
                        TimerRand = 1;
                        TimerRand2 = 0;
                        AIState = ActionState.DrinkRecall;
                        break;
                    }
                    if (AITimer++ >= TimerRand && BaseAI.HitTileOnSide(NPC, 3))
                    {
                        AITimer = 0;
                        AIState = ActionState.JumpToOrigin;
                    }
                    break;
                case ActionState.Slash:
                    NPC.knockBackResist = 0;
                    BaseAI.WalkupHalfBricks(NPC);
                    switch (TimerRand)
                    {
                        default:
                            if (AITimer++ == 0)
                            {
                                CustomBodyAni = 2;
                                TimerRand2 = Main.rand.Next(20, 31);
                                int damage = NPC.RedemptionNPCBuff().disarmed ? (int)(NPC.damage * .75f) : NPC.damage;
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<Calavia_BladeOfTheMountain>(), damage, Vector2.Zero, NPC.whoAmI, TimerRand2);
                            }
                            if (AITimer < TimerRand2)
                            {
                                NPC.LookAtEntity(player);
                                NPC.velocity.X *= 0.5f;
                            }
                            else
                                NPC.velocity *= 0.94f;
                            if (AITimer == TimerRand2 + 10)
                            {
                                NPC.LookByVelocity();
                                if (NPC.DistanceSQ(player.Center) > 70 * 70)
                                    NPC.velocity.X += 12 * swiftness * NPC.spriteDirection;
                            }
                            if (AITimer >= TimerRand2 + 40)
                            {
                                NPC.knockBackResist = 0.2f;
                                CustomBodyAni = 0;
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2 = 0;
                                AIState = ActionState.JumpToOrigin;
                            }
                            break;
                        case 1:
                            if (AITimer < -20 && BaseAI.HitTileOnSide(NPC, 3))
                            {
                                NPC.velocity.Y -= MathHelper.Distance(NPC.Center.Y, player.Center.Y) / 8;
                                AITimer = -20;
                            }

                            if (AITimer++ == 0)
                            {
                                CustomBodyAni = 2;
                                TimerRand2 = Main.rand.Next(10, 21);
                                int damage = NPC.RedemptionNPCBuff().disarmed ? (int)(NPC.damage * .75f) : NPC.damage;
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<Calavia_BladeOfTheMountain>(), damage, Vector2.Zero, NPC.whoAmI, TimerRand2);
                            }
                            if (AITimer >= 0 && AITimer < TimerRand2 + 10)
                            {
                                if (AITimer < TimerRand2)
                                    NPC.LookAtEntity(player);
                                NPC.velocity.Y = -.51f;
                                NPC.velocity *= 0.6f;
                            }
                            else
                                NPC.velocity *= 0.94f;
                            if (AITimer == TimerRand2 + 10)
                            {
                                NPC.LookByVelocity();
                                if (NPC.DistanceSQ(player.Center) > 70 * 70)
                                    NPC.velocity.X += 18 * swiftness * NPC.spriteDirection;
                            }
                            if (AITimer >= TimerRand2 + 40)
                            {
                                NPC.knockBackResist = 0.2f;
                                CustomBodyAni = 0;
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2 = 0;
                                AIState = ActionState.JumpToOrigin;
                            }
                            break;
                    }
                    break;
                case ActionState.Stab:
                    NPC.knockBackResist = 0;
                    BaseAI.WalkupHalfBricks(NPC);
                    switch (TimerRand)
                    {
                        default:
                            NPC.LookAtEntity(player);
                            if (AITimer++ == 0)
                            {
                                NPC.velocity *= 0;
                                customArm = true;
                                int damage = NPC.RedemptionNPCBuff().disarmed ? (int)(NPC.damage * .75f) : NPC.damage;
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<Calavia_BladeOfTheMountain2>(), damage, Vector2.Zero, NPC.whoAmI, TimerRand2);
                            }
                            break;
                        case 1:
                            NPC.LookByVelocity();
                            NPC.rotation = NPC.velocity.X * 0.07f;
                            dodgeCooldown = 20;
                            NPC.noGravity = true;
                            if (AITimer++ == 0 && !Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swoosh1, NPC.position);
                            if (AITimer >= 20)
                                TimerRand = 2;
                            break;
                        case 2:
                            NPC.noGravity = false;
                            customArm = false;
                            customArmRot = 0;
                            dodgeCooldown = 30;
                            AITimer = 0;
                            TimerRand = 0;
                            TimerRand2 = 0;
                            AIState = ActionState.JumpToOrigin;
                            break;
                    }
                    break;
                case ActionState.SpinSlash:
                    NPC.knockBackResist = 0;
                    BaseAI.WalkupHalfBricks(NPC);
                    if (AITimer++ == 0)
                    {
                        NPC.velocity *= 0;
                        customArm = true;
                        int damage = NPC.RedemptionNPCBuff().disarmed ? (int)(NPC.damage * .75f) : NPC.damage;
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Calavia_BladeOfTheMountain2>(), damage, Vector2.Zero, NPC.whoAmI, 3);
                    }
                    if (AITimer == 100)
                        NPC.velocity.Y = -12;
                    if (AITimer >= 120)
                    {
                        dodgeCooldown = 20;
                        NPC.rotation += 0.5f * NPC.spriteDirection;
                        NPC.velocity.Y = -.49f;
                        NPC.velocity.X += 0.04f * NPC.spriteDirection;
                    }
                    else
                    {
                        NPC.LookAtEntity(player);
                        NPC.velocity *= 0.94f;
                    }
                    if (AITimer >= 240)
                    {
                        NPC.noGravity = false;
                        customArm = false;
                        customArmRot = 0;
                        AITimer = 0;
                        TimerRand = 0;
                        TimerRand2 = 0;
                        AIState = ActionState.JumpToOrigin;
                    }
                    break;
                case ActionState.Icefall:
                    BaseAI.WalkupHalfBricks(NPC);
                    NPC.velocity.X *= .96f;
                    if (AITimer++ == 0)
                    {
                        NPC.velocity *= 0;
                        HoldIcefall = true;
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.IceMist, NPC.position);
                    }
                    if (AITimer == 30)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            NPC.Shoot(NPC.Center + new Vector2(100, -10), ModContent.ProjectileType<Calavia_IcefallMist>(), NPC.damage, new Vector2(Main.rand.NextFloat(-1, 1), 0));
                            NPC.Shoot(NPC.Center + new Vector2(-100, -10), ModContent.ProjectileType<Calavia_IcefallMist>(), NPC.damage, new Vector2(Main.rand.NextFloat(-1, 1), 0));
                        }
                    }
                    if (AITimer >= 80)
                    {
                        HoldIcefall = false;
                        AIState = ActionState.Slash;
                        AITimer = 0;
                        TimerRand = 0;
                        TimerRand2 = 0;
                    }
                    break;
                case ActionState.Bored:
                    NPC.LookByVelocity();
                    NPC.dontTakeDamage = true;
                    if (AITimer++ == 0)
                    {
                        NPC.velocity.X *= 0;
                        string s1 = "Mur yeborti?"; // You concede/give up/retreat?
                        string s2 = "Gorhal'on...";

                        DialogueChain chain = new();
                        chain.Add(new(NPC, s1, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble, endID: 1))
                             .Add(new(NPC, s2, Color.White, Color.Gray, voice, .05f, 2f, .5f, true, bubble: Bubble, endID: 2));
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    gathicPortalPos = new((RedeGen.gathicPortalVector.X + 47) * 16, (RedeGen.gathicPortalVector.Y + 20) * 16 + 8);
                    if (TimerRand > 0)
                    {
                        if (NPC.DistanceSQ(gathicPortalPos) < 6 * 6)
                            NPC.velocity.X *= .4f;
                        else
                        {
                            BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, 10, interactDoorStyle: 2);
                            NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 12, gathicPortalPos.Y);
                            NPCHelper.HorizontallyMove(NPC, gathicPortalPos, 0.18f, 3, 18, 18, NPC.Center.Y > gathicPortalPos.Y);
                            if (!Collision.CanHitLine(NPC.position, NPC.width, NPC.height, gathicPortalPos - Vector2.One, 2, 2))
                                TimerRand2++;
                            if (TimerRand2 >= 180)
                            {
                                HoldPotionType = ItemID.RecallPotion;
                                potion = ModContent.Request<Texture2D>("Terraria/Images/Item_" + HoldPotionType, AssetRequestMode.ImmediateLoad).Value;
                                AITimer = 0;
                                TimerRand = 0;
                                TimerRand2 = 0;
                                AIState = ActionState.DrinkRecall;
                            }
                        }
                    }
                    if (TimerRand is 2 && NPC.DistanceSQ(gathicPortalPos) < 6 * 6)
                    {
                        NPC.Center = gathicPortalPos;
                        NPC.velocity *= 0;
                        NPC.SetDefaults(ModContent.NPCType<Calavia_Intro>());
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.DrinkRecall:
                    NPC.velocity.X *= .5f;
                    if (AITimer++ == 0)
                    {
                        HoldPotionPos = 14;
                        CustomBodyCounter = 0;
                        CustomBodyAni = 1;
                        SoundEngine.PlaySound(SoundID.Item3, NPC.position);
                    }
                    HoldPotionOriginX = 10;
                    HoldPotionPos--;
                    TimerRand2 -= 0.12f * NPC.spriteDirection;
                    if (AITimer >= 16)
                    {
                        HoldPotionType = 0;
                        if (TimerRand is 1)
                        {
                            CustomBodyAni = 0;
                            TimerRand2 = 0;
                            AITimer = 0;
                        }
                        for (int i = 0; i < 30; i++)
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                        gathicPortalPos = new((RedeGen.gathicPortalVector.X + 47) * 16, (RedeGen.gathicPortalVector.Y + 20) * 16 + 8);
                        NPC.Center = gathicPortalPos;
                        NPC.velocity *= 0;
                        if (TimerRand is 0)
                            NPC.SetDefaults(ModContent.NPCType<Calavia_Intro>());
                        else if (TimerRand is 2)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (RedeQuest.calaviaVar < 3)
                                    RedeQuest.calaviaVar = 3;
                                RedeBossDowned.downedCalavia = true;
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            Main.BestiaryTracker.Kills.RegisterKill(NPC);
                            NPC.SetDefaults(ModContent.NPCType<Calavia_NPC>());
                        }
                        else
                        {
                            TimerRand = 0;
                            AIState = ActionState.Walk;
                        }
                        NPC.netUpdate = true;
                        for (int i = 0; i < 30; i++)
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                    }
                    break;
                case ActionState.DrinkRandom:
                    NPC.velocity.X *= .5f;
                    if (AITimer++ == 0)
                    {
                        List<int> potionType = new();
                        for (int i = 0; i < 3; i++)
                        {
                            if (potionAmount[i] > 0 && potionCooldown[i] <= 0)
                            {
                                int p = i switch
                                {
                                    1 => ItemID.IronskinPotion,
                                    2 => ItemID.SwiftnessPotion,
                                    _ => ItemID.RegenerationPotion,
                                };
                                potionType.Add(p);
                            }
                        }
                        if (potionType == null || potionType.Count is 0)
                        {
                            TimerRand = Main.rand.Next(240, 300);
                            AITimer = 0;
                            AIState = ActionState.Walk;
                            break;
                        }
                        HoldPotionPos = 10;
                        HoldPotionType = Utils.Next(Main.rand, potionType);
                        potion = ModContent.Request<Texture2D>("Terraria/Images/Item_" + HoldPotionType, AssetRequestMode.ImmediateLoad).Value;

                        CustomBodyCounter = 0;
                        CustomBodyAni = 1;
                        SoundEngine.PlaySound(SoundID.Item3, NPC.position);

                        switch (HoldPotionType)
                        {
                            default:
                                potionAmount[0]--;
                                potionCooldown[0] = 3600 * 8;
                                HoldPotionOriginX = 6;
                                break;
                            case ItemID.IronskinPotion:
                                potionAmount[1]--;
                                potionCooldown[1] = 3600 * 8;
                                HoldPotionOriginX = 6;
                                break;
                            case ItemID.SwiftnessPotion:
                                potionAmount[2]--;
                                potionCooldown[2] = 3600 * 8;
                                HoldPotionOriginX = 6;
                                break;
                        }
                    }
                    HoldPotionPos--;
                    TimerRand2 -= 0.12f * NPC.spriteDirection;
                    if (AITimer >= 16)
                    {
                        HoldPotionType = 0;
                        CustomBodyAni = 0;
                        TimerRand2 = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(240, 300);
                        AIState = ActionState.Walk;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Defeat:
                    switch (TimerRand)
                    {
                        default:
                            NPC.velocity.X *= .8f;
                            NPC.chaseable = false;
                            if (AITimer++ == 0)
                            {
                                NPC.LookAtEntity(player);
                                NPC.velocity = new Vector2(-4 * NPC.spriteDirection, -3);

                                if (!NPC.RedemptionGuard().GuardBroken)
                                {
                                    if (Main.netMode != NetmodeID.Server)
                                    {
                                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/CalaviaShieldGore1").Type, 1);
                                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/CalaviaShieldGore2").Type, 1);
                                    }
                                    NPC.RedemptionGuard().GuardPoints = 0;
                                    NPC.RedemptionGuard().GuardBroken = true;
                                }
                                if (Main.netMode != NetmodeID.Server)
                                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/CalaviaHelmGore1").Type, 1);
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.GuardBreak, NPC.position);

                                string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Fight.Defeat1");
                                string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Fight.Defeat2");
                                if (player.MinionAttackTargetNPC == NPC.whoAmI)
                                    s1 += Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Fight.Defeat3");
                                DialogueChain chain = new();
                                chain.Add(new(NPC, s1, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                                     .Add(new(NPC, s2, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble));
                                ChatUI.Visible = true;
                                ChatUI.Add(chain);
                            }
                            if (AITimer >= 180)
                            {
                                if (RedeWorld.alignmentGiven && !Main.dedServ && !RedeBossDowned.downedCalavia)
                                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.CalaviaChoice"), 180, 30, 0, Color.DarkGoldenrod);

                                AITimer = 0;
                                TimerRand = 1;
                            }
                            break;
                        case 1:
                            NPC.LookAtEntity(player);
                            NPC.velocity.X *= .8f;
                            if (AITimer++ >= 400)
                            {
                                NPC.dontTakeDamage = true;
                                string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Fight.Mercy1");
                                string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Fight.Mercy2");
                                string s3 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Fight.Mercy3");

                                DialogueChain chain = new();
                                chain.Add(new(NPC, s1, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                                     .Add(new(NPC, s2, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                                     .Add(new(NPC, s3, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble, endID: 3));
                                chain.OnEndTrigger += Chain_OnEndTrigger;
                                ChatUI.Visible = true;
                                ChatUI.Add(chain);
                                AITimer = 0;
                                TimerRand = 2;
                            }
                            break;
                        case 2:
                            NPC.LookAtEntity(player);
                            if (AITimer++ >= 1200)
                            {
                                AITimer = 0;
                                TimerRand = 3;
                            }
                            break;
                        case 3:
                            NPC.LookByVelocity();
                            NPC.dontTakeDamage = true;
                            if (AITimer++ < 90)
                                break;

                            gathicPortalPos = new((RedeGen.gathicPortalVector.X + 47) * 16, (RedeGen.gathicPortalVector.Y + 20) * 16 + 8);
                            if (NPC.DistanceSQ(gathicPortalPos) < 6 * 6)
                                NPC.velocity.X *= .4f;
                            else
                            {
                                BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, 10, interactDoorStyle: 2);
                                NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 12, gathicPortalPos.Y);
                                NPCHelper.HorizontallyMove(NPC, gathicPortalPos, 0.18f, 3, 18, 18, NPC.Center.Y > gathicPortalPos.Y);
                                if (!Collision.CanHitLine(NPC.position, NPC.width, NPC.height, gathicPortalPos - Vector2.One, 2, 2))
                                    TimerRand2++;
                                if (TimerRand2 >= 180)
                                {
                                    HoldPotionType = ItemID.RecallPotion;
                                    potion = ModContent.Request<Texture2D>("Terraria/Images/Item_" + HoldPotionType, AssetRequestMode.ImmediateLoad).Value;
                                    AITimer = 0;
                                    TimerRand = 2;
                                    TimerRand2 = 0;
                                    AIState = ActionState.DrinkRecall;
                                }
                            }
                            if (NPC.DistanceSQ(gathicPortalPos) < 6 * 6)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    if (RedeQuest.calaviaVar < 3)
                                        RedeQuest.calaviaVar = 3;
                                    RedeBossDowned.downedCalavia = true;
                                    if (Main.netMode != NetmodeID.SinglePlayer)
                                        NetMessage.SendData(MessageID.WorldData);
                                }

                                NPC.Center = gathicPortalPos;
                                NPC.velocity *= 0;
                                Main.BestiaryTracker.Kills.RegisterKill(NPC);
                                NPC.SetDefaults(ModContent.NPCType<Calavia_NPC>());
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    if (TimerRand < 3)
                        ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.None);
                    break;
            }
            if (AIState != ActionState.Bored && AIState != ActionState.DrinkRecall && !Defeat && player.DistanceSQ(origin) >= 600 * 600)
            {
                if (boredomTimer++ > 240)
                {
                    HoldIcefall = false;
                    NPC.noGravity = false;
                    customArm = false;
                    AITimer = 0;
                    TimerRand = 0;
                    AIState = ActionState.Bored;
                }
            }
            else
                boredomTimer--;
            boredomTimer = (int)MathHelper.Max(0, boredomTimer);
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            if (NPC.type == ModContent.NPCType<Calavia>())
            {
                if (ID is 3)
                    AITimer = 0;
                TimerRand = ID;
            }
        }
        int regenTimer;
        public void RegenCheck()
        {
            if (potionCooldown[0] > 0)
            {
                if (regenTimer++ % 30 == 0 && NPC.life < NPC.lifeMax)
                    NPC.life += 1;
            }
        }
        private void Defeated()
        {
            for (int k = 0; k < NPC.buffImmune.Length; k++)
                NPC.BecomeImmuneTo(k);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.ClearImmuneToBuffs(out _);

            HoldIcefall = false;
            Defeat = true;
            NPC.noGravity = false;
            customArm = false;
            NPC.life = 300;
            AITimer = 0;
            TimerRand = 0;
            TimerRand2 = 0;
            AIState = ActionState.Defeat;
            NPC.netUpdate = true;
        }
        public override bool CheckDead()
        {
            if (Defeat)
                return true;
            else
            {
                Defeated();
                return false;
            }
        }
        private Texture2D potion = null;
        public int BodyFrame;
        public int CustomBodyAni;
        private int CustomBodyCounter;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0)
            {
                if (AIState is not ActionState.SpinSlash || AITimer < 120)
                    NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= 19 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                }
            }
            else
            {
                if (AIState is not ActionState.SpinSlash || AITimer < 120)
                    NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 5 * frameHeight;
            }

            switch (CustomBodyAni)
            {
                default:
                    BodyFrame = NPC.frame.Y;
                    break;
                case 1:
                    if (CustomBodyCounter++ == 0)
                        BodyFrame = 4 * frameHeight;
                    if (CustomBodyCounter != 0 && CustomBodyCounter % 8 == 0)
                    {
                        BodyFrame -= frameHeight;
                        if (BodyFrame < 3 * frameHeight)
                        {
                            BodyFrame = 3 * frameHeight;
                            CustomBodyAni = 0;
                            CustomBodyCounter = 0;
                        }
                    }
                    break;
            }
            if (HoldIcefall)
                BodyFrame = 2 * frameHeight;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override bool? CanBeHitByItem(Player player, Item item) => dodgeCooldown <= 20 ? null : false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => dodgeCooldown <= 20 ? null : false;
        private int HoldPotionType;
        private int HoldPotionPos = 14;
        private int HoldPotionOriginX = 10;
        private bool customArm;
        public float customArmRot;
        private bool HoldIcefall;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rect = new(0, BodyFrame, NPC.frame.Width, NPC.frame.Height);

            spriteBatch.Draw(CloakTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(LegsTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Defeat ? AltTex.Value : TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (!NPC.RedemptionGuard().GuardBroken)
                spriteBatch.Draw(ShieldTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (HoldIcefall)
                spriteBatch.Draw(TextureAssets.Item[ModContent.ItemType<Icefall>()].Value, NPC.Center + new Vector2(14 * NPC.spriteDirection, 0) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (HoldPotionType != 0 && potion != null)
                spriteBatch.Draw(potion, NPC.Center + new Vector2(14 * NPC.spriteDirection, HoldPotionPos) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation + TimerRand2, (NPC.frame.Size() / 2) - new Vector2(HoldPotionOriginX, 10), NPC.scale, effects, 0);
            if (customArm)
                spriteBatch.Draw(ArmTex2.Value, NPC.Center + new Vector2(-8 * NPC.spriteDirection, 0) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation + customArmRot, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            else
                spriteBatch.Draw(ArmTex.Value, NPC.Center - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(ShoulderTex.Value, NPC.Center - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (!NPC.IsABestiaryIconDummy)
            {
                float fade = dodgeCooldown / 40f;
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(CloakTex.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    Main.spriteBatch.Draw(LegsTex.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(rect), drawColor * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    if (customArm)
                        spriteBatch.Draw(ArmTex2.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY) + new Vector2(-8 * NPC.spriteDirection, 0), null, drawColor * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i] + customArmRot, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    else
                        spriteBatch.Draw(ArmTex.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(rect), drawColor * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    spriteBatch.Draw(ShoulderTex.Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Rectangle?(rect), drawColor * MathHelper.Lerp(0, 1, fade) * ((NPC.oldPos.Length - i) / (float)NPC.oldPos.Length), oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
                }
            }
            return false;
        }
    }
}
