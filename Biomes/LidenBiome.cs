using Terraria;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public class LidenBiome : ModBiome
    {
        public override string BestiaryIcon => "Redemption/Textures/Bestiary/Wasteland";
        public override string BackgroundPath => "Redemption/Textures/MapBackgrounds/LidenMapBG";
        public override string MapBackground => BackgroundPath;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Liden");
        }
        public override bool IsBiomeActive(Player player) => false;
    }
    public class LidenBiomeAlpha : ModBiome
    {
        public override string BestiaryIcon => "Redemption/Textures/Bestiary/AlphaIcon";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Alpha");
        }
        public override bool IsBiomeActive(Player player) => false;
    }
    public class LidenBiomeOmega : ModBiome
    {
        public override string BestiaryIcon => "Redemption/Textures/Bestiary/OmegaIcon";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Omega");
        }
        public override bool IsBiomeActive(Player player) => false;
    }
}