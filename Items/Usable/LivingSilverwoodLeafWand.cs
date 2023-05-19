using Redemption.Items.Placeable.Tiles;
using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class LivingSilverwoodLeafWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Silverwood Leaf Wand");
            //Tooltip.SetDefault("Places silverwood leaves");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodLeafTile>(), 0);
            Item.width = 36;
            Item.height = 32;
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
