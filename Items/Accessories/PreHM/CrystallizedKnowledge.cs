using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Items.Accessories.PreHM
{
    public class CrystallizedKnowledge : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Hitting enemies using a weapon with any element will cause elemental crystals to gradually appear, circling around the user\n" +
                "\nThe element of the crystals are based on the element of the weapon used to create them\n" +
                "Getting hit will cause the crystals to break away from the user\n" +
                //"Once 6 crystals have been created, they will amass into an enchanted tome which fires bolts based on each crystal's element" +
                "\n4% increased elemental damage and resistance for each crystal of an element to be active"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Generic;
            Item.knockBack = 1;
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 3);
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().crystalKnowledge = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<TrappedSoulBauble>())
                .AddIngredient(ModContent.ItemType<LostSoul>(), 4)
                .AddIngredient(ItemID.CrystalShard, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
