using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class OmegaBattery : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Omega Battery");
			Main.RegisterItemAnimation(Item.type, (DrawAnimation)new DrawAnimationVertical(4, 3));
            SacrificeTotal = 25;
        }
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 40;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Red;
		}
	}
}
