using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Terraria.DataStructures;
using Redemption.DamageClasses;
using Redemption.Globals.Player;
using Redemption.Projectiles.Ritualist;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BuddingBoline_Slash : BlightedBoline_Slash
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Budding Boline");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (player.whoAmI == Main.myPlayer && player.GetModPlayer<RitualistPlayer>().bolineFlower)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, RedeHelper.PolarVector(8, (Main.MouseWorld - player.Center).ToRotation()), ModContent.ProjectileType<BolineFlower>(), 0, 0, player.whoAmI);
                player.GetModPlayer<RitualistPlayer>().bolineFlower = false;
            }
        }
    }
}