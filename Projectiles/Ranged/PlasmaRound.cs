using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class PlasmaRound : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Round");
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.GetGlobalProjectile<RedeProjectile>().Unparryable = true;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(255, 182, 49), new Color(255, 212, 140)), new RoundCap(), new ArrowGlowPosition(), 20f, 350f);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 16;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width / 2, Projectile.height / 2);
            BlastSpawn(oldVelocity / 2);
            return true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            BlastSpawn(Projectile.velocity / 3);
        }
        private void BlastSpawn(Vector2 vel)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + vel, Vector2.Zero, ModContent.ProjectileType<PlasmaRound_Blast>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);

            DustHelper.DrawCircle(Projectile.Center + vel, DustID.OrangeTorch, 1, 2, 2, nogravity: true);
            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/PlasmaBlast"), Projectile.position);
            if (Projectile.DistanceSQ(player.Center) < 600 * 600)
                player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 3;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.localAI[0]++ == 0)
                Projectile.velocity *= 3;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color colour = Color.Lerp(Color.White, new Color(255, 182, 49), 1f / Projectile.localAI[0] * 10f) * (1f / Projectile.localAI[0] * 10f);

            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour, Projectile.rotation, origin, 1.5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour * 0.4f, Projectile.rotation, origin, 2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class PlasmaRound_Blast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.frame < 5 ? null : false;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 5;
            target.immune[Projectile.owner] = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * 0.6f, Projectile.rotation, origin, Projectile.scale + 0.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}