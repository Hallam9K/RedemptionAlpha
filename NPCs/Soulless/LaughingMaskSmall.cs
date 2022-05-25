using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Base;
using Terraria.Audio;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using Redemption.Biomes;
using Terraria.DataStructures;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Particles;
using ParticleLibrary;

namespace Redemption.NPCs.Soulless
{
    public class LaughingMaskSmall : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laughing Mask");
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Bleeding,
                    BuffID.Poisoned,
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<LaceratedDebuff>(),
                    ModContent.BuffType<BlackenedHeartDebuff>()
                }
            });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1800;
            NPC.damage = 80;
            NPC.defense = 12;
            NPC.knockBackResist = 0f;
            NPC.width = 20;
            NPC.height = 26;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.NPCDeath50;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                    ParticleManager.NewParticle(NPC.Center, RedeHelper.Spread(4), new SoulParticle(), Color.White, 1);

                for (int i = 0; i < 20; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<MaskDust>(), Scale: 2);
                    Main.dust[dustIndex2].velocity *= 3f;
                }
            }
        }
        public override void OnKill()
        {
            NPC nPC = new();
            nPC.SetDefaults(ModContent.NPCType<LaughingMaskBig>());
            NPC nPC2 = new();
            nPC2.SetDefaults(ModContent.NPCType<LaughingMaskMedium>());
            Main.BestiaryTracker.Kills.RegisterKill(nPC);
            Main.BestiaryTracker.Kills.RegisterKill(nPC2);
        }
        Vector2 vector;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest(true);

            bool playerActive = player != null && player.active && !player.dead;
            BaseAI.LookAt(playerActive ? player.Center : (NPC.Center + NPC.velocity), NPC, 0);

            if (NPC.ai[0]++ % 60 == 0)
            {
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * 700);
                vector.Y = (float)(Math.Cos(angle) * 700);
            }
            if (NPC.ai[2] >= 60 && NPC.ai[1] == 0)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.MaskLaugh1);

                NPC.Shoot(NPC.Center, ProjectileID.LostSoulHostile, NPC.damage, RedeHelper.PolarVector(15, (player.Center - NPC.Center).ToRotation()), false, SoundID.Item1);
                NPC.ai[1] = 1;
            }
            else if (NPC.ai[1] == 1)
            {
                NPC.velocity = -NPC.DirectionTo(player.Center) * 7;
                if (NPC.DistanceSQ(player.Center) > 2000 * 2000)
                    NPC.active = false;
            }
            else
            {
                if (NPC.Sight(player, 90, false, true))
                    NPC.ai[2]++;

                float speed = 14;
                if (NPC.DistanceSQ(player.Center) < 1000 * 1000)
                    speed = 2;

                NPC.Move(vector, speed, 40, true);
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[1] == 1)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 2 * frameHeight)
                        NPC.frame.Y = frameHeight;
                }
            }
            else
                NPC.frame.Y = 0;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Dubious little tricksters up to no good. Their origin is not of soul, but of conjuring via an external entity, and as such are safe from other soulless.")
            });
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
    }
}