using Redemption.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Items.Usable
{
    public class MedicKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Permanently increases maximum life by 50"
                + "\nCan only be used if the max amount of life fruit has been consumed");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            return !player.Redemption().medKit && player.statLifeMax >= 500;
        }

        public override bool? UseItem(Player player)
        {
            player.statLifeMax2 += 50;
            player.statLife += 50;
            if (Main.myPlayer == player.whoAmI)
                player.HealEffect(50, true);

            player.Redemption().medKit = true;
            SoundEngine.PlaySound(SoundID.Item43, player.position);
            return true;
        }
    }
}