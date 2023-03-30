using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Keeper
{
    public class VeilFX : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Veil");
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 18;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200;
        }

        public float vectorOffset = 0f;
        public bool changeSway = false;
        public Vector2 originalVelocity = Vector2.Zero;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.localAI[0]++;
            Projectile.velocity.Y += 0.04f;
            Projectile.rotation += 0.05f;
            if (changeSway)
            {
                vectorOffset -= 0.01f;
                if (vectorOffset <= -1f)
                {
                    vectorOffset = -1f;
                    changeSway = false;
                }
            }
            else
            {
                vectorOffset += 0.01f;
                if (vectorOffset >= 1f)
                {
                    vectorOffset = 1f;
                    changeSway = true;
                }
            }
            float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
            Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(0f, Projectile.velocity.Length()), velRot + (vectorOffset * 0.5f));
        }
    }
}