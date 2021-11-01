using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class SlayerGun : ModItem
    {
        public override void SetStaticDefaults()
        {        
            DisplayName.SetDefault("Hyper-Tech Blaster");
            Tooltip.SetDefault("'Pewpewpewpewpewpewpew'"
                + "\nReplaces normal bullets with Phantasmal Bolts"
                + "\nRight-clicking fires 5 bolts in an arc");
        }

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 26;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item91;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 90;
            Item.useAmmo = AmmoID.Bullet;
            if (!Main.dedServ)
                Item.GetGlobalItem<ItemUseGlow>().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = Item.useTime * 3;
                player.itemTime = Item.useTime * 3;
                player.itemAnimation = Item.useTime * 3;
                damage = (int)(damage * 1.4f);

                float numberProjectiles = 5;
                float rotation = MathHelper.ToRadians(25);
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                    int projectile1 = Projectile.NewProjectile(source, position, perturbedSpeed, ProjectileID.PhantasmalBolt, damage, knockback, player.whoAmI);
                    Main.projectile[projectile1].hostile = false;
                    Main.projectile[projectile1].friendly = true;
                    Main.projectile[projectile1].DamageType = DamageClass.Ranged;
                    Main.projectile[projectile1].netUpdate2 = true;
                }
            }
            else
            {
                int projectile2 = Projectile.NewProjectile(source, position, velocity, ProjectileID.PhantasmalBolt, damage, knockback, player.whoAmI);
                Main.projectile[projectile2].hostile = false;
                Main.projectile[projectile2].friendly = true;
                Main.projectile[projectile2].DamageType = DamageClass.Ranged;
                Main.projectile[projectile2].netUpdate2 = true;
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
    }
}