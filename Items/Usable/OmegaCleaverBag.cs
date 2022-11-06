using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Donator.Gonk;

namespace Redemption.Items.Usable
{
    public class OmegaCleaverBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Box (Omega Cleaver)");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            ItemID.Sets.BossBag[Type] = true;
            SacrificeTotal = 3;
        }
		public override void SetDefaults()
		{
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Expert;
			Item.expert = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
		public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwordHeadband>(), 7));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<GonkPet>(), 10));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CorruptedXenomite>(), 1, 4, 8));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrokenBlade>()));
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

            if (Item.timeSinceItemSpawned % 12 == 0)
            {
                Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);

                Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
                float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
                Vector2 velocity = new(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

                Dust dust = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
                dust.scale = 0.5f;
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;
                dust.alpha = 0;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(223, 62, 55, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(150, 20, 54, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
