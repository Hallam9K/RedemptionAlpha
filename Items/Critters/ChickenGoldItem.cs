using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Redemption.NPCs.Critters;
using Terraria.DataStructures;

namespace Redemption.Items.Critters
{
    public class ChickenGoldItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gold Chicken");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = true;
            Item.makeNPC = ModContent.NPCType<ChickenGold>();
        }
    }
}
