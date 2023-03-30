using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class Violin : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Violin");
            // Tooltip.SetDefault("Playable Instrument");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<Violin_Proj>();
            Item.shootSpeed = 0;
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
                    SoundStyle b1 = CustomSounds.Violin with { Pitch = cursorPosFromPlayer };
                    SoundEngine.PlaySound(b1, player.Center);
                }
            }
            return true;
        }
    }
}
