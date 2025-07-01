using Redemption.BaseExtension;
using Redemption.Globals.Player;
using Redemption.Items.Weapons.HM.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Globals.Items
{
    public class RedeAmmoItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.Grenade:
                    item.ammo = ItemID.Grenade;
                    item.notAmmo = true;
                    item.shoot = ProjectileID.Grenade;
                    break;
                case ItemID.BouncyGrenade:
                    item.ammo = ItemID.Grenade;
                    item.notAmmo = true;
                    item.shoot = ProjectileID.BouncyGrenade;
                    break;
                case ItemID.StickyGrenade:
                    item.ammo = ItemID.Grenade;
                    item.notAmmo = true;
                    item.shoot = ProjectileID.StickyGrenade;
                    break;
                case ItemID.PartyGirlGrenade:
                    item.ammo = ItemID.Grenade;
                    item.notAmmo = true;
                    item.shoot = ProjectileID.PartyGirlGrenade;
                    break;
                case ItemID.Beenade:
                    item.ammo = ItemID.Grenade;
                    item.notAmmo = true;
                    item.shoot = ProjectileID.Beenade;
                    break;

                case ItemID.ThrowingKnife:
                    item.ammo = ItemID.ThrowingKnife;
                    item.notAmmo = true;
                    item.shoot = ProjectileID.ThrowingKnife;
                    break;
                case ItemID.PoisonedKnife:
                    item.ammo = ItemID.ThrowingKnife;
                    item.notAmmo = true;
                    item.shoot = ProjectileID.PoisonedKnife;
                    break;
            }
        }
    }
}