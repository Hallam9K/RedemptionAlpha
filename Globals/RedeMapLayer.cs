using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

                // Here we retrieve the texture of the Skeletron boss head so that we can draw it. Remember that not all textures are loaded by default, so you might need to do something like `Main.instance.LoadItem(ItemID.BoneKey);` in your code to ensure the texture is loaded.
                var hintTexture = ModContent.Request<Texture2D>("Redemption/Items/HintIcon").Value;

                // The MapOverlayDrawContext.Draw method used here handles many of the small details for drawing an icon and should be used if possible. It'll handle scaling, alignment, culling, framing, and accounting for map zoom. Handling these manually is a lot of work.
                // Note that the `position` argument expects tile coordinates expressed as a Vector2. Don't scale tile coordinates to world coordinates by multiplying by 16.
                // The return of MapOverlayDrawContext.Draw has a field that indicates if the mouse is currently over our icon.
                Vector2 pos = new(RedeGen.newbCaveVector.X + 35, RedeGen.newbCaveVector.Y + 6);
                context.Draw(hintTexture, pos, Color.White, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center);
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
                    var hintTexture = ModContent.Request<Texture2D>("Redemption/Items/HintIcon").Value;

                    Vector2 pos = new(RedeGen.gathicPortalVector.X + 51, RedeGen.gathicPortalVector.Y + 17);
                    context.Draw(hintTexture, pos, Color.White, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center);
                }
            }
        }
    }
}