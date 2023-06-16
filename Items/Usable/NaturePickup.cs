using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class NaturePickup : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.IsAPickup[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.White;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture).Value;
        }
        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab, player.position);
            for (int i = 0; i < 14; i++)
            {
                int dust = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height, DustID.GreenTorch, 0, 0, 20, Scale: 2);
                Main.dust[dust].noGravity = true;
            }
            player.AddBuff(BuffID.DryadsWard, player.RedemptionPlayerBuff().forestCore ? 420 : 300);
            return false;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.LightGreen.ToVector3() * 0.6f * Main.essScale);
            if (Item.timeSinceItemSpawned >= 600)
            {
                for (int i = 0; i < 14; i++)
                {
                    int dust = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height, DustID.GreenTorch, 0, 0, 20, Scale: 2);
                    Main.dust[dust].noGravity = true;
                }
                Item.active = false;
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
            }
            if (!Main.rand.NextBool(20))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.GreenTorch, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
    }
}
