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
        public static Asset<Texture2D> GunFlash;
        public static Asset<Texture2D> PixelCircle;
        public static Asset<Texture2D> EmberParticle;
        public static Asset<Texture2D> GlowParticle;
        public static Asset<Texture2D> Star;
        public static Asset<Texture2D> RainbowParticle2;
        public static Asset<Texture2D> RainbowParticle3;
        public static Asset<Texture2D> SoftGlow;
        public static Asset<Texture2D> Circle;
        public static Asset<Texture2D> WhiteOrb;
        public static Asset<Texture2D> StaticBall;
        public static Asset<Texture2D> Shockwave;
        public static Asset<Texture2D> Shockwave2;
        public static Asset<Texture2D> HolyGlow2;
        public static Asset<Texture2D> FadeTelegraph;
        public static Asset<Texture2D> FadeTelegraphCap;
        public static Asset<Texture2D> TransitionTex;
        public static Asset<Texture2D> Shine;

        public static Asset<Texture2D> StunVisual;
        public static Asset<Texture2D> RalliedBuffIcon;
        public static Asset<Texture2D> PortalIcon;
        public static Asset<Texture2D> PortalIcon2;
        public static Asset<Texture2D> HintIcon;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            TextBubble_Cave = Request<Texture2D>("Redemption/UI/TextBubble_Cave");
            TextBubble_Epidotra = Request<Texture2D>("Redemption/UI/TextBubble_Epidotra");
            TextBubble_Kingdom = Request<Texture2D>("Redemption/UI/TextBubble_Kingdom");
            TextBubble_Liden = Request<Texture2D>("Redemption/UI/TextBubble_Liden");
            TextBubble_Neb = Request<Texture2D>("Redemption/UI/TextBubble_Neb");
            TextBubble_Omega = Request<Texture2D>("Redemption/UI/TextBubble_Omega");
            TextBubble_Slayer = Request<Texture2D>("Redemption/UI/TextBubble_Slayer");

            BigFlare = Request<Texture2D>("Redemption/Textures/BigFlare");
            WhiteFlare = Request<Texture2D>("Redemption/Textures/WhiteFlare");
            WhiteEyeFlare = Request<Texture2D>("Redemption/Textures/WhiteEyeFlare");
            WhiteGlow = Request<Texture2D>("Redemption/Textures/WhiteGlow");
            GunFlash = Request<Texture2D>("Redemption/Textures/GunFlash");
            PixelCircle = Request<Texture2D>("Redemption/Particles/PixelCircle");
            EmberParticle = Request<Texture2D>("Redemption/Particles/EmberParticle");
            GlowParticle = Request<Texture2D>("Redemption/Particles/GlowParticle");
            Star = Request<Texture2D>("Redemption/Particles/Star");
            RainbowParticle2 = Request<Texture2D>("Redemption/Particles/RainbowParticle2");
            RainbowParticle3 = Request<Texture2D>("Redemption/Particles/RainbowParticle3");
            SoftGlow = Request<Texture2D>("Redemption/Textures/SoftGlow");
            Circle = Request<Texture2D>("Redemption/Textures/Circle");
            WhiteOrb = Request<Texture2D>("Redemption/Textures/WhiteOrb");
            StaticBall = Request<Texture2D>("Redemption/Textures/StaticBall");
            Shockwave = Request<Texture2D>("Redemption/Textures/Shockwave");
            Shockwave2 = Request<Texture2D>("Redemption/Textures/Shockwave2");
            HolyGlow2 = Request<Texture2D>("Redemption/Textures/HolyGlow2");
            FadeTelegraph = Request<Texture2D>("Redemption/Textures/FadeTelegraph");
            FadeTelegraphCap = Request<Texture2D>("Redemption/Textures/FadeTelegraphCap");
            TransitionTex = Request<Texture2D>("Redemption/Textures/TransitionTex");
            Shine = Request<Texture2D>("Redemption/Textures/Shine");

            StunVisual = Request<Texture2D>("Redemption/Textures/StunVisual");
            RalliedBuffIcon = Request<Texture2D>("Redemption/Buffs/NPCBuffs/FlagbearerBuff_Icon");
            PortalIcon = Request<Texture2D>("Redemption/UI/Map/PortalIcon");
            PortalIcon2 = Request<Texture2D>("Redemption/UI/Map/PortalIcon2");
            HintIcon = Request<Texture2D>("Redemption/Items/HintIcon");
        }
    }
}