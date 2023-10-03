using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoCloud_Thunder : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                int floor = BaseWorldGen.GetFirstTileFloor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16) * 16;
                int dist = floor - (int)Projectile.Center.Y;
                dist = (int)MathHelper.Clamp(dist, 0, 800);
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + new Vector2(0, dist), 1f, 30, 0.1f, 1);

                Projectile.localAI[0] = 1;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
        }
    }
}