using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Shield)]
    public class Holoshield : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("6% damage reduction"
                + "\nDouble tap a direction to dash" +
                "\nDashing into projectiles will reflect them" +
                "\nCan't reflect projectiles exceeding 200 damage");
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.knockBack = 8;
            Item.DamageType = DamageClass.Melee;
            Item.width = 22;
            Item.height = 26;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
            Item.defense = 2;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<HoloshieldDashPlayer>().DashAccessoryEquipped = true;
			player.endurance += 0.06f;
        }
    }
	public class HoloshieldDashPlayer : ModPlayer
	{
		public const int DashRight = 0;
		public const int DashLeft = 1;

		public const int DashCooldown = 50;
		public const int DashDuration = 35;

		public const float DashVelocity = 9f;

		public int DashDir = -1;

		public bool DashAccessoryEquipped;
		public int DashDelay = 0;
		public int DashTimer = 0;
        public int ShieldHit;

        public override void ResetEffects()
		{
			DashAccessoryEquipped = false;

			if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
				DashDir = DashRight;
			else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
				DashDir = DashLeft;
			else
				DashDir = -1;
		}

		public override void PreUpdateMovement()
		{
			if (CanUseDash() && DashDir != -1 && DashDelay == 0)
			{
				Vector2 newVelocity = Player.velocity;

				switch (DashDir)
				{
					case DashLeft when Player.velocity.X > -DashVelocity:
					case DashRight when Player.velocity.X < DashVelocity:
						{
							float dashDirection = DashDir == DashRight ? 1 : -1;
							newVelocity.X = dashDirection * DashVelocity;
							break;
						}
					default:
						return;
				}

				DashDelay = DashCooldown;
				DashTimer = DashDuration;
				Player.velocity = newVelocity;

                ShieldHit = -1;
                // Here you'd be able to set an effect that happens when the dash first activates
                // Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
            }

			if (DashDelay > 0)
				DashDelay--;

			if (DashTimer > 0)
			{
				Player.eocDash = DashTimer;
				Player.armorEffectDrawShadowEOCShield = true;

                if (ShieldHit < 0)
                {
                    Rectangle hitbox = new((int)(Player.position.X + Player.velocity.X * 0.5 - 4),
                    (int)(Player.position.Y + Player.velocity.Y * 0.5 - 4), Player.width + 8, Player.height + 8);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.active || npc.dontTakeDamage || npc.friendly)
                            continue;

                        if (!hitbox.Intersects(npc.Hitbox) || !npc.noTileCollide && !Collision.CanHit(Player.position, Player.width, Player.height, npc.position, npc.width, npc.height))
                            continue;

                        float damage = 20 * Player.GetDamage(DamageClass.Melee);
                        float knockback = 8;
                        bool crit = false;

                        if (Player.kbGlove)
                            knockback *= 2f;
                        if (Player.kbBuff)
                            knockback *= 1.5f;

                        if (Main.rand.Next(100) < Player.GetCritChance(DamageClass.Melee))
                            crit = true;

                        int hitDirection = Player.velocity.X < 0f ? -1 : 1;

                        if (Player.whoAmI == Main.myPlayer)
                        {
                            npc.StrikeNPC((int)damage, knockback, hitDirection, crit);
                            if (Main.netMode != NetmodeID.SinglePlayer)
                                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, i, damage, knockback, hitDirection, 0,
                                    0, 0);
                        }

                        Player.dashDelay = 30;
                        Player.velocity.X = -hitDirection * 1f;
                        Player.velocity.Y = -4f;
                        Player.immune = true;
                        Player.immuneTime = 10;
                        ShieldHit = 1;
                    }
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (!proj.active || !proj.hostile || proj.friendly || proj.damage >= 200 || proj.velocity.Length() <= 0)
                            continue;

                        if (!hitbox.Intersects(proj.Hitbox) || proj.tileCollide && !Collision.CanHit(Player.position, Player.width, Player.height, proj.position, proj.width, proj.height))
                            continue;

                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Reflect").WithVolume(.5f).WithPitchVariance(.1f));
                        proj.damage *= 2;
                        proj.velocity = -proj.velocity;
                        proj.friendly = true;
                        proj.hostile = false;

                        int hitDirection = Player.velocity.X < 0f ? -1 : 1;
                        Player.dashDelay = 30;
                        Player.velocity.X = -hitDirection * 1f;
                        Player.velocity.Y = -4f;
                        Player.immune = true;
                        Player.immuneTime = 10;
                        ShieldHit = 1;
                    }
                }

                DashTimer--;
			}
		}

		private bool CanUseDash()
		{
			return DashAccessoryEquipped
				&& Player.dashType == 0
				&& !Player.setSolar
				&& !Player.mount.Active
                && !Player.GetModPlayer<ThornshieldDashPlayer>().DashAccessoryEquipped;
        }
    }
}
