using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class PetrifiedWoodSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Petrified Wood Sword");
            // Tooltip.SetDefault("'About as useful as a burnt twig'");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.damage = 32;
            Item.DamageType = DamageClass.Melee;
            Item.width = 34;
            Item.height = 36;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7;
            Item.value = Item.buyPrice(0, 0, 0, 20);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PetrifiedWood>(), 7)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
