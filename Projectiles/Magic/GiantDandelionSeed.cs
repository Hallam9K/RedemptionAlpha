using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Base;
using Redemption.Globals;

namespace Redemption.Projectiles.Magic
{
    public class GiantDandelionSeed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ElementID.ProjWind[Type] = true;
            ElementID.ProjNature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 42;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 800;
            Projectile.alpha = 255;
        }

        public float vectorOffset = 0f;
        public bool changeSway = false;
        public Vector2 originalVelocity = Vector2.Zero;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.localAI[0] += 1f;
            Projectile.velocity.Y += 0.05f;
            if (Projectile.velocity.Y >= 3f)
                Projectile.velocity.Y = 2.9f;
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

            Projectile.alpha -= 5;
            Projectile.alpha = (int)MathHelper.Max(Projectile.alpha, 0);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, Scale: 1.5f);
        }
    }
}