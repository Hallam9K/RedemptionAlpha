using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class LightOrb_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Textures/WhiteOrb";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Light Orb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjHoly[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.alpha = 5;
        }
        private float squish;
        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            if (Projectile.localAI[1] == 0)
            {
                Projectile.ai[0] += 0.01f;
                if (Projectile.ai[0] > 0.04f)
                    Projectile.localAI[1] = 1;
            }
            else if (Projectile.localAI[1] == 1)
            {
                Projectile.ai[0] -= 0.01f;
                if (Projectile.ai[0] < -0.04f)
                    Projectile.localAI[1] = 0;
            }
            NPC target = null;
            if (RedeHelper.ClosestNPC(ref target, 500, Projectile.Center))
            {
                if (Projectile.localAI[0]++ >= 20 + Main.player[Projectile.owner].ownedProjectileCounts[Type] && Main.myPlayer == Projectile.owner)
                {
                    SoundEngine.PlaySound(SoundID.Item115 with { Volume = .5f }, Projectile.position);
                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LightOrbRay_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI, Projectile.whoAmI);
                    }
                    Projectile.localAI[0] = 0;
                    BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, Projectile, crit: RedeHelper.HeldItemCrit(Projectile));
                }
            }
            squish += Projectile.ai[0];
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1.3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0;
                Color dustColor = new(255, 255, 255) { A = 0 };
                Main.dust[dust].color = dustColor;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<HolyFireDebuff>(), 60);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>("Terraria/Images/Projectile_927").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Vector2 origin2 = new(glow.Width / 2, glow.Height / 2);
            Vector2 scale = new(Projectile.scale + squish, Projectile.scale - squish);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Main.EntitySpriteDraw(texture, oldPos + Projectile.Size / 2f - Main.screenPosition, null, Projectile.GetAlpha(Color.LightYellow) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length), Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightYellow), Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightYellow), Projectile.rotation, origin2, scale, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightYellow), Projectile.rotation + MathHelper.PiOver2, origin2, scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
    public class LightOrbRay_Proj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_536";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Light Ray");
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
        }
        Vector2 pos;
        private float randRot;
        public override void AI()
        {
            Projectile proj = Main.projectile[(int)Projectile.ai[1]];
            if (Projectile.localAI[0] == 0)
            {
                pos = Main.npc[(int)Projectile.ai[0]].Center;
                randRot = Main.rand.NextFloat(-.1f, .1f);
                Projectile.localAI[0] = Main.rand.Next(70, 101);
            }
            Projectile.Center = proj.Center;
            Projectile.alpha += 15;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<HolyFireDebuff>(), 180);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            if (pos != default)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (pos - Projectile.Center).ToRotation() + MathHelper.PiOver2 + randRot, new Vector2(10, 100), new Vector2(Projectile.width / 20f, Projectile.Distance(pos) / Projectile.localAI[0]), SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }
    }
}