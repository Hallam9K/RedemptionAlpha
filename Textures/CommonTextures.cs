using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Textures
{
    public class CommonTextures : ModSystem
    {
        public static Asset<Texture2D> TextBubble_Cave;
        public static Asset<Texture2D> TextBubble_Epidotra;
        public static Asset<Texture2D> TextBubble_Kingdom;
        public static Asset<Texture2D> TextBubble_Liden;
        public static Asset<Texture2D> TextBubble_Neb;
        public static Asset<Texture2D> TextBubble_Omega;
        public static Asset<Texture2D> TextBubble_Slayer;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            TextBubble_Cave = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Cave");
            TextBubble_Epidotra = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Epidotra");
            TextBubble_Kingdom = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Kingdom");
            TextBubble_Liden = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Liden");
            TextBubble_Neb = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Neb");
            TextBubble_Omega = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Omega");
            TextBubble_Slayer = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Slayer");
        }
        public override void Unload()
        {
            TextBubble_Cave = null;
            TextBubble_Epidotra = null;
            TextBubble_Kingdom = null;
            TextBubble_Liden = null;
            TextBubble_Neb = null;
            TextBubble_Omega = null;
            TextBubble_Slayer = null;
        }
    }
}