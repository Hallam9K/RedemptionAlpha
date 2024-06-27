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

        public static Asset<Texture2D> BigFlare;
        public static Asset<Texture2D> WhiteFlare;
        public static Asset<Texture2D> WhiteGlow;
        public static Asset<Texture2D> GunFlash;
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

            BigFlare = ModContent.Request<Texture2D>("Redemption/Textures/BigFlare");
            WhiteFlare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare");
            WhiteGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow");
            GunFlash = ModContent.Request<Texture2D>("Redemption/Textures/GunFlash");
        }
    }
}