using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class OmegaPowerCell : ModItem
	{
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Item.ResearchUnlockCount = 25;
        }
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 40;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Red;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Red.ToVector3() * 0.6f * Main.essScale);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D textureGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(textureGlow, Item.Center - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }

    }
}
