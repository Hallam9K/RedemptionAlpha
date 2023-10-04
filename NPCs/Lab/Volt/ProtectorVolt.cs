using Terraria;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Usable;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Utilities;
using Redemption.WorldGeneration;
using Terraria.Audio;
using Redemption.UI.ChatUI;
using Terraria.Localization;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Lab.Volt
{
    [AutoloadBossHead]
    public class ProtectorVolt : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Fly,
            Bolts,
            Orbs,
            TeslaBeam,
            ZapBeam,
            SweepingBeam,
            Defeat
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        private readonly float[] oldrot = new float[3];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Protector Volt");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 16),
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 70;
            NPC.friendly = false;
            NPC.damage = 120;
            NPC.defense = 90;
            NPC.lifeMax = 90000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.netAlways = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic");
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LabBiome>().Type };
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == ActionState.SweepingBeam;
        public override bool CanHitNPC(NPC target) => false;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Volt"))
            });
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[3])
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel4>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedVolt, -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        private static readonly SoundStyle voice = CustomSounds.Voice6 with { Pitch = -0.1f };
        private Vector2 Pos;
        private bool FloatPos;
        public float gunRot;
        private bool faceLeft;
        public readonly Vector2 modifier = new(0, -200);
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.DespawnHandler(1, 5))
                return;
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (!player.active || player.dead)
                return;

            Vector2 GunOrigin = NPC.Center + RedeHelper.PolarVector(60, gunRot) + RedeHelper.PolarVector(-4 * NPC.spriteDirection, gunRot - (float)Math.PI / 2);
            bool flip = false;
            if (NPC.spriteDirection == 1)
            {
                if (faceLeft)
                {
                    gunRot -= MathHelper.Pi;
                    faceLeft = false;
                    flip = true;
                }
            }
            else
            {
                if (!faceLeft)
                {
                    gunRot += MathHelper.Pi;
                    faceLeft = true;
                    flip = true;
                }
            }
            switch (AIState)
            {
                case ActionState.Begin:
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.TitleCard.Volt.Name"), 60, 90, 0.8f, 0, Color.Yellow, Language.GetTextValue("Mods.Redemption.TitleCard.Volt.Modifier"));
                    AIState = ActionState.Fly;
                    NPC.netUpdate = true;
                    break;

                case ActionState.Fly:
                    if (NPC.life <= (int)(NPC.lifeMax * 0.01f))
                    {
                        Pos = (RedeGen.LabVector + new Vector2(86, 119)) * 16;
                        NPC.dontTakeDamage = true;
                        NPC.noGravity = true;
                        NPC.noTileCollide = true;
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Defeat;
                        NPC.netUpdate = true;

                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        return;
                    }
                    NPC.LookAtEntity(player);
                    gunRot.SlowRotation(NPC.spriteDirection == 1 ? 0f : (float)Math.PI, flip ? (float)Math.PI : (float)Math.PI / 60f);
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    switch (TimerRand)
                    {
                        case 0:
                            if (!Main.rand.NextBool(5))
                            {
                                Pos = PickRandPos();
                                TimerRand = 1;
                            }
                            else
                            {
                                Pos = PickSidePos();
                                TimerRand = 2;
                            }
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            if (NPC.DistanceSQ(Pos) < 10 * 10)
                            {
                                if (!FloatPos)
                                {
                                    NPC.noGravity = false;
                                    NPC.noTileCollide = false;
                                }
                                NPC.velocity *= 0f;
                                AITimer = 0;
                                TimerRand = 0;
                                AIState = (ActionState)Main.rand.Next(2, 6);
                                NPC.netUpdate = true;
                            }
                            else
                                NPC.Move(Pos, 14, 20);
                            break;
                        case 2:
                            if (NPC.DistanceSQ(Pos) < 10 * 10)
                            {
                                if (!FloatPos)
                                {
                                    NPC.noGravity = false;
                                    NPC.noTileCollide = false;
                                }
                                NPC.velocity *= 0f;
                                AITimer = 0;
                                TimerRand = 0;
                                AIState = ActionState.SweepingBeam;
                                NPC.netUpdate = true;
                            }
                            else
                                NPC.Move(Pos, 14, 20);
                            break;
                    }
                    break;
                case ActionState.Bolts:
                    NPC.LookAtEntity(player);
                    gunRot.SlowRotation(NPC.DirectionTo(player.Center).ToRotation(), flip ? (float)Math.PI : (float)Math.PI / 60f);
                    if (AITimer++ == 0)
                    {
                        for (int k = 0; k < 20; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * 30);
                            vector.Y = (float)(Math.Cos(angle) * 30);
                            Dust dust2 = Main.dust[Dust.NewDust(GunOrigin + vector, 2, 2, DustID.Electric)];
                            dust2.noGravity = true;
                            dust2.velocity = dust2.position.DirectionTo(GunOrigin) * 4f;
                        }
                        TimerRand = Main.rand.NextBool() ? 1 : 0;
                    }


                    if (AITimer >= 40 && AITimer % (TimerRand == 0 ? 10 : 20) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), GunOrigin, RedeHelper.PolarVector(TimerRand == 0 ? 14 : 15, gunRot), ProjectileID.MartianTurretBolt, NPCHelper.HostileProjDamage(NPC.damage), 0, Main.myPlayer);
                        Main.projectile[proj].tileCollide = false;
                        Main.projectile[proj].timeLeft = 200;
                        Main.projectile[proj].netUpdate = true;
                    }
                    if (AITimer >= (TimerRand == 0 ? 100 : 120))
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Fly;
                    }
                    break;
                case ActionState.Orbs:
                    NPC.LookAtEntity(player);
                    if (NPC.spriteDirection == 1)
                        gunRot.SlowRotation(5.76f, flip ? (float)Math.PI : (float)Math.PI / 30f);
                    else
                        gunRot.SlowRotation(3.66f, flip ? (float)Math.PI : (float)Math.PI / 30f);

                    if (AITimer++ == 60)
                    {
                        for (int i = 0; i < 5; i++)
                            NPC.Shoot(GunOrigin, ModContent.ProjectileType<Volt_OrbProj>(), NPC.damage, RedeHelper.PolarVector(5 + (i * 3), gunRot), SoundID.Item61);
                    }
                    if (AITimer >= 170)
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Fly;
                    }
                    break;
                case ActionState.TeslaBeam:
                    if (AITimer++ < 40)
                    {
                        NPC.LookAtEntity(player);
                        for (int k = 0; k < 2; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * 30);
                            vector.Y = (float)(Math.Cos(angle) * 30);
                            Dust dust2 = Main.dust[Dust.NewDust(GunOrigin + vector, 2, 2, DustID.Electric)];
                            dust2.noGravity = true;
                            dust2.velocity = dust2.position.DirectionTo(GunOrigin) * 4f;
                        }
                        gunRot.SlowRotation(NPC.DirectionTo(player.Center).ToRotation() - 0.4f, flip ? (float)Math.PI : (float)Math.PI / 40f);
                    }
                    if (AITimer == 60)
                    {
                        NPC.Shoot(GunOrigin, ModContent.ProjectileType<TeslaBeam>(), NPC.damage, RedeHelper.PolarVector(10, gunRot), SoundID.Item73, NPC.whoAmI);
                    }
                    if (AITimer > 60)
                        gunRot += 0.01f;
                    if (AITimer >= 180)
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Fly;
                    }
                    break;
                case ActionState.ZapBeam:
                    if (AITimer++ < 40)
                    {
                        NPC.LookAtEntity(player);
                        for (int k = 0; k < 2; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * 30);
                            vector.Y = (float)(Math.Cos(angle) * 30);
                            Dust dust2 = Main.dust[Dust.NewDust(GunOrigin + vector, 2, 2, DustID.Electric)];
                            dust2.noGravity = true;
                            dust2.velocity = dust2.position.DirectionTo(GunOrigin) * 4f;
                        }
                        if (AITimer % 10 == 0)
                        {
                            for (int i = 0; i < 12; ++i)
                            {
                                Dust dust = Dust.NewDustDirect(GunOrigin, 2, 2, DustID.Electric);
                                dust.velocity = RedeHelper.PolarVector(20, gunRot - 0.6f);
                                dust.noGravity = true;
                                Dust dust2 = Dust.NewDustDirect(GunOrigin, 2, 2, DustID.Electric);
                                dust2.velocity = RedeHelper.PolarVector(20, gunRot + 0.6f);
                                dust2.noGravity = true;
                            }
                        }
                        gunRot.SlowRotation(NPC.DirectionTo(player.Center).ToRotation(), flip ? (float)Math.PI : (float)Math.PI / 40f);
                    }
                    if (AITimer == 60)
                    {
                        for (int i = 0; i < 2; i++)
                            NPC.Shoot(GunOrigin, ModContent.ProjectileType<TeslaZapBeam>(), NPC.damage, RedeHelper.PolarVector(1, gunRot + (i == 0 ? -1f : 1f)), CustomSounds.BallFire, NPC.whoAmI, i);
                    }
                    if (AITimer >= 160)
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Fly;
                    }
                    break;
                case ActionState.SweepingBeam:
                    if (AITimer++ < 40)
                    {
                        NPC.LookAtEntity(player);
                        for (int k = 0; k < 2; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * 30);
                            vector.Y = (float)(Math.Cos(angle) * 30);
                            Dust dust2 = Main.dust[Dust.NewDust(GunOrigin + vector, 2, 2, DustID.Electric)];
                            dust2.noGravity = true;
                            dust2.velocity = dust2.position.DirectionTo(GunOrigin) * 4f;
                        }

                        if (NPC.Center.Y > (RedeGen.LabVector.Y + 112) * 16)
                            gunRot.SlowRotation(-MathHelper.PiOver2, flip ? (float)Math.PI : (float)Math.PI / 30f);
                        else
                            gunRot.SlowRotation(MathHelper.PiOver2, flip ? (float)Math.PI : (float)Math.PI / 30f);
                    }
                    if (AITimer == 40)
                    {
                        NPC.noGravity = true;
                        NPC.noTileCollide = true;
                        if (NPC.Center.Y > (RedeGen.LabVector.Y + 112) * 16)
                            gunRot = -MathHelper.PiOver2;
                        else
                        {
                            NPC.velocity.Y -= 4;
                            gunRot = MathHelper.PiOver2;
                        }
                    }
                    if (AITimer == 60)
                    {
                        NPC.Shoot(GunOrigin, ModContent.ProjectileType<TeslaBeam>(), NPC.damage, RedeHelper.PolarVector(10, gunRot), SoundID.Item73, NPC.whoAmI);
                        if (NPC.Center.X > (RedeGen.LabVector.X + 86) * 16)
                            TimerRand = 1;
                    }
                    if (TimerRand < 2 && AITimer >= 60)
                    {
                        Vector2 v = new(TimerRand == 0 ? (RedeGen.LabVector.X + 125) * 16 : (RedeGen.LabVector.X + 48) * 16, Pos.Y - 30);
                        if (NPC.DistanceSQ(v) < 10 * 10)
                        {
                            NPC.noGravity = false;
                            NPC.noTileCollide = false;
                            NPC.velocity *= 0f;
                            AITimer = 180;
                            TimerRand = 2;
                            NPC.netUpdate = true;
                        }
                        else
                            NPC.Move(v, 12, 30);
                    }
                    if (TimerRand == 2 && AITimer >= 200)
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Fly;
                    }
                    break;
                case ActionState.Defeat:
                    gunRot.SlowRotation(NPC.spriteDirection == 1 ? 0f : (float)Math.PI, flip ? (float)Math.PI : (float)Math.PI / 60f);
                    switch (TimerRand)
                    {
                        case 0:
                            if (NPC.DistanceSQ(Pos) < 10 * 10)
                            {
                                NPC.noGravity = false;
                                NPC.noTileCollide = false;
                                NPC.velocity *= 0f;
                                AITimer = 0;
                                TimerRand = 1;
                                NPC.netUpdate = true;
                            }
                            else
                                NPC.Move(Pos, 14, 20);
                            break;
                        case 1:
                            AITimer++;
                            if (RedeBossDowned.downedVolt)
                            {
                                if (AITimer == 10 && !Main.dedServ)
                                {
                                    Dialogue d1 = new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Volt.Defeat.Refight"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, .5f, true, modifier: modifier); // 144

                                    ChatUI.Visible = true;
                                    ChatUI.Add(d1);
                                }
                                if (AITimer >= 30)
                                {
                                    AITimer = 0;
                                    TimerRand = 2;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                if (AITimer == 10 && !Main.dedServ)
                                {
                                    DialogueChain chain = new();
                                    chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Volt.Defeat.1"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, 0, false, modifier: modifier))
                                         .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Volt.Defeat.2"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, 0, false, modifier: modifier))
                                         .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Volt.Defeat.3"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, 0, false, modifier: modifier))
                                         .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Volt.Defeat.4"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, .5f, true, modifier: modifier, endID: 1));
                                    chain.OnEndTrigger += Chain_OnEndTrigger;
                                    ChatUI.Visible = true;
                                    ChatUI.Add(chain);
                                }
                                if (AITimer >= 2000)
                                {
                                    AITimer = 0;
                                    TimerRand = 2;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        case 2:
                            Vector2 VoltPos = new((RedeGen.LabVector.X + 49) * 16, (RedeGen.LabVector.Y + 120) * 16);
                            if (NPC.DistanceSQ(VoltPos) < 10 * 10)
                            {
                                NPC.noGravity = false;
                                NPC.noTileCollide = false;
                                NPC.velocity *= 0f;
                                TimerRand = 3;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.Move(VoltPos, 14, 20);
                                NPC.noGravity = true;
                                NPC.noTileCollide = true;
                            }
                            break;
                        case 3:
                            if (!LabArea.labAccess[3])
                                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel4>());

                            Main.BestiaryTracker.Kills.RegisterKill(NPC);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                RedeBossDowned.downedVolt = true;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            NPC.position.Y -= 10;
                            NPC.SetDefaults(ModContent.NPCType<ProtectorVolt_NPC>());
                            break;
                    }
                    break;
            }
            if (NPC.velocity.Length() != 0 || FloatPos)
            {
                if (Main.rand.NextBool(3))
                {
                    int dust1 = Dust.NewDust(new Vector2(NPC.Center.X + 6 * NPC.spriteDirection, NPC.Center.Y + 33), 2, 2, DustID.Torch, 0f, 3f, 100, Color.White, NPC.velocity.Length() == 0 ? 1.2f : 2f);
                    Main.dust[dust1].velocity.X = 0;
                    int dust2 = Dust.NewDust(new Vector2(NPC.Center.X + -14 * NPC.spriteDirection, NPC.Center.Y + 33), 2, 2, DustID.Torch, 0f, 3f, 100, Color.White, NPC.velocity.Length() == 0 ? 1.2f : 2f);
                    Main.dust[dust2].velocity.X = 0;
                }
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 3000;
        }
        public Vector2 PickRandPos()
        {
            WeightedRandom<Vector2> choice = new(Main.rand);
            choice.Add(new Vector2(52, 119));
            choice.Add(new Vector2(121, 119));
            choice.Add(new Vector2(86, 119));
            choice.Add(new Vector2(47, 103));
            choice.Add(new Vector2(126, 103));
            choice.Add(new Vector2(67, 103));
            choice.Add(new Vector2(104, 103));
            choice.Add(new Vector2(68, 114));
            choice.Add(new Vector2(105, 114));
            choice.Add(new Vector2(86, 101));

            Vector2 selection = choice;
            if (selection == new Vector2(68, 114) || selection == new Vector2(105, 114) || selection == new Vector2(86, 101))
                FloatPos = true;
            else
                FloatPos = false;

            return (RedeGen.LabVector + choice) * 16;
        }
        public Vector2 PickSidePos()
        {
            WeightedRandom<Vector2> choice = new(Main.rand);
            choice.Add(new Vector2(52, 119));
            choice.Add(new Vector2(121, 119));
            choice.Add(new Vector2(47, 103));
            choice.Add(new Vector2(126, 103));

            FloatPos = false;
            return (RedeGen.LabVector + choice) * 16;
        }
        public override bool CheckDead()
        {
            NPC.life = 1;
            return false;
        }
        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (NPC.velocity.Length() == 0 && !FloatPos)
            {
                NPC.rotation = 0;
                if (NPC.frameCounter++ >= 5)
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
            Texture2D GunTex = ModContent.Request<Texture2D>(Texture + "_Gun").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 value4 = NPC.oldPos[i];
                spriteBatch.Draw(texture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(drawColor) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            Vector2 gunCenter = new(NPC.Center.X, NPC.Center.Y + 6);
            int height = GunTex.Height / 4;
            if (NPC.IsABestiaryIconDummy)
                spriteBatch.Draw(GunTex, gunCenter - screenPos, new Rectangle?(new Rectangle(0, 0, GunTex.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(GunTex.Width / 2f, height / 2f), NPC.scale, effects, 0f);
            else
                spriteBatch.Draw(GunTex, gunCenter - screenPos, new Rectangle?(new Rectangle(0, 0, GunTex.Width, height)), NPC.GetAlpha(drawColor), (AIState is ActionState.SweepingBeam ? 0 : NPC.rotation) + gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), new Vector2(GunTex.Width / 2f, height / 2f), NPC.scale, effects, 0f);
            return false;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
    }
}
