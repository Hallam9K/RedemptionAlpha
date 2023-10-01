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
    public class KS3_Magnet : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<KS3>();

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Magnet Drone Mk.I");
            Main.npcFrameCount[NPC.type] = 12;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.width = 14;
            NPC.height = 46;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 12;
            NPC.lifeMax = 750;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath56;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

                new FlavorTextBestiaryInfoElement("This drone converts projectiles into energy to fire back at their targets.")
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 1.2f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 1.2f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        Vector2 vector;
        Vector2 playerOrigin;
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
                vector.X = (float)(Math.Sin(angle) * 200);
                vector.Y = (float)(Math.Cos(angle) * 200);
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
            if (NPC.ai[2] < 180)
            {
                for (int k = 0; k < 3; k++)
                {
                    Vector2 vector2;
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector2.X = (float)(Math.Sin(angle) * 200);
                    vector2.Y = (float)(Math.Cos(angle) * 200);
                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector2, 2, 2, DustID.Frost, 0f, 0f, 100, default, 1f)];
                    dust2.noGravity = true;
                    dust2.velocity *= 0f;
                }
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile target = Main.projectile[i];
                    if (!target.active || target.width >= 40 || target.height >= 40 || NPC.DistanceSQ(target.Center) >= 200 * 200 || !target.friendly || target.damage <= 0 || target.ProjBlockBlacklist())
                        continue;

                    NPC.Shoot(target.Center, ModContent.ProjectileType<KS3_MagnetPulse>(), 0, Vector2.Zero, NPC.whoAmI);
                    NPC.ai[3] += target.damage;
                    target.Kill();
                }
            }
            if (NPC.ai[2] >= 180 && NPC.ai[2] < 240)
            {
                for (int k = 0; k < 4; k++)
                {
                    Vector2 vector2;
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector2.X = (float)(Math.Sin(angle) * 90);
                    vector2.Y = (float)(Math.Cos(angle) * 90);
                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector2, 2, 2, DustID.Frost, 0f, 0f, 100, default, 2f)];
                    dust2.noGravity = true;
                    dust2.velocity = -NPC.DirectionTo(dust2.position) * 8f;
                }
            }
            if (NPC.ai[2] == 210)
                playerOrigin = player.Center;
            if (NPC.ai[2] == 240 && NPC.ai[3] > 10)
            {
                NPC.Shoot(NPC.Center, ModContent.ProjectileType<KS3_MagnetBeam>(), (int)NPC.ai[3] / 4,
                    RedeHelper.PolarVector(10, (playerOrigin - NPC.Center).ToRotation()), CustomSounds.BallFire, NPC.whoAmI);
            }
            if (NPC.ai[2] >= 400)
            {
                NPC.velocity.Y -= 0.5f;
                if (Vector2.Distance(NPC.Center, host.Center) > 1500)
                    NPC.active = false;
            }
            else if (NPC.ai[2] <= 200)
            {
                NPC.MoveToNPC(host, new Vector2(vector.X, vector.Y), 11, 15);
            }
            else
            {
                NPC.velocity *= 0.96f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 11 * frameHeight)
                    NPC.frame.Y = 8 * frameHeight;
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
    public class KS3_MagnetPulse : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Surge");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            int DustID2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
            Main.dust[DustID2].noGravity = true;

            NPC projAim = Main.npc[(int)Projectile.ai[0]];
            if (!projAim.active)
                Projectile.Kill();

            Projectile.Move(projAim.Center, 12, 1);

            if (Projectile.Hitbox.Intersects(projAim.Hitbox))
                Projectile.Kill();
        }
    }
}