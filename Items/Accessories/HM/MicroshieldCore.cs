using Redemption.Buffs.Cooldowns;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    public class MicroshieldCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 5));
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.expert = true;
            Item.rare = ItemRarityID.Expert;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (hideVisual)
                return;

            player.GetModPlayer<MicroshieldCore_Player>().microshieldDrone = true;
        }
    }
    public class MicroshieldCore_Player : ModPlayer
    {
        public bool microshieldDrone;

        public int restoreTimer;

        public float damageEndured;

        public bool shieldDisabled;
        public override void ResetEffects()
        {
            microshieldDrone = false;
        }
        public override void UpdateDead()
        {
            microshieldDrone = false;
        }
        public override void PostUpdate()
        {
            if (!microshieldDrone)
                return;
            if (Player.ownedProjectileCounts[ProjectileType<MicroshieldDrone>()] <= 0)
            {
                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ProjectileType<MicroshieldDrone>(), 0, 0, Player.whoAmI);
            }
            if (shieldDisabled)
                Player.AddBuff(BuffType<MicroshieldCoreCooldown>(), 2);
        }
    }
}
