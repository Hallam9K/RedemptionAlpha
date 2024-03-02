using Microsoft.Xna.Framework;
using Redemption.Tiles.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Trophies
{
    public abstract class BaseRelicItem : ModItem
    {
        protected abstract int Style { get; }
        protected abstract Point Size { get; }
        public virtual void SetSafeDefaults() { }
        public sealed override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RelicTile>(), Style);
            Item.width = Size.X;
            Item.height = Size.Y;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.value = Item.buyPrice(0, 5);
            SetSafeDefaults();
        }
    }
    public class AkkaRelic : BaseRelicItem
    {
        protected override int Style => 9;
        protected override Point Size => new(32, 48);
    }
    public class BasanRelic : BaseRelicItem
    {
        protected override int Style => 14;
        protected override Point Size => new(30, 50);
    }
    public class CleaverRelic : BaseRelicItem
    {
        protected override int Style => 5;
        protected override Point Size => new(32, 46);
    }
    public class CockatriceRelic : BaseRelicItem
    {
        protected override int Style => 13;
        protected override Point Size => new(30, 42);
        public override void SetSafeDefaults()
        {
            Item.value = Item.buyPrice(0, 1);
        }
    }
    public class EaglecrestGolemRelic : BaseRelicItem
    {
        protected override int Style => 19;
        protected override Point Size => new(30, 38);
    }
    public class ErhanRelic : BaseRelicItem
    {
        protected override int Style => 0;
        protected override Point Size => new(30, 52);
    }
    public class FowlEmperorRelic : BaseRelicItem
    {
        protected override int Style => 12;
        protected override Point Size => new(30, 40);
        public override void SetSafeDefaults()
        {
            Item.value = Item.buyPrice(0, 1);
        }
    }
    public class GigaporaRelic : BaseRelicItem
    {
        protected override int Style => 6;
        protected override Point Size => new(30, 44);
    }
    public class KeeperRelic : BaseRelicItem
    {
        protected override int Style => 4;
        protected override Point Size => new(30, 46);
    }
    public class KS3Relic : BaseRelicItem
    {
        protected override int Style => 1;
        protected override Point Size => new(30, 42);
    }
    public class NebRelic : BaseRelicItem
    {
        protected override int Style => 11;
        protected override Point Size => new(30, 42);
    }
    public class OORelic : BaseRelicItem
    {
        protected override int Style => 7;
        protected override Point Size => new(30, 40);
    }
    public class PZRelic : BaseRelicItem
    {
        protected override int Style => 8;
        protected override Point Size => new(34, 50);
    }
    public class SkullDiggerRelic : BaseRelicItem
    {
        protected override int Style => 18;
        protected override Point Size => new(30, 40);
    }
    public class SoIRelic : BaseRelicItem
    {
        protected override int Style => 2;
        protected override Point Size => new(34, 46);
    }
    public class ThornRelic : BaseRelicItem
    {
        protected override int Style => 3;
        protected override Point Size => new(30, 50);
    }
    public class UkkoRelic : BaseRelicItem
    {
        protected override int Style => 10;
        protected override Point Size => new(30, 48);
    }
}