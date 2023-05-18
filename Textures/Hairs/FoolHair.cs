using Terraria.ModLoader;

namespace Redemption.Textures.Hairs
{
    public class FoolHair : ModHair
    {
        public override string AltTexture => Texture;
        public override Gender RandomizedCharacterCreationGender => Gender.Male;
        public override bool AvailableDuringCharacterCreation => true;
    }
}