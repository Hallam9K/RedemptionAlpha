using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals.NPC;
using Redemption.Items.Armor.Vanity.TBot;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Redemption.BaseExtension;
using Redemption.NPCs.HM;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals.World;

namespace Redemption.Globals
{
    public static class RedeHelper
    {
        public static bool IsFullTBot(this Terraria.Player player)
        {
            return ((BasePlayer.HasChestplate(player, ModContent.ItemType<TBotVanityChestplate>(), true) && BasePlayer.HasLeggings(player, ModContent.ItemType<TBotVanityLegs>(), true)) || (BasePlayer.HasChestplate(player, ModContent.ItemType<AndroidArmour>(), true) && BasePlayer.HasLeggings(player, ModContent.ItemType<AndroidPants>(), true)) || (BasePlayer.HasChestplate(player, ModContent.ItemType<JanitorOutfit>(), true) && BasePlayer.HasLeggings(player, ModContent.ItemType<JanitorPants>(), true))) && (BasePlayer.HasHelmet(player, ModContent.ItemType<TBotEyes_Femi>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotEyes_Masc>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotVanityEyes>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotGoggles_Femi>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotGoggles_Masc>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotVanityGoggles>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<AdamHead>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<OperatorHead>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<VoltHead>(), true));
        }
        public static bool IsTBotHead(this Terraria.Player player)
        {
            return BasePlayer.HasHelmet(player, ModContent.ItemType<TBotEyes_Femi>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotEyes_Masc>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotVanityEyes>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotGoggles_Femi>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotGoggles_Masc>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<TBotVanityGoggles>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<AdamHead>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<OperatorHead>(), true) || BasePlayer.HasHelmet(player, ModContent.ItemType<VoltHead>(), true);
        }
        public static bool HasFireDebuff(Terraria.NPC npc)
        {
            return npc.HasBuff(BuffID.OnFire) || npc.HasBuff(BuffID.OnFire3) || npc.HasBuff(BuffID.ShadowFlame) || npc.HasBuff(BuffID.CursedInferno) || npc.HasBuff(BuffID.Frostburn) || npc.HasBuff(BuffID.Frostburn2) || npc.HasBuff<HolyFireDebuff>() || npc.HasBuff<DragonblazeDebuff>();
        }
        public static bool ProjBlockBlacklist(this Projectile proj, bool countHoming = false)
        {
            return proj.minion || proj.ownerHitCheck || proj.Redemption().TechnicallyMelee || proj.Redemption().ParryBlacklist || Main.projPet[proj.type] || proj.sentry || (countHoming && ProjectileID.Sets.CultistIsResistantTo[proj.type]);
        }
        public static bool? CanHitSpiritCheck(Terraria.Player player, Item item)
        {
            return player.RedemptionAbility().SpiritwalkerActive || item.HasElement(ElementID.Arcane) || item.HasElement(ElementID.Celestial) || item.HasElement(ElementID.Holy) || item.HasElement(ElementID.Psychic) || RedeConfigClient.Instance.ElementDisable ? null : false;
        }
        public static bool? CanHitSpiritCheck(Projectile proj)
        {
            Terraria.Player player = Main.player[proj.owner];
            return player.RedemptionAbility().SpiritwalkerActive || proj.Redemption().RitDagger || proj.HasElement(ElementID.Arcane) || proj.HasElement(ElementID.Celestial) || proj.HasElement(ElementID.Holy) || proj.HasElement(ElementID.Psychic) || RedeConfigClient.Instance.ElementDisable ? null : false;
        }
        public static float RandomRotation() => Main.rand.NextFloat() * MathHelper.TwoPi;
        public static Vector2 TurnRight(this Vector2 vec) => new(-vec.Y, vec.X);
        public static Vector2 TurnLeft(this Vector2 vec) => new(vec.Y, -vec.X);
        public static int RightOfDir(this Entity toRight, Entity toLeft)
        {
            if (toRight.Center.X < toLeft.Center.X)
                return -1;
            return 1;
        }
        public static int RightOfDir(this Vector2 toRight, Vector2 toLeft)
        {
            if (toRight.X < toLeft.X)
                return -1;
            return 1;
        }
        public static bool RightOf(this Entity toRight, Entity toLeft) => toLeft.Center.X < toRight.Center.X;
        public static bool RightOf(this Vector2 toRight, Vector2 toLeft) => toLeft.X < toRight.X;
        public static bool Below(this Entity toBelow, Entity toAbove) => toAbove.Center.Y < toBelow.Center.Y;

        public static Vector2 PolarVector(float radius, float theta) =>
            new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;

        public static object GetFieldValue(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
        {
            if (flags == null)
            {
                flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            }
            FieldInfo field = type.GetField(fieldName, flags.Value);
            return field.GetValue(obj);
        }

        public static T GetFieldValue<T>(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
        {
            if (flags == null)
            {
                flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            }
            FieldInfo field = type.GetField(fieldName, flags.Value);
            return (T)field.GetValue(obj);
        }

        public delegate bool SpecialCondition(Terraria.NPC possibleTarget);

        //used for homing projectile
        public static bool ClosestNPC(ref Terraria.NPC target, float maxDistance, Vector2 position,
            bool ignoreTiles = false, int overrideTarget = -1, SpecialCondition specialCondition = null)
        {
            //very advance users can use a delegate to insert special condition into the function like only targeting enemies not currently having local iFrames, but if a special condition isn't added then just return it true
            specialCondition ??= delegate { return true; };

            bool foundTarget = false;
            //If you want to priorities a certain target this is where it's processed, mostly used by minions that have a target priority
            if (overrideTarget != -1 && (Main.npc[overrideTarget].Center - position).Length() < maxDistance)
            {
                target = Main.npc[overrideTarget];
                return true;
            }

            //this is the meat of the targetting logic, it loops through every NPC to check if it is valid the miniomum distance and target selected are updated so that the closest valid NPC is selected
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                Terraria.NPC npc = Main.npc[i];
                float distance = (npc.Center - position).Length();
                if (!(distance < maxDistance) || !npc.active || (!npc.chaseable && npc.type != NPCID.CultistBoss) || npc.dontTakeDamage || npc.friendly ||
                    npc.lifeMax <= 5 || npc.Redemption().invisible || npc.immortal ||
                    !Collision.CanHit(position, 0, 0, npc.Center, 0, 0) && !ignoreTiles ||
                    !specialCondition(npc))
                    continue;

                target = npc;
                foundTarget = true;
                maxDistance = (target.Center - position).Length();
            }

            return foundTarget;
        }

        public static bool ClosestNPCToNPC(this Terraria.NPC npc, ref Terraria.NPC target, float maxDistance,
            Vector2 position, bool ignoreTiles = false)
        {
            bool foundTarget = false;
            //this is the meat of the targeting logic, it loops through every NPC to check if it is valid the minimum distance and target selected are updated so that the closest valid NPC is selected
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                Terraria.NPC candidate = Main.npc[i];
                float distance = (candidate.Center - position).Length();
                if (!(distance < maxDistance) || !npc.active || !candidate.active || candidate.type == npc.type ||
                    !Collision.CanHit(position, 0, 0, candidate.Center, 0, 0) && !ignoreTiles)
                    continue;

                target = candidate;
                foundTarget = true;
                maxDistance = (target.Center - position).Length();
            }

            return foundTarget;
        }
        public static bool ClosestProj(this Projectile projectile, ref Projectile target, float maxDistance, Vector2 position,
    bool ignoreTiles = false, int overrideTarget = -1, int type = -1)
        {
            bool foundTarget = false;
            if (overrideTarget != -1 && (Main.npc[overrideTarget].Center - position).Length() < maxDistance)
            {
                target = Main.projectile[overrideTarget];
                return true;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                float distance = (proj.Center - position).Length();
                if (!(distance < maxDistance) || !proj.active || proj.whoAmI == projectile.whoAmI || !Collision.CanHit(position, 0, 0, proj.Center, 0, 0) && !ignoreTiles)
                    continue;

                if (type != 1 && proj.type != type)
                    continue;

                target = proj;
                foundTarget = true;
                maxDistance = (target.Center - position).Length();
            }

            return foundTarget;
        }
        //used by minions to give each minion of the same type a unique identifier so they don't stack
        public static int MinionHordeIdentity(Projectile projectile)
        {
            int identity = 0;
            for (int p = 0; p < 1000; p++)
            {
                if (!Main.projectile[p].active || Main.projectile[p].type != projectile.type ||
                    Main.projectile[p].owner != projectile.owner)
                    continue;

                if (projectile.whoAmI == p)
                    break;

                identity++;
            }

            return identity;
        }

        //used for projectiles using ammo, the vanilla PickAmmo had a bunch of clutter we don't need
        public static bool UseAmmo(this Projectile projectile, int ammoID, ref int shoot, ref float speed,
            ref int damage, ref float knockBack, bool doNotConsume = false)
        {
            Terraria.Player player = Main.player[projectile.owner];
            Item item = new();
            bool hasFoundAmmo = false;

            for (int i = 54; i < 58; i++)
            {
                if (player.inventory[i].ammo != ammoID || player.inventory[i].stack <= 0)
                    continue;

                item = player.inventory[i];
                hasFoundAmmo = true;
                break;
            }

            if (!hasFoundAmmo)
            {
                for (int j = 0; j < 54; j++)
                {
                    if (player.inventory[j].ammo != ammoID || player.inventory[j].stack <= 0)
                        continue;

                    item = player.inventory[j];
                    hasFoundAmmo = true;
                    break;
                }
            }

            if (hasFoundAmmo)
            {
                shoot = item.shoot;

                if (player.magicQuiver && (ammoID == AmmoID.Arrow || ammoID == AmmoID.Stake))
                {
                    knockBack = (int)(knockBack * 1.1);
                    speed *= 1.1f;
                }

                speed += item.shootSpeed;

                if (item.CountsAsClass(DamageClass.Ranged))
                {
                    if (item.damage > 0)
                        damage += (int)(item.damage * player.GetDamage(DamageClass.Ranged).Additive);
                }
                else
                    damage += item.damage;

                if (ammoID == AmmoID.Arrow && player.archery)
                {
                    if (speed < 20f)
                    {
                        speed *= 1.2f;

                        if (speed > 20f)
                            speed = 20f;
                    }

                    damage = (int)(damage * 1.2);
                }

                knockBack += item.knockBack;
                bool flag2 = doNotConsume
                             || player.magicQuiver && ammoID == AmmoID.Arrow && Main.rand.NextBool(5) || player.ammoBox && Main.rand.NextBool(5) || player.ammoPotion && Main.rand.NextBool(5) || player.ammoCost80 && Main.rand.NextBool(5) || player.ammoCost75 && Main.rand.NextBool(4);

                if (flag2 || !item.consumable)
                    return true;

                item.stack--;

                if (item.stack > 0)
                    return true;

                item.active = false;
                item.TurnToAir();
            }
            else
                return false;

            return true;
        }

        public static void SlowRotation(this ref float currentRotation, float targetAngle, float speed)
        {
            float actDirection = new Vector2((float)Math.Cos(currentRotation), (float)Math.Sin(currentRotation))
                .ToRotation();
            targetAngle = new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle)).ToRotation();

            int f;
            //this makes f 1 or -1 to rotate the shorter distance
            if (Math.Abs(actDirection - targetAngle) > Math.PI)
            {
                f = -1;
            }
            else
            {
                f = 1;
            }

            if (actDirection <= targetAngle + speed * 2 && actDirection >= targetAngle - speed * 2)
            {
                actDirection = targetAngle;
            }
            else if (actDirection <= targetAngle)
            {
                actDirection += speed * f;
            }
            else if (actDirection >= targetAngle)
            {
                actDirection -= speed * f;
            }

            actDirection = new Vector2((float)Math.Cos(actDirection), (float)Math.Sin(actDirection)).ToRotation();
            currentRotation = actDirection;
        }

        public static float AngularDifference(float angle1, float angle2)
        {
            angle1 = PolarVector(1f, angle1).ToRotation();
            angle2 = PolarVector(1f, angle2).ToRotation();
            if (Math.Abs(angle1 - angle2) > Math.PI)
            {
                return (float)Math.PI * 2 - Math.Abs(angle1 - angle2);
            }

            return Math.Abs(angle1 - angle2);
        }

        private static float X(float t,
            float x0, float x1, float x2, float x3)
        {
            return (float)(
                x0 * Math.Pow(1 - t, 3) +
                x1 * 3 * t * Math.Pow(1 - t, 2) +
                x2 * 3 * Math.Pow(t, 2) * (1 - t) +
                x3 * Math.Pow(t, 3)
            );
        }

        private static float Y(float t,
            float y0, float y1, float y2, float y3)
        {
            return (float)(
                y0 * Math.Pow(1 - t, 3) +
                y1 * 3 * t * Math.Pow(1 - t, 2) +
                y2 * 3 * Math.Pow(t, 2) * (1 - t) +
                y3 * Math.Pow(t, 3)
            );
        }

        public static void DrawBezier(SpriteBatch spriteBatch, Texture2D texture, string glowMaskTexture,
            Color drawColor, Vector2 startingPos, Vector2 endPoints, Vector2 c1, Vector2 c2, float chainsPerUse,
            float rotDis)
        {
            for (float i = 0; i <= 1; i += chainsPerUse)
            {
                if (i == 0)
                    continue;

                Vector2 distBetween = new(X(i, startingPos.X, c1.X, c2.X, endPoints.X) -
                                          X(i - chainsPerUse, startingPos.X, c1.X, c2.X, endPoints.X),
                    Y(i, startingPos.Y, c1.Y, c2.Y, endPoints.Y) -
                    Y(i - chainsPerUse, startingPos.Y, c1.Y, c2.Y, endPoints.Y));
                float projTrueRotation = distBetween.ToRotation() - (float)Math.PI / 2 + rotDis;
                spriteBatch.Draw(texture,
                    new Vector2(X(i, startingPos.X, c1.X, c2.X, endPoints.X) - Main.screenPosition.X,
                        Y(i, startingPos.Y, c1.Y, c2.Y, endPoints.Y) - Main.screenPosition.Y),
                    new Rectangle(0, 0, texture.Width, texture.Height), drawColor, projTrueRotation,
                    new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), 1, SpriteEffects.None, 0f);
            }

            //  spriteBatch.Draw(neckTex2D, new Vector2(head.Center.X - Main.screenPosition.X, head.Center.Y - Main.screenPosition.Y), head.frame, drawColor, head.rotation, new Vector2(36 * 0.5f, 32 * 0.5f), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(mod.GetTexture(glowMaskTexture), new Vector2(head.Center.X - Main.screenPosition.X, head.Center.Y - Main.screenPosition.Y), head.frame, Color.White, head.rotation, new Vector2(36 * 0.5f, 32 * 0.5f), 1f, SpriteEffects.None, 0f);
        }
        public static Vector2 GetArcVel(Vector2 startingPos, Vector2 targetPos, float gravity, float? minArcHeight = null, float? maxArcHeight = null, float? maxXvel = null, float? heightabovetarget = null, float downwardsYVelMult = 1f)
        {
            Vector2 DistanceToTravel = targetPos - startingPos;
            float MaxHeight = DistanceToTravel.Y - (heightabovetarget ?? 0);
            if (minArcHeight != null)
                MaxHeight = Math.Min(MaxHeight, -(float)minArcHeight);

            if (maxArcHeight != null)
                MaxHeight = Math.Max(MaxHeight, -(float)maxArcHeight);

            float TravelTime;
            float neededYvel;
            if (MaxHeight <= 0)
            {
                neededYvel = -(float)Math.Sqrt(-2 * gravity * MaxHeight);
                TravelTime = (float)Math.Sqrt(-2 * MaxHeight / gravity) + (float)Math.Sqrt(2 * Math.Max(DistanceToTravel.Y - MaxHeight, 0) / gravity); //time up, then time down
            }

            else
            {
                neededYvel = Vector2.Normalize(DistanceToTravel).Y * downwardsYVelMult;
                TravelTime = (-neededYvel + (float)Math.Sqrt(Math.Pow(neededYvel, 2) - (4 * -DistanceToTravel.Y * gravity / 2))) / (gravity); //time down
            }

            if (maxXvel != null)
                return new Vector2(MathHelper.Clamp(DistanceToTravel.X / TravelTime, -(float)maxXvel, (float)maxXvel), neededYvel);

            return new Vector2(DistanceToTravel.X / TravelTime, neededYvel);
        }

        public static Vector2 GetArcVel(this Entity ent, Vector2 targetPos, float gravity, float? minArcHeight = null, float? maxArcHeight = null, float? maxXvel = null, float? heightabovetarget = null, float downwardsYVelMult = 1f) => GetArcVel(ent.Center, targetPos, gravity, minArcHeight, maxArcHeight, maxXvel, heightabovetarget, downwardsYVelMult);

        public static bool BossActive(bool fromThisMod = false)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                Terraria.NPC npc = Main.npc[i];
                if (!npc.active || !npc.boss)
                    continue;

                if (fromThisMod)
                    return npc.ModNPC?.Mod is Redemption;
                return true;
            }
            return false;
        }
        public static bool HeldItemCrit(this Projectile projectile)
        {
            Terraria.Player player = Main.player[projectile.owner];
            float critChance = player.HeldItem.crit + player.GetTotalCritChance(projectile.DamageType);
            ItemLoader.ModifyWeaponCrit(player.HeldItem, player, ref critChance);
            PlayerLoader.ModifyWeaponCrit(player, player.HeldItem, ref critChance);
            if (critChance >= 100 || Main.rand.Next(1, 101) <= critChance)
                return true;
            return false;
        }
        public static Vector2 RandAreaInEntity(this Entity entity)
        {
            return entity.position + new Vector2(Main.rand.Next(0, entity.width), Main.rand.Next(0, entity.height));
        }

        public static float GradToRad(float grad) => grad * (float)Math.PI / 180.0f;

        public static Vector2 RandomPosition(Vector2 pos1, Vector2 pos2) =>
            new(Main.rand.Next((int)pos1.X, (int)pos2.X) + 1, Main.rand.Next((int)pos1.Y, (int)pos2.Y) + 1);

        public static int GetNearestAlivePlayer(this Terraria.NPC npc)
        {
            float nearestPlayerDist = 4815162342f;
            int nearestPlayer = -1;

            foreach (Terraria.Player player in Main.player)
            {
                if (!player.active || !(player.Distance(npc.Center) < nearestPlayerDist))
                    continue;

                nearestPlayerDist = player.Distance(npc.Center);
                nearestPlayer = player.whoAmI;
            }

            return nearestPlayer;
        }

        public static int GetNearestAlivePlayer(this Projectile projectile)
        {
            float nearestPlayerDist = 4815162342f;
            int nearestPlayer = -1;
            foreach (Terraria.Player player in Main.player)
            {
                if (!(player.Distance(projectile.Center) < nearestPlayerDist) || !player.active)
                    continue;

                nearestPlayerDist = player.Distance(projectile.Center);
                nearestPlayer = player.whoAmI;
            }

            return nearestPlayer;
        }

        public static Vector2 GetNearestAlivePlayerVector(this Terraria.NPC npc)
        {
            float nearestPlayerDist = 4815162342f;
            Vector2 nearestPlayer = Vector2.Zero;

            foreach (Terraria.Player player in Main.player)
            {
                if (!(player.Distance(npc.Center) < nearestPlayerDist) || !player.active)
                    continue;

                nearestPlayerDist = player.Distance(npc.Center);
                nearestPlayer = player.Center;
            }

            return nearestPlayer;
        }

        public static Vector2 VelocityFptp(Vector2 pos1, Vector2 pos2, float speed)
        {
            Vector2 move = pos2 - pos1;
            return move * (speed / (float)Math.Sqrt(move.X * move.X + move.Y * move.Y));
        }

        public static float RadToGrad(float rad) => rad * 180.0f / (float)Math.PI;

        public static int GetNearestNPC(Vector2 point, bool friendly = false, bool noBoss = false, bool canBeChasedBy = false)
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                Terraria.NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;

                if (noBoss && npc.boss)
                    continue;

                if (canBeChasedBy && !npc.CanBeChasedBy())
                    continue;

                if (!friendly && (npc.friendly || npc.lifeMax <= 5))
                    continue;

                if (nearestNPCDist != -1 && !(npc.Distance(point) < nearestNPCDist))
                    continue;

                nearestNPCDist = npc.Distance(point);
                nearestNPC = npc.whoAmI;
            }

            return nearestNPC;
        }

        public static int GetNearestPlayer(Vector2 point, bool alive = false)
        {
            float nearestPlayerDist = -1;
            int nearestPlayer = -1;

            foreach (Terraria.Player player in Main.player)
            {
                if (alive && (!player.active || player.dead))
                    continue;

                if (nearestPlayerDist != -1 && !(player.Distance(point) < nearestPlayerDist))
                    continue;

                nearestPlayerDist = player.Distance(point);
                nearestPlayer = player.whoAmI;
            }

            return nearestPlayer;
        }

        public static Vector2 VelocityToPoint(Vector2 a, Vector2 b, float speed)
        {
            Vector2 move = b - a;
            return move * (speed / (float)Math.Sqrt(move.X * move.X + move.Y * move.Y));
        }

        public static Vector2 RandomPointInArea(Vector2 a, Vector2 b) =>
            new(Main.rand.Next((int)a.X, (int)b.X) + 1, Main.rand.Next((int)a.Y, (int)b.Y) + 1);

        public static Vector2 RandomPointInArea(Rectangle area) =>
            new(Main.rand.Next(area.X, area.X + area.Width), Main.rand.Next(area.Y, area.Y + area.Height));

        public static float RotateBetween2Points(Vector2 a, Vector2 b) => (float)Math.Atan2(a.Y - b.Y, a.X - b.X);

        public static Vector2 CenterPoint(Vector2 a, Vector2 b) => new((a.X + b.X) / 2.0f, (a.Y + b.Y) / 2.0f);

        public static Vector2 PolarPos(Vector2 point, float distance, float angle, int xOffset = 0, int yOffset = 0)
        {
            Vector2 returnedValue = new()
            {
                X = distance * (float)Math.Sin(RadToGrad(angle)) + point.X + xOffset,
                Y = distance * (float)Math.Cos(RadToGrad(angle)) + point.Y + yOffset
            };

            return returnedValue;
        }

        public static bool Chance(float chance) => Main.rand.NextFloat() <= chance;
        public static bool GenChance(float chance) => WorldGen.genRand.NextFloat() <= chance;

        public static Vector2 SmoothFromTo(Vector2 from, Vector2 to, float smooth = 60f) => from + (to - from) / smooth;

        public static float DistortFloat(float @float, float percent)
        {
            float distortNumber = @float * percent;
            int counter = 0;

            while (distortNumber.ToString(CultureInfo.InvariantCulture).Split(',').Length > 1)
            {
                distortNumber *= 10;
                counter++;
            }

            return @float + Main.rand.Next(0, (int)distortNumber + 1) / (float)Math.Pow(10, counter) *
                (Main.rand.NextBool(2) ? -1 : 1);
        }

        public static Vector2 FoundPosition(Vector2 tilePos)
        {
            Vector2 screen = new(Main.screenWidth / 2f, Main.screenHeight / 2f);
            Vector2 fullScreen = tilePos - Main.mapFullscreenPos;
            fullScreen *= Main.mapFullscreenScale / 16;
            fullScreen = fullScreen * 16 + screen;
            Vector2 draw = new((int)fullScreen.X, (int)fullScreen.Y);
            return draw;
        }

        public static void MoveTowards(this Terraria.NPC npc, Vector2 playerTarget, float speed, float turnResistance)
        {
            Vector2 move = playerTarget - npc.Center;
            float length = move.Length();

            if (length > speed)
                move *= speed / length;

            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            length = move.Length();

            if (length > speed)
                move *= speed / length;

            npc.velocity = move;
        }

        public static bool NextBool(this UnifiedRandom rand, int chance, int total) => rand.Next(total) < chance;

        public static Vector2 Spread(float xy) =>
            new(Main.rand.NextFloat(-xy, xy), Main.rand.NextFloat(-xy, xy));

        public static Vector2 SpreadUp(float xy) => new(Main.rand.NextFloat(-xy, xy), Main.rand.NextFloat(-xy, 0));

        public static void CreateDust(Terraria.Player player, int dust, int count)
        {
            for (int i = 0; i < count; i++)
                Dust.NewDust(player.position, player.width, player.height / 2, dust);
        }

        public static Vector2 RotateVector(Vector2 origin, Vector2 vecToRot, float rot) =>
            new((float)(Math.Cos(rot) * (vecToRot.X - (double)origin.X) -
                         Math.Sin(rot) * (vecToRot.Y - (double)origin.Y)) +
                origin.X, (float)(Math.Sin(rot) * (vecToRot.X - (double)origin.X) +
                                   Math.Cos(rot) * (vecToRot.Y - (double)origin.Y)) + origin.Y);

        public static bool Contains(this Rectangle rect, Vector2 pos) => rect.Contains((int)pos.X, (int)pos.Y);

        public static bool AnyProjectiles(int type)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
                if (Main.projectile[i].active && Main.projectile[i].type == type)
                    return true;

            return false;
        }

        public static int CountProjectiles(int type)
        {
            int p = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
                if (Main.projectile[i].active && Main.projectile[i].type == type)
                    p++;

            return p;
        }

        public static Vector2 GetOrigin(Texture2D tex, int frames = 1)
        {
            return new(tex.Width / 2f, tex.Height / frames / 2);
        }

        public static Vector2 GetOrigin(Rectangle rect, int frames = 1)
        {
            return new(rect.Width / 2f, rect.Height / frames / 2f);
        }

        public static void ProjectileExplosion(IEntitySource source, Vector2 pos, float startAngle, int streams,
            int type, int damage, float projSpeed, float ai0 = 0, float ai1 = 0)
        {
            float currentAngle = startAngle;
            SoundEngine.PlaySound(SoundID.Item27, pos);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            for (int i = 0; i < streams; ++i)
            {
                Vector2 direction = Vector2.Normalize(new Vector2(1, 1))
                    .RotatedBy(MathHelper.ToRadians(360 / streams * i + currentAngle));
                direction.X *= projSpeed;
                direction.Y *= projSpeed;
                int proj = Projectile.NewProjectile(source, pos.X, pos.Y, direction.X, direction.Y, type, damage,
                    0f, Main.myPlayer, ai0, ai1);
                Main.projectile[proj].Center = pos;
            }
        }

        public static bool ZephosActive()
        {
            return Terraria.NPC.AnyNPCs(ModContent.NPCType<Zephos>()) || Terraria.NPC.AnyNPCs(ModContent.NPCType<ZephosUnconscious>());
        }
        public static bool DaerelActive()
        {
            return Terraria.NPC.AnyNPCs(ModContent.NPCType<Daerel>()) || Terraria.NPC.AnyNPCs(ModContent.NPCType<DaerelUnconscious>());
        }
        public static bool TBotActive()
        {
            return Terraria.NPC.AnyNPCs(ModContent.NPCType<TBot>()) || Terraria.NPC.AnyNPCs(ModContent.NPCType<TBotUnconscious>());
        }
        public static bool WayfarerActive()
        {
            return ZephosActive() || DaerelActive();
        }

        public static bool CanTarget(Projectile proj, Entity codable, Vector2 startPos, int maxDistToAttack = 800)
        {
            if (codable is Terraria.NPC npc)
            {
                return npc.active && npc.life > 0 && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5 && Vector2.Distance(startPos, npc.Center) < maxDistToAttack && Math.Abs(npc.Center.Y - startPos.Y) < (16f * (5 - 1)) && (BaseUtility.CanHit(proj.Hitbox, npc.Hitbox) || BaseUtility.CanHit(Main.player[proj.owner].Hitbox, npc.Hitbox));
            }
            return false;
        }

        public static bool PlayerDead(this Terraria.NPC npc)
        {
            RedeNPC globalNPC = npc.Redemption();
            if (globalNPC.attacker is Terraria.Player && ((globalNPC.attacker as Terraria.Player).dead || !(globalNPC.attacker as Terraria.Player).active))
                return true;

            return false;
        }
        /// <summary>
        /// For spawning NPCs from NPCs without any extra stuff.
        /// </summary>
        public static void SpawnNPC(IEntitySource source, int posX, int posY, int npcType, float ai0 = 0, float ai1 = 0, float ai2 = 0,
            float ai3 = 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int index = Terraria.NPC.NewNPC(source, posX, posY, npcType, 0, ai0, ai1, ai2, ai3);
                Main.npc[index].netUpdate = true;
            }
        }
        public static void NPCRadiusDamage(int radius, Entity hitter, int damage, float knockBack, int immuneTime = 20)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                Terraria.NPC target = Main.npc[i];
                if (!target.active || (!target.CanBeChasedBy() && target.type != NPCID.TargetDummy))
                    continue;

                if ((hitter is Projectile proj && target.immune[proj.whoAmI] > 0) || hitter.Distance(target.Center) > radius)
                    continue;

                if (hitter is Projectile proj2)
                    target.immune[proj2.whoAmI] = immuneTime;
                int hitDirection = target.RightOfDir(hitter);
                BaseAI.DamageNPC(target, damage, knockBack, hitDirection, hitter, crit: hitter is Projectile proj3 && proj3.HeldItemCrit());
            }
        }
        public static void NPCRadiusDamage(Rectangle radius, Entity hitter, int damage, float knockBack)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                Terraria.NPC target = Main.npc[i];
                if (!target.active || (!target.CanBeChasedBy() && target.type != NPCID.TargetDummy))
                    continue;

                if ((hitter is Projectile proj && target.immune[proj.whoAmI] > 0) || !target.Hitbox.Intersects(radius))
                    continue;

                if (hitter is Projectile proj2)
                    target.immune[proj2.whoAmI] = 20;
                int hitDirection = target.RightOfDir(hitter);
                BaseAI.DamageNPC(target, damage, knockBack, hitDirection, hitter, crit: hitter is Projectile proj3 && proj3.HeldItemCrit());
            }
        }
        public static void PlayerRadiusDamage(int radius, Entity hitter, int damage, float knockBack)
        {
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Terraria.Player target = Main.player[p];
                if (!target.active || target.dead)
                    continue;

                if (hitter.Distance(target.Center) > radius)
                    continue;

                int hitDirection = target.RightOfDir(hitter);
                BaseAI.DamagePlayer(target, damage, knockBack, hitDirection, hitter);
            }
        }
        public static void PlayerRadiusDamage(Rectangle radius, Entity hitter, int damage, float knockBack)
        {
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Terraria.Player target = Main.player[p];
                if (!target.active || target.dead)
                    continue;

                if (!target.Hitbox.Intersects(radius))
                    continue;

                int hitDirection = target.RightOfDir(hitter);
                BaseAI.DamagePlayer(target, damage, knockBack, hitDirection, hitter);
            }
        }
    }
    public static class NPCHelper
    {
        #region NPC Methods
        public static int HostileProjDamage(int damage)
        {
            damage /= 2;
            if (Main.expertMode)
                damage /= Main.masterMode ? 3 : 2;
            return damage;
        }
        public static int HostileProjDamageInc(int damage)
        {
            if (Main.expertMode)
                damage *= Main.masterMode ? 3 : 2;
            return damage;
        }
        /// <summary>
        /// For methods that have 'this NPC npc', instead of doing RedeHelper.Shoot(), you can do NPC.Shoot() instead.
        /// </summary>
        public static void Shoot(this Terraria.NPC npc, Vector2 position, int projType, int damage, Vector2 velocity, SoundStyle sound, float ai0 = 0, float ai1 = 0, int knockback = 0)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(sound, npc.position);
            Shoot(npc, position, projType, damage, velocity, ai0, ai1, knockback);
        }
        public static void Shoot(this Terraria.NPC npc, Vector2 position, int projType, int damage, Vector2 velocity, float ai0 = 0, float ai1 = 0, int knockback = 0)
        {
            damage /= 2;
            if (Main.expertMode)
                damage /= Main.masterMode ? 3 : 2;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(npc.GetSource_FromAI(), position, velocity, projType, damage, knockback,
                    Main.myPlayer, ai0, ai1);
            }
        }
        public static void Shoot(this Projectile proj, Vector2 position, int projType, int damage, Vector2 velocity,
            bool playSound, SoundStyle sound, float ai0 = 0, float ai1 = 0)
        {
            if (playSound)
                SoundEngine.PlaySound(sound, proj.position);

            if (proj.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(proj.GetSource_FromThis(), position, velocity, projType, damage / 4, 0,
                    Main.myPlayer, ai0, ai1);
            }
        }

        /// <summary>
        /// A simple Dash method for npcs charging at the player, use npc.Dash(20, false); for example.
        /// </summary>
        public static void Dash(this Terraria.NPC npc, int speed, bool directional, SoundStyle sound,
            Vector2 target)
        {
            Terraria.Player player = Main.player[npc.target];
            SoundEngine.PlaySound(sound, npc.position);
            if (target == Vector2.Zero)
            {
                target = player.Center;
            }

            if (directional)
            {
                npc.velocity = npc.DirectionTo(target) * speed;
            }
            else
            {
                npc.velocity.X = target.X > npc.Center.X ? speed : -speed;
            }
        }

        /// <summary>
        /// Makes the npc flip to the direction of the player. npc.LookAtPlayer();
        /// </summary>
        public static void LookAtEntity(this Terraria.NPC npc, Entity target, bool opposite = false)
        {
            int dir = 1;
            if (opposite)
                dir = -1;
            if (target.RightOf(npc))
            {
                npc.spriteDirection = dir;
                npc.direction = dir;
            }
            else
            {
                npc.spriteDirection = -dir;
                npc.direction = -dir;
            }
        }

        public static void LookAtEntity(this Projectile projectile, Entity target, bool opposite = false)
        {
            int dir = 1;
            if (opposite)
                dir = -1;
            if (target.RightOf(projectile))
            {
                projectile.spriteDirection = dir;
                projectile.direction = dir;
            }
            else
            {
                projectile.spriteDirection = -dir;
                projectile.direction = -dir;
            }
        }

        /// <summary>
        /// Makes the npc flip to the direction of it's X velocity. npc.LookByVelocity();
        /// </summary>
        public static void LookByVelocity(this Terraria.NPC npc)
        {
            if (npc.velocity.X > 0)
            {
                npc.spriteDirection = 1;
                npc.direction = 1;
            }
            else if (npc.velocity.X < 0)
            {
                npc.spriteDirection = -1;
                npc.direction = -1;
            }
        }
        public static void LookByVelocity(this Projectile projectile)
        {
            if (projectile.velocity.X > 0)
            {
                projectile.spriteDirection = 1;
                projectile.direction = 1;
            }
            else if (projectile.velocity.X < 0)
            {
                projectile.spriteDirection = -1;
                projectile.direction = -1;
            }
        }

        /// <summary>
        /// Moves the npc to a position without turn resistance. npc.MoveToVector2(new Vector2(player.Center + 100, player.Center - 30), 10);
        /// </summary>
        public static void MoveToVector2(this Terraria.NPC npc, Vector2 p, float moveSpeed)
        {
            float velMultiplier = 1f;
            Vector2 dist = p - npc.Center;
            float length = dist == Vector2.Zero ? 0f : dist.Length();
            if (length < moveSpeed)
            {
                velMultiplier = MathHelper.Lerp(0f, 1f, length / moveSpeed);
            }

            if (length < 100f)
            {
                moveSpeed *= 0.5f;
            }

            if (length < 50f)
            {
                moveSpeed *= 0.5f;
            }

            npc.velocity = length == 0f ? Vector2.Zero : Vector2.Normalize(dist);
            npc.velocity *= moveSpeed;
            npc.velocity *= velMultiplier;
        }

        public static void MoveToVector2(this Projectile projectile, Vector2 p, float moveSpeed)
        {
            float velMultiplier = 1f;
            Vector2 dist = p - projectile.Center;
            float length = dist == Vector2.Zero ? 0f : dist.Length();
            if (length < moveSpeed)
            {
                velMultiplier = MathHelper.Lerp(0f, 1f, length / moveSpeed);
            }

            if (length < 100f)
            {
                moveSpeed *= 0.5f;
            }

            if (length < 50f)
            {
                moveSpeed *= 0.5f;
            }

            projectile.velocity = length == 0f ? Vector2.Zero : Vector2.Normalize(dist);
            projectile.velocity *= moveSpeed;
            projectile.velocity *= velMultiplier;
        }

        /// <summary>
        /// Moves the npc to a Vector2.
        /// The lower the turnResistance, the less time it takes to adjust direction.
        /// Example: npc.MoveToPlayer(new Vector2(100, 0), 10, 14);
        /// toPlayer makes the vector consider the player.Center for you.
        /// </summary>
        public static void Move(this Terraria.NPC npc, Vector2 vector, float speed, float turnResistance = 10f,
            bool toPlayer = false, bool reverse = false)
        {
            Terraria.Player player = Main.player[npc.target];
            Vector2 moveTo = toPlayer ? player.Center + vector : vector;
            Vector2 move = moveTo - npc.Center;
            float magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            move = ((reverse ? -npc.velocity : npc.velocity) * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            npc.velocity = reverse ? -move : move;
        }

        public static void Move(this Projectile projectile, Vector2 vector, float speed, float turnResistance = 10f,
            bool toPlayer = false)
        {
            Terraria.Player player = Main.player[projectile.owner];
            Vector2 moveTo = toPlayer ? player.Center + vector : vector;
            Vector2 move = moveTo - projectile.Center;
            float magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            move = (projectile.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            projectile.velocity = move;
        }

        public static void MoveToNPC(this Terraria.NPC npc, Terraria.NPC target, Vector2 vector, float speed,
            float turnResistance = 10f)
        {
            Vector2 moveTo = target.Center + vector;
            Vector2 move = moveTo - npc.Center;
            float magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            npc.velocity = move;
        }

        public static float Magnitude(Vector2 mag) // For the Move code above
        {
            return (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);
        }

        /// <summary>
        /// Sight method for NPCs.
        /// </summary>
        /// <param name="range">Sets how close the target would need to be before the Sight is true.</param>
        /// <param name="lineOfSight">Sets if Sight can be blocked by the target standing behind tiles.</param>
        /// <param name="facingTarget">Sets if Sight requires the NPC to face the target's direction.</param>
        /// <param name="canSeeHiding">Sets if the enemy can't see invisible players or enemies.</param>
        /// <param name="blind">Sets if the enemy can't see the target if they don't move much.</param>
        /// <param name="moveThreshold">Sets how much velocity is needed before being detectable, use if 'blind' is true.</param>
        public static bool Sight(this Terraria.NPC npc, Entity codable, float range = -1, bool facingTarget = true,
            bool lineOfSight = false, bool canSeeHiding = false, bool blind = false, float moveThreshold = 2, int headOffset = 0)
        {
            if (codable == null || !codable.active || (codable is Terraria.Player codablePlayer && codablePlayer.dead))
                return false;

            if (!canSeeHiding)
            {
                if (codable is Terraria.NPC codableNPC && codableNPC.Redemption().invisible)
                    return false;
                if (codable is Terraria.Player codablePlayer2 && codablePlayer2.invis)
                    return false;
                if (codable is Projectile codableProj && codableProj.alpha >= 200)
                    return false;
            }
            if (blind && codable.velocity.Length() <= moveThreshold)
                return false;

            if (range >= 0)
            {
                if (npc.DistanceSQ(codable.Center) > range * range)
                    return false;
            }
            if (lineOfSight)
            {
                if (!Collision.CanHit(npc.position - new Vector2(0, headOffset), npc.width, npc.height, codable.position, codable.width, codable.height))
                    return false;
            }
            if (facingTarget)
            {
                if (npc.DistanceSQ(codable.Center) <= (codable.width + 32) * (codable.width + 32))
                    return true;

                return npc.RightOf(codable) && npc.spriteDirection == -1 ||
                       codable.RightOf(npc) && npc.spriteDirection == 1;
            }

            return true;
        }

        public static void Dodge(this Terraria.NPC npc, Projectile proj, float vel = 6, float jumpVel = 2, float maxJump = 8)
        {
            npc.velocity = RedeHelper.PolarVector(vel, npc.DirectionTo(proj.Center).ToRotation() + (proj.Center.X < npc.Center.X ? MathHelper.PiOver2 : -MathHelper.PiOver2));
            npc.velocity.Y -= jumpVel;
            npc.velocity.Y = MathHelper.Max(-maxJump, npc.velocity.Y);
            if (npc.position.Y > proj.Bottom.Y && (proj.velocity.X > 2 || proj.velocity.X < -2) && proj.velocity.Y <= 1)
            {
                npc.velocity.Y = 1;
                npc.velocity.X = -npc.velocity.X / 2;
            }
        }

        public static void PlatformFallCheck(this Terraria.NPC npc, ref bool canJump, int yOffset = 12, float playerY = 0)
        {
            Terraria.Player player = Main.player[npc.target];
            if (playerY == 0)
                playerY = player.Center.Y;
            if (npc.Center.Y + yOffset < playerY)
            {
                canJump = true;
                return;
            }
            canJump = false;
        }

        /// <summary>
        /// Checks for if the npc has any buffs on it.
        /// </summary>
        public static bool NPCHasAnyBuff(this Terraria.NPC npc)
        {
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (npc.HasBuff(i))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool NPCHasAnyDebuff(this Terraria.NPC npc)
        {
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (npc.HasBuff(i) && (Main.debuff[i] || npc.HasBuff(BuffID.BetsysCurse) || npc.HasBuff(BuffID.Daybreak) || npc.HasBuff(BuffID.Frostburn2) || npc.HasBuff(BuffID.OnFire3) || npc.HasBuff(BuffID.ShadowFlame)))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Makes this NPC horizontally move towards the Player (Take Fighter AI, as an example)
        /// </summary>
        public static void HorizontallyMove(Terraria.NPC npc, Vector2 vector, float moveInterval, float moveSpeed,
            int maxJumpTilesX, int maxJumpTilesY, bool jumpUpPlatforms, Entity target = null)
        {
            //if velocity is less than -1 or greater than 1...
            if (npc.velocity.X < -moveSpeed || npc.velocity.X > moveSpeed)
            {
                //...and npc is not falling or jumping, slow down x velocity.
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity *= 0.8f;
                }
            }
            else if (
                npc.velocity.X < moveSpeed &&
                vector.X > npc.Center.X) //handles movement to the right. Clamps at velMaxX.
            {
                if (npc.confused && !npc.boss)
                {
                    npc.velocity.X -= moveInterval;
                    if (npc.velocity.X < -moveSpeed)
                    {
                        npc.velocity.X = -moveSpeed;
                    }
                }
                else
                {
                    npc.velocity.X += moveInterval;
                    if (npc.velocity.X > moveSpeed)
                    {
                        npc.velocity.X = moveSpeed;
                    }
                }
            }
            else if (
                npc.velocity.X > -moveSpeed &&
                vector.X < npc.Center.X) //handles movement to the left. Clamps at -velMaxX.
            {
                if (npc.confused && !npc.boss)
                {
                    npc.velocity.X += moveInterval;
                    if (npc.velocity.X < moveSpeed)
                    {
                        npc.velocity.X = moveSpeed;
                    }
                }
                else
                {
                    npc.velocity.X -= moveInterval;
                    if (npc.velocity.X < -moveSpeed)
                    {
                        npc.velocity.X = -moveSpeed;
                    }
                }
            }
            BaseAI.WalkupHalfBricks(npc);

            //if there's a solid floor under us...
            if (BaseAI.HitTileOnSide(npc, 3))
            {
                //if the npc's velocity is going in the same direction as the npc's direction...
                if (npc.velocity.X < 0f && npc.direction == -1 || npc.velocity.X > 0f && npc.direction == 1)
                {
                    //...attempt to jump if needed.
                    Vector2 newVec = BaseAI.AttemptJump(npc.position, npc.velocity, npc.width, npc.height,
                        npc.direction, npc.directionY, maxJumpTilesX, maxJumpTilesY, moveSpeed, jumpUpPlatforms, target);
                    if (!npc.noTileCollide)
                    {
                        Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed,
                            ref npc.gfxOffY);
                        newVec = Collision.TileCollision(npc.position, newVec, npc.width, npc.height);
                        Vector4 slopeVec = Collision.SlopeCollision(npc.position, newVec, npc.width, npc.height);
                        Vector2 slopeVel = new(slopeVec.Z, slopeVec.W);
                        npc.position = new Vector2(slopeVec.X, slopeVec.Y);
                        npc.velocity = slopeVel;
                    }
                    if (npc.velocity != newVec)
                    {
                        npc.velocity = newVec;
                        npc.netUpdate = true;
                    }
                }
            }
        }

        public static Vector2 FindGround(this Terraria.NPC npc, int range, Func<int, int, bool> canTeleportTo = null)
        {
            int tileX = (int)npc.position.X / 16;
            int tileY = (int)npc.position.Y / 16;
            int teleportCheckCount = 0;
            while (teleportCheckCount < 100)
            {
                teleportCheckCount++;
                int tpTileX = Main.rand.Next(tileX - range, tileX + range);
                int tpTileY = Main.rand.Next(tileY - range, tileY + range);
                for (int tpY = tpTileY; tpY < tileY + range; tpY++)
                {
                    if ((tpY < tileY - 4 || tpY > tileY + 4 || tpTileX < tileX - 4 || tpTileX > tileX + 4) &&
                        (tpY < tileY - 1 || tpY > tileY + 1 || tpTileX < tileX - 1 || tpTileX > tileX + 1) &&
                        Main.tile[tpTileX, tpY].HasUnactuatedTile)
                    {
                        if (canTeleportTo != null && canTeleportTo(tpTileX, tpY) ||
                            Main.tile[tpTileX, tpY - 1].LiquidType != 2 &&
                            Main.tileSolid[Main.tile[tpTileX, tpY].TileType] &&
                            !Collision.SolidTiles(tpTileX - 1, tpTileX + 1, tpY - 4, tpY - 1))
                        {
                            if (WorldGen.InWorld(tpTileX, tpY))
                                return new Vector2(tpTileX, tpY);
                        }
                    }
                }
            }
            return new Vector2(npc.Center.X, npc.Center.Y) / 16;
        }

        public static Vector2 FindGroundPlayer(this Terraria.NPC npc, int distFromPlayer, Func<int, int, bool> canTeleportTo = null)
        {
            int playerTileX = (int)Main.player[npc.target].position.X / 16;
            int playerTileY = (int)Main.player[npc.target].position.Y / 16;
            int tileX = (int)npc.position.X / 16;
            int tileY = (int)npc.position.Y / 16;
            int teleportCheckCount = 0;

            while (teleportCheckCount < 1000)
            {
                teleportCheckCount++;
                int tpTileX = Main.rand.Next(playerTileX - distFromPlayer, playerTileX + distFromPlayer);
                int tpTileY = Main.rand.Next(playerTileY - distFromPlayer, playerTileY + distFromPlayer);
                for (int tpY = tpTileY; tpY < playerTileY + distFromPlayer; tpY++)
                {
                    if ((tpY < playerTileY - 4 || tpY > playerTileY + 4 || tpTileX < playerTileX - 4 || tpTileX > playerTileX + 4) &&
                        (tpY < tileY - 1 || tpY > tileY + 1 || tpTileX < tileX - 1 || tpTileX > tileX + 1) &&
                        Main.tile[tpTileX, tpY].HasUnactuatedTile)
                    {
                        if (canTeleportTo != null && canTeleportTo(tpTileX, tpY) ||
                            Main.tile[tpTileX, tpY - 1].LiquidType != 2 &&
                            Main.tileSolid[Main.tile[tpTileX, tpY].TileType] &&
                            !Collision.SolidTiles(tpTileX - 1, tpTileX + 1, tpY - 4, tpY - 1))
                        {
                            return new Vector2(tpTileX, tpY) * 16;
                        }
                    }
                }
            }
            return new Vector2(npc.Center.X, npc.Center.Y);
        }

        public static Vector2 FindGroundVector(this Terraria.NPC npc, Vector2 vector, int distFromVector, Func<int, int, bool> canTeleportTo = null)
        {
            int vectorX = (int)vector.X / 16;
            int vectorY = (int)vector.Y / 16;
            int tileX = (int)npc.position.X / 16;
            int tileY = (int)npc.position.Y / 16;
            int teleportCheckCount = 0;

            while (teleportCheckCount < 1000)
            {
                teleportCheckCount++;
                int tpTileX = Main.rand.Next(vectorX - distFromVector, vectorX + distFromVector);
                int tpTileY = Main.rand.Next(vectorY - distFromVector, vectorY + distFromVector);
                for (int tpY = tpTileY; tpY < vectorY + distFromVector; tpY++)
                {
                    if ((tpY < vectorY - 4 || tpY > vectorY + 4 || tpTileX < vectorX - 4 || tpTileX > vectorX + 4) &&
                        (tpY < tileY - 1 || tpY > tileY + 1 || tpTileX < tileX - 1 || tpTileX > tileX + 1) &&
                        Main.tile[tpTileX, tpY].HasUnactuatedTile)
                    {
                        if (canTeleportTo != null && canTeleportTo(tpTileX, tpY) ||
                            Main.tile[tpTileX, tpY - 1].LiquidType != 2 &&
                            Main.tileSolid[Main.tile[tpTileX, tpY].TileType] &&
                            !Collision.SolidTiles(tpTileX - 1, tpTileX + 1, tpY - 4, tpY - 1))
                        {
                            return new Vector2(tpTileX, tpY) * 16;
                        }
                    }
                }
            }
            return new Vector2(npc.Center.X, npc.Center.Y);
        }
        public static Vector2 FindGroundVector(Vector2 vector, int distFromVector, Func<int, int, bool> canTeleportTo = null)
        {
            int vectorX = (int)vector.X / 16;
            int vectorY = (int)vector.Y / 16;
            int teleportCheckCount = 0;

            while (teleportCheckCount < 1000)
            {
                teleportCheckCount++;
                int tpTileX = Main.rand.Next(vectorX - distFromVector, vectorX + distFromVector);
                int tpTileY = Main.rand.Next(vectorY - distFromVector, vectorY + distFromVector);
                for (int tpY = tpTileY; tpY < vectorY + distFromVector; tpY++)
                {
                    if ((tpY < vectorY - 4 || tpY > vectorY + 4 || tpTileX < vectorX - 4 || tpTileX > vectorX + 4) &&
                        Main.tile[tpTileX, tpY].HasUnactuatedTile)
                    {
                        if (canTeleportTo != null && canTeleportTo(tpTileX, tpY) ||
                            Main.tile[tpTileX, tpY - 1].LiquidType != 2 &&
                            Main.tileSolid[Main.tile[tpTileX, tpY].TileType] &&
                            !Collision.SolidTiles(tpTileX - 1, tpTileX + 1, tpY - 4, tpY - 1))
                        {
                            return new Vector2(tpTileX, tpY) * 16;
                        }
                    }
                }
            }
            return vector;
        }

        public static void DamageHostileAttackers(this Terraria.NPC npc, float dmgModify = 0, int knockback = 0, List<int> AlwaysDmgNPC = default)
        {
            if (AlwaysDmgNPC == default)
                AlwaysDmgNPC = new() { 0 };
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                Terraria.NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy() || target.whoAmI == npc.whoAmI || target != npc.Redemption().attacker)
                    continue;

                if (!AlwaysDmgNPC.Contains(target.type) && target.friendly)
                    continue;

                if (target.immune[npc.whoAmI] > 0 || !npc.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.immune[npc.whoAmI] = 30;
                int hitDirection = npc.RightOfDir(target);
                BaseAI.DamageNPC(target, npc.damage + (int)dmgModify, knockback, hitDirection, npc);
            }
        }
        public static void DamageAnyAttackers(this Terraria.NPC npc, float dmgModify = 0, int knockback = 0, List<int> DontDmgNPC = default)
        {
            if (DontDmgNPC == default)
                DontDmgNPC = new() { 0 };
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                Terraria.NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy() || target.whoAmI == npc.whoAmI || target != npc.Redemption().attacker)
                    continue;

                if (DontDmgNPC.Contains(target.type))
                    continue;

                if (target.immune[npc.whoAmI] > 0 || !npc.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.immune[npc.whoAmI] = 30;
                int hitDirection = npc.RightOfDir(target);
                BaseAI.DamageNPC(target, npc.damage + (int)dmgModify, knockback, hitDirection, npc);
            }
        }
        public static void LockMoveRadius(this Entity npc, Terraria.Player player, int radius = 1000)
        {
            bool lockedScreen = player.RedemptionScreen().lockScreen;
            if (!RedeConfigClient.Instance.CameraLockDisable && npc.Distance(player.Center) > radius)
            {
                if (!lockedScreen || npc.Distance(player.Center) < radius + 200)
                {
                    Vector2 movement = npc.Center - player.Center;
                    float difference = movement.Length() - radius;
                    movement.Normalize();
                    movement *= difference < 17f ? difference : 17f;
                    player.position += movement;
                }
            }
        }
        public static void LockMoveRadius(Vector2 npc, Terraria.Player player, int radius = 1000)
        {
            bool lockedScreen = player.RedemptionScreen().lockScreen;
            if (!RedeConfigClient.Instance.CameraLockDisable && npc.Distance(player.Center) > radius)
            {
                if (!lockedScreen || npc.Distance(player.Center) < radius + 200)
                {
                    Vector2 movement = npc - player.Center;
                    float difference = movement.Length() - radius;
                    movement.Normalize();
                    movement *= difference < 17f ? difference : 17f;
                    player.position += movement;
                }
            }
        }
        public static bool DespawnHandler(this Terraria.NPC npc, int type = 0, int vel = 2)
        {
            Terraria.Player player = Main.player[npc.target];
            switch (type)
            {
                default:
                    if (!player.active || player.dead)
                    {
                        npc.TargetClosest(false);
                        player = Main.player[npc.target];
                        if (!player.active || player.dead)
                        {
                            if (type is 1)
                            {
                                npc.alpha += vel;
                                if (npc.alpha >= 255)
                                    npc.active = false;
                            }
                            else if (type is 2)
                                npc.active = false;
                            else
                            {
                                npc.velocity *= 0.96f;
                                npc.velocity.Y -= vel;
                            }
                            if (npc.timeLeft > 10)
                                npc.timeLeft = 10;
                            return true;
                        }
                    }
                    break;
                case 3:
                    if (!player.active || player.dead || !FowlMorningWorld.FowlMorningActive)
                    {
                        npc.TargetClosest(false);
                        player = Main.player[npc.target];
                        if (!player.active || player.dead || !FowlMorningWorld.FowlMorningActive)
                        {
                            npc.alpha += 2;
                            if (npc.alpha >= 255)
                                npc.active = false;
                            if (npc.timeLeft > 10)
                                npc.timeLeft = 10;
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }
        public static bool TargetCheck(this Terraria.NPC npc, int ID = 0, bool bool1 = false, bool bool2 = false)
        {
            Terraria.Player player = Main.player[npc.target];
            RedeNPC globalNPC = npc.Redemption();
            if (globalNPC.attacker is Terraria.Player)
                return npc.Redemption().spiritSummon;

            Terraria.NPC target = Main.npc[globalNPC.attacker.whoAmI];
            if (!target.active || target.whoAmI == npc.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                return true;

            switch (ID)
            {
                case 1:
                    if (NPCLists.Plantlike.Contains(target.type))
                        return true;
                    break;
                case 2:
                    if (player.RedemptionPlayerBuff().skeletonFriendly && NPCLists.SkeletonHumanoid.Contains(target.type))
                        return true;
                    break;
                case 3:
                    if (target.type == npc.type || target.type == ModContent.NPCType<PrototypeSilver>() || target.type == ModContent.NPCType<SpacePaladin>())
                        return true;
                    break;
                case 4:
                    if (target.type == npc.type)
                        return true;
                    break;
            }
            return false;
        }
        public static bool ThreatenedCheck(this Terraria.NPC npc, ref int runCD, int runCDNum = 180, int ID = 0)
        {
            RedeNPC globalNPC = npc.Redemption();
            if (globalNPC.attacker == null || !globalNPC.attacker.active || npc.TargetCheck(ID) || npc.PlayerDead() || npc.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCD > runCDNum)
                return true;
            return false;
        }
        #endregion
    }
}