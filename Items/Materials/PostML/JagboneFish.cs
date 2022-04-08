using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class JagboneFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 38;
            Item.value = Item.sellPrice(0, 1, 5, 0);
            Item.maxStack = 999;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(player.GetItemSource_OpenItem(Type), ModContent.ItemType<VesselFragment>(), Main.rand.Next(3, 8));
        }
    }
}