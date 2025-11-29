using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Thorn;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class ThornCircletSplinter : Thorn_Splinter
    {
        public override string Texture => "Redemption/NPCs/Bosses/Thorn/Thorn_Splinter";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            ElementID.ProjNature[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.Redemption().friendlyHostile = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int chance = Main.expertMode ? 2 : 4;
            if (Main.rand.NextBool(chance))
                target.AddBuff(BuffID.Poisoned, 80);
        }
    }
}