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

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Bleeding,
                    BuffID.Poisoned
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
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
        }
        public override void HitEffect(int hitDirection, double damage)
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
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            bool vDmg = false;
            if (NPC.RedemptionGuard().GuardPoints >= 0)
            {
                NPC.RedemptionGuard().GuardHit(NPC, ref vDmg, ref damage, ref knockback, SoundID.NPCHit4);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                if (NPC.RedemptionGuard().GuardPoints >= 0)
                    return vDmg;
            }
            NPC.RedemptionGuard().GuardBreakCheck(NPC, ModContent.DustType<VoidFlame>(), CustomSounds.GuardBreak, 10, 2);

            if (PsychicHit)
            {
                SoundEngine.PlaySound(SoundID.NPCHit48, NPC.position);
                if (NPC.life < NPC.lifeMax)
                {
                    NPC.life += (int)(damage / 10);
                    NPC.HealEffect((int)(damage / 10));
                }
                if (NPC.life > NPC.lifeMax)
                    NPC.life = NPC.lifeMax;

                damage = 0;
                PsychicHit = false;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                return false;
            }
            return true;
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (ItemLists.Holy.Contains(item.type))
                    damage = (int)(damage * 2f);

                if (ItemLists.Psychic.Contains(item.type))
                    PsychicHit = true;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                if (ProjectileLists.Holy.Contains(projectile.type))
                    damage = (int)(damage * 2f);

                if (ProjectileLists.Psychic.Contains(projectile.type))
                    PsychicHit = true;
            }
        }

        private Vector2 moveTo;
        private int runCooldown;
        private int dodgeCooldown;
        private readonly float[] doorVars = new float[3];
        public override void OnSpawn(IEntitySource source)
        {
            WeightedRandom<int> NPCType = new(Main.rand);
            NPCType.Add(ModContent.NPCType<SkeletonWanderer>());
            NPCType.Add(ModContent.NPCType<SkeletonAssassin>());
            NPCType.Add(ModContent.NPCType<SkeletonDuelist>());
            NPCType.Add(ModContent.NPCType<EpidotrianSkeleton>());

            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                Vector2 pos = RedeHelper.FindGround(NPC, 8);
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
            }

            TimerRand = Main.rand.Next(80, 280);
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
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 4, 30, interactDoorStyle: 2);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.8f, 6, 6, NPC.Center.Y > player.Center.Y);
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
                            if (!proj.active || !proj.friendly || proj.damage <= 0 || proj.velocity.Length() == 0)
                                continue;

                            if (!NPC.Sight(proj, 80 + (proj.velocity.Length() * 3), true, true))
                                continue;

                            for (int l = 0; l < 10; l++)
                            {
                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith, Scale: 2);
                                Main.dust[dust].velocity *= 0.2f;
                                Main.dust[dust].noGravity = true;
                            }
                            NPC.Dodge(proj);
                            dodgeCooldown = 90;
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

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
                    RedeHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 2.4f * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y);

                    break;

                case ActionState.Slash:
                    if (NPC.ThreatenedCheck(ref runCooldown, 320))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (NPC.frame.Y == 6 * 62 && globalNPC.attacker.Hitbox.Intersects(SlashHitbox))
                    {
                        int damage = NPC.RedemptionNPCBuff().disarmed ? (int)(NPC.damage * 0.2f) : NPC.damage;
                        if (globalNPC.attacker is NPC attackerNPC && attackerNPC.immune[NPC.whoAmI] <= 0)
                        {
                            attackerNPC.immune[NPC.whoAmI] = 10;
                            int hitDirection = NPC.Center.X > attackerNPC.Center.X ? -1 : 1;
                            BaseAI.DamageNPC(attackerNPC, damage, 5, hitDirection, NPC);
                            attackerNPC.AddBuff(BuffID.Bleeding, 1000);
                            attackerNPC.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), 1400);
                        }
                        else if (globalNPC.attacker is Player attackerPlayer)
                        {
                            int hitDirection = NPC.Center.X > attackerPlayer.Center.X ? -1 : 1;
                            BaseAI.DamagePlayer(attackerPlayer, damage, 5, hitDirection, NPC);
                            if (globalNPC.attacker is Player && (Main.rand.NextBool(2) || Main.expertMode))
                            {
                                attackerPlayer.AddBuff(BuffID.Bleeding, 1000);
                                attackerPlayer.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), 1400);
                            }
                        }
                    }
                    break;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        private bool Flare;
        private float FlareTimer;
        public override void FindFrame(int frameHeight)
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
                WhitelistNPC = new int[] { NPCID.TargetDummy };

            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan)
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
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.VoidDye);
            if (!NPC.IsABestiaryIconDummy && !NPC.RedemptionGuard().GuardBroken)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - new Vector2(0, 4) - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        private float Opacity { get => FlareTimer; set => FlareTimer = value; }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = NPC.Center - screenPos + new Vector2(-1 * NPC.spriteDirection, -8 + NPC.gfxOffY);
            Color colour = Color.Lerp(Color.White, Color.DarkRed, 1f / Opacity * 10f) * (1f / Opacity * 10f);
            if (Flare)
            {
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour, 0, origin, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour * 0.4f, 0, origin, 1f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GraveSteelShards>(), 1, 8, 16));
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
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(
                    "The body of a noble knight of Gathuram, the soul of a criminal. Now dead and stuck under the earth in the dark labyrinthine caves, they have lost their split minds and gone insane.")
            });
        }
    }
}