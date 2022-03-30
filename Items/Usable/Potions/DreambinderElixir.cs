using Redemption.Buffs;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Shade;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class DreambinderElixir : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("Increases length of invincibility after taking damage" +
                "\nNot consumable\n" +
                "'Remembering the warmth of a much brighter day'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
		{
			Item.width = 28;
            Item.height = 42;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.healLife = 250;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = false;
            Item.potion = true;
            Item.buffType = ModContent.BuffType<DreambinderBuff>();
            Item.buffTime = 1200;
            Item.value = Item.sellPrice(0, 15, 50, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            //if (quickHeal)
            //    player.AddBuff(ModContent.BuffType<DreambinderBuff>(), 1200);
        }
        public override bool ConsumeItem(Player player) => false;
    }
    public class DreambinderElixirPlaceable : ModItem
    {
        public override string Texture => "Redemption/Items/Usable/Potions/DreambinderElixir";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dreambinder Elixir (Placeable)");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 42;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.consumable = true;
            Item.createTile = ModContent.TileType<DreambinderElixirTile>();
        }
    }
}