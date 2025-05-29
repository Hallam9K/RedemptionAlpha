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
        public static Asset<Texture2D> WhiteEyeFlare;
        public static Asset<Texture2D> WhiteGlow;
        public static Asset<Texture2D> WhiteOrb;
        public static Asset<Texture2D> GunFlash;
        public static Asset<Texture2D> GlowParticle;
        public static Asset<Texture2D> RainbowParticle1;
        public static Asset<Texture2D> RainbowParticle2;
        public static Asset<Texture2D> RainbowParticle3;
        public static Asset<Texture2D> Shockwave2;


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
            WhiteEyeFlare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteEyeFlare");
            WhiteGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow");
            WhiteOrb = ModContent.Request<Texture2D>("Redemption/Textures/WhiteOrb");
            GunFlash = ModContent.Request<Texture2D>("Redemption/Textures/GunFlash");
            GlowParticle = Request<Texture2D>("Redemption/Particles/GlowParticle");
            RainbowParticle1 = Request<Texture2D>("Redemption/Particles/RainbowParticle1");
            RainbowParticle2 = Request<Texture2D>("Redemption/Particles/RainbowParticle2");
            RainbowParticle3 = Request<Texture2D>("Redemption/Particles/RainbowParticle3");
            Shockwave2 = Request<Texture2D>("Redemption/Textures/Shockwave2");
        }
    }
}