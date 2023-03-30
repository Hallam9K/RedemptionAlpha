using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class JoesBanjo : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Joe's Banjo");
            /* Tooltip.SetDefault("Playable Instrument" +
                "\n'Kazooie not included'"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12;
            Item.autoReuse = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!Main.dedServ)
            {
                float cursorPosFromPlayer = player.Distance(Main.MouseWorld) / (Main.screenHeight / 2 / 24);
                if (cursorPosFromPlayer > 24) cursorPosFromPlayer = 1;
                else cursorPosFromPlayer = (cursorPosFromPlayer / 12) - 1;
                if (!Main.dedServ)
                {
                    SoundStyle b1 = CustomSounds.Banjo with { Pitch = cursorPosFromPlayer };
                    SoundEngine.PlaySound(b1, player.Center);
                }
            }
            return false;
        }
    }
}