using Redemption.Buffs.Debuffs;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class XenomiteItem : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("'Infects living things'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
            SacrificeTotal = 25;
        }
        public override void SetDefaults()
		{
			Item.width = 14;
            Item.height = 24;
			Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Lime;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XenomiteShard>(), 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void HoldItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 10);
        }
    }
}
