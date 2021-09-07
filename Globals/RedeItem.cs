using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Globals.Player;
using Redemption.Items.Usable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeItem : GlobalItem
    {
        public override void ModifyHitNPC(Item item, Terraria.Player player, Terraria.NPC target, ref int damage,
            ref float knockBack, ref bool crit)
        {
            if (item.axe > 0 && crit)
                damage += damage / 2;
        }
        public override bool OnPickup(Item item, Terraria.Player player)
        {
            if (item.type == ItemID.Heart && player.GetModPlayer<BuffPlayer>().heartInsignia)
                player.AddBuff(ModContent.BuffType<HeartInsigniaBuff>(), 180);

            return true;
        }
        public override void PostUpdate(Item item)
        {
            if (item.type == ItemID.Heart && Main.LocalPlayer.GetModPlayer<BuffPlayer>().heartInsignia)
            {
                if (!Main.rand.NextBool(6))
                    return;

                int sparkle = Dust.NewDust(new Vector2(item.position.X, item.position.Y), item.width, item.height,
                    DustID.ShadowbeamStaff, Scale: 2);
                Main.dust[sparkle].velocity.X = 0;
                Main.dust[sparkle].velocity.Y = -2;
                Main.dust[sparkle].noGravity = true;
            }
        }
        public override void OnCreate(Item item, ItemCreationContext context)
        {
            if (item.type == ModContent.ItemType<AlignmentTeller>())
                RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Greetings, I am the Chalice of Alignment, and I believe any action can be redeemed.", 260, 30, 0, Color.DarkGoldenrod);
        }
    }
}