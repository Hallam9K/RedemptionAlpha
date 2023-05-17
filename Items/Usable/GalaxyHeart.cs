using Redemption.BaseExtension;
using Redemption.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class GalaxyHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Permanently increases maximum life by 50"
                + "\nCan only be used if the max amount of life fruit has been consumed"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<CosmicRarity>();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ConsumedLifeCrystals == Player.LifeCrystalMax && player.ConsumedLifeFruit == Player.LifeFruitMax;
        }

        public override bool? UseItem(Player player)
        {
            player.Redemption().heartStyle = 2;
            if (player.Redemption().galaxyHeart)
                return null;

            player.UseHealthMaxIncreasingItem(50);

            player.Redemption().galaxyHeart = true;
            SoundEngine.PlaySound(SoundID.Item43, player.position);
            return true;
        }
    }
}