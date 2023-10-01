using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.NPCBuffs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class DrillRevolver_Bullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drill Bit");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 20;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 1;
        }
        private Vector2 origVelocity;
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item22, Projectile.position);
            origVelocity = Projectile.velocity;
        }
        public override void AI()
        {
            Projectile.localAI[0] = 0;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                if (Projectile.localAI[1] % 10 == 0)
                {
                    Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                    SoundEngine.PlaySound(SoundID.Dig with { Volume = .2f }, Projectile.position);
                }
                Projectile.localAI[0] = 1;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                    continue;
                if (!Projectile.Hitbox.Intersects(npc.Hitbox))
                    continue;
                Projectile.localAI[0] = 1;
            }

            if (Projectile.localAI[0] == 1)
            {
                Projectile.velocity = origVelocity / 8;
                if (Projectile.localAI[1]++ % 10 == 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke,
                            -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f);
                        Main.dust[d].noGravity = true;
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MartianSaucerSpark,
                            -Projectile.velocity.X * 2f, -Projectile.velocity.Y * 2f, Scale: 0.5f);
                        Main.dust[d].noGravity = true;
                    }
                }
                if (Projectile.localAI[1] % 20 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item23 with { Volume = .4f, Pitch = .2f }, Projectile.position);
                }
            }
            else
                Projectile.velocity = origVelocity;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0;
            modifiers.FinalDamage *= Projectile.penetrate / 30f;

            if (Projectile.penetrate <= 2)
                Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrokenArmorDebuff>(), 40);
            Projectile.localNPCImmunity[target.whoAmI] = 8;
            target.immune[Projectile.owner] = 0;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 10;
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke,
                    -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f);
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 6; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SparksMech,
                    -Projectile.velocity.X * 0.7f, -Projectile.velocity.Y * 0.7f);
                Main.dust[d].noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.IndianRed) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}