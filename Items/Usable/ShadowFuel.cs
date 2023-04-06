using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class ShadowFuel : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.IsAPickup[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.White;
        }
        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab, player.position);
            for (int i = 0; i < 14; i++)
            {
                int dust = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height, DustID.PurpleCrystalShard, 0, 0, 20, Scale: 2);
                Main.dust[dust].noGravity = true;
            }
            player.AddBuff(ModContent.BuffType<ShadowFuelBuff>(), 300);
            return false;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Purple.ToVector3() * 0.6f * Main.essScale);
            if (Item.timeSinceItemSpawned >= 600)
            {
                for (int i = 0; i < 14; i++)
                {
                    int dust = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height, DustID.PurpleCrystalShard, 0, 0, 20, Scale: 2);
                    Main.dust[dust].noGravity = true;
                }
                Item.active = false;
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
            }
            if (!Main.rand.NextBool(20))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.PurpleCrystalShard, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
