using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class BloodstainedPike : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ArcaneS);
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Holding out the pike and charging into weak enemies skewers them\n" +
                "If a target is skewered for 10 seconds or once 5 are skewered at a time:\n" +
                "The pike takes their life and becomes enchanted for 10 seconds"); */

            ItemID.Sets.Spears[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 72;
            Item.height = 72;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 5);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 23;
            Item.useTime = 23;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 60;
            Item.knockBack = 6.2f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 3.7f;
            Item.shoot = ProjectileType<BloodstainedPike_Proj>();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ProjectileType<BloodstainedPike_Proj2>()] < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteGlaive)
                .AddIngredient(ItemID.SoulofNight, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumTrident)
                .AddIngredient(ItemID.SoulofNight, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}