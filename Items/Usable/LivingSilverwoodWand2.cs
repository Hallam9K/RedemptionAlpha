using Redemption.Items.Placeable.Tiles;
using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class LivingSilverwoodWand2 : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Living Twisted Silverwood Wand");
            //Tooltip.SetDefault("Places twisted living silverwood");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LivingSilverwoodTwistedTile>(), 0);
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override bool CanUseItem(Player player)
        {
            int wood = player.FindItem(ModContent.ItemType<Silverwood>());
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (wood >= 0 && (!tile.HasTile || Main.tileCut[tile.TileType]))
            {
                player.inventory[wood].stack--;
                if (player.inventory[wood].stack <= 0)
                    player.inventory[wood] = new Item();
                return true;
            }
            else
                return false;
        }
    }
}
