using Redemption.BaseExtension;
using Redemption.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class GalaxyHeart : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Permanently increases maximum life by 50"
                + "\nCan only be used if the max amount of life fruit has been consumed");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            return !player.Redemption().galaxyHeart && player.statLifeMax >= 500;
        }

        public override bool? UseItem(Player player)
        {
            player.statLifeMax2 += 50;
            player.statLife += 50;
            if (Main.myPlayer == player.whoAmI)
                player.HealEffect(50, true);

            player.Redemption().galaxyHeart = true;
            SoundEngine.PlaySound(SoundID.Item43, player.position);
            return true;
        }
    }
}