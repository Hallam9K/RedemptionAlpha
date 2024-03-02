using Redemption.Items.Placeable.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Tools.HM
{
    public class PetrifiedWoodHammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 17;
            Item.hammer = 55;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PetrifiedWood>(), 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
