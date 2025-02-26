using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class NoblesHalberd : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.SpearS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Noble's Halberd");
            /* Tooltip.SetDefault("Right-click to thrust the halberd\n" +
                "'One of the primary weapons used by Anglon's Common Guard'"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 58;
            Item.height = 58;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 30);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 34;
            Item.knockBack = 10.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 3.7f;
            Item.shoot = ProjectileType<NoblesHalberd_SlashProj>();
            Item.Redemption().TechnicallyAxe = true;
        }
        public override bool MeleePrefix() => true;

        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                damage = (int)(damage * 0.34f);
                knockback -= 4;
                type = ProjectileType<NoblesHalberd_Proj>();
            }
            else
            {
                type = ProjectileType<NoblesHalberd_SlashProj>();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<GraveSteelAlloy>(), 18)
                .AddRecipeGroup(RecipeGroupID.Wood, 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
