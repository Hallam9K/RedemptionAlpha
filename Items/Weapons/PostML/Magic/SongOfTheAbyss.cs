using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Projectiles.Magic;
using Redemption.Rarities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class SongOfTheAbyss : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Song of the Abyss");
            Tooltip.SetDefault("Plays a sorrowful tune\n" +
                "'Cry a requiem for sunlight'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ShadeTreble_Proj>();
            Item.shootSpeed = 19;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public int shot;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!Main.dedServ)
            {
                float cursorPosFromPlayer = player.Distance(Main.MouseWorld) / (Main.screenHeight / 2 / 24);
                if (cursorPosFromPlayer > 24) cursorPosFromPlayer = 1;
                else cursorPosFromPlayer = (cursorPosFromPlayer / 12) - 1;
                LegacySoundStyle b1 = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Lyre1");
                SoundEngine.PlaySound(b1.SoundId, (int)player.Center.X, (int)player.Center.Y, b1.Style, 1, cursorPosFromPlayer);
            }
            switch (shot)
            {
                case 0:
                    Projectile.NewProjectile(source, position, velocity + new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.ProjectileType<ShadeTreble_Proj>(), damage, knockback, Main.myPlayer);
                    shot = 1;
                    break;
                case 1:
                    Projectile.NewProjectile(source, position, velocity + new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.ProjectileType<ShadeTreble_Proj>(), damage, knockback, Main.myPlayer, 1);
                    shot = 0;
                    break;
            }
            return false;
        }
    }
}
