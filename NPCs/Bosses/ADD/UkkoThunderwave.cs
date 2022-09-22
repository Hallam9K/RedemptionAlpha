using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Effects.PrimitiveTrails;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoThunderwave : ModProjectile, ITrailProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ukko's Thunderwave");
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 60;
            Projectile.timeLeft = 180;
        }
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new StandardColorTrail(Color.White), new RoundCap(), new ZigZagTrailPosition(6), 80f, 250f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.1f, 1f, 1f));
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity, Projectile.Opacity, Projectile.Opacity);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Electrified, target.HasBuff(BuffID.Wet) ? 320 : 160);
        }
    }
}