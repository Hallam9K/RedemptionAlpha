using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Globals;
using Redemption.Globals.Players;
using Redemption.Particles;
using System;
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
            float num = Item.scale;
            for (int i = 0; i < 14; i++)
            {
                RedeParticleManager.CreateSpiritParticle(RedeHelper.RandAreaInEntity(Item), RedeHelper.SpreadUp(1), 0.5f * num, Main.rand.Next(20, 30));
            }
            player.GetModPlayer<RitualistPlayer>().SpiritGauge += 5 * Item.scale;
            return false;
        }
        public override void PostUpdate()
        {
            float particleScale = MathF.Pow(Item.scale, 0.5f);

            Lighting.AddLight(Item.Center, Color.GhostWhite.ToVector3() * 0.6f * Main.essScale);
            if (Item.timeSinceItemSpawned >= 600)
            {
                for (int i = 0; i < 14; i++)
                {
                    RedeParticleManager.CreateSpiritParticle(RedeHelper.RandAreaInEntity(Item), RedeHelper.SpreadUp(1), particleScale, Main.rand.Next(20, 30));
                }
                Item.active = false;
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
            }
            RedeParticleManager.CreateSpiritParticle(RedeHelper.RandAreaInEntity(Item), RedeHelper.SpreadUp(1), particleScale, Main.rand.Next(20, 30));
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }
    }
}
