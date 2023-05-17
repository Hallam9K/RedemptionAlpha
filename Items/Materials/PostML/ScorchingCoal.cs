using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class ScorchingCoal : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorching Coal");
            Tooltip.SetDefault("Fuel for the Ancient Smeltery");
        }

		public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ScorchedCoalTile>(), 0);
            Item.width = 22;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 0;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(30))
                return;

            ParticleManager.NewParticle(Item.Center, RedeHelper.SpreadUp(1), new EmberParticle(), Color.White, 1);
        }
        private float drawTimer;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D glow2 = ModContent.Request<Texture2D>(Texture + "_Glow2").Value;

            RedeDraw.DrawTreasureBagEffect(spriteBatch, glow2, ref drawTimer, position, new Rectangle(0, 0, texture.Width, texture.Height), RedeColor.HeatColour, 0, origin, scale);

            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), drawColor, 0, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, position, new Rectangle(0, 0, texture.Width, texture.Height), RedeColor.HeatColour, 0, origin, scale, 0, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D glow2 = ModContent.Request<Texture2D>(Texture + "_Glow2").Value;
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;

            RedeDraw.DrawTreasureBagEffect(spriteBatch, glow2, ref drawTimer, Item.Center - Main.screenPosition, frame, RedeColor.HeatColour, rotation, origin, scale);

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, frame, RedeColor.HeatColour, rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
