using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Terraria.DataStructures;
using Redemption.Globals;
using Redemption.Buffs.Debuffs;
using System.IO;
using Redemption.Projectiles.Magic;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class PZ_Kari : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public ref float TimerRand2 => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kari Johansson");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<BileDebuff>(),
                    ModContent.BuffType<GreenRashesDebuff>(),
                    ModContent.BuffType<GlowingPustulesDebuff>(),
                    ModContent.BuffType<FleshCrystalsDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 52;
            NPC.height = 66;
            NPC.friendly = false;
            NPC.damage = 140;
            NPC.defense = 10;
            NPC.lifeMax = 120000;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
            NPC.netAlways = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic2");
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 20, default, 4f);
                    Main.dust[dustIndex].velocity *= 1.9f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 20, default, 1f);
        }
        public override bool CheckDead()
        {
            NPC.life = 1;
            NPC host = Main.npc[(int)NPC.ai[0]];
            if (host.ai[0] != 4)
            {
                host.ai[0] = 4;
                host.ai[1] = 0;
                host.ai[2] = 0;
                host.ai[3] = 0;
                host.netUpdate = true;
            }
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(Exposed);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Exposed = reader.ReadBoolean();
            }
        }
        private bool Exposed;
        public override void AI()
        {
            NPC host = Main.npc[(int)NPC.ai[0]];
            Player player = Main.player[host.target];
            if (!host.active || host.type != ModContent.NPCType<PZ>())
                NPC.active = false;

            if (host.ai[0] == 3)
            {
                if (!Exposed)
                {
                    Exposed = true;
                    NPC.dontTakeDamage = false;
                    NPC.netUpdate = true;
                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                }
                int hostPhase;
                if (host.life > host.lifeMax / 2)
                    hostPhase = 1;
                else if (host.life <= host.lifeMax / 2 && host.life > (int)(host.lifeMax * 0.35f))
                    hostPhase = 2;
                else if (host.life > (int)(host.lifeMax * 0.1f) && host.life <= (int)(host.lifeMax * 0.35f))
                    hostPhase = 3;
                else
                    hostPhase = 4;

                if (hostPhase <= 1 && NPC.life <= (int)(NPC.lifeMax * 0.75f))
                {
                    host.ai[1] = -1;
                    host.netUpdate = true;
                }
                else if (hostPhase == 2 && NPC.life <= NPC.lifeMax / 2)
                {
                    host.ai[1] = -1;
                    host.netUpdate = true;
                }
                else if (hostPhase == 3 && NPC.life <= (int)(NPC.lifeMax * 0.25f))
                {
                    host.ai[1] = -1;
                    host.netUpdate = true;
                }

                switch (TimerRand2)
                {
                    case 0:
                        AITimer++;
                        if (AITimer % 5 == 0 && AITimer >= 120)
                        {
                            switch (hostPhase)
                            {
                                case 1:
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<TearOfInfectionBall>(), (int)(NPC.damage * 0.9f), RedeHelper.PolarVector(12, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), true, SoundID.Item20);
                                    break;
                                case 2:
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<CausticTearBall>(), (int)(NPC.damage * 0.95f), RedeHelper.PolarVector(11, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), true, SoundID.Item20);
                                    break;
                                case 3:
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<TearOfPainBall>(), NPC.damage, RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), true, SoundID.Item20);
                                    break;
                                case 4:
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<TearOfPainBall>(), NPC.damage, RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), true, SoundID.Item20);
                                    break;
                            }
                        }
                        if (AITimer >= 138)
                        {
                            TimerRand2++;
                            AITimer = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    case 1:
                        AITimer++;
                        switch (hostPhase)
                        {
                            case 1:
                                if (AITimer % 100 == 0)
                                {
                                    for (int i = 0; i < 6; i++)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<PoisonBeat>(), (int)(NPC.damage * 0.9f), new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-16, -4)), true, SoundID.Item72);
                                }
                                break;
                            case 2:
                                if (AITimer % 90 == 0)
                                {
                                    for (int i = 0; i < 6; i++)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<PoisonBeat>(), (int)(NPC.damage * 0.9f), new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-16, -4)), true, SoundID.Item72);
                                    for (int i = 0; i < 4; i++)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<InfectiousBeat>(), (int)(NPC.damage * 0.9f), new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-16, -4)), true, SoundID.Item72);
                                }
                                break;
                            case 3:
                                if (AITimer % 80 == 0)
                                {
                                    for (int i = 0; i < 8; i++)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<PoisonBeat>(), (int)(NPC.damage * 0.9f), new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-16, -4)), true, SoundID.Item72);
                                    for (int i = 0; i < 4; i++)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<InfectiousBeat>(), (int)(NPC.damage * 0.9f), new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-16, -4)), true, SoundID.Item72);
                                }
                                break;
                            case 4:
                                if (AITimer % 70 == 0)
                                {
                                    for (int i = 0; i < 8; i++)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<PoisonBeat>(), (int)(NPC.damage * 0.9f), new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-16, -4)), true, SoundID.Item72);
                                    for (int i = 0; i < 4; i++)
                                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<InfectiousBeat>(), (int)(NPC.damage * 0.9f), new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-16, -4)), true, SoundID.Item72);
                                }
                                break;
                        }
                        if (AITimer >= 460)
                        {
                            TimerRand2++;
                            AITimer = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                    case 2:
                        if (host.life > host.lifeMax / 2)
                        {
                            AITimer = 0;
                            TimerRand2 = 0;
                        }
                        else
                        {
                            AITimer++;
                            if (AITimer % (hostPhase >= 3 ? 30 : 60) == 0 && AITimer >= 120)
                            {
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<PZ_Kari_Laser>(), NPC.damage, RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.06f, 0.06f)), true, SoundID.Item103, NPC.whoAmI);
                            }
                            if (AITimer >= 340)
                            {
                                TimerRand2 = 0;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        break;
                }
            }
            else
            {
                if (Exposed)
                {
                    AITimer = 0;
                    Exposed = false;
                    NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                    if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                }
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ProjectileID.LastPrismLaser)
                damage /= 3;
            if (projectile.type == ModContent.ProjectileType<LightOrb_Proj>())
                damage /= 2;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
    }
}