using Redemption.DamageClasses;
using Redemption.Globals.Player;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Accessories.PreHM
{
    public class EldritchRoot : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Eldritch Root");
            Tooltip.SetDefault("Increased life regen after killing an enemy"
                + "\nDruidic damage increased by 15% when below 50% health" +
                "\n'Nature can reap, too.'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 22;
            Item.height = 26;
            Item.value = Item.sellPrice(0, 0, 65, 0);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().eldritchRoot = true;
            if (player.statLife <= (player.statLifeMax2 * 0.5f))
                player.GetDamage<DruidClass>() += 0.15f;
        }
	}
}
