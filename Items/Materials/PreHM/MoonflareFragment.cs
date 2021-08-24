using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class MoonflareFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Shines in the moon's reflective light'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = 15;
            Item.rare = ItemRarityID.Blue;
        }
        public override void PostUpdate()
        {
            if (!Main.dayTime && Main.moonPhase != 4)
                Lighting.AddLight(Item.Center, Color.AntiqueWhite.ToVector3() * 0.55f * Main.essScale);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D textureFaded = ModContent.Request<Texture2D>("Redemption/Items/Materials/PreHM/MoonflareFragment_Faded").Value;
            Vector2 origin = new(textureFaded.Width / 2, textureFaded.Height / 2);

            if (Main.dayTime || Main.moonPhase == 4)
            {
                spriteBatch.Draw(textureFaded, Item.Center - Main.screenPosition, new Rectangle(0, 0, textureFaded.Width, textureFaded.Height), lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }
        /*Rectangle frame;
        if (Main.itemAnimations[Item.type] != null)
        {
            //In case this item is animated, this picks the correct frame
            frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
	        }
        else
        {
            frame = texture.Frame();
        }
        and then use frame instead of new Rectangle(0, 0, textureFaded.Width, textureFaded.Height)
        and after that, define and assign origin like that
        Vector2 origin = frame.Size() / 2f;*/
    }
}