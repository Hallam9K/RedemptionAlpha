using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class Zweihander : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zweihander");
            Tooltip.SetDefault("'Parry this you filthy casual!'" +
                "\nParries physical projectiles");

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 74;
            Item.height = 74;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 50;
            Item.knockBack = 7;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<Zweihander_SlashProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ZweihanderFragment1>())
                .AddIngredient(ModContent.ItemType<ZweihanderFragment2>())
                .AddCondition(new Recipe.Condition(NetworkText.FromLiteral("Repaired by the Fallen"), _ => false))
                .Register();
        }
    }
}
