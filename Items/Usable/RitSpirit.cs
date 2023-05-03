using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class RitSpirit : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.IsAPickup[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.White;
        }
        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab, player.position);
            for (int i = 0; i < 14; i++)
            {
                ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(Item), RedeHelper.Spread(2), new SpiritParticle(), Color.White, 0.5f * Item.scale, 0, 1);
            }
            player.GetModPlayer<RitualistPlayer>().SpiritGauge += 5 * Item.scale;
            return false;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.GhostWhite.ToVector3() * 0.6f * Main.essScale);
            if (Item.timeSinceItemSpawned >= 600)
            {
                for (int i = 0; i < 14; i++)
                {
                    ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(Item), RedeHelper.Spread(2), new SpiritParticle(), Color.White, 0.5f * Item.scale, 0, 1);
                }
                Item.active = false;
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
            }
            ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(Item), Vector2.Zero, new SpiritParticle(), Color.White, 0.5f * Item.scale, 0, 1);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }
    }
}
