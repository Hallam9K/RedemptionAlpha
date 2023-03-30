using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class GraveSteelShards : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 55;
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.OreDropsFromSlime[Type] = (3, 13);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 0, 2);
            Item.rare = ItemRarityID.Gray;
        }
    }
}
