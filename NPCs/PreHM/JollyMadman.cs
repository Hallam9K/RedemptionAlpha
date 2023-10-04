using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Armor.Single;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;
using Redemption.BaseExtension;
using System.IO;
using Terraria.Localization;

namespace Redemption.NPCs.PreHM
{
    public class JollyMadman : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Slash
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
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCBlood[Type] = true;
            ElementID.NPCShadow[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 48;
            NPC.damage = 28;
            NPC.friendly = false;
            NPC.defense = 10;
            NPC.lifeMax = 125;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 2500;
            NPC.knockBackResist = 0.3f;
            NPC.aiStyle = -1;
            NPC.rarity = 1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<JollyMadmanBanner>();
            NPC.RedemptionGuard().GuardPoints = 25;

            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Holy] *= 2f;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Lead, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 5; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/JollyMadmanGore" + (i + 1)).Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Lead, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                SoundEngine.PlaySound(SoundID.Zombie2, NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        private bool PsychicHit;
        private int PsychicDamage;
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (NPC.RedemptionGuard().GuardPoints >= 0)
            {
                modifiers.DisableCrit();
                modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => NPC.RedemptionGuard().GuardHit(ref n, NPC, SoundID.NPCHit4, .25f, false, ModContent.DustType<VoidFlame>(), default, 10, 2);
            }

            if (PsychicHit)
            {
                SoundEngine.PlaySound(SoundID.NPCHit48, NPC.position);
                if (NPC.life < NPC.lifeMax)
                {
                    NPC.life += PsychicDamage / 10;
                    NPC.HealEffect(PsychicDamage / 10);
                }
                if (NPC.life > NPC.lifeMax)
                    NPC.life = NPC.lifeMax;

                modifiers.FinalDamage *= 0;
                modifiers.HideCombatText();
                NPC.life++;
                PsychicDamage = 0;
                PsychicHit = false;
                return;
            }
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (item.HasElement(ElementID.Psychic))
                {
                    PsychicHit = true;
                    PsychicDamage = damageDone;
                }
            }
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (projectile.HasElement(ElementID.Psychic))
                {
                    PsychicHit = true;
                    PsychicDamage = damageDone;
                }
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            moveTo = reader.ReadVector2();
        }
        private Vector2 moveTo;
        private int runCooldown;
        private int dodgeCooldown;
        private readonly float[] doorVars = new float[3];
        public override void OnSpawn(IEntitySource source)
        {
            if (NPC.ai[3] != 4)
            {
                WeightedRandom<int> NPCType = new(Main.rand);
                NPCType.Add(ModContent.NPCType<SkeletonWanderer>());
                NPCType.Add(ModContent.NPCType<SkeletonAssassin>());
                NPCType.Add(ModContent.NPCType<SkeletonDuelist>());
                NPCType.Add(ModContent.NPCType<EpidotrianSkeleton>());

                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Vector2 pos = NPCHelper.FindGround(NPC, 8);
                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                }
            }
            TimerRand = Main.rand.Next(80, 280);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            if (AIState != ActionState.Slash)
                NPC.LookByVelocity();
            Rectangle SlashHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 45 : NPC.Center.X + 7), (int)(NPC.Center.Y - 34), 38, 60);
            dodgeCooldown--;
            dodgeCooldown = (int)MathHelper.Max(0, dodgeCooldown);
            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    SightCheck();
                    break;

                case ActionState.Wander:
                    SightCheck();

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 280);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 4, 30, interactDoorStyle: 2);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.8f, 6, 6, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 320))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, 700, false, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (dodgeCooldown <= 0 && NPC.velocity.Y == 0)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile proj = Main.projectile[i];
                            if (!proj.active || !proj.friendly || proj.damage <= 0 || proj.sentry || proj.minion || proj.velocity.Length() == 0)
                                continue;

                            if (!NPC.Sight(proj, 80 + (proj.velocity.Length() * 3), true, true))
                                continue;

                            for (int l = 0; l < 10; l++)
                            {
                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith, Scale: 2);
                                Main.dust[dust].velocity *= 0.2f;
                                Main.dust[dust].noGravity = true;
                            }
                            if (Main.rand.NextBool())
                                NPC.velocity.X *= -1;
                            NPC.velocity.X *= 2f;
                            NPC.velocity.Y -= Main.rand.NextFloat(1, 3);
                            dodgeCooldown = 90;
                            break;
                        }
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 4, 30, interactDoorStyle: 2);

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) < 60 * 60)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity.Y = 0;
                        NPC.velocity.X = 0;
                        AIState = ActionState.Slash;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 2.4f * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);

                    break;

                case ActionState.Slash:
                    if (NPC.ThreatenedCheck(ref runCooldown, 320))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (NPC.frame.Y == 6 * 62 && globalNPC.attacker.Hitbox.Intersects(SlashHitbox))
                    {
                        int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                        if (globalNPC.attacker is NPC attackerNPC && attackerNPC.immune[NPC.whoAmI] <= 0)
                        {
                            attackerNPC.immune[NPC.whoAmI] = 10;
                            int hitDirection = attackerNPC.RightOfDir(NPC);
                            BaseAI.DamageNPC(attackerNPC, damage, 5, hitDirection, NPC);
                            if (attackerNPC.life <= 0)
                            {
                                for (int i = 0; i < 30; i++)
                                {
                                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LifeDrain);
                                    Main.dust[dustIndex].velocity.Y = -3;
                                    Main.dust[dustIndex].velocity.X = 0;
                                    Main.dust[dustIndex].noGravity = true;
                                }
                                SoundEngine.PlaySound(SoundID.NPCHit48, NPC.position);
                                NPC.life += 250;
                                if (NPC.life >= NPC.lifeMax)
                                    NPC.life = NPC.lifeMax;
                                NPC.HealEffect(250);
                            }
                        }
                        else if (globalNPC.attacker is Player attackerPlayer)
                        {
                            int hitDirection = attackerPlayer.RightOfDir(NPC);
                            BaseAI.DamagePlayer(attackerPlayer, damage, 5, hitDirection, NPC);
                        }
                    }
                    break;
            }
        }
        public override void PostAI()
        {
            CustomFrames(62);
        }
        private void CustomFrames(int frameHeight)
        {
            if (AIState is ActionState.Alert or ActionState.Slash)
            {
                Flare = true;
                FlareTimer = 10;
            }
            else
            {
                if (Flare)
                {
                    FlareTimer++;
                    if (FlareTimer > 60)
                    {
                        Flare = false;
                        FlareTimer = 0;
                    }
                }
            }
            if (AIState is ActionState.Slash)
            {
                if (NPC.frame.Y < 4 * frameHeight)
                    NPC.frame.Y = 4 * frameHeight;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y == 6 * frameHeight)
                    {
                        SoundEngine.PlaySound(SoundID.Item71 with { Volume = .5f }, NPC.position);
                        NPC.velocity.X += 4 * NPC.spriteDirection;
                    }
                    if (NPC.frame.Y > 8 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        AIState = ActionState.Alert;
                    }
                }
                return;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        private bool Flare;
        private float FlareTimer;
        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Slash)
                return;
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frame.Y < 1 * frameHeight)
                        NPC.frame.Y = 1 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 1 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 3 * frameHeight;
            }
        }
        public int GetNearestNPC(int[] WhitelistNPC = default)
        {
            if (WhitelistNPC == null)
                WhitelistNPC = new int[] { NPCID.Guide };

            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                if (!WhitelistNPC.Contains(target.type) && (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])))
                    continue;

                if (nearestNPCDist != -1 && !(target.Distance(NPC.Center) < nearestNPCDist))
                    continue;

                nearestNPCDist = target.Distance(NPC.Center);
                nearestNPC = target.whoAmI;
            }

            return nearestNPC;
        }
        public void SightCheck()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = GetNearestNPC(NPCLists.HasLostSoul.ToArray());
            if (NPC.Sight(player, 650, true, true))
            {
                SoundEngine.PlaySound(SoundID.Zombie3, NPC.position);
                globalNPC.attacker = player;
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Alert;
                NPC.netUpdate = true;
            }
            if (Main.rand.NextBool(600))
            {
                if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 650, true, true))
                {
                    SoundEngine.PlaySound(SoundID.Zombie3, NPC.position);
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                    NPC.netUpdate = true;
                }
                return;
            }
            gotNPC = GetNearestNPC(new[] { ModContent.NPCType<LostSoulNPC>() });
            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 650, true, true))
            {
                SoundEngine.PlaySound(SoundID.Zombie3, NPC.position);
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Alert;
                NPC.netUpdate = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            target.AddBuff(BuffID.Bleeding, 1000);
            target.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), 1400);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
            {
                target.AddBuff(BuffID.Bleeding, 1000);
                target.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), 1400);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.VoidDye);
            if (!NPC.IsABestiaryIconDummy && !NPC.RedemptionGuard().GuardBroken)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - new Vector2(0, 4) - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.BeginDefault();
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Flare)
            {
                Vector2 position = NPC.Center - screenPos + new Vector2(-1 * NPC.spriteDirection, -8 + NPC.gfxOffY);
                RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, position, Color.DarkRed, NPC.rotation);
            }
        }

        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => dodgeCooldown <= 80 ? null : false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => dodgeCooldown <= 80 ? null : false;

        public override void OnKill()
        {
            for (int i = 0; i < 3; i++)
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0.2f, 0.6f));
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<ZweihanderFragment1>(), ModContent.ItemType<ZweihanderFragment2>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JollyHelm>(), 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientGoldCoin>(), 1, 4, 12));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GraveSteelShards>(), 1, 16, 28));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 1, 3, 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.Cavern.Chance;
            float multiplier = TileLists.AncientTileArray.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType) ? .01f : 0.002f;

            return baseChance * multiplier;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CustomCollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], false, 5);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.JollyMadman"))
            });
        }
    }
}
