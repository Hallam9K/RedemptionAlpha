using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Shade;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class SielukaivoShadowbinder : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Absorbs the souls of victims slain" +
                "\nVictims below 5000 life are too weak to be contained" +
                "\nUp to 100 Shadowbound Souls can be contained at once\n" +
                "'A gift, a curse, but not my own...'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.sellPrice(0, 15, 50, 0);
            Item.accessory = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SielukaivoShadowbinderTile>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().shadowBinder = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            string text = "Souls Captured: " + player.RedemptionPlayerBuff().shadowBinderCharge + "/100";
            TooltipLine line = new(Mod, "text", text) { overrideColor = Color.DarkGray };
            tooltips.Insert(2, line);
        }
    }
}