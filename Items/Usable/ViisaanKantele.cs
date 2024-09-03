using Microsoft.Xna.Framework;
using Redemption.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class ViisaanKantele : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12;
            Item.autoReuse = true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!Main.dedServ)
            {
                float cursorPosFromPlayer = player.Distance(Main.MouseWorld) / (Main.screenHeight / 2 / 24);
                if (cursorPosFromPlayer > 24) cursorPosFromPlayer = 1;
                else cursorPosFromPlayer = (cursorPosFromPlayer / 12) - 1;
                if (!Main.dedServ)
                {
                    SoundStyle b1 = (player.altFunctionUse == 2 ? CustomSounds.Kantele2 : CustomSounds.Kantele1) with { Pitch = cursorPosFromPlayer };
                    SoundEngine.PlaySound(b1, player.Center);
                }
            }
            return false;
        }
    }
}