using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.Buffs.Debuffs;
using Terraria.GameContent.Creative;
using Redemption.Tiles.Ores;

namespace Redemption.Items.Materials.PreHM
{
    public class XenomiteShard : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Xenomite Shard");
            Tooltip.SetDefault("Holding this may infect you");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 5));

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
		{
			Item.width = 14;
            Item.height = 14;
			Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 25);
            Item.rare = ItemRarityID.Green;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<XenomiteShardTile>();
        }
    }
}
