using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Buffs.Cooldowns;
using Redemption.Items.Accessories.PreHM;
using Redemption.Projectiles.Misc;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Accessories.HM
{
    public class SlayerController : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slayer's Gamer Controller");
            Tooltip.SetDefault("Changes your cursor to a cyan crosshair");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.width = 30;
            Item.height = 26;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Redemption().slayerCursor = true;
        }
        public override void UpdateVanity(Player player)
        {
            player.Redemption().slayerCursor = true;
        }
    }
}

