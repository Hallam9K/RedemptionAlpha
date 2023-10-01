using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Redemption.Globals;
using Terraria.GameContent;

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OO_BarrageMissile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Barrage Missile");
            Main.projFrames[Projectile.type] = 3;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 2)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, 1 * Projectile.Opacity, 0.4f * Projectile.Opacity, 0.4f * Projectile.Opacity);

            Projectile projAim = Main.projectile[(int)Projectile.ai[0]];

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }
            else
                Projectile.localAI[0]++;

            if (Projectile.localAI[0] > 20)
            {
                Vector2 move = Vector2.Zero;
                float distance = 5000f;
                bool target = false;
                for (int k = 0; k < Main.maxProjectiles; k++)
                {
                    if (Main.projectile[k].active && Main.projectile[k].whoAmI == projAim.whoAmI)
                    {
                        Vector2 newMove = Main.projectile[k].Center - Projectile.Center;
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
            var list = Main.projectile.Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
            foreach (var proj in list)
            {
                if (Projectile != proj && proj.whoAmI == projAim.whoAmI)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OO_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);
                    proj.Kill();
                    Projectile.Kill();
                }
            }
        }
        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 24f)
            {
                vector *= 24f / magnitude;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OO_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);
            Projectile.Kill();
        }
        Projectile clearCheck;
        public override void OnKill(int timeLeft)
        {
            Projectile projAim = Main.projectile[(int)Projectile.ai[0]];

            for (int p = 0; p < Main.maxProjectiles; p++)
            {
                clearCheck = Main.projectile[p];
                if (clearCheck.whoAmI == projAim.whoAmI && clearCheck.type != Projectile.type && clearCheck.active)
                    clearCheck.Kill();
            }

            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.MissileExplosion with { Volume = 0.7f }, Projectile.position);
            RedeDraw.SpawnExplosion(Projectile.Center, Color.IndianRed, DustID.LifeDrain);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(glow, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
    public class OO_MissileBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
            Main.projFrames[Projectile.type] = 5;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 144;
            Projectile.height = 144;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                    Projectile.Kill();
            }
            Projectile.scale += 0.06f;
            if (Projectile.frame == 1)
            {
                Projectile.width = 264;
                Projectile.height = 264;
            }
            if (Projectile.frame > 2)
                Projectile.hostile = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 5;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}
