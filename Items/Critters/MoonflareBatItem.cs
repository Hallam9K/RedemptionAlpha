using Redemption.NPCs.Critters;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Critters
{
    public class MoonflareBatItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Moonflare Bat");

            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(silver: 6);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = true;
            Item.makeNPC = ModContent.NPCType<MoonflareBat>();
        }
    }
}