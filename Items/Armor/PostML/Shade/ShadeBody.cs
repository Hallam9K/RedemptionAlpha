using Redemption.DamageClasses;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Armor.PostML.Shade
{
    [AutoloadEquip(EquipType.Body)]
    public class ShadeBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadeplate");
            /* Tooltip.SetDefault("10% increased ritual damage\n" +
                "\n[c/bdffff:Maximum Spirit Level +2]"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.sellPrice(7, 50, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.defense = 32;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RitualistClass>() += .10f;
            RedePlayer modPlayer = player.Redemption();
            modPlayer.maxSpiritLevel += 2;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Shadesoul>(), 4)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}