using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb.Phase2
{
    public class CosmicEye2 : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/CosmicEye";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Eye");
        }
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 46;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 500;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<CosmicEye_Beam2>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.whoAmI);
                    Projectile.localAI[0] = 1;
                }
            }
            else
            {
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] >= 300)
                {
                    Projectile.alpha += 6;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                }
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool ShouldUpdatePosition() => false;
    }
}