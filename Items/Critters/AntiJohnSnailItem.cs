using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.NPCs.Critters;

namespace Redemption.Items.Critters
{
    public class AntiJohnSnailItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Anti-John Snail");
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(silver: 80);
            Item.rare = ItemRarityID.Blue;
            Item.bait = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = true;
            Item.makeNPC = ModContent.NPCType<AntiJohnSnail>();
        }
    }
}
