using Redemption.Tiles.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Trophies
{
    public abstract class BaseTrophyItem : ModItem
    {
        protected abstract int Tile { get; }
        public sealed override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(Tile, 0);
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.Blue;
        }
    }
    public class AkanKirvesTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<AkanKirvesTrophyTile>();
    }
    public class BasanTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<BasanTrophyTile>();
    }
    public class CockatriceTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<CockatriceTrophyTile>();
    }
    public class EaglecrestGolemTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<EaglecrestGolemTrophyTile>();
    }
    public class ErhanTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<ErhanTrophyTile>();
    }
    public class FowlEmperorTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<FowlEmperorTrophyTile>();
    }
    public class KeeperTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<KeeperTrophyTile>();
    }
    public class KS3Trophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<KS3TrophyTile>();
    }
    public class NebuleusTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<NebuleusTrophyTile>();
    }
    public class OmegaCleaverTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<OmegaCleaverTrophyTile>();
    }
    public class OmegaGigaporaTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<OmegaGigaporaTrophyTile>();
    }
    public class OmegaObliteratorTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<OmegaObliteratorTrophyTile>();
    }
    public class PZTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<PZTrophyTile>();
    }
    public class SkullDiggerTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<SkullDiggerTrophyTile>();
    }
    public class SoITrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<SoITrophyTile>();
    }
    public class ThornTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<ThornTrophyTile>();
    }
    public class UkonKirvesTrophy : BaseTrophyItem
    {
        protected override int Tile => ModContent.TileType<UkonKirvesTrophyTile>();
    }
}