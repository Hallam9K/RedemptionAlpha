using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Redemption.Base;
using Terraria.Graphics.Shaders;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Weapons.PreHM.Ritualist;

namespace Redemption.NPCs.Bosses.Keeper
{
    [AutoloadBossHead]
    public class KeeperSpirit : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public float[] oldrot = new float[5];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Keeper's Spirit");
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 3500;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            NPC.width = 52;
            NPC.height = 128;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 3, 50, 0);
            NPC.SpawnWithHigherTime(30);
            NPC.alpha = 255;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossKeeper");
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit, Scale: 3);
                    Main.dust[dustIndex].velocity *= 4f;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<KeeperBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KeeperTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<KeeperRelic>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<OcciesCollar>(), 4));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<KeepersVeil>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<SoulScepter>(), ModContent.ItemType<KeepersClaw>(), ModContent.ItemType<FanOShivs>(), ModContent.ItemType<KeepersKnife>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GrimShard>(), 1, 2, 4));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedKeeper, -1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(ID);
                writer.Write(Unveiled);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ID = reader.ReadInt32();
                Unveiled = reader.ReadBoolean();
            }
        }

        void AttackChoice()
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                attempts++;
            }
        }

        public List<int> AttackList = new() { 0, 1, 2, 3, 4 };
        public List<int> CopyList = null;

        private bool Unveiled;
        private float move;
        private float speed = 6;
        private Vector2 origin;

        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            Rectangle SlashHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 64 : NPC.Center.X + 26), (int)(NPC.Center.Y - 38), 38, 86);

            DespawnHandler();

            if (AIState != ActionState.Attacks)
                NPC.LookAtEntity(player);

            switch (AIState)
            {
                case ActionState.Begin:
                    if (AITimer++ == 0)
                    {
                        NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 160 : player.Center.X + 160, player.Center.Y - 90);
                        NPC.netUpdate = true;
                    }
                    NPC.alpha -= 2;
                    if (NPC.alpha <= 0)
                    {
                        AIState = ActionState.Idle;
                        AITimer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Idle:
                    if (AITimer++ == 0)
                    {
                        move = NPC.Center.X;
                        speed = 6;
                    }
                    NPC.Move(new Vector2(move, player.Center.Y - 50), speed, 20, false);
                    MoveClamp();
                    if (NPC.DistanceSQ(player.Center) > 400 * 400)
                        speed *= 1.03f;

                    if (!Unveiled && NPC.life < NPC.lifeMax / 2)
                    {
                        Unveiled = true;
                        NPC.netUpdate = true;
                    }
                    if (AITimer > 60)
                    {
                        NPC.dontTakeDamage = false;
                        AttackChoice();
                        AITimer = 0;
                        AIState = ActionState.Attacks;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Attacks:
                    if (!Unveiled && NPC.life < NPC.lifeMax / 2)
                    {
                        Unveiled = true;
                        NPC.netUpdate = true;
                        break;
                    }
                    switch (ID)
                    {
                        #region Reaper Slash
                        case 0:
                            int alphaTimer = Main.expertMode ? 20 : 10;
                            AITimer++;
                            if (AITimer < 100)
                            {
                                if (AITimer < 40)
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.velocity *= 0.9f;
                                }
                                if (AITimer == 40)
                                {
                                    SoundEngine.PlaySound(SoundID.Zombie83 with { Pitch = 0.3f }, NPC.position);
                                    NPC.velocity.Y = 0;
                                    NPC.velocity.X = -6f * NPC.spriteDirection;
                                }
                                if (AITimer >= 40)
                                {
                                    NPC.alpha += alphaTimer;
                                    NPC.velocity *= 0.96f;
                                }
                                if (NPC.alpha >= 255)
                                {
                                    NPC.velocity *= 0f;
                                    NPC.position = new Vector2(player.Center.X + (player.velocity.X > 0 ? 200 : -200) + (player.velocity.X * 20), player.Center.Y - 70);
                                    AITimer = 100;
                                }
                            }
                            else
                            {
                                if (AITimer == 100)
                                {
                                    NPC.velocity.X = 6f * NPC.spriteDirection;
                                }
                                if (AITimer >= 100 && AITimer < 200)
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.alpha -= alphaTimer;
                                    NPC.velocity *= 0.96f;
                                }
                                if (NPC.alpha <= 0 && AITimer < 200)
                                {
                                    AITimer = 200;
                                    NPC.frameCounter = 0;
                                    NPC.frame.Y = 0;
                                }
                                if (AITimer >= 200 && NPC.frame.Y >= 4 * 71 && NPC.frame.Y <= 6 * 71)
                                {
                                    for (int i = 0; i < Main.maxNPCs; i++)
                                    {
                                        NPC target = Main.npc[i];
                                        if (!target.active || target.whoAmI == NPC.whoAmI || (!target.friendly &&
                                            !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type]))
                                            continue;

                                        if (target.immune[NPC.whoAmI] > 0 || !target.Hitbox.Intersects(SlashHitbox))
                                            continue;

                                        target.immune[NPC.whoAmI] = 30;
                                        int hitDirection = NPC.Center.X > target.Center.X ? -1 : 1;
                                        BaseAI.DamageNPC(target, NPC.damage, 3, hitDirection, NPC);
                                        target.AddBuff(BuffID.Bleeding, 600);
                                    }
                                    for (int p = 0; p < Main.maxPlayers; p++)
                                    {
                                        Player target = Main.player[p];
                                        if (!target.active || target.dead)
                                            continue;

                                        if (!target.Hitbox.Intersects(SlashHitbox))
                                            continue;

                                        int hitDirection = NPC.Center.X > target.Center.X ? -1 : 1;
                                        BaseAI.DamagePlayer(target, NPC.damage, 3, hitDirection, NPC);
                                        target.AddBuff(BuffID.Bleeding, 600);
                                    }
                                }
                                if (AITimer >= 235)
                                {
                                    NPC.frameCounter = 0;
                                    NPC.frame.Y = 0;
                                    NPC.velocity *= 0f;
                                    if (TimerRand >= (Main.expertMode ? 2 : 1) + (Unveiled ? 1 : 0))
                                    {
                                        TimerRand = 0;
                                        AITimer = 0;
                                        AIState = ActionState.Idle;
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        TimerRand++;
                                        AITimer = 30;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Blood Wave
                        case 1:
                            NPC.LookAtEntity(player);

                            NPC.velocity *= 0.96f;

                            if (++AITimer == 30)
                                NPC.velocity = player.Center.DirectionTo(NPC.Center) * 6;

                            if (AITimer == 60)
                            {
                                BaseAI.DamageNPC(NPC, 50, 0, player, false, true);
                                for (int i = 0; i < 6; i++)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KeeperBloodWave>(), NPC.damage,
                                        RedeHelper.PolarVector(Main.rand.NextFloat(8, 16), (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.3f, 0.3f)),
                                        true, SoundID.NPCDeath19, NPC.whoAmI);
                                }
                                for (int i = 0; i < 30; i++)
                                {
                                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, Scale: 3);
                                    Main.dust[dustIndex].velocity *= 5f;
                                }
                            }
                            if (AITimer >= 90)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Shadow Bolts
                        case 2:
                            NPC.LookAtEntity(player);

                            if (AITimer++ == 0)
                                speed = 6;
                            NPC.Move(new Vector2(move, player.Center.Y - 50), speed, 20, false);
                            MoveClamp();
                            if (NPC.DistanceSQ(player.Center) > 400 * 400)
                                speed *= 1.03f;
                            else if (NPC.velocity.Length() > 6 && NPC.DistanceSQ(player.Center) <= 400 * 400)
                                speed *= 0.96f;

                            if (AITimer >= 60 && AITimer % (Unveiled ? 20 : 25) == 0)
                            {
                                Vector2 pos = NPC.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(TimerRand)) * 60;
                                NPC.Shoot(pos, ModContent.ProjectileType<ShadowBolt>(), NPC.damage,
                                       RedeHelper.PolarVector(Main.expertMode ? 4 : 3, (player.Center - NPC.Center).ToRotation()), true, SoundID.Item20);

                                TimerRand += 45;
                            }
                            if (TimerRand >= 360)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Soul Charge
                        case 3:
                            AITimer++;
                            if (AITimer < 100)
                            {
                                if (AITimer == 5)
                                {
                                    NPC.LookAtEntity(player);
                                    SoundEngine.PlaySound(SoundID.Zombie83 with { Pitch = 0.3f }, NPC.position);
                                    NPC.velocity.Y = 0;
                                    NPC.velocity.X = -6f * NPC.spriteDirection;
                                }
                                if (AITimer >= 5)
                                {
                                    NPC.alpha += 20;
                                    NPC.velocity *= 0.9f;
                                }
                                if (NPC.alpha >= 255)
                                {
                                    NPC.velocity *= 0f;
                                    NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 180 : player.Center.X + 180, player.Center.Y - 70);
                                    AITimer = 100;
                                }
                            }
                            else
                            {
                                if (AITimer == 100)
                                    NPC.velocity.X = 6f * NPC.spriteDirection;

                                if (AITimer >= 100 && AITimer < 200)
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.alpha -= 20;
                                    NPC.velocity *= 0.9f;
                                }
                                if (NPC.alpha <= 0 && AITimer < 200)
                                    AITimer = 200;

                                if (AITimer < (Unveiled ? 260 : 280))
                                {
                                    NPC.LookAtEntity(player);
                                    NPC.MoveToVector2(new Vector2(player.Center.X - 160 * NPC.spriteDirection, player.Center.Y - 70), 4);
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Dust dust2 = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.DungeonSpirit, 1);
                                        dust2.velocity = -NPC.DirectionTo(dust2.position);
                                        dust2.noGravity = true;
                                    }
                                    origin = player.Center;
                                }
                                if (AITimer >= (Unveiled ? 260 : 280) && AITimer < 320)
                                {
                                    NPC.velocity.Y = 0;
                                    NPC.velocity.X = -0.1f * NPC.spriteDirection;
                                    Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                    player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(player.RedemptionScreen().ScreenShakeIntensity, 3);

                                    if (AITimer % 2 == 0)
                                    {
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<KeeperSoulCharge>(), (int)(NPC.damage * 1.4f), RedeHelper.PolarVector(Main.rand.NextFloat(14, 16), (origin - NPC.Center).ToRotation()), true, SoundID.NPCDeath52 with { Volume = .5f });
                                    }
                                }
                                if (AITimer >= 320)
                                    NPC.velocity *= 0.98f;
                            }
                            if (AITimer >= 360)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Dread Coil
                        case 4:
                            if (Unveiled)
                            {
                                NPC.LookAtEntity(player);

                                if (AITimer++ == 0)
                                    speed = 6;
                                NPC.Move(new Vector2(move, player.Center.Y - 50), speed, 20, false);
                                MoveClamp();
                                if (NPC.DistanceSQ(player.Center) > 400 * 400)
                                    speed *= 1.03f;
                                else if (NPC.velocity.Length() > 6 && NPC.DistanceSQ(player.Center) <= 400 * 400)
                                    speed *= 0.96f;
                                if (AITimer >= 30 && AITimer % 30 == 0)
                                {
                                    NPC.Shoot(new Vector2(NPC.Center.X + 3 * NPC.spriteDirection, NPC.Center.Y - 37), ModContent.ProjectileType<KeeperDreadCoil>(),
                                        NPC.damage, RedeHelper.PolarVector(7, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.08f, 0.08f)),
                                        true, SoundID.Item20);
                                }
                                if (AITimer >= 130)
                                {
                                    TimerRand = 0;
                                    AITimer = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                TimerRand = 0;
                                AITimer = 60;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
            }
        }

        public void MoveClamp()
        {
            Player player = Main.player[NPC.target];
            int xFar = 240;
            if (NPC.Center.X < player.Center.X)
            {
                if (move < player.Center.X - xFar)
                {
                    move = player.Center.X - xFar;
                }
                else if (move > player.Center.X - 120)
                {
                    move = player.Center.X - 120;
                }
            }
            else
            {
                if (move > player.Center.X + xFar)
                {
                    move = player.Center.X + xFar;
                }
                else if (move < player.Center.X + 120)
                {
                    move = player.Center.X + 120;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Player player = Main.player[NPC.target];

                for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                {
                    oldrot[k] = oldrot[k - 1];
                }
                oldrot[0] = NPC.rotation;

                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
                if (AIState is ActionState.Attacks && ID == 0 && AITimer >= 200)
                {
                    NPC.frame.X = NPC.frame.Width;
                    if (++NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        NPC.velocity *= 0.8f;
                        if (NPC.frame.Y == 4 * frameHeight)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                            NPC.velocity.X = MathHelper.Clamp(Math.Abs((player.Center.X - NPC.Center.X) / 30), 30, 50) * NPC.spriteDirection;
                        }
                        if (NPC.frame.Y > 7 * frameHeight)
                            NPC.frame.Y = 0 * frameHeight;
                    }
                    return;
                }
                else
                    NPC.frame.X = 0;

                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 5 * frameHeight)
                        NPC.frame.Y = 0 * frameHeight;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightSkyBlue) * 0.5f, oldrot[i], NPC.frame.Size() / 2, (NPC.scale * 2) + 0.1f, effects, 0);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White) * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
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
                    NPC.alpha += 2;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }
    }
}