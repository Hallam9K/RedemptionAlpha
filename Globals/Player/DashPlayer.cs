using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.DamageClasses;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Redemption.Globals.RedeNet;

namespace Redemption
{
    public enum DashType : byte
    {
        None,
        Infected,
        Holoshield
    }
    public class DashPlayer : ModPlayer
    {
        public bool infectedThornshield = false;
        public bool plasmaShield = false;
        public int infectedShieldHit;
        public int holoshieldHit;
        public DashType ActiveDash { get; private set; }

        public override void ResetEffects()
        {
            plasmaShield = false;
            infectedThornshield = false;
        }
        public override void NaturalLifeRegen(ref float regen)
        {
            // Last hook before player.DashMovement
            DashType dash = FindDashes();
            if (dash != DashType.None)
            {
                // Prevent vanilla dashes
                Player.dash = 0;

                if (Player.pulley)
                {
                    DashMovement(dash);
                }
            }
        }
        private void DashEnd()
        {
        }
        private void DashMovement(DashType dash)
        {
            if (Player.dashDelay > 0)
            {
                if (ActiveDash != DashType.None)
                {
                    DashEnd();
                    ActiveDash = DashType.None;
                }
            }
            else if (Player.dashDelay < 0)
            {
                float speedCap = 20f;
                float decayCapped = 0.992f;
                float speedMax = Math.Max(Player.accRunSpeed, Player.maxRunSpeed);
                float decayMax = 0.96f;
                int delay = 20;
                switch (ActiveDash)
                {
                    case DashType.Infected:
                        if (infectedShieldHit < 0)
                        {
                            Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy);
                            Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy);
                            Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy);
                            Rectangle hitbox = new((int)(Player.position.X + Player.velocity.X * 0.5 - 4), (int)(Player.position.Y + Player.velocity.Y * 0.5 - 4), Player.width + 8, Player.height + 8);
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC npc = Main.npc[i];
                                if (npc.active && !npc.dontTakeDamage && !npc.friendly)
                                {
                                    if (hitbox.Intersects(npc.Hitbox) && (npc.noTileCollide || Collision.CanHit(Player.position, Player.width, Player.height, npc.position, npc.width, npc.height)))
                                    {
                                        float damage = 40 * Player.GetDamage<DruidClass>();
                                        float knockback = 10;
                                        bool crit = false;

                                        if (Player.kbGlove)
                                            knockback *= 2f;
                                        if (Player.kbBuff)
                                            knockback *= 1.5f;

                                        if (Main.rand.Next(100) < Player.GetCritChance<DruidClass>())
                                            crit = true;

                                        int hitDirection = Player.velocity.X < 0f ? -1 : 1;

                                        if (Player.whoAmI == Main.myPlayer)
                                        {
                                            npc.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 600);
                                            if (Main.rand.NextBool(5))
                                            {
                                                npc.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 300);
                                            }
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
                                        infectedShieldHit = i;
                                    }
                                }
                            }
                        }
                        break;
                    case DashType.Holoshield:
                        if (holoshieldHit < 0)
                        {
                            Rectangle hitbox = new((int)(Player.position.X + Player.velocity.X * 0.5 - 4), (int)(Player.position.Y + Player.velocity.Y * 0.5 - 4), Player.width + 8, Player.height + 8);
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC npc = Main.npc[i];
                                if (npc.active && !npc.dontTakeDamage && !npc.friendly)
                                {
                                    if (hitbox.Intersects(npc.Hitbox) && (npc.noTileCollide || Collision.CanHit(Player.position, Player.width, Player.height, npc.position, npc.width, npc.height)))
                                    {
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
                                        holoshieldHit = i;
                                    }
                                }
                            }
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile proj = Main.projectile[i];
                                if (proj.active && proj.hostile && !proj.friendly && proj.damage < 100 && proj.velocity.Length() > 0)
                                {
                                    if (hitbox.Intersects(proj.Hitbox) && (!proj.tileCollide || Collision.CanHit(Player.position, Player.width, Player.height, proj.position, proj.width, proj.height)))
                                    {
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
                                        holoshieldHit = i;
                                    }
                                }
                            }
                        }
                        break;
                }
                if (ActiveDash != DashType.None)
                {
                    if (speedCap < speedMax)
                        speedCap = speedMax;

                    Player.vortexStealthActive = false;
                    if (Player.velocity.X > speedCap || Player.velocity.X < -speedCap)
                    {
                        Player.velocity.X = Player.velocity.X * decayCapped;
                    }
                    else if (Player.velocity.X > speedMax || Player.velocity.X < -speedMax)
                    {
                        Player.velocity.X = Player.velocity.X * decayMax;
                    }
                    else
                    {
                        Player.dashDelay = delay;

                        if (Player.velocity.X < 0f)
                        {
                            Player.velocity.X = -speedMax;
                        }
                        else if (Player.velocity.X > 0f)
                        {
                            Player.velocity.X = speedMax;
                        }
                    }
                }
            }
            else if (dash != DashType.None && Player.whoAmI == Main.myPlayer)
            {
                sbyte dir = 0;
                bool dashInput = false;
                if (Player.dashTime > 0)
                    Player.dashTime--;
                else if (Player.dashTime < 0)
                    Player.dashTime++;
                if (Player.controlRight && Player.releaseRight)
                {
                    if (Player.dashTime > 0)
                    {
                        dir = 1;
                        dashInput = true;
                        Player.dashTime = 0;
                    }
                    else
                        Player.dashTime = 15;
                }
                else if (Player.controlLeft && Player.releaseLeft)
                {
                    if (Player.dashTime < 0)
                    {
                        dir = -1;
                        dashInput = true;
                        Player.dashTime = 0;
                    }
                    else
                        Player.dashTime = -15;
                }

                if (dashInput)
                    PerformDash(dash, dir);
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            DashMovement(FindDashes());
        }
        internal void PerformDash(DashType dash, sbyte dir, bool local = true)
        {
            float velocity = dir;
            switch (dash)
            {
                case DashType.Infected:
                    infectedShieldHit = -1;
                    Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy, 0f, 0f, 0, default, 1f);
                    Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy, 0f, 0f, 0, default, 1f);
                    velocity *= 20f;
                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].position.X += Main.rand.Next(-5, 6);
                        Main.dust[dust].position.Y += Main.rand.Next(-5, 6);
                        Main.dust[dust].velocity *= 0.2f;
                    }
                    break;
                case DashType.Holoshield:
                    holoshieldHit = -1;
                    velocity *= 14f;
                    break;
            }
            Player.velocity.X = velocity;

            Point feet = (Player.Center + new Vector2(dir * (Player.width >> 1) + 2, Player.gravDir * -Player.height * .5f + Player.gravDir * 2f)).ToTileCoordinates();
            Point legs = (Player.Center + new Vector2(dir * (Player.width >> 1) + 2, 0f)).ToTileCoordinates();
            if (WorldGen.SolidOrSlopedTile(feet.X, feet.Y) || WorldGen.SolidOrSlopedTile(legs.X, legs.Y))
                Player.velocity.X = Player.velocity.X / 2f;
            Player.dashDelay = -1;
            ActiveDash = dash;

            if (!local || Main.netMode == NetmodeID.SinglePlayer)
            {
                return;
            }

            ModPacket packet = Redemption.Instance.GetPacket(ModMessageType.Dash, 3);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)dash);
            packet.Write(dir);
            packet.Send();
        }

        public DashType FindDashes()
        {
            if (Player.mount.Active)
                return DashType.None;

            if (infectedThornshield)
                return DashType.Infected;

            if (plasmaShield)
                return DashType.Holoshield;

            return DashType.None;
        }
    }
}
