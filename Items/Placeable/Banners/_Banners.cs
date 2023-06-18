using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Banners;

namespace Redemption.Items.Placeable.Banners
{
    public abstract class BaseBannerItem : ModItem
    {
        protected abstract int Tile { get; }
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public sealed override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(Tile, 0);
            Item.width = 12;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 10, 0);
        }
    }
    public class AncientGladestoneGolemBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<AncientGladestoneGolemBannerTile>();
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.KillsToBanner[Type] = 25;
        }
    }
    public class AndroidBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<AndroidBannerTile>();
    }
    public class BlisteredScientistBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<BlisteredScientistBannerTile>();
    }
    public class BloatedGhoulBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<BloatedGhoulBannerTile>();
    }
    public class BloatedGoldfishBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<BloatedGoldfishBannerTile>();
    }
    public class BloatedScientistBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<BloatedScientistBannerTile>();
    }
    public class BloatedSwarmerBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<BloatedSwarmerBannerTile>();
    }
    public class BlobbleBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<BlobbleBannerTile>();
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.KillsToBanner[Type] = 25;
        }
    }
    public class BobTheBlobBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<BobTheBlobBannerTile>();
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.KillsToBanner[Type] = 10;
        }
    }
    public class BoneSpiderBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<BoneSpiderBannerTile>();
    }
    public class ChickenBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<ChickenBannerTile>();
    }
    public class ChickenBomberBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<ChickenBomberBannerTile>();
    }
    public class ChickenScratcherBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<ChickenScratcherBannerTile>();
    }
    public class CoastScarabBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<CoastScarabBannerTile>();
    }
    public class CorpseWalkerPriestBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<CorpseWalkerPriestBannerTile>();
    }
    public class DevilsTongueBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<DevilsTongueBannerTile>();
    }
    public class EpidotrianSkeletonBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<EpidotrianSkeletonBannerTile>();
    }
    public class ForestNymphBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<ForestNymphBannerTile>();
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.KillsToBanner[Type] = 10;
        }
    }
    public class GrandLarvaBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<GrandLarvaBannerTile>();
    }
    public class HaymakerBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<HaymakerBannerTile>();
    }
    public class HazmatZombieBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<HazmatZombieBannerTile>();
    }
    public class HeadlessChickenBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<HeadlessChickenBannerTile>();
    }
    public class JollyMadmanBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<JollyMadmanBannerTile>();
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.KillsToBanner[Type] = 10;
        }
    }
    public class KabucraBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<KabucraBannerTile>();
    }
    public class LivingBloomBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<LivingBloomBannerTile>();
    }
    public class MoonflareBatBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<MoonflareBatBannerTile>();
    }
    public class MutatedLivingBloomBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<MutatedLivingBloomBannerTile>();
    }
    public class NuclearShadowBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<NuclearShadowBannerTile>();
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.KillsToBanner[Type] = 25;
        }
    }
    public class NuclearSlimeBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<NuclearSlimeBannerTile>();
    }
    public class OozeBlobBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<OozeBlobBannerTile>();
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.KillsToBanner[Type] = 250;
        }
    }
    public class OozingScientistBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<OozingScientistBannerTile>();
    }
    public class PrototypeSilverBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<PrototypeSilverBannerTile>();
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.KillsToBanner[Type] = 25;
        }
    }
    public class RadioactiveJellyBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<RadioactiveJellyBannerTile>();
    }
    public class RadioactiveSlimeBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<RadioactiveSlimeBannerTile>();
    }
    public class RoosterBoosterBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<RoosterBoosterBannerTile>();
    }
    public class SandskinSpiderBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SandskinSpiderBannerTile>();
    }
    public class SickenedBunnyBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SickenedBunnyBannerTile>();
    }
    public class SickenedDemonEyeBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SickenedDemonEyeBannerTile>();
    }
    public class SicklyPenguinBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SicklyPenguinBannerTile>();
    }
    public class SicklyWolfBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SicklyWolfBannerTile>();
    }
    public class SkeletonAssassinBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SkeletonAssassinBannerTile>();
    }
    public class SkeletonDuelistBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SkeletonDuelistBannerTile>();
    }
    public class SkeletonFlagbearerBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SkeletonFlagbearerBannerTile>();
    }
    public class SkeletonNobleBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SkeletonNobleBannerTile>();
    }
    public class SkeletonWandererBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SkeletonWandererBannerTile>();
    }
    public class SkeletonWardenBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SkeletonWardenBannerTile>();
    }
    public class SneezyFlinxBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SneezyFlinxBannerTile>();
    }
    public class SpacePaladinBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<SpacePaladinBannerTile>();
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.KillsToBanner[Type] = 10;
        }
    }
    public class TreeBugBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<TreeBugBannerTile>();
    }
    public class VagrantSpiritBanner : BaseBannerItem
    {
        protected override int Tile => ModContent.TileType<VagrantSpiritBannerTile>();
    }
}