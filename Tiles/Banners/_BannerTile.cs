using Microsoft.Xna.Framework;
using Redemption.NPCs.Critters;
using Redemption.NPCs.FowlMorning;
using Redemption.NPCs.HM;
using Redemption.NPCs.Kingdom;
using Redemption.NPCs.Lab;
using Redemption.NPCs.PreHM;
using Redemption.NPCs.Soulless;
using Redemption.NPCs.Wasteland;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Banners
{
    public abstract class BaseBannerTile : ModTile
    {
        protected abstract int NPC { get; }
        protected abstract Color MapColor { get; }
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Banners, 0));
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(MapColor, Language.GetText("MapObject.Banner"));
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            Tile tile = Main.tile[i, j];
            TileObjectData data = TileObjectData.GetTileData(tile);
            int topLeftX = i - tile.TileFrameX / 18 % data.Width;
            int topLeftY = j - tile.TileFrameY / 18 % data.Height;
            if (WorldGen.IsBelowANonHammeredPlatform(topLeftX, topLeftY))
            {
                offsetY -= 8;
            }
        }
        public override bool CreateDust(int i, int j, ref int type) => false;
        public virtual void SafeNearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.SceneMetrics.hasBanner = true;
                Main.SceneMetrics.NPCBannerBuff[NPC] = true;
            }
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            SafeNearbyEffects(i, j, closer);
        }
    }
    public class AncientGladestoneGolemBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<AncientGladestoneGolem>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class AndroidBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<Android>();
        protected override Color MapColor => Color.LightGray;
    }
    public class BlisteredScientistBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BlisteredScientist>();
        protected override Color MapColor => Color.DarkGreen;
    }
    public class BloatedClingerBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BloatedClinger>();
        protected override Color MapColor => Color.DarkGreen;
    }
    public class BloatedDiggerBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BloatedDiggerHead>();
        protected override Color MapColor => Color.DarkGreen;
    }
    public class BloatedFaceMonsterBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BloatedFaceMonster>();
        protected override Color MapColor => Color.DarkGreen;
    }
    public class BloatedGhoulBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BloatedGhoul>();
        protected override Color MapColor => Color.DarkGreen;
    }
    public class BloatedGoldfishBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BloatedGoldfish>();
        protected override Color MapColor => Color.DarkGreen;
    }
    public class BloatedScientistBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BloatedScientist>();
        protected override Color MapColor => Color.DarkGreen;
    }
    public class BloatedSwarmerBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BloatedSwarmer>();
        protected override Color MapColor => Color.DarkGreen;
    }
    public class BlobbleBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<Blobble>();
        protected override Color MapColor => Color.SeaGreen;
    }
    public class BobTheBlobBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BobTheBlob>();
        protected override Color MapColor => Color.Green;
    }
    public class BoneSpiderBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<BoneSpider>();
        protected override Color MapColor => Color.LightGray;
    }
    public class ChickenBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<Chicken>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class ChickenBomberBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<ChickenBomber>();
        protected override Color MapColor => Color.GhostWhite;
    }
    public class ChickenScratcherBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<ChickenScratcher>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class CoastScarabBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<CoastScarab>();
        protected override Color MapColor => Color.LightCyan;
    }
    public class CorpseWalkerPriestBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<CorpseWalkerPriest>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class DevilsTongueBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<DevilsTongue>();
        protected override Color MapColor => Color.LimeGreen;
    }
    public class EpidotrianSkeletonBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<EpidotrianSkeleton>();
        protected override Color MapColor => Color.SlateGray;
        public override void SafeNearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.SceneMetrics.hasBanner = true;
                Main.SceneMetrics.NPCBannerBuff[ModContent.NPCType<EpidotrianSkeleton>()] = true;
                Main.SceneMetrics.NPCBannerBuff[ModContent.NPCType<RaveyardSkeleton>()] = true;
            }
        }
    }
    public class ForestNymphBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<ForestNymph>();
        protected override Color MapColor => Color.ForestGreen;
    }
    public class GrandLarvaBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<GrandLarva>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class HaymakerBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<Haymaker>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class HazmatZombieBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<HazmatZombie>();
        protected override Color MapColor => Color.PaleGoldenrod;
    }
    public class HeadlessChickenBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<HeadlessChicken>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class JollyMadmanBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<JollyMadman>();
        protected override Color MapColor => Color.Brown;
    }
    public class KabucraBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<Kabucra>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class LivingBloomBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<LivingBloom>();
        protected override Color MapColor => Color.ForestGreen;
    }
    public class MoonflareBatBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<MoonflareBat>();
        protected override Color MapColor => Color.LightGoldenrodYellow;
    }
    public class MutatedLivingBloomBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<MutatedLivingBloom>();
        protected override Color MapColor => Color.Brown;
    }
    public class NuclearShadowBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<NuclearShadow>();
        protected override Color MapColor => Color.Black;
    }
    public class NuclearSlimeBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<NuclearSlime>();
        protected override Color MapColor => Color.Green;
    }
    public class OozeBlobBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<OozeBlob>();
        protected override Color MapColor => Color.Green;
    }
    public class OozingScientistBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<OozingScientist>();
        protected override Color MapColor => Color.Green;
    }
    public class PrototypeSilverBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<PrototypeSilver>();
        protected override Color MapColor => Color.LightGray;
    }
    public class RadioactiveJellyBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<RadioactiveJelly>();
        protected override Color MapColor => Color.DarkGreen;
    }
    public class RadioactiveSlimeBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<RadioactiveSlime>();
        protected override Color MapColor => Color.Green;
    }
    public class RoosterBoosterBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<RoosterBooster>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class SandskinSpiderBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SandskinSpider>();
        protected override Color MapColor => Color.Yellow;
    }
    public class SickenedBunnyBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SickenedBunny>();
        protected override Color MapColor => Color.GhostWhite;
    }
    public class SickenedDemonEyeBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SickenedDemonEye>();
        protected override Color MapColor => Color.SandyBrown;
    }
    public class SicklyPenguinBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SicklyPenguin>();
        protected override Color MapColor => Color.GhostWhite;
    }
    public class SicklyWolfBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SicklyWolf>();
        protected override Color MapColor => Color.LightGray;
    }
    public class SkeletonAssassinBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SkeletonAssassin>();
        protected override Color MapColor => Color.SlateGray;
    }
    public class SkeletonDuelistBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SkeletonDuelist>();
        protected override Color MapColor => Color.SlateGray;
    }
    public class SkeletonFlagbearerBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SkeletonFlagbearer>();
        protected override Color MapColor => Color.SlateGray;
    }
    public class SkeletonNobleBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SkeletonNoble>();
        protected override Color MapColor => Color.SlateGray;
    }
    public class SkeletonWandererBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SkeletonWanderer>();
        protected override Color MapColor => Color.SlateGray;
    }
    public class SkeletonWardenBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SkeletonWarden>();
        protected override Color MapColor => Color.SlateGray;
    }
    public class SneezyFlinxBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SneezyFlinx>();
        protected override Color MapColor => Color.LightGray;
    }
    public class SpacePaladinBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<SpacePaladin>();
        protected override Color MapColor => Color.LightGray;
    }
    public class TreeBugBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<TreeBug>();
        protected override Color MapColor => Color.ForestGreen;
    }
    public class VagrantSpiritBannerTile : BaseBannerTile
    {
        protected override int NPC => ModContent.NPCType<VagrantSpirit>();
        protected override Color MapColor => Color.LightGreen;
    }
}