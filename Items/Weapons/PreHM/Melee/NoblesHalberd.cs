using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Items.Materials.PreHM;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class NoblesHalberd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Noble's Halberd");
            Tooltip.SetDefault("'One of the primary weapons used by Anglon's Common Guard'"
                + "\nRight-click to thrust the halberd");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 58;
            Item.height = 58;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;	

            // Weapon Properties
            Item.damage = 34;
            Item.knockBack = 10.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<NoblesHalberd_SlashProj>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                damage = (int)(damage * 0.34f);
                knockback -= 4;
                type = ModContent.ProjectileType<NoblesHalberd_Proj>();
            }
            else
            {
                type = ModContent.ProjectileType<NoblesHalberd_SlashProj>();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GraveSteelAlloy>(), 18)
                .AddRecipeGroup(RecipeGroupID.Wood, 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}