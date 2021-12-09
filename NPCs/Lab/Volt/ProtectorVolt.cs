using Terraria;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Redemption.Projectiles.Hostile;
using Redemption.Items.Usable;
using Terraria.DataStructures;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Utilities;
using Redemption.WorldGeneration;
using Terraria.Audio;

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
            Laser
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
            DisplayName.SetDefault("Protector Volt");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom
                }
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 70;
            NPC.friendly = false;
            NPC.damage = 120;
            NPC.defense = 90;
            NPC.lifeMax = 160000;
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
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("") // TODO: Volt bestiary
            });
        }
        public override void OnKill()
        {
            if (!LabArea.labAccess[3])
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel4>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedVolt, -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }

        private Vector2 Pos;
        private bool FloatPos;
        public float gunRot;
        private bool faceLeft;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            DespawnHandler();
            NPC.LookAtEntity(player);
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (NPC.spriteDirection == 1)
            {
                if (faceLeft)
                {
                    gunRot -= MathHelper.Pi;
                    faceLeft = false;
                }
            }
            else
            {
                if (!faceLeft)
                {
                    gunRot += MathHelper.Pi;
                    faceLeft = true;
                }
            }

            if (!player.active || player.dead)
                return;

            Vector2 GunOrigin = NPC.Center + RedeHelper.PolarVector(60, gunRot) + RedeHelper.PolarVector(-4 * NPC.spriteDirection, gunRot - (float)Math.PI / 2);
            switch (AIState)
            {
                case ActionState.Begin:
                    if (!Main.dedServ)
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Protector Volt", 60, 90, 0.8f, 0, Color.Yellow, "Omega Division Commander");

                    AIState = ActionState.Fly;
                    NPC.netUpdate = true;
                    break;

                case ActionState.Fly:
                    gunRot.SlowRotation(NPC.spriteDirection == 1 ? 0f : (float)Math.PI, (float)Math.PI / 60f);
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    switch (TimerRand)
                    {
                        case 0:
                            if (!Main.rand.NextBool(3))
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
                                AIState = (ActionState)Main.rand.Next(2, 4);
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
                                AIState = (ActionState)Main.rand.Next(2, 4);
                                NPC.netUpdate = true;
                            }
                            else
                                NPC.Move(Pos, 14, 20);
                            break;
                    }
                    break;
                case ActionState.Bolts:
                    gunRot.SlowRotation(NPC.DirectionTo(player.Center).ToRotation(), (float)Math.PI / 60f);
                    if (AITimer++ == 0)
                        TimerRand = Main.rand.NextBool() ? 1 : 0;

                    if (AITimer % (TimerRand == 0 ? 10 : 20) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), GunOrigin, RedeHelper.PolarVector(TimerRand == 0 ? 14 : 15, gunRot), ProjectileID.MartianTurretBolt, NPC.damage / 4, 0, Main.myPlayer);
                        //SoundEngine.PlaySound(SoundID.Item91, NPC.position);
                        Main.projectile[proj].tileCollide = false;
                        Main.projectile[proj].timeLeft = 200;
                        Main.projectile[proj].netUpdate2 = true;
                    }
                    if (AITimer >= (TimerRand == 0 ? 60 : 80))
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Fly;
                    }
                    break;
                case ActionState.Orbs:
                    if (NPC.spriteDirection == 1)
                        gunRot.SlowRotation(5.76f, (float)Math.PI / 30f);
                    else
                        gunRot.SlowRotation(3.66f, (float)Math.PI / 30f);

                    if (AITimer++ == 60)
                    {
                        for (int i = 0; i < 5; i++)
                            NPC.Shoot(GunOrigin, ModContent.ProjectileType<Volt_OrbProj>(), NPC.damage, RedeHelper.PolarVector(5 + (i * 3), gunRot), false, SoundID.Item61);
                    }
                    if (AITimer >= 170)
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Fly;
                    }
                    break;
            }
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

            return (RedeGen.LabVector + selection) * 16;
        }
        public static Vector2 PickSidePos()
        {
            WeightedRandom<Vector2> choice = new(Main.rand);
            choice.Add(new Vector2(52, 119));
            choice.Add(new Vector2(121, 119));
            choice.Add(new Vector2(47, 103));
            choice.Add(new Vector2(126, 103));

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
                if (Main.rand.NextBool(3))
                {
                    int dust1 = Dust.NewDust(new Vector2(NPC.Center.X + 6 * NPC.spriteDirection, NPC.Center.Y + 33), 2, 2, DustID.Torch, 0f, 3f, 100, Color.White, NPC.velocity.Length() == 0 ? 1.2f : 2f);
                    Main.dust[dust1].velocity.X = 0;
                    int dust2 = Dust.NewDust(new Vector2(NPC.Center.X + -14 * NPC.spriteDirection, NPC.Center.Y + 33), 2, 2, DustID.Torch, 0f, 3f, 100, Color.White, NPC.velocity.Length() == 0 ? 1.2f : 2f);
                    Main.dust[dust2].velocity.X = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D GunTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Gun").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 value4 = NPC.oldPos[i];
                spriteBatch.Draw(texture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(drawColor) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            Vector2 gunCenter = new(NPC.Center.X, NPC.Center.Y + 6);
            int height = GunTex.Height / 4;
            spriteBatch.Draw(GunTex, gunCenter - screenPos, new Rectangle?(new Rectangle(0, 0, GunTex.Width, height)), NPC.GetAlpha(drawColor), NPC.rotation + gunRot + (NPC.spriteDirection == -1 ? (float)Math.PI : 0), new Vector2(GunTex.Width / 2f, height / 2f), NPC.scale, effects, 0f);
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