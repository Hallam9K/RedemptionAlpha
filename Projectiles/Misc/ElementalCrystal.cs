using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Redemption.Dusts;
using Terraria.GameContent;
using System;
using Redemption.Buffs;

namespace Redemption.Projectiles.Misc
{
    public class ElementalCrystal : ModProjectile
    {
        public float[] oldrot = new float[8];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Elemental Crystal");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
            Projectile.alpha = 255;
        }
        public override bool? CanCutTiles() => false;
        public ref float Element => ref Projectile.ai[1];
        private Color elemColor;
        public int speed;
        public override void OnSpawn(IEntitySource source)
        {
            speed = Main.rand.Next(2, 5);
            elemColor = Element switch
            {
                2 => Color.Orange,
                3 => Color.SkyBlue,
                4 => Color.LightCyan,
                5 => Color.SandyBrown,
                6 => Color.LightGray,
                7 => Color.LightYellow,
                8 => Color.LightGoldenrodYellow,
                9 => Color.MediumSlateBlue,
                10 => Color.LawnGreen,
                11 => Color.MediumPurple,
                12 => Color.IndianRed,
                13 => Color.LightPink,
                14 => Color.Pink,
                _ => Color.LightBlue,
            };
            Projectile.GetGlobalProjectile<ElementalProjectile>().OverrideElement[(int)Element] = 1;
            Projectile.netUpdate = true;
        }
        private bool onSpawn;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!onSpawn)
            {
                bool noIntersect = false;
                while (!noIntersect)
                {
                    noIntersect = true;
                    for (int n = 0; n < Main.maxProjectiles; n++)
                    {
                        Projectile proj = Main.projectile[n];
                        if (!proj.active || proj.owner != Projectile.owner || proj.whoAmI == Projectile.whoAmI || proj.type != Type)
                            continue;

                        if (proj.alpha > 0)
                        {
                            Projectile.active = false;
                            return;
                        }

                        if (!Projectile.Hitbox.Intersects(proj.Hitbox))
                            continue;

                        noIntersect = false;
                        Projectile.localAI[0] = RedeHelper.RandomRotation();
                        Projectile.localAI[1] = Main.rand.Next(50, 100);
                        Projectile.Center = player.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.localAI[0])) * Projectile.localAI[1];
                    }
                }
                onSpawn = true;
            }
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            if (!CheckActive(player))
                return;

            Projectile.rotation = (player.Center - Projectile.Center).ToRotation();

            Projectile.alpha -= 4;
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.localAI[0] is -1)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    if (Projectile.alpha > 0)
                        Projectile.Kill();
                    Projectile.tileCollide = true;
                    Projectile.velocity.Y += .05f;
                    return;
                }
                if (player.Redemption().onHit)
                {
                    player.ClearBuff(ModContent.BuffType<CrystalKnowledgeBuff>());
                    Projectile.timeLeft = 300;
                    Projectile.penetrate = 1;
                    Projectile.localAI[0] = -1;
                    Projectile.localAI[1] = 0;
                    Projectile.velocity = RedeHelper.PolarVector(10, Projectile.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2);
                    return;
                }
                Projectile.localAI[0] += speed;
                Projectile.Center = player.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.localAI[0])) * Projectile.localAI[1];
            }
            Projectile.alpha = (int)MathHelper.Max(Projectile.alpha, 0);
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<CrystalKnowledgeBuff>());
                return false;
            }
            if (owner.HasBuff(ModContent.BuffType<CrystalKnowledgeBuff>()))
                Projectile.timeLeft = 2;
            return true;
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            float pulse = (float)Math.Abs(Math.Sin(drawTimer++ / 20));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin - new Vector2(13, 26) + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(elemColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, oldrot[k], origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(elemColor) * pulse * 10, Projectile.rotation, origin, Projectile.scale * pulse, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 25; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Alpha: Projectile.alpha, Scale: .5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                Color dustColor = new(elemColor.R, elemColor.G, elemColor.B) { A = 0 };
                Main.dust[dust].color = dustColor;
            }
        }
    }
}