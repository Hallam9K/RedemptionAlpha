using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class SorrowfulEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Having this in your inventory may attract the Keeper's first creation underground" +
                "\n[i:" + ModContent.ItemType<BadRedemptionRoute>() + "][c/ff5533: This item may have a redeemable negative impact onto the world]"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 11));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 34;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Blue;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Purple.ToVector3() * 0.6f * Main.essScale);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, scale, scale * 0.9f, scale);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(184, 161, 255), new Color(79, 15, 255), new Color(184, 161, 255));
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(0, 0, glow.Width, glow.Height), color * 0.8f, rotation, origin, scale * 0.6f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }
}
