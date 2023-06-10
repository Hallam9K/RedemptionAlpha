using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Textures.Hairs
{
    public class ForestNymphHair : ModHair
    {
        public override Gender RandomizedCharacterCreationGender => Gender.Unspecified;
        public override bool AvailableDuringCharacterCreation => false;
        public override IEnumerable<Condition> GetUnlockConditions()
        {
            yield return RedeConditions.ForestNymphTrust;
        }
    }
}