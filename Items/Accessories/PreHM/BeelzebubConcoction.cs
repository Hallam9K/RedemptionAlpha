using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class BeelzebubConcoction : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 54;
            Item.value = Item.sellPrice(0, 1, 80, 0);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<DevilScentedDebuff>(), 30);
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.erleasFlower = true;
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[ModContent.BuffType<SpiderSwarmedDebuff>()] = true;
            modPlayer.spiderFriendly = true;
            modPlayer.beelzebub = true;

        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DevilishResin>()
                .AddIngredient<ErleasFlower>()
                .AddIngredient<SpiderSerum>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}