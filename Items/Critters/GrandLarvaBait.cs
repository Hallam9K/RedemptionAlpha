using Redemption.NPCs.Critters;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Critters
{
    public class GrandLarvaBait : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Grand Larva");

            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 2);
            Item.rare = ItemRarityID.Blue;
            Item.bait = 55;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = true;
            Item.makeNPC = ModContent.NPCType<GrandLarva>();
        }
    }
}