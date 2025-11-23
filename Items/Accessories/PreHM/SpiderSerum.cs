using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals.Players;
using Redemption.Items.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class SpiderSerum : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 34;
            Item.value = Item.buyPrice(0, 0, 35, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[ModContent.BuffType<SpiderSwarmedDebuff>()] = true;
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.spiderFriendly = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SpiderSwarmerItem>(20)
                .AddIngredient(ItemID.Bottle)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}