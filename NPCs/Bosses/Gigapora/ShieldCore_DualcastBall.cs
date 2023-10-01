using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Effects.PrimitiveTrails;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class ShieldCore_DualcastBall : ModProjectile, ITrailProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dualcast");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(223, 62, 55), new Color(150, 20, 54)), new RoundCap(), new DefaultTrailPosition(), 100f, 200f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            if (Projectile.localAI[0] == 0)
            {
                DustHelper.DrawCircle(Projectile.Center, DustID.LifeDrain, 4, dustSize: 2, nogravity: true);
                Projectile.localAI[0] = 1;
            }
            Vector2 move = Vector2.Zero;
            float distance = 2000f;
            bool targetted = false;
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player target = Main.player[p];
                if (!target.active || target.dead || target.invis)
                    continue;

                Vector2 newMove = target.Center - Projectile.Center;
                float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                if (distanceTo < distance)
                {
                    move = target.Center;
                    distance = distanceTo;
                    targetted = true;
                }
            }
            if (targetted)
                Projectile.Move(move, 18, 80);
            else
                Projectile.velocity *= 0.94f;

            if (Projectile.timeLeft <= 260)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile target = Main.projectile[i];
                    if (!target.active || target.whoAmI == Projectile.whoAmI || target.type != Type)
                        continue;

                    if (!Projectile.Hitbox.Intersects(target.Hitbox))
                        continue;

                    target.Kill();
                    Projectile.Kill();
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => Projectile.Kill();
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 0f) * Projectile.Opacity;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Dust dust2 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, Scale: 2)];
                dust2.shader = GameShaders.Armor.GetSecondaryShader(GameShaders.Armor.GetShaderIdFromItemId(ItemID.RedandBlackDye), Main.LocalPlayer);
                dust2.velocity *= 14;
                dust2.noGravity = true;
                dust2.noLight = true;
            }
        }
    }
}
