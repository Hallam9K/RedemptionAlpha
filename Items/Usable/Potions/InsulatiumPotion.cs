using Redemption.Buffs;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class InsulatiumPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Insulatium Potion");
            Tooltip.SetDefault("Provides immunity to the Electrified debuff" +
                "\nElectrifies nearby enemies");
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 4));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.width = 20;
            Item.height = 34;
            Item.maxStack = 30;
            Item.value = Item.sellPrice(0, 0, 85, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.buffType = ModContent.BuffType<InsulatiumPotionBuff>();
            Item.buffTime = 36000;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AbyssStinger>())
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
