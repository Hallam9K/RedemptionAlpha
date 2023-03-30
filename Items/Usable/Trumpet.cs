using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class Trumpet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Trumpet");
            /* Tooltip.SetDefault("Playable Instrument" +
                "\n'Doot'"); */
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 14;
            Item.value = Item.sellPrice(0, 0, 75, 0);
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<Trumpet_Proj>();
            Item.shootSpeed = 1;
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
                    SoundStyle b1 = CustomSounds.Doot with { Pitch = cursorPosFromPlayer / 2 };
                    SoundEngine.PlaySound(b1, player.Center);
                }
            }
            return true;
        }
    }
}