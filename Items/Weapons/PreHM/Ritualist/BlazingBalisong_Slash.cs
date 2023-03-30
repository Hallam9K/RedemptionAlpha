using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.ID;
using Redemption.Projectiles.Ritualist;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BlazingBalisong_Slash : Incisor_Slash
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blazing Balisong");
            Main.projFrames[Projectile.type] = 5;
            ElementID.ProjFire[Type] = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
            target.AddBuff(BuffID.OnFire, 260);
            if (Main.rand.NextBool(2) && Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<HellfireCharge_Proj>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, target.whoAmI);
            }
        }
    }
}