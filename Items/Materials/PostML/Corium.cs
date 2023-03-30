using Redemption.Items.Accessories.HM;
using Redemption.Tiles.Ores;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Materials.PostML
{
    public class Corium : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Holding this will cause severe radiation poisoning without proper equipment");
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SolidCoriumTile>(), 0);
            Item.width = 24;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 40000;
            Item.rare = ItemRarityID.Red;
        }
        public override void HoldItem(Player player)
        {
            SoundStyle muller = CustomSounds.Muller5;
            if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                SoundEngine.PlaySound(muller, player.position);

            if (Main.rand.NextBool(50) && player.RedemptionRad().irradiatedLevel < 5)
                player.RedemptionRad().irradiatedLevel += 2;
        }
    }
}
