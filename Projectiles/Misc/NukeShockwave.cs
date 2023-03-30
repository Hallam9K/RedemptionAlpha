using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;
using Redemption.Buffs.Debuffs;

namespace Redemption.Projectiles.Misc
{
    class NukeShockwave : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave");
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
        }
        public override bool? CanCutTiles()
        {
            return Projectile.timeLeft >= 220;
        }
        public override void AI()
        {
            for(int i = 0; i < 30; ++i)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 3);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 3);
            }
            Projectile.height += Math.Abs((int)(Projectile.velocity.X / 4));
            Projectile.width += Math.Abs((int)(Projectile.velocity.Y / 4));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 900);
            Vector2 groundZeroPos = new(Projectile.ai[0], Projectile.ai[1]);
            Vector2 throwVelocity = target.Center - groundZeroPos;
            throwVelocity.Normalize();
            throwVelocity *= Projectile.velocity.Length() / 4;
            target.velocity += throwVelocity;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!target.behindBackWall)
            {
                target.AddBuff(BuffID.OnFire, 600);
                target.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 900);
                target.AddBuff(ModContent.BuffType<RadiationDebuff>(), 300);
                
                Vector2 groundZeroPos = new(Projectile.ai[0], Projectile.ai[1]);
                Vector2 throwVelocity = target.Center - groundZeroPos;
                throwVelocity.Normalize();
                throwVelocity *= Projectile.velocity.Length() / 4;
                target.velocity += throwVelocity;
            }
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.NukeExplosion, target.position);
        }
    }
}
