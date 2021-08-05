using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using SoundType = Terraria.ModLoader.SoundType;

namespace Redemption.Globals
{
    public static class RedeHelper
    {
        public static Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
        }
        public delegate bool SpecialCondition(Terraria.NPC possibleTarget);
        //used for homing projectile
        public static bool ClosestNPC(ref Terraria.NPC target, float maxDistance, Vector2 position, bool ignoreTiles = false, int overrideTarget = -1, SpecialCondition specialCondition = null)
        {
            //very advance users can use a delegate to insert special condition into the function like only targetting enemies not currently having local iFrames, but if a special condition isn't added then just return it true
            if (specialCondition == null)
            {
                specialCondition = delegate { return true; };
            }
            bool foundTarget = false;
            //If you want to prioritse a certain target this is where it's processed, mostly used by minions that haave a target priority
            if (overrideTarget != -1)
            {
                if ((Main.npc[overrideTarget].Center - position).Length() < maxDistance)
                {
                    target = Main.npc[overrideTarget];
                    return true;
                }
            }
            //this is the meat of the targetting logic, it loops through every NPC to check if it is valid the miniomum distance and target selected are updated so that the closest valid NPC is selected
            for (int k = 0; k < Main.npc.Length; k++)
            {
                Terraria.NPC possibleTarget = Main.npc[k];
                float distance = (possibleTarget.Center - position).Length();
                if (distance < maxDistance && possibleTarget.active && possibleTarget.chaseable && !possibleTarget.dontTakeDamage && !possibleTarget.friendly && possibleTarget.lifeMax > 5 && !possibleTarget.immortal && (Collision.CanHit(position, 0, 0, possibleTarget.Center, 0, 0) || ignoreTiles) && specialCondition(possibleTarget))
                {
                    target = Main.npc[k];
                    foundTarget = true;

                    maxDistance = (target.Center - position).Length();
                }
            }
            return foundTarget;
        }
        public static bool ClosestNPCToNPC(this Terraria.NPC npc, ref Terraria.NPC target, float maxDistance, Vector2 position, bool ignoreTiles = false)
        {
            bool foundTarget = false;
            //this is the meat of the targetting logic, it loops through every NPC to check if it is valid the miniomum distance and target selected are updated so that the closest valid NPC is selected
            for (int k = 0; k < Main.npc.Length; k++)
            {
                Terraria.NPC possibleTarget = Main.npc[k];
                float distance = (possibleTarget.Center - position).Length();
                if (distance < maxDistance && possibleTarget.active && possibleTarget.type != npc.type && (Collision.CanHit(position, 0, 0, possibleTarget.Center, 0, 0) || ignoreTiles))
                {
                    target = Main.npc[k];
                    foundTarget = true;

                    maxDistance = (target.Center - position).Length();
                }
            }
            return foundTarget;
        }
        //used by minions to give each minion of the same type a unique identifier so they don't stack
        public static int MinionHordeIdentity(Projectile projectile)
        {
            int identity = 0;
            for (int p = 0; p < 1000; p++)
            {
                if (Main.projectile[p].active && Main.projectile[p].type == projectile.type && Main.projectile[p].owner == projectile.owner)
                {
                    if (projectile.whoAmI == p)
                    {
                        break;
                    }

                    identity++;
                }
            }
            return identity;
        }
        //used for projectiles using ammo, the vanilla PickAmmo had a bunch of clutter we don't need
        public static bool UseAmmo(this Projectile projectile, int ammoID, ref int shoot, ref float speed, ref int Damage, ref float KnockBack, bool dontConsume = false)
        {
            Terraria.Player player = Main.player[projectile.owner];
            Item item = new();
            bool hasFoundAmmo = false;
            for (int i = 54; i < 58; i++)
            {
                if (player.inventory[i].ammo == ammoID && player.inventory[i].stack > 0)
                {
                    item = player.inventory[i];
                    hasFoundAmmo = true;
                    break;
                }
            }
            if (!hasFoundAmmo)
            {
                for (int j = 0; j < 54; j++)
                {
                    if (player.inventory[j].ammo == ammoID && player.inventory[j].stack > 0)
                    {
                        item = player.inventory[j];
                        hasFoundAmmo = true;
                        break;
                    }
                }
            }

            if (hasFoundAmmo)
            {
                shoot = item.shoot;
                if (player.magicQuiver && (ammoID == AmmoID.Arrow || ammoID == AmmoID.Stake))
                {
                    KnockBack = (int)(KnockBack * 1.1);
                    speed *= 1.1f;
                }
                speed += item.shootSpeed;
                if (item.CountsAsClass(DamageClass.Ranged))
                {
                    if (item.damage > 0)
                    {
                        Damage += (int)(item.damage * player.GetDamage(DamageClass.Ranged));
                    }
                }
                else
                {
                    Damage += item.damage;
                }
                if (ammoID == AmmoID.Arrow && player.archery)
                {
                    if (speed < 20f)
                    {
                        speed *= 1.2f;
                        if (speed > 20f)
                        {
                            speed = 20f;
                        }
                    }
                    Damage = (int)((float)Damage * 1.2);
                }
                KnockBack += item.knockBack;
                bool flag2 = dontConsume;

                if (player.magicQuiver && ammoID == AmmoID.Arrow && Main.rand.Next(5) == 0)
                {
                    flag2 = true;
                }
                if (player.ammoBox && Main.rand.Next(5) == 0)
                {
                    flag2 = true;
                }
                if (player.ammoPotion && Main.rand.Next(5) == 0)
                {
                    flag2 = true;
                }

                if (player.ammoCost80 && Main.rand.Next(5) == 0)
                {
                    flag2 = true;
                }
                if (player.ammoCost75 && Main.rand.Next(4) == 0)
                {
                    flag2 = true;
                }
                if (!flag2 && item.consumable)
                {
                    item.stack--;
                    if (item.stack <= 0)
                    {
                        item.active = false;
                        item.TurnToAir();
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        public static void SlowRotation(this ref float currentRotation, float targetAngle, float speed)
        {
            int f = 1; //this is used to switch rotation direction
            float actDirection = new Vector2((float)Math.Cos(currentRotation), (float)Math.Sin(currentRotation)).ToRotation();
            targetAngle = new Vector2((float)Math.Cos(targetAngle), (float)Math.Sin(targetAngle)).ToRotation();

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
        public static void DrawBezier(SpriteBatch spriteBatch, Texture2D texture, string glowMaskTexture, Color drawColor, Vector2 startingPos, Vector2 endPoints, Vector2 c1, Vector2 c2, float chainsPerUse, float rotDis)
        {
            for (float i = 0; i <= 1; i += chainsPerUse)
            {
                Vector2 distBetween;
                float projTrueRotation;
                if (i != 0)
                {
                    distBetween = new Vector2(X(i, startingPos.X, c1.X, c2.X, endPoints.X) -
                    X(i - chainsPerUse, startingPos.X, c1.X, c2.X, endPoints.X),
                    Y(i, startingPos.Y, c1.Y, c2.Y, endPoints.Y) -
                    Y(i - chainsPerUse, startingPos.Y, c1.Y, c2.Y, endPoints.Y));
                    projTrueRotation = distBetween.ToRotation() - (float)Math.PI / 2 + rotDis;
                    spriteBatch.Draw(texture, new Vector2(X(i, startingPos.X, c1.X, c2.X, endPoints.X) - Main.screenPosition.X, Y(i, startingPos.Y, c1.Y, c2.Y, endPoints.Y) - Main.screenPosition.Y),
                    new Rectangle(0, 0, texture.Width, texture.Height), drawColor, projTrueRotation,
                    new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), 1, SpriteEffects.None, 0f);
                }
            }
            //  spriteBatch.Draw(neckTex2D, new Vector2(head.Center.X - Main.screenPosition.X, head.Center.Y - Main.screenPosition.Y), head.frame, drawColor, head.rotation, new Vector2(36 * 0.5f, 32 * 0.5f), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(mod.GetTexture(glowMaskTexture), new Vector2(head.Center.X - Main.screenPosition.X, head.Center.Y - Main.screenPosition.Y), head.frame, Color.White, head.rotation, new Vector2(36 * 0.5f, 32 * 0.5f), 1f, SpriteEffects.None, 0f);
        }

        public static float GradtoRad(float Grad)
        {
            return Grad * (float)Math.PI / 180.0f;
        }

        public static Vector2 RandomPositin(Vector2 pos1, Vector2 pos2)
        {
            var rand = new Random();
            return new Vector2(rand.Next((int)pos1.X, (int)pos2.X) + 1, rand.Next((int)pos1.Y, (int)pos2.Y) + 1);
        }

        public static int GetNearestAlivePlayer(this Terraria.NPC npc)
        {
            var NearestPlayerDist = 4815162342f;
            var NearestPlayer = -1;
            foreach (Terraria.Player player in Main.player)
            {
                if (player.Distance(npc.Center) < NearestPlayerDist && player.active)
                {
                    NearestPlayerDist = player.Distance(npc.Center);
                    NearestPlayer = player.whoAmI;
                }
            }
            return NearestPlayer;
        }
        public static int GetNearestAlivePlayer(this Projectile projectile)
        {
            var NearestPlayerDist = 4815162342f;
            var NearestPlayer = -1;
            foreach (Terraria.Player player in Main.player)
            {
                if (player.Distance(projectile.Center) < NearestPlayerDist && player.active)
                {
                    NearestPlayerDist = player.Distance(projectile.Center);
                    NearestPlayer = player.whoAmI;
                }
            }
            return NearestPlayer;
        }
        public static Vector2 GetNearestAlivePlayerVector(this Terraria.NPC npc)
        {
            var NearestPlayerDist = 4815162342f;
            Vector2 NearestPlayer = Vector2.Zero;
            foreach (Terraria.Player player in Main.player)
            {
                if (player.Distance(npc.Center) < NearestPlayerDist && player.active)
                {
                    NearestPlayerDist = player.Distance(npc.Center);
                    NearestPlayer = player.Center;
                }
            }
            return NearestPlayer;
        }
        public static Vector2 VelocityFPTP(Vector2 pos1, Vector2 pos2, float speed)
        {
            var move = pos2 - pos1;
            return move * (speed / (float)Math.Sqrt(move.X * move.X + move.Y * move.Y));
        }

        public static float RadtoGrad(float Rad)
        {
            return Rad * 180.0f / (float)Math.PI;
        }

        public static int GetNearestNPC(Vector2 Point, bool Friendly = false, bool NoBoss = false)
        {
            float NearestNPCDist = -1;
            int NearestNPC = -1;
            foreach (Terraria.NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;
                if (NoBoss && npc.boss)
                    continue;
                if (!Friendly && (npc.friendly || npc.lifeMax <= 5))
                    continue;
                if (NearestNPCDist == -1 || npc.Distance(Point) < NearestNPCDist)
                {
                    NearestNPCDist = npc.Distance(Point);
                    NearestNPC = npc.whoAmI;
                }
            }
            return NearestNPC;
        }

        public static int GetNearestPlayer(Vector2 Point, bool Alive = false)
        {
            float NearestPlayerDist = -1;
            int NearestPlayer = -1;
            foreach (Terraria.Player player in Main.player)
            {
                if (Alive && (!player.active || player.dead))
                    continue;
                if (NearestPlayerDist == -1 || player.Distance(Point) < NearestPlayerDist)
                {
                    NearestPlayerDist = player.Distance(Point);
                    NearestPlayer = player.whoAmI;
                }
            }
            return NearestPlayer;
        }

        public static Vector2 VelocityToPoint(Vector2 A, Vector2 B, float Speed)
        {
            var Move = B - A;
            return Move * (Speed / (float)Math.Sqrt(Move.X * Move.X + Move.Y * Move.Y));
        }

        public static Vector2 RandomPointInArea(Vector2 A, Vector2 B)
        {
            return new(Main.rand.Next((int)A.X, (int)B.X) + 1, Main.rand.Next((int)A.Y, (int)B.Y) + 1);
        }

        public static Vector2 RandomPointInArea(Rectangle Area)
        {
            return new(Main.rand.Next(Area.X, Area.X + Area.Width), Main.rand.Next(Area.Y, Area.Y + Area.Height));
        }

        public static float RotateBetween2Points(Vector2 A, Vector2 B)
        {
            return (float)Math.Atan2(A.Y - B.Y, A.X - B.X);
        }

        public static Vector2 CenterPoint(Vector2 A, Vector2 B)
        {
            return new((A.X + B.X) / 2.0f, (A.Y + B.Y) / 2.0f);
        }

        public static Vector2 PolarPos(Vector2 Point, float Distance, float Angle, int XOffset = 0, int YOffset = 0)
        {
            var ReturnedValue = new Vector2
            {
                X = Distance * (float)Math.Sin(RadtoGrad(Angle)) + Point.X + XOffset,
                Y = Distance * (float)Math.Cos(RadtoGrad(Angle)) + Point.Y + YOffset
            };
            return ReturnedValue;
        }

        public static bool Chance(float chance)
        {
            return Main.rand.NextFloat() <= chance;
        }

        public static Vector2 SmoothFromTo(Vector2 From, Vector2 To, float Smooth = 60f)
        {
            return From + (To - From) / Smooth;
        }

        public static float DistortFloat(float Float, float Percent)
        {
            var DistortNumber = Float * Percent;
            var Counter = 0;
            while (DistortNumber.ToString().Split(',').Length > 1)
            {
                DistortNumber *= 10;
                Counter++;
            }
            return Float + Main.rand.Next(0, (int)DistortNumber + 1) / (float)Math.Pow(10, Counter) * (Main.rand.Next(2) == 0 ? -1 : 1);
        }

        public static Vector2 FoundPosition(Vector2 tilePos)
        {
            var Screen = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            var FullScreen = tilePos - Main.mapFullscreenPos;
            FullScreen *= Main.mapFullscreenScale / 16;
            FullScreen = FullScreen * 16 + Screen;
            var Draw = new Vector2((int)FullScreen.X, (int)FullScreen.Y);
            return Draw;
        }

        public static void MoveTowards(this Terraria.NPC npc, Vector2 playerTarget, float speed, float turnResistance)
        {
            var Move = playerTarget - npc.Center;
            float Length = Move.Length();
            if (Length > speed)
            {
                Move *= speed / Length;
            }
            Move = (npc.velocity * turnResistance + Move) / (turnResistance + 1f);
            Length = Move.Length();
            if (Length > speed)
            {
                Move *= speed / Length;
            }
            npc.velocity = Move;
        }
        public static bool NextBool(this UnifiedRandom rand, int chance, int total)
        {
            return rand.Next(total) < chance;
        }

        public static Vector2 Spread(float xy)
        {
            return new(Main.rand.NextFloat(-xy, xy - 1), Main.rand.NextFloat(-xy, xy - 1));
        }
        public static Vector2 SpreadUp(float xy)
        {
            return new(Main.rand.NextFloat(-xy, xy - 1), Main.rand.NextFloat(-xy, 0));
        }

        public static void CreateDust(Terraria.Player player, int dust, int count)
        {
            for (var i = 0; i < count; i++)
            {
                Dust.NewDust(player.position, player.width, player.height / 2, dust);
            }
        }

        public static Vector2 RotateVector(Vector2 origin, Vector2 vecToRot, float rot)
        {
            return new((float)(Math.Cos(rot) * (vecToRot.X - (double)origin.X) - Math.Sin(rot) * (vecToRot.Y - (double)origin.Y)) + origin.X, (float)(Math.Sin(rot) * (vecToRot.X - (double)origin.X) + Math.Cos(rot) * (vecToRot.Y - (double)origin.Y)) + origin.Y);
        }

        public static bool Contains(this Rectangle rect, Vector2 pos)
        {
            return rect.Contains((int)pos.X, (int)pos.Y);
        }

        public static bool AnyProjectiles(int type)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == type)
                {
                    return true;
                }
            }
            return false;
        }

        public static int CountProjectiles(int type)
        {
            int p = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == type)
                {
                    p++;
                }
            }
            return p;
        }
        public static Vector2 GetOrigin(Texture2D tex, int frames = 1)
        {
            return new(tex.Width / 2, tex.Height / frames / 2);
        }
        public static Vector2 GetOrigin(Rectangle rect, int frames = 1)
        {
            return new(rect.Width / 2, rect.Height / frames / 2);
        }

        public static void ProjectileExploson(IProjectileSource source, Vector2 pos, float StartAngle, int Streams, int type, int damage, float ProjSpeed, float ai0 = 0, float ai1 = 0)
        {
            float currentAngle = StartAngle;
            //Main.PlaySound(SoundID.Item27, projectile.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Streams; ++i)
                {
                    Vector2 direction = Vector2.Normalize(new Vector2(1, 1)).RotatedBy(MathHelper.ToRadians(360 / Streams * i + currentAngle));
                    direction.X *= ProjSpeed;
                    direction.Y *= ProjSpeed;
                    int proj = Projectile.NewProjectile(source, pos.X, pos.Y, direction.X, direction.Y, type, damage, 0f, Main.myPlayer, ai0, ai1);
                    Main.projectile[proj].Center = pos;
                }
            }


        }

        #region NPC Methods
        /// <summary>
        /// For methods that have 'this NPC npc', instead of doing TTHelper.Shoot(), you can do npc.Shoot() instead.
        /// For shooting projectiles easier. 'aimed' will make the projectile shoot at the target without the extra code, if thats true, also set 'speed'.
        /// 'speed' and 'spread' is only needed if 'aimed' is true. 'spread' is optional.
        /// Example: npc.Shoot(npc.Center, ModContent.ProjectileType<Bullet>(), 40, new Vector2(-5, 0), false, false, SoundID.Item1);
        /// </summary>
        public static void Shoot(this Terraria.NPC npc, Vector2 position, int projType, int damage, Vector2 velocity, bool customSound, LegacySoundStyle sound, string soundString = "", float ai0 = 0, float ai1 = 0)
        {
            Mod mod = Redemption.Instance;
            if (customSound)
            {
                if (!Main.dedServ) { SoundEngine.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, soundString), (int)npc.position.X, (int)npc.position.Y); }
            }
            else
            {
                SoundEngine.PlaySound(sound, (int)npc.position.X, (int)npc.position.Y);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient) { Projectile.NewProjectile(npc.GetProjectileSpawnSource(), position, velocity, projType, damage / 3, 0, Main.myPlayer, ai0, ai1); }
        }

        /// <summary>
        /// For spawning NPCs from NPCs without any extra stuff.
        /// </summary>
        public static void SpawnNPC(int posX, int posY, int npcType, float ai0 = 0, float ai1 = 0, float ai2 = 0, float ai3 = 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int index = Terraria.NPC.NewNPC(posX, posY, npcType, 0, ai0, ai1, ai2, ai3);
                if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: index);
                }
            }
        }

        /// <summary>
        /// A simple Dash method for npcs charging at the player, use npc.Dash(20, false); for example.
        /// </summary>
        public static void Dash(this Terraria.NPC npc, int speed, bool directional, LegacySoundStyle sound, Vector2 target)
        {
            Terraria.Player player = Main.player[npc.target];
            SoundEngine.PlaySound(sound, (int)npc.position.X, (int)npc.position.Y);
            if (target == Vector2.Zero) { target = player.Center; }
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
        public static void LookAtPlayer(this Terraria.NPC npc)
        {
            Terraria.Player player = Main.player[npc.target];
            if (player.Center.X > npc.Center.X)
            {
                npc.spriteDirection = 1;
            }
            else
            {
                npc.spriteDirection = -1;
            }
        }
        public static void LookAtPlayer(this Projectile projectile)
        {
            Terraria.Player player = Main.player[projectile.owner];
            if (player.Center.X > projectile.Center.X)
            {
                projectile.spriteDirection = 1;
            }
            else
            {
                projectile.spriteDirection = -1;
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
        public static void Move(this Terraria.NPC npc, Vector2 vector, float speed, float turnResistance = 10f, bool toPlayer = false)
        {
            Terraria.Player player = Main.player[npc.target];
            Vector2 moveTo = toPlayer ? player.Center + vector : vector;
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
        public static void Move(this Projectile projectile, Vector2 vector, float speed, float turnResistance = 10f, bool toPlayer = false)
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
        public static void MoveToNPC(this Terraria.NPC npc, Terraria.NPC target, Vector2 vector, float speed, float turnResistance = 10f)
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
        /// Checks if the npc is facing the player.
        /// </summary>
        /// <param name="range">Sets how close the player would need to be before the Sight is true.</param>
        /// <param name="lineOfSight">Sets if Sight can be blocked by the player standing behind tiles.</param>
        public static bool Sight(this Terraria.NPC npc, Entity codable, float range = -1, bool facingTarget = true, bool lineOfSight = false)
        {
            if (lineOfSight)
            {
                if (!Collision.CanHit(npc.position, npc.width, npc.height, codable.position, codable.width, codable.height))
                    return false;
            }
            if (range >= 0)
            {
                if (npc.Distance(codable.Center) > range)
                    return false;
            }
            if (facingTarget)
            {
                return npc.Center.X > codable.Center.X && npc.spriteDirection == -1 || npc.Center.X < codable.Center.X && npc.spriteDirection == 1;
            }
            return true;
        }

        /// <summary>
        /// Checks if the npc can fall through platforms.
        /// </summary>
        /// <param name="canJump">Bool for if it can fall, set this to a bool in the npc.</param>
        /// <param name="yOffset">Offset so the npc doesnt fall when the player is on the same plaform as it.</param>
        public static void JumpDownPlatform(this Terraria.NPC npc, ref bool canJump, int yOffset = 12)
        {
            Terraria.Player player = Main.player[npc.target];
            Point tile = npc.Bottom.ToTileCoordinates();
            if (Main.tileSolidTop[Framing.GetTileSafely(tile.X, tile.Y).type] && Main.tile[tile.X, tile.Y].IsActive && npc.Center.Y + yOffset < player.Center.Y)
            {
                Point tile2 = npc.BottomRight.ToTileCoordinates();
                canJump = true;
                if (Main.tile[tile.X - 1, tile.Y - 1].IsActiveUnactuated && Main.tileSolid[Framing.GetTileSafely(tile.X - 1, tile.Y - 1).type] || Main.tile[tile2.X + 1, tile2.Y - 1].IsActiveUnactuated && Main.tileSolid[Framing.GetTileSafely(tile2.X + 1, tile2.Y - 1).type] || npc.collideX)
                {
                    npc.velocity.X = 0;
                }
            }
        }
        public static void JumpDownPlatform(this Terraria.NPC npc, Vector2 vector, ref bool canJump, int yOffset = 12)
        {
            Point tile = npc.Bottom.ToTileCoordinates();
            if (Main.tileSolidTop[Framing.GetTileSafely(tile.X, tile.Y).type] && Main.tile[tile.X, tile.Y].IsActive && npc.Center.Y + yOffset < vector.Y)
            {
                Point tile2 = npc.BottomRight.ToTileCoordinates();
                canJump = true;
                if (Main.tile[tile.X - 1, tile.Y - 1].IsActiveUnactuated && Main.tileSolid[Framing.GetTileSafely(tile.X - 1, tile.Y - 1).type] || Main.tile[tile2.X + 1, tile2.Y - 1].IsActiveUnactuated && Main.tileSolid[Framing.GetTileSafely(tile2.X + 1, tile2.Y - 1).type] || npc.collideX)
                {
                    npc.velocity.X = 0;
                }
            }
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

        /// <summary>
        ///     Makes this NPC horizontally move towards the Player (Take Fighter AI, as an example)
        /// </summary>
        public static void HorizontallyMove(Terraria.NPC npc, Vector2 vector, float moveInterval, float moveSpeed, int maxJumpTilesX, int maxJumpTilesY, bool jumpUpPlatforms)
        {
            //if velocity is less than -1 or greater than 1...
            if (npc.velocity.X < -moveSpeed || npc.velocity.X > moveSpeed)
            {
                //...and npc is not falling or jumping, slow down x velocity.
                if (npc.velocity.Y == 0f) { npc.velocity *= 0.8f; }
            }
            else
            if (npc.velocity.X < moveSpeed && vector.X > npc.Center.X) //handles movement to the right. Clamps at velMaxX.
            {
                npc.velocity.X += moveInterval;
                if (npc.velocity.X > moveSpeed) { npc.velocity.X = moveSpeed; }
            }
            else
            if (npc.velocity.X > -moveSpeed && vector.X < npc.Center.X) //handles movement to the left. Clamps at -velMaxX.
            {
                npc.velocity.X -= moveInterval;
                if (npc.velocity.X < -moveSpeed) { npc.velocity.X = -moveSpeed; }
            }
            //if there's a solid floor under us...
            if (BaseAI.HitTileOnSide(npc, 3))
            {
                //if the npc's velocity is going in the same direction as the npc's direction...
                if (npc.velocity.X < 0f && npc.direction == -1 || npc.velocity.X > 0f && npc.direction == 1)
                {
                    //...attempt to jump if needed.
                    Vector2 newVec = BaseAI.AttemptJump(npc.position, npc.velocity, npc.width, npc.height, npc.direction, npc.directionY, maxJumpTilesX, maxJumpTilesY, moveSpeed, jumpUpPlatforms);
                    if (!npc.noTileCollide)
                    {
                        Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
                    }
                    if (npc.velocity != newVec) { npc.velocity = newVec; npc.netUpdate = true; }
                }
            }
        }
        public static Vector2 FindGround(this Terraria.NPC npc, int range, Func<int, int, bool> CanTeleportTo = null)
        {
            int tileX = (int)npc.position.X / 16;
            int tileY = (int)npc.position.Y / 16;
            int teleportCheckCount = 0;
            bool hasTeleportPoint = false;
            while (!hasTeleportPoint && teleportCheckCount < 100)
            {
                teleportCheckCount++;
                int tpTileX = Main.rand.Next(tileX - range, tileX + range);
                int tpTileY = Main.rand.Next(tileY - range, tileY + range);
                for (int tpY = tpTileY; tpY < tileY + range; tpY++)
                {
                    if ((tpY < tileY - 4 || tpY > tileY + 4 || tpTileX < tileX - 4 || tpTileX > tileX + 4) && (tpY < tileY - 1 || tpY > tileY + 1 || tpTileX < tileX - 1 || tpTileX > tileX + 1) && Main.tile[tpTileX, tpY].IsActiveUnactuated)
                    {
                        if (CanTeleportTo != null && CanTeleportTo(tpTileX, tpY) || Main.tile[tpTileX, tpY - 1].LiquidType != 2 && Main.tileSolid[Main.tile[tpTileX, tpY].type] && !Collision.SolidTiles(tpTileX - 1, tpTileX + 1, tpY - 4, tpY - 1))
                        {
                            return new Vector2(tpTileX, tpY);
                        }
                    }
                }
            }
            return new Vector2(npc.Center.X, npc.Center.Y);
        }
        #endregion
    }
}