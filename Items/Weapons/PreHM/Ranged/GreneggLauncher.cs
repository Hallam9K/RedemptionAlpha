using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class GreneggLauncher : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Grenegg Launcher");
            // Tooltip.SetDefault("Uses Egg Bombs as ammo");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8;
            Item.UseSound = SoundID.Item61;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.shoot = ModContent.ProjectileType<EggBomb_Proj>();
            Item.shootSpeed = 3f;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Blue;
            Item.useAmmo = ModContent.ItemType<EggBomb>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage -= 28;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}