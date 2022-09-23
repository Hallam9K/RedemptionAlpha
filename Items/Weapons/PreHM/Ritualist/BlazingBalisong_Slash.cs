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
            DisplayName.SetDefault("Blazing Balisong");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            RedeProjectile.Decapitation(target, ref damage, ref crit);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 260);
            if (Main.rand.NextBool(2) && Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<HellfireCharge_Proj>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, target.whoAmI);
            }
        }
    }
}