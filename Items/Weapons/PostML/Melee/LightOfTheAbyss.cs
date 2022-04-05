using Microsoft.Xna.Framework;
using Redemption.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.Rarities;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class LightOfTheAbyss : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light of the Abyss");
            Tooltip.SetDefault("'The abyss hungers...'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 3));
        }

        public override void SetDefaults()
        {
            Item.damage = 310;
            Item.width = 66;
            Item.height = 70;
            Item.value = Item.sellPrice(0, 35, 0, 0);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 6;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<LightOfTheAbyss_Proj>();
            Item.shootSpeed = 4;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 6; i++)
                Projectile.NewProjectile(source, position, velocity * (i * 0.5f), ModContent.ProjectileType<CandleLight_Proj>(), damage / 2, knockback, player.whoAmI);
            return true;
        }
    }
}