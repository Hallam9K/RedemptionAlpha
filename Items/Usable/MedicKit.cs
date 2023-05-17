using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.Audio;

namespace Redemption.Items.Usable
{
    public class MedicKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Permanently increases maximum life by 50"
                + "\nCan only be used if the max amount of life fruit has been consumed"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ConsumedLifeCrystals == Player.LifeCrystalMax && player.ConsumedLifeFruit == Player.LifeFruitMax;
        }

        public override bool? UseItem(Player player)
        {
            player.Redemption().heartStyle = 1;
            if (player.Redemption().medKit)
                return null;

            player.UseHealthMaxIncreasingItem(50);

            player.Redemption().medKit = true;
            SoundEngine.PlaySound(SoundID.Item43, player.position);
            return true;
        }
    }
}