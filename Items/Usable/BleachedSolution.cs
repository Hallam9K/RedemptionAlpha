using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class BleachedSolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
            ItemID.Sets.SortingPriorityTerraforming[Type] = 101;
        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Solutions;
        }
        public override void SetDefaults()
        {
            Item.DefaultToSolution(ProjectileType<BleachedSolution_Proj>());
            Item.value = Item.buyPrice(0, 0, 25, 0);
            Item.rare = ItemRarityID.Orange;
        }
    }
}