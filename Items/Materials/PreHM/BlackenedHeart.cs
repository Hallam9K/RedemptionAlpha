using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Rarities;

namespace Redemption.Items.Materials.PreHM
{
    public class BlackenedHeart : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Blackened Heart");
            // Tooltip.SetDefault("'May cause instant death'");
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 20;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 3000;
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
        }
    }
}
