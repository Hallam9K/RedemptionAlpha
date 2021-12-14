using Microsoft.Xna.Framework;
using Redemption.DamageClasses;
using Redemption.Globals.Player;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
	public class LastBurden : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Last Burden");
			Tooltip.SetDefault("'It crumbles in your hand...'\nIncreases spirits summoned to max\nSpirits home in on enemies\n[c/bdffff:Spirit Level +5]");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 6));
		}
		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 56;
			Item.value = Item.sellPrice(0, 7, 50, 0);
			Item.accessory = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<Shadesoul>(), 4)
			.AddTile(TileID.LunarCraftingStation)
			.Register();
		}
        public override void UpdateEquip(Player player)
        {
            RedePlayer modPlayer = player.GetModPlayer<RedePlayer>();
            modPlayer.maxSpiritLevel += 5;
            //modPlayer.extraSpirit += 4; Add these when working on ritualist
            //modPlayer.spiritHoming = true;
        }
    }
}
