using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Textures;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals.NPC
{
    public class RedeDrawNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        private int stunFrame;
        private int stunCounter;
        public override void FindFrame(Terraria.NPC npc, int frameHeight)
        {
            if (++stunCounter > 4)
            {
                stunCounter = 0;
                if (stunFrame++ >= 3)
                    stunFrame = 0;
            }
        }
        public override void PostDraw(Terraria.NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.RedemptionNPCBuff().rallied || npc.RedemptionNPCBuff().roosterBoost)
            {
                Asset<Texture2D> flagTex = CommonTextures.RalliedBuffIcon;
                Vector2 drawOrigin = flagTex.Size() / 2;

                spriteBatch.Draw(flagTex.Value, npc.position + new Vector2(npc.width / 2, -30) - screenPos, null, Color.White * npc.Opacity, 0, drawOrigin, 1, 0, 0);
            }
            if (npc.RedemptionNPCBuff().stunned)
            {
                Texture2D starTex = ModContent.Request<Texture2D>("Redemption/Textures/StunVisual").Value;
                int height = starTex.Height / 4;
                int y = height * stunFrame;
                Vector2 drawOrigin = new(starTex.Width / 2, height / 2);

                spriteBatch.Draw(starTex, npc.position + new Vector2(npc.width / 2, -10) - screenPos, new Rectangle?(new Rectangle(0, y, starTex.Width, height)), Color.White * npc.Opacity, 0, drawOrigin, 1, 0, 0);
            }
            if (npc.RedemptionNPCBuff().iceFrozen)
            {
                Texture2D tex = TextureAssets.Frozen.Value;
                float a = 0;
                float b = 0;
                if (npc.width >= 44)
                    a = (npc.width - 44) / 28f;
                if (npc.height >= 46)
                    b = (npc.height - 46) / 28f;
                float c = MathHelper.Max(a, b);
                if (c >= .2f)
                {
                    tex = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.CultistBossIceMist, AssetRequestMode.ImmediateLoad).Value;
                    a = 0;
                    b = 0;
                    if (npc.width >= 68)
                        a = (npc.width - 68) / 46f;
                    if (npc.height >= 78)
                        b = (npc.height - 78) / 51f;
                    c = MathHelper.Max(a, b);
                }
                Vector2 drawOrigin = new(tex.Width / 2, tex.Height / 2);
                Vector2 scale = new(npc.scale + c, npc.scale + c);
                spriteBatch.Draw(tex, npc.Center - screenPos, null, npc.GetAlpha(drawColor) * .5f, 0, drawOrigin, scale, 0, 0);
            }
        }
    }
}
