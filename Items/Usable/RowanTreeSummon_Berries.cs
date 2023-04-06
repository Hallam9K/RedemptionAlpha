using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class RowanTreeSummon_Berries : ModItem
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
            Item.width = 24;
            Item.height = 30;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.White;
        }
        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item2 with { Pitch = .1f }, player.position);
            for (int i = 0; i < 14; i++)
            {
                int dust = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height, DustID.Firework_Red, Alpha: 20);
                Main.dust[dust].noGravity = true;
            }
            player.AddBuff(BuffID.WellFed3, 180);
            player.statLife += 5;
            player.HealEffect(5);
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100);
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.IndianRed.ToVector3() * 0.6f * Main.essScale);
            if (Item.timeSinceItemSpawned >= 600)
            {
                for (int i = 0; i < 14; i++)
                {
                    int dust = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height, DustID.Firework_Red, Alpha: 20);
                    Main.dust[dust].noGravity = true;
                }
                Item.active = false;
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
            }
        }
    }
}
