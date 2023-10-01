using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ritualist
{
    public class HellfireCharge_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hellfire Charge");
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.alpha = 255;
        }
        private Vector2 pos;
        private float rot;
        public override void OnSpawn(IEntitySource source)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            rot = Main.rand.NextBool(2) ? 0.1f : -0.1f;
            pos = new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height));
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.3f, Projectile.Opacity * 0.1f, Projectile.Opacity * 0.1f);
            Projectile.velocity *= 0;
            Projectile.Center = npc.position + pos;
            Projectile.rotation += rot;

            if (Projectile.ai[1] == 1)
            {
                if (Main.rand.NextBool(4))
                    Projectile.Kill();
            }

            if (Projectile.alpha >= 0)
            {
                Projectile.alpha -= 10;
            }
            else
                Projectile.alpha = 0;
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] == 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                RedeDraw.SpawnExplosion(Projectile.Center, Color.Orange, 6, 4, 10, 1, 0.5f);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || !target.CanBeChasedBy())
                        continue;

                    if (target.immune[Projectile.whoAmI] > 0 || Projectile.DistanceSQ(target.Center) > 100 * 100)
                        continue;

                    target.immune[Projectile.whoAmI] = 1;
                    int hitDirection = target.RightOfDir(Projectile);
                    BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Vector2 drawOrigin = new(glow.Width / 2, glow.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.Orange * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale * 0.3f, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.Orange * Projectile.Opacity, -Projectile.rotation, drawOrigin, Projectile.scale * 0.3f, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}