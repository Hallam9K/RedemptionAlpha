using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Textures;
using Redemption.WorldGeneration;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.Globals
{
    public class RedeMapLayer : ModMapLayer
    {
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            if (RedeQuest.wayfarerVars[0] == 1 && RedeGen.newbCaveVector.X != -1)
            {
                const float scaleIfNotSelected = 1f;
                const float scaleIfSelected = scaleIfNotSelected * 1.2f;

                Vector2 pos = new(RedeGen.newbCaveVector.X + 35, RedeGen.newbCaveVector.Y + 6);
                context.Draw(CommonTextures.PortalIcon.Value, pos, Color.White, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center);
                context.Draw(CommonTextures.PortalIcon2.Value, pos, Color.LightGreen, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center);
            }
        }
        public class UGPortalMapLayer : ModMapLayer
        {
            public override void Draw(ref MapOverlayDrawContext context, ref string text)
            {
                if (RedeQuest.calaviaVar == 1 && RedeGen.gathicPortalVector.X != -1)
                {
                    const float scaleIfNotSelected = 1f;
                    const float scaleIfSelected = scaleIfNotSelected * 1.2f;

                    Vector2 pos = new(RedeGen.gathicPortalVector.X + 51, RedeGen.gathicPortalVector.Y + 17);
                    context.Draw(CommonTextures.PortalIcon.Value, pos, Color.White, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center);
                    context.Draw(CommonTextures.PortalIcon2.Value, pos, Color.LightBlue, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center);
                }
            }
        }
    }
}