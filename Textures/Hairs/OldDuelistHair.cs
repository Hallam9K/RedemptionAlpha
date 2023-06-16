using Terraria.ModLoader;

namespace Redemption.Textures.Hairs
{
    public class OldDuelistHair : ModHair
    {
        public override string AltTexture => Texture;
        public override Gender RandomizedCharacterCreationGender => Gender.Unspecified;
        public override bool AvailableDuringCharacterCreation => true;
    }
}