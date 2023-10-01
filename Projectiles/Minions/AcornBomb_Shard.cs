using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class AcornBomb_Shard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn Shard");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ElementID.ProjNature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            Projectile.rotation += 0.06f;
            Projectile.velocity.Y += 0.2f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig with { Volume = .2f }, Projectile.position);
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture, Scale: 1.5f);
        }
    }
}