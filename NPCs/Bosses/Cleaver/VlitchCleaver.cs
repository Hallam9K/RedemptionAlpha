using Microsoft.Xna.Framework;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Cleaver
{
    [AutoloadBossHead]
    public class VlitchCleaver : ModNPC
    {
        public float[] oldrot = new float[6];

        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Death,
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public int AITimer;

        public ref float TimerRand => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prototype Cleaver");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 280;
            NPC.friendly = false;
            NPC.damage = 160;
            NPC.defense = 60;
            NPC.lifeMax = 55000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 600f;
            NPC.boss = true;
            NPC.knockBackResist = 0.0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossVlitch1");
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 80; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.LifeDrain, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dustIndex].velocity *= 1.9f;
                }
                for (int i = 0; i < 45; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.SparksMech, 0f, 0f, 100, default, 1.2f);
                    Main.dust[dustIndex].velocity *= 1.8f;
                }
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 1.2f);
                    Main.dust[dustIndex].velocity *= 1.8f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.LifeDrain, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.LifeDrain, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public int floatTimer;
        public float rot;
        public float dist;
        public int repeat;


        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            return !player.active || player.dead || Main.dayTime;
        }

        public override void AI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = NPC.rotation;

            NPC host = Main.npc[(int)NPC.ai[3]];
            DespawnHandler();
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);

            Vector2 DefaultPos = new Vector2(host.spriteDirection == 1 ? -180 : 180, -60);
            Vector2 PosLeft = RedeHelper.PolarVector(-200, host.rotation);
            Vector2 PosRight = RedeHelper.PolarVector(200, host.rotation);
            Vector2 PosPlayer = new Vector2(NPC.Center.X > player.Center.X ? 300 : -300, -80);
            Vector2 PosPlayer2 = new Vector2(NPC.Center.X > player.Center.X ? 600 : -600, -80);
            Vector2 PosPlayer3 = new Vector2(NPC.Center.X > player.Center.X ? 200 : -200, -160);
            Vector2 PosPlayer3Check = new Vector2(NPC.Center.X > player.Center.X ? player.Center.X + 200 : player.Center.X - 200, player.Center.Y - 160);

            if (NPC.AnyNPCs(ModContent.NPCType<Wielder>()))
            {
                switch (AIState)
                {
                    case ActionState.Begin:
                        switch (TimerRand)
                        {
                            case 0:
                                {
                                    NPC.rotation = host.spriteDirection == 1 ? (float)-Math.PI / 2 : (float)Math.PI / 2;
                                    NPC.velocity.X = host.spriteDirection == 1 ? 40 : -40;
                                    if (NPC.Distance(host.Center) < 200)
                                    {
                                        for (int i = 0; i < 30; i++)
                                        {
                                            int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.LifeDrain, 0f, 0f, 100, default, 3f);
                                            Main.dust[dustIndex].velocity *= 10f;
                                            Main.dust[dustIndex].noGravity = true;
                                        }
                                        SoundEngine.PlaySound(SoundID.Item74, (int)NPC.position.X, (int)NPC.position.Y);
                                        host.ai[3] = 1;
                                        NPC.velocity *= 0;
                                        TimerRand = 1;
                                        NPC.netUpdate = true;
                                    }
                                }
                                break;
                            case 1:
                                AITimer++;
                                player.GetModPlayer<ScreenPlayer>().Rumble(20, 7);
                                rot = NPC.rotation;
                                if (AITimer > 20) { AIState = ActionState.Idle; AITimer = 0; NPC.netUpdate = true; }
                                break;
                        }
                        break;

                    case ActionState.Idle:
                        if (host.ai[0] >= 1)
                        {
                            TimerRand = 0;
                            AITimer = 0;
                            NPC.MoveToNPC(host, DefaultPos, 24, 20);
                            if (NPC.Distance(host.Center) < 200 || host.ai[3] == 2)
                            {
                                host.ai[3] = 2;
                                rot.SlowRotation(0, (float)Math.PI / 60f);
                                NPC.rotation = rot;
                            }
                            else
                            {
                                NPC.rotation += NPC.velocity.X / 50;
                                rot = NPC.rotation;
                            }
                            if (host.ai[3] == 3)
                            {
                                AIState = ActionState.Attacks;
                            }
                        }

                        break;

                    case ActionState.Attacks:
                        switch (host.ai[3])
                        {
                            case 3:
                                AITimer++;
                                if (AITimer < 80)
                                {
                                    NPC.MoveToNPC(host, DefaultPos, 8, 2);
                                    rot.SlowRotation(NPC.DirectionTo(host.Center).ToRotation() - 1.57f, (float)Math.PI / 30f);
                                    NPC.rotation = rot;
                                }
                                else
                                {
                                    NPC.MoveToNPC(host, DefaultPos, 16, 1);
                                    AITimer = 100;
                                    NPC.rotation = NPC.DirectionTo(host.Center).ToRotation() - 1.57f;
                                }



                                break;
                            case 4:
                                rot = NPC.rotation;
                                NPC.rotation = NPC.DirectionTo(host.Center).ToRotation() - 1.57f;
                                if (AITimer == 0 || AITimer >= 100)
                                {
                                    SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                    NPC.velocity = NPC.DirectionTo(host.Center).RotatedBy(NPC.spriteDirection == 1 ? -Math.PI / 2 : Math.PI / 2) * 40;
                                    AITimer = 1;
                                }
                                else
                                {
                                    AITimer++;
                                    if (AITimer < 20)
                                    {
                                        //NPC.Shoot(new Vector2(npc.Center.X, npc.Center.Y) + RedeHelper.PolarVector(134, npc.rotation + (float)-Math.PI / 2), ModContent.ProjectileType<OmegaBlast3>(), 92, RedeHelper.PolarVector(2, npc.rotation + (float)-Math.PI / 2), false, SoundID.Item1.WithVolume(0));
                                        NPC.velocity -= NPC.velocity.RotatedBy(Math.PI / 2) * NPC.velocity.Length() / NPC.Distance(host.Center);
                                    }
                                    else
                                    {
                                        NPC.velocity *= .7f;
                                    }
                                }

                                if (host.ai[2] > 150)
                                {
                                    AIState = ActionState.Idle;
                                }

                                break;
                            #region Stab
                            case 5:
                                if (AITimer == 0 || AITimer >= 100)
                                {
                                    AITimer = 1;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    AITimer++;
                                    if (AITimer < 50)
                                    {
                                        NPC.velocity *= .94f;
                                        rot.SlowRotation(NPC.DirectionTo(player.Center).ToRotation() + 1.57f, (float)Math.PI / 30f);
                                        NPC.rotation = rot;
                                    }
                                    else if (AITimer <= 70)
                                    {
                                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;
                                    }
                                    if (AITimer == 50)
                                    {
                                        NPC.Dash(40, true, SoundID.Item74, player.Center);
                                    }
                                    if (AITimer == 70 && RedeHelper.Chance(.4f))
                                    {
                                        host.ai[2] = 60;
                                        AITimer = 0;
                                        NPC.netUpdate = true;
                                    }
                                    if (AITimer > 70)
                                    {
                                        NPC.velocity *= .94f;
                                    }
                                }

                                if (host.ai[2] > 200)
                                {
                                    AIState = ActionState.Idle;
                                }


                                break;
                            #endregion

                            #region Speen I
                            case 6:
                                if (AITimer == 0 || AITimer >= 100)
                                {
                                    AITimer = 1;
                                    NPC.netUpdate = true;
                                }
                                else
                                {
                                    AITimer++;
                                    if (AITimer % 10 == 0)
                                    {
                                        //NPC.Shoot(new Vector2(NPC.Center.X, NPC.Center.Y) + RedeHelper.PolarVector(134, NPC.rotation + (float)-Math.PI / 2), ModContent.ProjectileType<OmegaBlast3>(), 92, RedeHelper.PolarVector(5, NPC.rotation + (float)-Math.PI / 2), false, SoundID.Item1.WithVolume(0));
                                    }
                                    if (AITimer < 60)
                                    {
                                        NPC.MoveToNPC(host, PosLeft, 18, 20);
                                    }
                                    else
                                    {
                                        NPC.MoveToNPC(host, PosRight, 18, 20);
                                    }
                                    if (AITimer > 120)
                                    {
                                        AITimer = 1;
                                        NPC.netUpdate = true;
                                    }
                                    NPC.rotation += NPC.velocity.X / 30;
                                }

                                break;
                                #endregion
                        }
                        break;
                }
            }
        }


        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.velocity *= 0.96f;
                NPC.velocity.Y -= 1;
                if (NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                }
                return;
            }
        }
    }
}