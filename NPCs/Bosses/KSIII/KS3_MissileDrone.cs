using Terraria;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Base;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_MissileDrone : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<KS3>();

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Missile Drone Mk.I");
            Main.npcFrameCount[NPC.type] = 4;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 36;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 600;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath56;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

                new FlavorTextBestiaryInfoElement("A missile drone built by King Slayer III during his million year voyage.")
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 1.2f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 1.2f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        Vector2 vector;
        public int shotCount;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.LookAtEntity(player);
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            float soundVolume = NPC.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (NPC.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = soundVolume }, NPC.position);
                NPC.soundDelay = 10;
            }
            NPC host = Main.npc[(int)NPC.ai[0]];
            if (!host.active || (host.type != ModContent.NPCType<KS3>() && host.type != ModContent.NPCType<KS3_Clone>()))
                NPC.active = false;

            if (++NPC.ai[1] % 80 == 0)
            {
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * 100);
                vector.Y = (float)(Math.Cos(angle) * 100);
                NPC.ai[1] = 0;
            }
            if (NPC.ai[2]++ == 0)
            {
                for (int m = 0; m < 16; m++)
                {
                    int dustID = Dust.NewDust(new Vector2(NPC.Center.X - 1, NPC.Center.Y - 1), 2, 2, DustID.Frost, 0f, 0f, 100, Color.White, 2f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)16 * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
            }
            if (NPC.ai[2] > 120 && NPC.ai[2] < 500 && shotCount < 4)
            {
                if (NPC.ai[2] % 30 == 0)
                {
                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<SlayerMissile>(), 96, RedeHelper.PolarVector(10, (player.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(0.2f, 0.2f)), SoundID.Item74);
                    shotCount++;
                }
            }
            if (NPC.ai[2] >= 500 || shotCount >= 4)
            {
                NPC.velocity.Y -= 0.5f;
                if (Vector2.Distance(NPC.Center, host.Center) > 1500)
                    NPC.active = false;
            }
            else
            {
                NPC.MoveToNPC(host, new Vector2(vector.X, vector.Y), 11, 15);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
    public class SlayerMissile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drone Missile");
            Main.projFrames[Projectile.type] = 2;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                    Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Alarm2 with { Volume = .2f, PitchVariance = .1f }, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            if (Projectile.localAI[0]++ < 20)
            {
                Vector2 move = Vector2.Zero;
                float distance = 1200f;
                bool target = false;
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    Player player = Main.player[k];
                    if (player.active && !player.dead && !player.invis)
                    {
                        Vector2 newMove = player.Center - Projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                        }
                    }
                }
                if (target)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }
        }
        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 20f)
            {
                vector *= 19f / magnitude;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, texture.Width, height)), lightColor, Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, effects, 0);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, texture.Width, height)), Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, effects, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }

        }
    }
}