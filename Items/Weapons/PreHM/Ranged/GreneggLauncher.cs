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
            DisplayName.SetDefault("Grenegg Launcher");
            Tooltip.SetDefault("Uses Egg Bombs as ammo");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8;
            Item.UseSound = SoundID.Item61;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.shoot = ModContent.ProjectileType<EggBomb_Proj>();
            Item.shootSpeed = 10f;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Blue;
            Item.useAmmo = ModContent.ItemType<EggBomb>();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}