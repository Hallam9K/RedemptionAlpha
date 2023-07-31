using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Base
{
    public class BaseAI
    {
        //------------------------------------------------------//
        //-------------------BASE AI CLASS----------------------//
        //------------------------------------------------------//
        // Contains methods for various AI functions for both   //
        // NPCs and Projectiles, such as adding lighting,       //
        // movement, etc.                                       //
        //------------------------------------------------------//
        //  Author(s): Grox the Great, Yoraiz0r                 //
        //------------------------------------------------------//

        #region Custom AI Methods

        public static void AIDive(Projectile projectile, ref int chargeTime, int chargeTimeMax, Vector2 targetCenter)
        {
            chargeTime = Math.Max(0, chargeTime - 1);
            if (chargeTime > 0)
            {
                //while this is running, the velocity of the proj should not be touched, as doing so will break this AI.
                //this AI also ignores tile collision, just a quick concept
                Vector2 positionOld = projectile.position - projectile.velocity;
                float percent = chargeTime / (float)chargeTimeMax;
                float place = (float)Math.Sin((float)Math.PI * percent); //where on the sin path it is
                float distX = Math.Abs(targetCenter.X - projectile.Center.X), distY = Math.Abs(targetCenter.Y - projectile.Center.Y);
                float distPercentX = distX / chargeTimeMax, distPercentY = distY / chargeTimeMax;
                projectile.velocity = new Vector2(distPercentX * chargeTime * projectile.direction, place * distPercentY); //move on the x axis a fixed amount per tick and on the Y axis by how much the sine is multiplied by the a percentage of the distance on the y axis.
                projectile.position = positionOld;
            }
        }

        public static void AIMinionPlant(Projectile projectile, ref float[] ai, Entity owner, Vector2 endPoint, bool setTime = true, float vineLength = 150f, float vineLengthLong = 200f, int vineTimeExtend = 300, int vineTimeMax = 450, float moveInterval = 0.035f, float speedMax = 2f, Vector2 targetOffset = default, Func<Entity, Entity, Entity> getTarget = null, Func<Entity, Entity, Entity, bool> shootTarget = null)
        {
            if (setTime) { projectile.timeLeft = 10; }
            Entity target = getTarget == null ? null : getTarget(projectile, owner);
            if (target == null) { target = owner; }
            bool targetOwner = target == owner;
            ai[0] += 1f;
            if (ai[0] > vineTimeExtend)
            {
                vineLength = vineLengthLong;
                if (ai[0] > vineTimeMax) { ai[0] = 0f; }
            }
            Vector2 targetCenter = target.Center + targetOffset + (target == owner ? new Vector2(0f, owner is Player player ? player.gfxOffY : owner is NPC nPC ? nPC.gfxOffY : ((Projectile)owner).gfxOffY) : default);
            if (!targetOwner)
            {
                float distTargetX = targetCenter.X - endPoint.X;
                float distTargetY = targetCenter.Y - endPoint.Y;
                float distTarget = (float)Math.Sqrt(distTargetX * distTargetX + distTargetY * distTargetY);
                if (distTarget > vineLength)
                {
                    projectile.velocity *= 0.85f;
                    projectile.velocity += owner.velocity;
                    distTarget = vineLength / distTarget;
                    distTargetX *= distTarget;
                    distTargetY *= distTarget;
                }
                bool dontMove = shootTarget != null && shootTarget(projectile, owner, target);
                if (!dontMove)
                {
                    if (projectile.position.X < endPoint.X + distTargetX)
                    {
                        projectile.velocity.X += moveInterval;
                        if (projectile.velocity.X < 0f && distTargetX > 0f) { projectile.velocity.X += moveInterval * 1.5f; }
                    }
                    else
                    if (projectile.position.X > endPoint.X + distTargetX)
                    {
                        projectile.velocity.X -= moveInterval;
                        if (projectile.velocity.X > 0f && distTargetX < 0f) { projectile.velocity.X -= moveInterval * 1.5f; }
                    }
                    if (projectile.position.Y < endPoint.Y + distTargetY)
                    {
                        projectile.velocity.Y += moveInterval;
                        if (projectile.velocity.Y < 0f && distTargetY > 0f) { projectile.velocity.Y += moveInterval * 1.5f; }
                    }
                    else
                    if (projectile.position.Y > endPoint.Y + distTargetY)
                    {
                        projectile.velocity.Y -= moveInterval;
                        if (projectile.velocity.Y > 0f && distTargetY < 0f) { projectile.velocity.Y -= moveInterval * 1.5f; }
                    }
                }
                else
                {
                    projectile.velocity *= 0.85f;
                    if (Math.Abs(projectile.velocity.X) < moveInterval + 0.01f) { projectile.velocity.X = 0f; }
                    if (Math.Abs(projectile.velocity.Y) < moveInterval + 0.01f) { projectile.velocity.Y = 0f; }
                }
                projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -speedMax, speedMax);
                projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -speedMax, speedMax);
                if (distTargetX > 0f) { projectile.spriteDirection = 1; projectile.rotation = (float)Math.Atan2(distTargetY, distTargetX); }
                else
                if (distTargetX < 0f) { projectile.spriteDirection = -1; projectile.rotation = (float)Math.Atan2(distTargetY, distTargetX) + 3.14f; }
                if (projectile.tileCollide)
                {
                    Vector4 slopeVec = Collision.SlopeCollision(projectile.position, projectile.velocity, projectile.width, projectile.height);
                    projectile.position = new Vector2(slopeVec.X, slopeVec.Y);
                    projectile.velocity = new Vector2(slopeVec.Z, slopeVec.W);
                }
                projectile.position += owner.position - owner.oldPosition;
            }
            else
            {
                projectile.position += owner.position - owner.oldPosition;
                projectile.spriteDirection = owner.Center.X > projectile.Center.X ? -1 : 1;
                projectile.velocity = AIVelocityLinear(projectile, targetCenter, moveInterval, speedMax, true);
                if (Vector2.Distance(projectile.Center, targetCenter) < speedMax * 1.1f)
                {
                    projectile.rotation = 0f;
                    projectile.velocity *= 0f; projectile.Center = targetCenter;
                }
                else
                {
                    projectile.rotation = BaseUtility.RotationTo(targetCenter, projectile.Center) + (projectile.spriteDirection == -1 ? 3.14f : 0f);
                }
            }
        }

        public static void TileCollidePlant(Projectile projectile, ref Vector2 velocity, float speedMax)
        {
            if (projectile.velocity.X != velocity.X)
            {
                projectile.netUpdate = true;
                projectile.velocity.X *= -0.7f;
                projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -speedMax, speedMax);
            }
            if (projectile.velocity.Y != velocity.Y)
            {
                projectile.netUpdate = true;
                projectile.velocity.Y *= -0.7f;
                projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -speedMax, speedMax);
            }
        }


        public static void AIMinionFlier(Projectile projectile, ref float[] ai, Entity owner, bool pet = false, bool movementFixed = false, bool hover = false, int hoverHeight = 40, int lineDist = 40, int returnDist = 400, int teleportDist = 800, float moveInterval = -1f, float maxSpeed = -1f, float maxSpeedFlying = -1f, bool autoSpriteDir = true, bool dummyTileCollide = false, Func<Entity, Entity, Entity> getTarget = null, Func<Entity, Entity, Entity, bool> shootTarget = null)
        {
            if (moveInterval == -1f) { moveInterval = 0.08f * Main.player[projectile.owner].moveSpeed; }
            if (maxSpeed == -1f) { maxSpeed = Math.Max(Main.player[projectile.owner].maxRunSpeed, Main.player[projectile.owner].accRunSpeed); }
            if (maxSpeedFlying == -1f) { maxSpeedFlying = Math.Max(maxSpeed, Math.Max(Main.player[projectile.owner].maxRunSpeed, Main.player[projectile.owner].accRunSpeed)); }
            projectile.timeLeft = 10;
            bool tileCollide = projectile.tileCollide;
            AIMinionFlier(projectile, ref ai, owner, ref tileCollide, ref projectile.netUpdate, pet ? 0 : projectile.minionPos, movementFixed, hover, hoverHeight, lineDist, returnDist, teleportDist, moveInterval, maxSpeed, maxSpeedFlying, getTarget, shootTarget);
            if (!dummyTileCollide) projectile.tileCollide = tileCollide;
            if (autoSpriteDir) { projectile.spriteDirection = projectile.direction; }
            if (ai[0] == 1) { projectile.spriteDirection = owner.velocity.X == 0 ? projectile.spriteDirection : owner.velocity.X > 0 ? 1 : -1; }
            if ((getTarget == null || getTarget(projectile, owner) == null || getTarget(projectile, owner) == owner) && Math.Abs(projectile.velocity.X + projectile.velocity.Y) <= 0.025f) { projectile.spriteDirection = owner.Center.X > projectile.Center.X ? 1 : -1; }
        }

        /*
		 * Custom AI that works similarly to fighter minion AI. (uses ai[0, 1])
		 * 
		 * owner : The Projectile or NPC who is this minion's owner.
		 * tileCollide : A bool, set to say wether or not the minion can tile collide or not.
		 * netUpdate : set to say wether or not the minion should sync if in multiplayer.
		 * gfxOffsetY : The graphics offset for Y, used for walking up slopes.
		 * stepSpeed : Used for walking up slopes.
		 * minionPos : The minion's position in the minion lineup.
		 * lineDist : The distance between each minion when they line up.
		 * returnDist : The distance to 'fly' back to the player.
		 * teleportDist : The distance to instantly teleport to the player.
		 * moveInterval : How much to move each tick.
		 * maxSpeed : The maxmimum speed of the minion.
		 * maxSpeedFlying : The maximum speed whist 'flying' back to the player.
		 * GetTarget : a Func(Entity codable, Entity owner), returns a Vector2 of the a target's position. If GetTarget is null or it returns default(Vector2) the target is assumed to be the owner.
		 */
        public static void AIMinionFlier(Entity codable, ref float[] ai, Entity owner, ref bool tileCollide, ref bool netUpdate, int minionPos, bool movementFixed, bool hover = false, int hoverHeight = 40, int lineDist = 40, int returnDist = 400, int teleportDist = 800, float moveInterval = 0.2f, float maxSpeed = 4.5f, float maxSpeedFlying = 4.5f, Func<Entity, Entity, Entity> getTarget = null, Func<Entity, Entity, Entity, bool> shootTarget = null)
        {
            float dist = Vector2.Distance(codable.Center, owner.Center);
            if (dist > teleportDist) { codable.Center = owner.Center; }
            int tileX = (int)(codable.Center.X / 16f), tileY = (int)(codable.Center.Y / 16f);
            Tile tile = Framing.GetTileSafely(tileX, tileY);
            bool inTile = tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType];
            float prevAI = ai[0];
            ai[0] = ai[0] == 1 && (dist > Math.Max(lineDist, returnDist / 2f) || !BaseUtility.CanHit(codable.Hitbox, owner.Hitbox)) || dist > returnDist || inTile ? 1 : 0;
            if (ai[0] != prevAI) { netUpdate = true; }
            if (ai[0] == 0 || ai[0] == 1)
            {
                if (ai[0] == 1) { moveInterval *= 1.5f; maxSpeedFlying *= 1.5f; }
                tileCollide = ai[0] == 0;
                Entity target = getTarget == null ? owner : getTarget(codable, owner);
                if (target == null) { target = owner; }
                Vector2 targetCenter = target.Center;
                bool isOwner = target == owner;
                bool dontMove = ai[0] == 0 && shootTarget != null && shootTarget(codable, owner, target);
                if (isOwner)
                {
                    targetCenter.Y -= hoverHeight;
                    if (hover) { targetCenter.X += (lineDist + lineDist * minionPos) * -target.direction; }
                }
                if (!hover || !isOwner)
                {
                    float dirDist = hover ? 1.2f : 1.8f;
                    float dir = dist < lineDist * minionPos + lineDist * dirDist ? codable.velocity.X > 0 ? 1f : -1f : target.Center.X > codable.Center.X ? 1f : -1f;
                    //Semierratic movement so it looks more like a swarm and less like synchronized swimmers.
                    targetCenter.X += (minionPos == 0 ? 0f : minionPos % 5 == 0 ? lineDist / 4f : minionPos % 4 == 0 ? lineDist / 2f : minionPos % 3 == 0 ? lineDist / 3f : 0f) * dir;
                    targetCenter.X += lineDist * 2f * dir;
                    targetCenter.Y -= hoverHeight / 4f * minionPos;
                    targetCenter.Y -= (codable.velocity.X < 0 ? lineDist * 0.25f : -lineDist * 0.25f) * (minionPos % 2 == 0 ? 1 : -1);
                }
                float targetDistX = Math.Abs(codable.Center.X - targetCenter.X);
                float targetDistY = Math.Abs(codable.Center.Y - targetCenter.Y);
                bool slowdownX = hover && owner.velocity.X < 0.025f && targetDistX < 8f * Math.Max(1f, maxSpeed / 4f);
                bool slowdownY = hover && owner.velocity.Y < 0.025f && targetDistY < 8f * Math.Max(1f, maxSpeed / 4f);
                Vector2 vel = AIVelocityLinear(codable, targetCenter, moveInterval, ai[0] == 0 ? maxSpeed : maxSpeedFlying, true);
                if (!dontMove && !slowdownX) { codable.velocity.X += vel.X * 0.125f; }
                if (!dontMove && !slowdownY) { codable.velocity.Y += vel.Y * 0.125f; }
                if (dontMove || slowdownX) { codable.velocity.X *= Math.Abs(codable.velocity.X) > 0.01f ? 0.85f : 0f; }
                if (vel.X > 0 && codable.velocity.X > vel.X || vel.X < 0 && codable.velocity.X < vel.X) { codable.velocity.X = vel.X; }
                if (dontMove || slowdownY) { codable.velocity.Y *= Math.Abs(codable.velocity.Y) > 0.01f ? 0.85f : 0f; }
                if (vel.Y > 0 && codable.velocity.Y > vel.Y || vel.Y < 0 && codable.velocity.X < vel.Y) { codable.velocity.Y = vel.Y; }
            }
        }




        public static void AIMinionFighter(Projectile projectile, ref float[] ai, Entity owner, bool pet = false, int jumpDistX = 4, int jumpDistY = 5, int lineDist = 40, int returnDist = 400, int teleportDist = 800, float moveInterval = -1f, float maxSpeed = -1f, float maxSpeedFlying = -1f, Func<Entity, Entity, Entity> getTarget = null)
        {
            if (moveInterval == -1f) { moveInterval = 0.08f * Main.player[projectile.owner].moveSpeed; }
            if (maxSpeed == -1f) { maxSpeed = Math.Max(Main.player[projectile.owner].maxRunSpeed, Main.player[projectile.owner].accRunSpeed); }
            if (maxSpeedFlying == -1f) { maxSpeedFlying = Math.Max(maxSpeed, Math.Max(Main.player[projectile.owner].maxRunSpeed, Main.player[projectile.owner].accRunSpeed)); }
            AIMinionFighter(projectile, ref ai, owner, ref projectile.tileCollide, ref projectile.netUpdate, ref projectile.gfxOffY, ref projectile.stepSpeed, pet ? 0 : projectile.minionPos, jumpDistX, jumpDistY, lineDist, returnDist, teleportDist, moveInterval, maxSpeed, maxSpeedFlying, getTarget);
            projectile.spriteDirection = projectile.direction;
            if (ai[0] == 1) { projectile.spriteDirection = owner.velocity.X == 0 ? projectile.spriteDirection : owner.velocity.X > 0 ? 1 : -1; }
            if ((getTarget == null || getTarget(projectile, owner) == null || getTarget(projectile, owner) == owner) && projectile.velocity.X is >= -0.025f or <= 0.025f && projectile.velocity.Y == 0) { projectile.spriteDirection = owner.Center.X > projectile.Center.X ? 1 : -1; }
        }


        /*
		 * Custom AI that works similarly to fighter minion AI. (uses ai[0, 1])
		 * 
		 * owner : The Projectile or NPC who is this minion's owner.
		 * tileCollide : A bool, set to say wether or not the minion can tile collide or not.
		 * netUpdate : set to say wether or not the minion should sync if in multiplayer.
		 * gfxOffsetY : The graphics offset for Y, used for walking up slopes.
		 * stepSpeed : Used for walking up slopes.
		 * minionPos : The minion's position in the minion lineup.
		 * jumpDistX : The minion's max jump distance on the X axis.
		 * jumpDistY : The minion's max jump distance on the Y axis.
		 * lineDist : The distance between each minion when they line up.
		 * returnDist : The distance to 'fly' back to the player.
		 * teleportDist : The distance to instantly teleport to the player.
		 * moveInterval : How much to move each tick.
		 * maxSpeed : The maxmimum speed of the minion.
		 * maxSpeedFlying : The maximum speed whist 'flying' back to the player.
		 * GetTarget : a Func(Entity codable, Entity owner), returns a Vector2 of the a target's position. If GetTarget is null or it returns default(Vector2) the target is assumed to be the owner.
		 */
        public static void AIMinionFighter(Entity codable, ref float[] ai, Entity owner, ref bool tileCollide, ref bool netUpdate, ref float gfxOffY, ref float stepSpeed, int minionPos, int jumpDistX = 4, int jumpDistY = 5, int lineDist = 40, int returnDist = 400, int teleportDist = 800, float moveInterval = 0.2f, float maxSpeed = 4.5f, float maxSpeedFlying = 4.5f, Func<Entity, Entity, Entity> getTarget = null)
        {
            float dist = Vector2.Distance(codable.Center, owner.Center);
            if (dist > teleportDist) { codable.Center = owner.Center; }
            int tileX = (int)(codable.Center.X / 16f), tileY = (int)(codable.Center.Y / 16f);
            Tile tile = Framing.GetTileSafely(tileX, tileY);
            bool inTile = tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType];
            float prevAI = ai[0];
            ai[0] = ai[0] == 1 && (owner.velocity.Y != 0 || dist > Math.Max(lineDist, returnDist / 10f)) || dist > returnDist || inTile ? 1 : 0;
            if (ai[0] != prevAI) { netUpdate = true; }
            if (ai[0] == 0) //walking
            {
                tileCollide = true;
                Entity target = getTarget == null ? null : getTarget(codable, owner);
                Vector2 targetCenter = target == null ? default : target.Center;
                bool isOwner = target == null || targetCenter == owner.Center;
                if (targetCenter == default)
                {
                    targetCenter = owner.Center;
                    targetCenter.X += (owner.width + 10 + lineDist * minionPos) * -owner.direction;
                }
                float targetDistX = Math.Abs(codable.Center.X - targetCenter.X);
                float targetDistY = Math.Abs(codable.Center.Y - targetCenter.Y);
                int moveDirection = targetCenter.X > codable.Center.X ? 1 : -1;
                int moveDirectionY = targetCenter.Y > codable.Center.Y ? 1 : -1;
                if (isOwner && owner.velocity.X < 0.025f && codable.velocity.Y == 0f && targetDistX < 8f)
                {
                    codable.velocity.X *= Math.Abs(codable.velocity.X) > 0.01f ? 0.8f : 0f;
                }
                else
                if (codable.velocity.X < -maxSpeed || codable.velocity.X > maxSpeed)
                {
                    if (codable.velocity.Y == 0f) { codable.velocity *= 0.85f; }
                }
                else
                if (codable.velocity.X < maxSpeed && moveDirection == 1)
                {
                    if (codable.velocity.X < 0) { codable.velocity.X *= 0.85f; }
                    codable.velocity.X += moveInterval * (codable.velocity.X < 0 ? 2f : 1f);
                    if (codable.velocity.X > maxSpeed) { codable.velocity.X = maxSpeed; }
                }
                else
                if (codable.velocity.X > -maxSpeed && moveDirection == -1)
                {
                    if (codable.velocity.X > 0) { codable.velocity.X *= 0.8f; }
                    codable.velocity.X -= moveInterval * (codable.velocity.X > 0 ? 2f : 1f);
                    if (codable.velocity.X < -maxSpeed) { codable.velocity.X = -maxSpeed; }
                }
                WalkupHalfBricks(codable, ref gfxOffY, ref stepSpeed);
                if (HitTileOnSide(codable, 3))
                {
                    if (codable.velocity.X < 0f && moveDirection == -1 || codable.velocity.X > 0f && moveDirection == 1)
                    {
                        bool test = target != null && !isOwner && targetDistX < 50f && targetDistY > codable.height + codable.height / 2 && targetDistY < 16f * (jumpDistY + 1) && BaseUtility.CanHit(codable.Hitbox, target.Hitbox);
                        Vector2 newVec = AttemptJump(codable.position, codable.velocity, codable.width, codable.height, moveDirection, moveDirectionY, jumpDistX, jumpDistY, maxSpeed, true, target, test);
                        if (tileCollide)
                        {
                            newVec = Collision.TileCollision(codable.position, newVec, codable.width, codable.height);
                            Vector4 slopeVec = Collision.SlopeCollision(codable.position, newVec, codable.width, codable.height);
                            codable.position = new Vector2(slopeVec.X, slopeVec.Y);
                            codable.velocity = new Vector2(slopeVec.Z, slopeVec.W);
                        }
                        if (codable.velocity != newVec) { codable.velocity = newVec; netUpdate = true; }
                    }
                }
                else { codable.velocity.Y += 0.35f; } //gravity
            }
            else //flying
            {
                tileCollide = false;
                Vector2 targetCenter = owner.Center;
                if (owner.velocity.Y != 0f && dist < 80)
                {
                    targetCenter = owner.Center + BaseUtility.RotateVector(default, new Vector2(10, 0f), BaseUtility.RotationTo(codable.Center, owner.Center));
                }
                Vector2 newVel = BaseUtility.RotateVector(default, new Vector2(maxSpeedFlying, 0f), BaseUtility.RotationTo(codable.Center, targetCenter));
                if (owner.velocity.Y != 0f && (newVel.X > 0 && codable.velocity.X < 0 || newVel.X < 0 && codable.velocity.X > 0))
                {
                    codable.velocity *= 0.98f; newVel *= 0.02f; codable.velocity += newVel;
                }
                else { codable.velocity = newVel; }
                codable.position += owner.velocity;
            }
        }

        public static void AIMinionSlime(Projectile projectile, ref float[] ai, Entity owner, bool pet = false, int lineDist = 40, int returnDist = 400, int teleportDist = 800, float jumpVelX = -1f, float jumpVelY = 20f, float maxSpeedFlying = -1f, Func<Entity, Entity, Entity> getTarget = null)
        {
            if (jumpVelX == -1f) { jumpVelX = 2f + Main.player[projectile.owner].velocity.X; }
            if (maxSpeedFlying == -1f) { maxSpeedFlying = Math.Max(jumpVelX, jumpVelY); }
            AIMinionSlime(projectile, ref ai, owner, ref projectile.tileCollide, ref projectile.netUpdate, pet ? 0 : projectile.minionPos, lineDist, returnDist, teleportDist, jumpVelX, jumpVelY, maxSpeedFlying, getTarget);
            projectile.spriteDirection = projectile.direction;
            if (ai[0] == 1) { projectile.spriteDirection = owner.velocity.X == 0 ? projectile.spriteDirection : owner.velocity.X > 0 ? 1 : -1; }
            if ((getTarget == null || getTarget(projectile, owner) == null || getTarget(projectile, owner) == owner) && projectile.velocity.X is >= -0.025f or <= 0.025f && projectile.velocity.Y == 0) { projectile.spriteDirection = owner.Center.X > projectile.Center.X ? 1 : -1; }
        }

        /*
		 * Custom AI that works similarly to slime minion AI. (uses ai[0, 1])
		 * 
		 * owner : The Projectile or NPC who is this minion's owner.
		 * tileCollide : A bool, set to say wether or not the minion can tile collide or not.
		 * netUpdate : set to say wether or not the minion should sync if in multiplayer.
		 * gfxOffsetY : The graphics offset for Y, used for walking up slopes.
		 * stepSpeed : Used for walking up slopes.
		 * minionPos : The minion's position in the minion lineup.
		 * lineDist : The distance between each minion when they line up.
		 * returnDist : The distance to 'fly' back to the player.
		 * teleportDist : The distance to instantly teleport to the player.
		 * jumpVelX : The velocity to bounce on the X axis.
		 * jumpVelY : The velocity to boucne on the Y axis.
		 * maxSpeedFlying : The maximum speed whist 'flying' back to the player.
		 * GetTarget : a Func(Entity codable, Entity owner), returns a Vector2 of the a target's position. If GetTarget is null or it returns default(Vector2) the target is assumed to be the owner.
		 */
        public static void AIMinionSlime(Entity codable, ref float[] ai, Entity owner, ref bool tileCollide, ref bool netUpdate, int minionPos, int lineDist = 40, int returnDist = 400, int teleportDist = 800, float jumpVelX = 2f, float jumpVelY = 20f, float maxSpeedFlying = 4.5f, Func<Entity, Entity, Entity> getTarget = null)
        {
            float dist = Vector2.Distance(codable.Center, owner.Center);
            if (dist > teleportDist) { codable.Center = owner.Center; }
            int tileX = (int)(codable.Center.X / 16f), tileY = (int)(codable.Center.Y / 16f);
            Tile tile = Framing.GetTileSafely(tileX, tileY);
            bool inTile = tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType];
            float prevAI = ai[0];
            ai[0] = ai[0] == 1 && (owner.velocity.Y != 0 || dist > Math.Max(lineDist, returnDist / 10f)) || dist > returnDist || inTile ? 1 : 0;
            if (ai[0] != prevAI) { netUpdate = true; }
            if (ai[0] == 0) //walking
            {
                tileCollide = true;
                Entity target = getTarget == null ? null : getTarget(codable, owner);
                Vector2 targetCenter = target == null ? default : target.Center;
                bool isOwner = target == null || targetCenter == owner.Center;
                if (targetCenter == default)
                {
                    targetCenter = owner.Center;
                    targetCenter.X += (lineDist + lineDist * minionPos) * -owner.direction;
                }
                float targetDistX = Math.Abs(codable.Center.X - targetCenter.X);
                int moveDirection = targetCenter.X > codable.Center.X ? 1 : -1;
                if (isOwner && owner.velocity.X < 0.025f && codable.velocity.Y == 0f && targetDistX < 8f)
                {
                    codable.velocity.X *= Math.Abs(codable.velocity.X) > 0.01f ? 0.8f : 0f;
                }
                else
                if (codable.velocity.Y == 0f)
                {
                    codable.velocity.X *= 0.8f;
                    if (codable.velocity.X is > -0.1f and < 0.1f) { codable.velocity.X = 0f; }
                    codable.velocity.Y = -jumpVelY;
                    codable.velocity.X += jumpVelX * moveDirection;
                    codable.position += codable.velocity;
                }
                if (HitTileOnSide(codable, 3))
                {
                    if (codable.velocity.X < 0f && moveDirection == -1 || codable.velocity.X > 0f && moveDirection == 1)
                    {
                        Vector2 newVec = codable.velocity;
                        if (tileCollide)
                        {
                            newVec = Collision.TileCollision(codable.position, newVec, codable.width, codable.height);
                            Vector4 slopeVec = Collision.SlopeCollision(codable.position, newVec, codable.width, codable.height);
                            codable.position = new Vector2(slopeVec.X, slopeVec.Y);
                            codable.velocity = new Vector2(slopeVec.Z, slopeVec.W);
                        }
                        if (codable.velocity != newVec) { codable.velocity = newVec; netUpdate = true; }
                    }
                }
                else { codable.velocity.Y += 0.35f; } //gravity*/			
                /*if (isOwner && owner.velocity.X < 0.025f && codable.velocity.Y == 0f && targetDistX < 8f)
				{
					codable.velocity.X *= (Math.Abs(codable.velocity.X) > 0.01f ? 0.8f : 0f);
				}else
				if (codable.velocity.X < -maxSpeed || codable.velocity.X > maxSpeed)
				{
					if (codable.velocity.Y == 0f){ codable.velocity *= 0.85f; }
				}else
				if (codable.velocity.X < maxSpeed && moveDirection == 1)
				{
					if(codable.velocity.X < 0){ codable.velocity.X *= 0.85f; }
					codable.velocity.X += moveInterval * (codable.velocity.X < 0 ? 2f : 1f);
					if (codable.velocity.X > maxSpeed){ codable.velocity.X = maxSpeed; }
				}else
				if (codable.velocity.X > -maxSpeed && moveDirection == -1)
				{
					if(codable.velocity.X > 0) { codable.velocity.X *= 0.8f; }
					codable.velocity.X -= moveInterval * (codable.velocity.X > 0 ? 2f : 1f);
					if(codable.velocity.X < -maxSpeed){ codable.velocity.X = -maxSpeed; }
				}
				WalkupHalfBricks(codable, ref gfxOffY, ref stepSpeed);
				if (HitTileOnSide(codable, 3))
				{
					if ((codable.velocity.X < 0f && moveDirection == -1) || (codable.velocity.X > 0f && moveDirection == 1))
					{
						Vector2 newVec = AttemptJump(codable.position, codable.velocity, codable.width, codable.height, moveDirection, moveDirectionY, 4, 5, maxSpeed, true, null);
						if (tileCollide)
						{
							newVec = Collision.TileCollision(codable.position, newVec, codable.width, codable.height);
							Vector4 slopeVec = Collision.SlopeCollision(codable.position, newVec, codable.width, codable.height);
							codable.position = new Vector2(slopeVec.X, slopeVec.Y);
							codable.velocity = new Vector2(slopeVec.Z, slopeVec.W);
						}
						if (codable.velocity != newVec) { codable.velocity = newVec; netUpdate = true; }
					}
				}else{ codable.velocity.Y += 0.35f; } //gravity*/
            }
            else //flying
            {
                tileCollide = false;
                Vector2 targetCenter = owner.Center;
                if (owner.velocity.Y != 0f && dist < 80)
                {
                    targetCenter = owner.Center + BaseUtility.RotateVector(default, new Vector2(10, 0f), BaseUtility.RotationTo(codable.Center, owner.Center));
                }
                Vector2 newVel = BaseUtility.RotateVector(default, new Vector2(maxSpeedFlying, 0f), BaseUtility.RotationTo(codable.Center, targetCenter));
                if (owner.velocity.Y != 0f && (newVel.X > 0 && codable.velocity.X < 0 || newVel.X < 0 && codable.velocity.X > 0))
                {
                    codable.velocity *= 0.98f; newVel *= 0.02f; codable.velocity += newVel;
                }
                else { codable.velocity = newVel; }
                codable.position += owner.velocity;
            }
        }


        /*
		 * Custom AI that will cause the npc to rotate around a point in a fixed circle.
		 * 
		 * rotation : The codable's rotation.
		 * moveRot : A value storing the internal rotation of the codable.
		 * rotateCenter : The center to be rotating around.
		 * absolute : If true, moves it by position instead of by velocity.
		 * rotDistance : How far from the rotateCenter to rotate.
		 * rotThreshold : Only used if absolute is false, used to determine how much 'give' the codable has before it forces itself back into the rotation.
		 * rotAmount : How much to rotate each tick.
		 * moveTowards : Only used if absolute is false, if outside the rotation, move towards it.
		 */
        public static void AIRotate(Entity codable, ref float rotation, ref float moveRot, Vector2 rotateCenter, bool absolute = false, float rotDistance = 50f, float rotThreshold = 20f, float rotAmount = 0.024f, bool moveTowards = true)
        {
            if (absolute)
            {
                moveRot += rotAmount;
                Vector2 rotVec = BaseUtility.RotateVector(default, new Vector2(rotDistance, 0f), moveRot) + rotateCenter;
                codable.Center = rotVec;
                rotVec.Normalize();
                rotation = BaseUtility.RotationTo(codable.Center, rotateCenter) - 1.57f;
                codable.velocity *= 0f;
            }
            else
            {
                float dist = Vector2.Distance(codable.Center, rotateCenter);
                if (dist < rotDistance)//close enough to rotate
                {
                    if (rotDistance - dist > rotThreshold) //too close, get back into position
                    {
                        moveRot += rotAmount;
                        Vector2 rotVec = BaseUtility.RotateVector(default, new Vector2(rotDistance, 0f), moveRot) + rotateCenter;
                        float rot2 = BaseUtility.RotationTo(codable.Center, rotVec);
                        codable.velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), rot2);
                        rotation = BaseUtility.RotationTo(codable.Center, codable.Center + codable.velocity);
                    }
                    else
                    {
                        moveRot += rotAmount;
                        Vector2 rotVec = BaseUtility.RotateVector(default, new Vector2(rotDistance, 0f), moveRot) + rotateCenter;
                        float rot2 = BaseUtility.RotationTo(codable.Center, rotVec);
                        codable.velocity = BaseUtility.RotateVector(default, new Vector2(5f, 0f), rot2);
                        rotation = BaseUtility.RotationTo(codable.Center, codable.Center + codable.velocity);
                    }
                }
                else
                if (moveTowards)
                {
                    codable.velocity = AIVelocityLinear(codable, rotateCenter, rotAmount, rotAmount, true);
                    rotation = BaseUtility.RotationTo(codable.Center, rotateCenter) - 1.57f;
                }
                else { codable.velocity *= 0.95f; }
            }
        }


        public static void AIPounce(Entity codable, Player player, float pounceScalar = 3f, float maxSpeed = 5f, float yBoost = -5.2f, float minDistance = 50, float maxDistance = 60)
        {
            if (player is not { active: true } || player.dead) { return; }
            AIPounce(codable, player.Center, pounceScalar, maxSpeed, yBoost, minDistance, maxDistance);
        }

        /*
         * Custom AI that will cause the npc to 'pounce' at the given target when it is within range.
         * 
         * pounceCenter : the central point of which to pounce.
         * pounceScalar : How much to scale the X-axis velocity by.
         * maxSpeed : the maximum speed to pounce by on the X-axis of the codable.
         * yBoost : the amount to jump by on the y-axis.
         * minDistance/maxDistance : the minimum and maximum distance from the pounce center that the codable is allowed to pounce, respectively.
         */
        public static void AIPounce(Entity codable, Vector2 pounceCenter, float pounceScalar = 3.5f, float maxSpeed = 5f, float yBoost = -5.2f, float minDistance = 50, float maxDistance = 60)
        {
            int direction = codable is NPC nPc ? nPc.direction : codable is Projectile projectile ? projectile.direction : 0;
            float dist = Vector2.Distance(codable.Center, pounceCenter);
            if (pounceCenter.Y <= codable.Center.Y && dist > minDistance && dist < maxDistance)
            {
                bool onLeft = pounceCenter.X < codable.Center.X;
                if (codable.velocity.Y == 0 && (onLeft && direction == -1 || !onLeft && direction == 1))
                {
                    codable.velocity.X *= pounceScalar;
                    if (codable.velocity.X > maxSpeed) { codable.velocity.X = maxSpeed; }
                    if (codable.velocity.X < -maxSpeed) { codable.velocity.X = -maxSpeed; }
                    codable.velocity.Y = yBoost;
                    if (codable is NPC nPc1) { nPc1.netUpdate = true; }
                }
            }
        }

        /*
         * Custom AI that will cause the npc to follow the path of given points. (uses ai[0, 1, 2])
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * points : the array of points to follow.
         * moveInterval : the amount to move by per tick.
         * maxSpeed : the maximum speed of the npc.
         * direct : If true npc's velocity is set so it moves in a straight line. If false, moves similarly to Flier AI.
         */
        public static void AIPath(NPC npc, ref float[] ai, Vector2[] points, float moveInterval = 0.11f, float maxSpeed = 3f, bool direct = false)
        {
            Vector2 destVec = new(ai[0], ai[1]);
            if (Main.netMode != NetmodeID.MultiplayerClient && destVec != default && Vector2.Distance(npc.Center, destVec) <= Math.Max(5f, (npc.width + npc.height) / 2f * 0.45f))
            {
                ai[0] = 0f; ai[1] = 0f; destVec = default;
            }
            if (npc.ai[2] < points.Length)
            {
                //if the destination vec is default (0, 0), get the current point.
                if (destVec == default)
                {
                    npc.velocity *= 0.95f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        destVec = points[(int)npc.ai[2]];
                        ai[0] = destVec.X; ai[1] = destVec.Y;
                        ai[2]++;
                        npc.netUpdate = true;
                    }
                }
                else //otherwise move to the point.
                {
                    npc.velocity = AIVelocityLinear(npc, destVec, moveInterval, maxSpeed, direct);
                }
            }
        }

        /*
         * Custom AI that will cause the npc to 'tackle' a specific point given. (uses ai[0, 1, 2])
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * point : the central point of which to 'gravitate'.
         * moveInterval : the amount to move by per tick.
         * maxSpeed : the maximum speed of the npc.
         * direct : If true npc's velocity is set so it moves in a straight line. If false, moves similarly to Flier AI.
         * tackleDelay : the amount of time between tackles in ticks.
         */
        public static void AITackle(NPC npc, ref float[] ai, Vector2 point, float moveInterval = 0.11f, float maxSpeed = 3f, bool direct = false, int tackleDelay = 50, float drift = 0.95f)
        {
            Vector2 destVec = new(ai[0], ai[1]);
            if (destVec != default && Vector2.Distance(npc.Center, destVec) <= Math.Max(5f, (npc.width + npc.height) / 2f * 0.45f))
            {
                ai[0] = 0f; ai[1] = 0f; destVec = default;
            }
            //if the destination vec is default (0, 0), get the current point.
            if (destVec == default)
            {
                npc.velocity *= drift;
                ai[2]--;
                if (ai[2] <= 0)
                {
                    ai[2] = tackleDelay;
                    destVec = point;
                    ai[0] = destVec.X; ai[1] = destVec.Y;
                }
                if (Main.netMode == NetmodeID.Server) { npc.netUpdate = true; }
            }
            else //otherwise move to the point.
            {
                npc.velocity = AIVelocityLinear(npc, destVec, moveInterval, maxSpeed, direct);
            }
        }

        public static Random GetSyncedRand(NPC npc)
        {
            return new(npc.whoAmI);
        }

        /*
         * Custom AI that will cause the npc to 'gravitate' near a specific point given. (uses ai[0, 1, 2])
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * rand : a Random which should be syncronized on both sides of an npc.
         * point : the central point of which to 'gravitate'.
         * moveInterval : the amount to move by per tick.
         * maxSpeed : the maximum speed of the npc.
         * canCrossCenter : If true, npc can cross the central point. If false it will never directly cross the central point.
         * direct : If true npc's velocity is set so it moves in a straight line. If false, moves similarly to Flier AI.
         * minDistance/maxDistance : the minimum and maximum distance the calculated points have to be from the central point, respectively.
         */
        public static void AIGravitate(NPC npc, ref float[] ai, UnifiedRandom rand, Vector2 point, float moveInterval = 0.06f, float maxSpeed = 2f, bool canCrossCenter = true, bool direct = false, int minDistance = 50, int maxDistance = 200)
        {
            Vector2 destVec = new(ai[0], ai[1]);
            bool idleTooLong = false;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //used to prevent the npc from getting 'stuck' trying to reach a point
                if (!idleTooLong && destVec != default && Vector2.Distance(npc.Center, destVec) <= Math.Max(12f, (npc.width + npc.height) / 2f * 3f * (moveInterval / 0.06f)))
                {
                    ai[2]++;
                    if (ai[2] > 100) { ai[2] = 0; idleTooLong = true; }
                }
                //if the destination vec is not null and the npc is close to the point (or has been idle too long), set it to default.
                if (idleTooLong || destVec != default && Vector2.Distance(npc.Center, destVec) <= Math.Max(5f, (npc.width + npc.height) / 2f * 0.75f))
                {
                    ai[0] = 0f; ai[1] = 0f; destVec = default;
                }
            }
            //if the destination vec is default (0, 0)...
            if (destVec == default)
            {
                if (npc.velocity.X > 0.3f || npc.velocity.Y > 0.3f) { npc.velocity.X *= 0.95f; }
                if (canCrossCenter)
                {
                    destVec = BaseUtility.GetRandomPosNear(point, rand, minDistance, maxDistance);
                }
                else
                {
                    int distance = maxDistance - minDistance;
                    Vector2 topLeft = new(point.X - (minDistance + rand.Next(distance)), point.Y - (minDistance + rand.Next(distance)));
                    Vector2 topRight = new(point.X + (minDistance + rand.Next(distance)), topLeft.Y);
                    Vector2 bottomLeft = new(topLeft.X, point.Y + (minDistance + rand.Next(distance)));
                    Vector2 bottomRight = new(topRight.X, bottomLeft.Y);
                    float tempDist = 9999999f;
                    Vector2 closestPoint = default;
                    for (int m = 0; m < 4; m++)
                    {
                        Vector2 corner = m == 0 ? topLeft : m == 1 ? topRight : m == 2 ? bottomLeft : bottomRight;
                        if (Vector2.Distance(npc.Center, corner) < tempDist)
                        {
                            tempDist = Vector2.Distance(npc.Center, corner);
                            closestPoint = corner;
                        }
                    }
                    if (closestPoint == topLeft || closestPoint == bottomRight) { destVec = rand.NextBool(2) ? topRight : bottomLeft; }
                    else
                    if (closestPoint == topRight || closestPoint == bottomLeft) { destVec = rand.NextBool(2) ? topLeft : bottomRight; }
                }
                ai[0] = destVec.X; ai[1] = destVec.Y;
                if (Main.netMode == NetmodeID.Server) { npc.netUpdate = true; }
            }
            else
            if (destVec != default) //otherwise move towards the point.
            {
                npc.velocity = AIVelocityLinear(npc, destVec, moveInterval, maxSpeed, direct);
            }
        }


        public static Vector2 AIVelocityLinear(Entity codable, Vector2 destVec, float moveInterval, float maxSpeed, bool direct = false)
        {
            Vector2 returnVelocity = codable.velocity;
            bool tileCollide = codable is NPC nPC ? !nPC.noTileCollide : codable is Projectile projectile && projectile.tileCollide;
            if (direct)
            {
                Vector2 rotVec = BaseUtility.RotateVector(codable.Center, codable.Center + new Vector2(maxSpeed, 0f), BaseUtility.RotationTo(codable.Center, destVec));
                returnVelocity = rotVec - codable.Center;
            }
            else
            {
                if (codable.Center.X > destVec.X) { returnVelocity.X = Math.Max(-maxSpeed, returnVelocity.X - moveInterval); } else if (codable.Center.X < destVec.X) { returnVelocity.X = Math.Min(maxSpeed, returnVelocity.X + moveInterval); }
                if (codable.Center.Y > destVec.Y) { returnVelocity.Y = Math.Max(-maxSpeed, returnVelocity.Y - moveInterval); } else if (codable.Center.Y < destVec.Y) { returnVelocity.Y = Math.Min(maxSpeed, returnVelocity.Y + moveInterval); }
            }
            if (tileCollide)
            {
                returnVelocity = Collision.TileCollision(codable.position, returnVelocity, codable.width, codable.height);
            }
            return returnVelocity;
        }

        #endregion

        #region Vanilla Projectile AI Copy Methods
        /*-----------------------------------------
         * 
         * These are methods of vanilla projectile AIs
         * cleaned up. If a method has Entity instead
         * of Projectile as it's first argument, it
         * means npcs can use the method too.
         * 
         * ----------------------------------------
         */

        public static void AILightningBolt(Projectile projectile, float changeAngleAt = 40)
        {
            int projFrameCounter = projectile.frameCounter;
            projectile.frameCounter = projFrameCounter + 1;
            if (projectile.velocity == Vector2.Zero)
            {
                if (projectile.frameCounter >= projectile.extraUpdates * 2)
                {
                    projectile.frameCounter = 0;
                    bool shouldKillProjectile = true;
                    for (int m = 1; m < projectile.oldPos.Length; m = projFrameCounter + 1)
                    {
                        if (projectile.oldPos[m] != projectile.oldPos[0])
                        {
                            shouldKillProjectile = false;
                        }
                        projFrameCounter = m;
                    }
                    if (shouldKillProjectile)
                    {
                        projectile.Kill();
                    }
                }
                /*if (Main.rand.Next(projectile.extraUpdates) == 0)
				{
					for (int m2 = 0; m2 < 2; m2 = projFrameCounter + 1)
					{
						float newRot = projectile.rotation + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
						float rotMultiplier = (float)Main.rand.NextDouble() * 0.8f + 1f;
						Vector2 dustVel = new Vector2((float)Math.Cos((double)newRot) * rotMultiplier, (float)Math.Sin((double)newRot) * rotMultiplier);
						int dustID = Dust.NewDust(projectile.Center, 0, 0, 226, dustVel.X, dustVel.Y, 0, default(Color), 1f);
						Main.dust[dustID].noGravity = true;
						Main.dust[dustID].scale = 1.2f;
						projFrameCounter = m2;
					}
					if (Main.rand.Next(5) == 0)
					{
						Vector2 dustPos = projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width;
						int dustID = Dust.NewDust(projectile.Center + dustPos - Vector2.One * 4f, 8, 8, 31, 0f, 0f, 100, default(Color), 1.5f);
						Dust dust = Main.dust[dustID];
						dust.velocity *= 0.5f;
						Main.dust[dustID].velocity.Y = -Math.Abs(Main.dust[dustID].velocity.Y);
						return;
					}
				}*/
            }
            else if (projectile.frameCounter >= projectile.extraUpdates * 2)
            {
                projectile.frameCounter = 0;
                float velSpeed = projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new((int)projectile.ai[1]);
                int newFrameCounter = 0;
                Vector2 projVelocity = -Vector2.UnitY;
                Vector2 angleVector;
                do
                {
                    int percentile = unifiedRandom.Next();
                    projectile.ai[1] = percentile;
                    percentile %= 100;
                    float f = percentile / 100f * 6.28318548f;
                    angleVector = f.ToRotationVector2();
                    if (angleVector.Y > 0f)
                    {
                        angleVector.Y *= -1f;
                    }
                    bool moreFrames = false;
                    if (angleVector.Y > -0.02f)
                    {
                        moreFrames = true;
                    }
                    if (angleVector.X * (projectile.extraUpdates + 1) * 2f * velSpeed + projectile.localAI[0] > changeAngleAt)
                    {
                        moreFrames = true;
                    }
                    if (angleVector.X * (projectile.extraUpdates + 1) * 2f * velSpeed + projectile.localAI[0] < -changeAngleAt)
                    {
                        moreFrames = true;
                    }
                    if (!moreFrames)
                    {
                        goto IL_2608D;
                    }
                    projFrameCounter = newFrameCounter;
                    newFrameCounter = projFrameCounter + 1;
                }
                while (projFrameCounter < 100);
                projectile.velocity = Vector2.Zero;
                projectile.localAI[1] = 1f;
                goto IL_26099;
            IL_2608D:
                projVelocity = angleVector;
            IL_26099:
                if (projectile.velocity != Vector2.Zero)
                {
                    projectile.localAI[0] += projVelocity.X * (projectile.extraUpdates + 1) * 2f * velSpeed;
                    projectile.velocity = projVelocity.RotatedBy(projectile.ai[0] + 1.57079637f) * velSpeed;
                    projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
                }
            }
        }

        public static void AIProjWorm(Projectile p, int[] wormTypes, int wormLength, float velScalar = 1f, float velScalarIdle = 1f, float velocityMax = 30f, float velocityMaxIdle = 15f)
        {
            int[] wtypes = new int[wormTypes.Length == 1 ? 1 : wormLength];
            wtypes[0] = wormTypes[0];
            if (wormTypes.Length > 1)
            {
                wtypes[^1] = wormTypes[2];
                for (int m = 1; m < wtypes.Length - 1; m++)
                {
                    wtypes[m] = wormTypes[1];
                }
            }
            int dummyNPC = -1;
            AIProjWorm(p, ref dummyNPC, wtypes, velScalar, velScalarIdle, velocityMax, velocityMaxIdle);
        }

        public static void AIProjWorm(Projectile p, ref int npcTargetToAttack, int[] wormTypes, float velScalar = 1f, float velScalarIdle = 1f, float velocityMax = 30f, float velocityMaxIdle = 15f)
        {
            Player plrOwner = Main.player[p.owner];
            if ((int)Main.time % 120 == 0) p.netUpdate = true;
            if (!plrOwner.active) { p.active = false; return; }
            bool isHead = p.type == wormTypes[0];
            bool isWorm = BaseUtility.InArray(wormTypes, p.type);
            bool isTail = p.type == wormTypes[^1];
            int wormWidthHeight = 10;
            if (isWorm)
            {
                p.timeLeft = 2;
                wormWidthHeight = 30;
            }
            if (isHead)
            {
                Vector2 plrCenter = plrOwner.Center;
                float minAttackDist = 700f;
                float returnDist = 1000f;
                float teleportDist = 2000f;
                int target = -1;
                if (p.Distance(plrCenter) > teleportDist)
                {
                    p.Center = plrCenter;
                    p.netUpdate = true;
                }
                bool flag66 = true;
                if (flag66)
                {
                    NPC ownerMinionAttackTargetNPC5 = p.OwnerMinionAttackTargetNPC;
                    if (ownerMinionAttackTargetNPC5 != null && ownerMinionAttackTargetNPC5.CanBeChasedBy(p))
                    {
                        float ownerTargetDist = p.Distance(ownerMinionAttackTargetNPC5.Center);
                        if (ownerTargetDist < minAttackDist * 2f)
                        {
                            target = ownerMinionAttackTargetNPC5.whoAmI;
                            /*if (ownerMinionAttackTargetNPC5.boss)
							{
								int whoAmI = ownerMinionAttackTargetNPC5.whoAmI;
							}
							else
							{
								int whoAmI2 = ownerMinionAttackTargetNPC5.whoAmI;
							}*/
                        }
                    }
                    if (target < 0)
                    {
                        int dummy;
                        for (int m = 0; m < 200; m = dummy + 1)
                        {
                            NPC npcTarget = Main.npc[m];
                            if (npcTarget.CanBeChasedBy(p) && plrOwner.Distance(npcTarget.Center) < returnDist)
                            {
                                float npcTargetDist = p.Distance(npcTarget.Center);
                                if (npcTargetDist < minAttackDist)
                                {
                                    target = m;
                                }
                            }
                            dummy = m;
                        }
                    }
                }
                npcTargetToAttack = target;
                if (target != -1)
                {
                    NPC npcTarget2 = Main.npc[target];
                    Vector2 npcDist = npcTarget2.Center - p.Center;
                    (npcDist.X > 0f).ToDirectionInt();
                    (npcDist.Y > 0f).ToDirectionInt();
                    float velocityScalar = 0.4f;
                    if (npcDist.Length() < 600f) velocityScalar = 0.6f;
                    if (npcDist.Length() < 300f) velocityScalar = 0.8f;
                    velocityScalar *= velScalar;
                    if (npcDist.Length() > npcTarget2.Size.Length() * 0.75f)
                    {
                        p.velocity += Vector2.Normalize(npcDist) * velocityScalar * 1.5f;
                        if (Vector2.Dot(p.velocity, npcDist) < 0.25f)
                        {
                            p.velocity *= 0.8f;
                        }
                    }
                    if (p.velocity.Length() > velocityMax)
                    {
                        p.velocity = Vector2.Normalize(p.velocity) * velocityMax;
                    }
                }
                else
                {
                    float velocityScalarIdle = 0.2f;
                    Vector2 newPointDist = plrCenter - p.Center;
                    if (newPointDist.Length() < 200f)
                    {
                        velocityScalarIdle = 0.12f;
                    }
                    if (newPointDist.Length() < 140f)
                    {
                        velocityScalarIdle = 0.06f;
                    }
                    velocityScalarIdle *= velScalarIdle;
                    if (newPointDist.Length() > 100f)
                    {
                        if (Math.Abs(plrCenter.X - p.Center.X) > 20f)
                        {
                            p.velocity.X += velocityScalarIdle * Math.Sign(plrCenter.X - p.Center.X);
                        }
                        if (Math.Abs(plrCenter.Y - p.Center.Y) > 10f)
                        {
                            p.velocity.Y += velocityScalarIdle * Math.Sign(plrCenter.Y - p.Center.Y);
                        }
                    }
                    else if (p.velocity.Length() > 2f)
                    {
                        p.velocity *= 0.96f;
                    }
                    if (Math.Abs(p.velocity.Y) < 1f)
                    {
                        p.velocity.Y -= 0.1f;
                    }
                    if (p.velocity.Length() > velocityMaxIdle)
                    {
                        p.velocity = Vector2.Normalize(p.velocity) * velocityMaxIdle;
                    }
                }
                p.rotation = p.velocity.ToRotation() + 1.57079637f;
                int direction = p.direction;
                p.direction = p.spriteDirection = p.velocity.X > 0f ? 1 : -1;
                if (direction != p.direction)
                {
                    p.netUpdate = true;
                }
                float scaleChange = MathHelper.Clamp(p.localAI[0], 0f, 50f);
                p.position = p.Center;
                p.scale = 1f + scaleChange * 0.01f;
                p.width = p.height = (int)(wormWidthHeight * p.scale);
                p.Center = p.position;
                if (p.alpha > 0)
                {
                    p.alpha -= 42;
                    if (p.alpha < 0)
                    {
                        p.alpha = 0;
                    }
                }
            }
            else
            {
                bool npcInFront = false;
                Vector2 projCenter = Vector2.Zero;
                float projRot = 0f;
                float tileScalar = 0f;
                float projectileScale = 1f;
                if (p.ai[1] == 1f)
                {
                    p.ai[1] = 0f;
                    p.netUpdate = true;
                }
                int byUuid = Projectile.GetByUUID(p.owner, (int)p.ai[0]);
                if (isWorm && byUuid >= 0 && Main.projectile[byUuid].active && !isTail)
                {
                    npcInFront = true;
                    projCenter = Main.projectile[byUuid].Center;
                    projRot = Main.projectile[byUuid].rotation;
                    float projScale = MathHelper.Clamp(Main.projectile[byUuid].scale, 0f, 50f);
                    projectileScale = projScale;
                    tileScalar = 16f;
                    Main.projectile[byUuid].localAI[0] = p.localAI[0] + 1f;
                    if (Main.projectile[byUuid].type != wormTypes[0])
                    {
                        Main.projectile[byUuid].localAI[1] = p.whoAmI;
                    }
                    if (p.owner == Main.myPlayer && Main.projectile[byUuid].type == wormTypes[0] && p.type == wormTypes[^1])
                    {
                        Main.projectile[byUuid].Kill();
                        p.Kill();
                        return;
                    }
                }
                if (!npcInFront)
                {
                    return;
                }
                p.alpha -= 42;
                if (p.alpha < 0)
                {
                    p.alpha = 0;
                }
                p.velocity = Vector2.Zero;
                Vector2 centerDist = projCenter - p.Center;
                if (projRot != p.rotation)
                {
                    float rotDist = MathHelper.WrapAngle(projRot - p.rotation);
                    centerDist = centerDist.RotatedBy(rotDist * 0.1f);
                }
                p.rotation = centerDist.ToRotation() + 1.57079637f;
                p.position = p.Center;
                p.scale = projectileScale;
                p.width = p.height = (int)(wormWidthHeight * p.scale);
                p.Center = p.position;
                if (centerDist != Vector2.Zero)
                {
                    p.Center = projCenter - Vector2.Normalize(centerDist) * tileScalar * projectileScale;
                }
                p.spriteDirection = centerDist.X > 0f ? 1 : -1;
            }
        }

        public static void AIProjSpaceOctopus(Projectile p, ref float[] ai, int parentNPCType, int fireProjType = -1, float shootVelocity = 16f, float hoverTime = 210f, float xMult = 0.15f, float yMult = 0.075f, Action<int, Projectile> spawnDust = null)
        {
            AIProjSpaceOctopus(p, ref ai, parentNPCType, fireProjType, shootVelocity, hoverTime, xMult, yMult, -1, true, false, spawnDust);
        }

        public static void AIProjSpaceOctopus(Projectile projectile, ref float[] ai, int parentNPCType, int fireProjType = -1, float shootVelocity = 16f, float hoverTime = 210f, float xMult = 0.15f, float yMult = 0.075f, int fireDmg = -1, bool useParentTarget = true, bool noParentHover = false, Action<int, Projectile> spawnDust = null)
        {
            if (fireDmg == -1) fireDmg = projectile.damage;

            ai[0] += 1f;
            if (ai[0] < hoverTime)
            {
                bool parentAlive = true;
                int parentID = (int)ai[1];
                if (Main.npc[parentID].active && Main.npc[parentID].type == parentNPCType)
                {
                    if (!noParentHover && Main.npc[parentID].oldPos[1] != Vector2.Zero)
                    {
                        projectile.position += Main.npc[parentID].position - Main.npc[parentID].oldPos[1];
                    }
                }
                else
                {
                    ai[0] = hoverTime;
                    parentAlive = false;
                }
                if (parentAlive && !noParentHover)
                {
                    projectile.velocity += new Vector2(Math.Sign(Main.npc[parentID].Center.X - projectile.Center.X), Math.Sign(Main.npc[parentID].Center.Y - projectile.Center.Y)) * new Vector2(xMult, yMult);
                    if (projectile.velocity.Length() > 6f)
                    {
                        projectile.velocity *= 6f / projectile.velocity.Length();
                    }
                }
                spawnDust?.Invoke(0, projectile);
                projectile.rotation = projectile.velocity.X * 0.1f;
            }
            if (ai[0] == hoverTime)
            {
                bool hasParentTarget = true;
                int parentTarget = -1;
                if (!useParentTarget)
                {
                    int parentID2 = (int)ai[1];
                    if (Main.npc[parentID2].active && Main.npc[parentID2].type == parentNPCType)
                    {
                        parentTarget = Main.npc[parentID2].target;
                    }
                    else
                    {
                        hasParentTarget = false;
                    }
                }
                else
                {
                    hasParentTarget = false;
                }
                if (!hasParentTarget)
                {
                    parentTarget = Player.FindClosest(projectile.position, projectile.width, projectile.height);
                }
                Vector2 distanceVec = Main.player[parentTarget].Center - projectile.Center;
                distanceVec.X += Main.rand.Next(-50, 51);
                distanceVec.Y += Main.rand.Next(-50, 51);
                distanceVec.X *= Main.rand.Next(80, 121) * 0.01f;
                distanceVec.Y *= Main.rand.Next(80, 121) * 0.01f;
                Vector2 distVecNormal = Vector2.Normalize(distanceVec);
                if (distVecNormal.HasNaNs())
                {
                    distVecNormal = Vector2.UnitY;
                }
                if (fireProjType == -1)
                {
                    projectile.velocity = distVecNormal * shootVelocity;
                    projectile.netUpdate = true;
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Collision.CanHitLine(projectile.Center, 0, 0, Main.player[parentTarget].Center, 0, 0))
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center.X, projectile.Center.Y, distVecNormal.X * shootVelocity, distVecNormal.Y * shootVelocity, fireProjType, fireDmg, 1f, Main.myPlayer);
                    }
                    ai[0] = 0f;
                }
            }
            if (ai[0] >= hoverTime)
            {
                projectile.rotation = projectile.rotation.AngleLerp(projectile.velocity.ToRotation() + 1.57079637f, 0.4f);
                spawnDust?.Invoke(1, projectile);
            }
        }

        public static void AIYoyo(Projectile p, ref float[] ai, ref float[] localAI, float yoyoTimeMax = -1, float maxRange = -1, float topSpeed = -1, bool dontChannel = false, float rotAmount = 0.45f)
        {
            if (yoyoTimeMax == -1) yoyoTimeMax = ProjectileID.Sets.YoyosLifeTimeMultiplier[p.type];
            if (maxRange == -1) maxRange = ProjectileID.Sets.YoyosMaximumRange[p.type];
            if (topSpeed == -1) topSpeed = ProjectileID.Sets.YoyosTopSpeed[p.type];
            AIYoyo(p, ref ai, ref localAI, Main.player[p.owner], Main.player[p.owner].channel, default, yoyoTimeMax, maxRange, topSpeed, dontChannel, rotAmount);
        }

        public static void AIYoyo(Projectile p, ref float[] ai, ref float[] localAI, Entity owner, bool isChanneling, Vector2 targetPos = default, float yoyoTimeMax = 120, float maxRange = 150, float topSpeed = 8f, bool dontChannel = false, float rotAmount = 0.45f)
        {
            bool playerYoyo = owner is Player;
            Player powner = playerYoyo ? (Player)owner : null;
            float meleeSpeed = playerYoyo ? powner.GetAttackSpeed(DamageClass.Melee) : 1f;
            Vector2 targetP = targetPos;
            if (playerYoyo && Main.myPlayer == p.owner && targetPos == default) targetP = Main.ReverseGravitySupport(Main.MouseScreen) + Main.screenPosition;

            bool yoyoFound = false;
            if (owner is Player)
            {
                for (int i = 0; i < p.whoAmI; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == p.owner && Main.projectile[i].type == p.type) yoyoFound = true;
                }
            }
            if (playerYoyo && p.owner == Main.myPlayer || !playerYoyo && Main.netMode != NetmodeID.MultiplayerClient)
            {
                localAI[0] += 1f;
                if (yoyoFound) localAI[0] += Main.rand.Next(10, 31) * 0.1f;
                float yoyoTimeLeft = localAI[0] / 60f;
                yoyoTimeLeft /= (1f + meleeSpeed) / 2f;
                if (yoyoTimeMax != -1f && yoyoTimeLeft > yoyoTimeMax) ai[0] = -1f;
            }
            if (playerYoyo && powner.dead || !playerYoyo && !owner.active) { p.Kill(); return; }
            if (playerYoyo && !dontChannel && !yoyoFound)
            {
                powner.heldProj = p.whoAmI;
                powner.itemAnimation = 2;
                powner.itemTime = 2;
                if (p.position.X + p.width / 2 > powner.position.X + powner.width / 2)
                {
                    powner.ChangeDir(1);
                    p.direction = 1;
                }
                else
                {
                    powner.ChangeDir(-1);
                    p.direction = -1;
                }
            }
            if (p.velocity.HasNaNs()) p.Kill();
            p.timeLeft = 6;
            float pMaxRange = maxRange;
            float pTopSpeed = topSpeed;
            if (playerYoyo && powner.yoyoString)
            {
                pMaxRange = pMaxRange * 1.25f + 30f;
            }
            pMaxRange /= (1f + meleeSpeed * 3f) / 4f;
            pTopSpeed /= (1f + meleeSpeed * 3f) / 4f;
            float topSpeedX = 14f - pTopSpeed / 2f;
            float topSpeedY = 5f + pTopSpeed / 2f;
            if (yoyoFound)
            {
                topSpeedY += 20f;
            }
            if (ai[0] >= 0f)
            {
                if (p.velocity.Length() > pTopSpeed)
                {
                    p.velocity *= 0.98f;
                }
                bool yoyoTooFar = false;
                bool yoyoWayTooFar = false;
                Vector2 centerDist = owner.Center - p.Center;
                if (centerDist.Length() > pMaxRange)
                {
                    yoyoTooFar = true;
                    if (centerDist.Length() > pMaxRange * 1.3)
                    {
                        yoyoWayTooFar = true;
                    }
                }
                if (playerYoyo && p.owner == Main.myPlayer || !playerYoyo && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (playerYoyo && (!isChanneling || powner.stoned || powner.frozen) || !playerYoyo && !isChanneling)
                    {
                        ai[0] = -1f;
                        ai[1] = 0f;
                        p.netUpdate = true;
                    }
                    else
                    {
                        Vector2 mousePos = targetP;
                        float x = mousePos.X;
                        float y = mousePos.Y;
                        Vector2 mouseDist = new Vector2(x, y) - owner.Center;
                        if (mouseDist.Length() > pMaxRange)
                        {
                            mouseDist.Normalize();
                            mouseDist *= pMaxRange;
                            mouseDist = owner.Center + mouseDist;
                            x = mouseDist.X;
                            y = mouseDist.Y;
                        }
                        if (ai[0] != x || ai[1] != y)
                        {
                            Vector2 coord = new(x, y);
                            Vector2 coordDist = coord - owner.Center;
                            if (coordDist.Length() > pMaxRange - 1f)
                            {
                                coordDist.Normalize();
                                coordDist *= pMaxRange - 1f;
                                coord = owner.Center + coordDist;
                                x = coord.X;
                                y = coord.Y;
                            }
                            ai[0] = x;
                            ai[1] = y;
                            p.netUpdate = true;
                        }
                    }
                }
                if (yoyoWayTooFar && p.owner == Main.myPlayer)
                {
                    ai[0] = -1f;
                    p.netUpdate = true;
                }
                if (ai[0] >= 0f)
                {
                    if (yoyoTooFar)
                    {
                        topSpeedX /= 2f;
                        pTopSpeed *= 2f;
                        if (p.Center.X > owner.Center.X && p.velocity.X > 0f) p.velocity.X *= 0.5f;
                        if (p.Center.Y > owner.Center.Y && p.velocity.Y > 0f) p.velocity.Y *= 0.5f;
                        if (p.Center.X < owner.Center.X && p.velocity.X > 0f) p.velocity.X *= 0.5f;
                        if (p.Center.Y < owner.Center.Y && p.velocity.Y > 0f) p.velocity.Y *= 0.5f;
                    }
                    Vector2 coord = new(ai[0], ai[1]);
                    Vector2 coordDist = coord - p.Center;
                    p.velocity.Length();
                    float coordLength = coordDist.Length();
                    if (coordLength > topSpeedY)
                    {
                        coordDist.Normalize();
                        float scaleFactor = coordLength > pTopSpeed * 2f ? pTopSpeed : coordLength / 2f;
                        coordDist *= scaleFactor;
                        p.velocity = (p.velocity * (topSpeedX - 1f) + coordDist) / topSpeedX;
                    }
                    else if (yoyoFound)
                    {
                        if (p.velocity.Length() < pTopSpeed * 0.6)
                        {
                            coordDist = p.velocity;
                            coordDist.Normalize();
                            coordDist *= pTopSpeed * 0.6f;
                            p.velocity = (p.velocity * (topSpeedX - 1f) + coordDist) / topSpeedX;
                        }
                    }
                    else
                    {
                        p.velocity *= 0.8f;
                    }
                    if (yoyoFound && !yoyoTooFar && p.velocity.Length() < pTopSpeed * 0.6)
                    {
                        p.velocity.Normalize();
                        p.velocity *= pTopSpeed * 0.6f;
                    }
                }
            }
            else
            {
                topSpeedX = (int)(topSpeedX * 0.8);
                pTopSpeed *= 1.5f;
                p.tileCollide = false;
                Vector2 posDist = owner.position - p.Center;
                float posLength = posDist.Length();
                if (posLength < pTopSpeed + 10f || posLength == 0f)
                {
                    p.Kill();
                }
                else
                {
                    posDist.Normalize();
                    posDist *= pTopSpeed;
                    p.velocity = (p.velocity * (topSpeedX - 1f) + posDist) / topSpeedX;
                }
            }
            p.rotation += rotAmount;
        }

        /*
		public static void AICounterweight(Projectile p, ref float[] ai)
		{
			p.timeLeft = 6;
			bool flag = true;
			float num = 250f;
			float scaleFactor = 0.1f;
			float num2 = 15f;
			float num3 = 12f;
			num *= 0.5f;
			num2 *= 0.8f;
			num3 *= 1.5f;
			if (p.owner == Main.myPlayer)
			{
				bool flag2 = false;
				for (int i = 0; i < 1000; i++)
				{
					if (Main.projectile[i].active && Main.projectile[i].owner == p.owner && Main.projectile[i].aiStyle == 99 && (Main.projectile[i].type < 556 || Main.projectile[i].type > 561))
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					p.ai[0] = -1f;
					p.netUpdate = true;
				}
			}
			if (Main.player[p.owner].yoyoString)
			{
				num += num * 0.25f + 10f;
			}
			p.rotation += 0.5f;
			if (Main.player[p.owner].dead)
			{
				p.Kill();
				return;
			}
			if (!flag)
			{
				Main.player[p.owner].heldProj = p.whoAmI;
				Main.player[p.owner].itemAnimation = 2;
				Main.player[p.owner].itemTime = 2;
				if (p.position.X + (float)(p.width / 2) > Main.player[p.owner].position.X + (float)(Main.player[p.owner].width / 2))
				{
					Main.player[p.owner].ChangeDir(1);
					p.direction = 1;
				}
				else
				{
					Main.player[p.owner].ChangeDir(-1);
					p.direction = -1;
				}
			}
			if (p.ai[0] == 0f || p.ai[0] == 1f)
			{
				if (p.ai[0] == 1f)
				{
					num *= 0.75f;
				}
				num3 *= 0.5f;
				bool flag3 = false;
				Vector2 vector = Main.player[p.owner].Center - base.Center;
				if ((double)vector.Length() > (double)num * 0.9)
				{
					flag3 = true;
				}
				if (vector.Length() > num)
				{
					float num4 = vector.Length() - num;
					Vector2 value;
					value.X = vector.Y;
					value.Y = vector.X;
					vector.Normalize();
					vector *= num;
					p.position = Main.player[p.owner].Center - vector;
					p.position.X = p.position.X - (float)(p.width / 2);
					p.position.Y = p.position.Y - (float)(p.height / 2);
					float num5 = p.velocity.Length();
					p.velocity.Normalize();
					if (num4 > num5 - 1f)
					{
						num4 = num5 - 1f;
					}
					p.velocity *= num5 - num4;
					num5 = p.velocity.Length();
					Vector2 vector2 = new Vector2(base.Center.X, base.Center.Y);
					Vector2 vector3 = new Vector2(Main.player[p.owner].Center.X, Main.player[p.owner].Center.Y);
					if (vector2.Y < vector3.Y)
					{
						value.Y = Math.Abs(value.Y);
					}
					else if (vector2.Y > vector3.Y)
					{
						value.Y = -Math.Abs(value.Y);
					}
					if (vector2.X < vector3.X)
					{
						value.X = Math.Abs(value.X);
					}
					else if (vector2.X > vector3.X)
					{
						value.X = -Math.Abs(value.X);
					}
					value.Normalize();
					value *= p.velocity.Length();
					new Vector2(value.X, value.Y);
					if (Math.Abs(p.velocity.X) > Math.Abs(p.velocity.Y))
					{
						Vector2 vector4 = p.velocity;
						vector4.Y += value.Y;
						vector4.Normalize();
						vector4 *= p.velocity.Length();
						if ((double)Math.Abs(value.X) < 0.1 || (double)Math.Abs(value.Y) < 0.1)
						{
							p.velocity = vector4;
						}
						else
						{
							p.velocity = (vector4 + p.velocity * 2f) / 3f;
						}
					}
					else
					{
						Vector2 vector5 = p.velocity;
						vector5.X += value.X;
						vector5.Normalize();
						vector5 *= p.velocity.Length();
						if ((double)Math.Abs(value.X) < 0.2 || (double)Math.Abs(value.Y) < 0.2)
						{
							p.velocity = vector5;
						}
						else
						{
							p.velocity = (vector5 + p.velocity * 2f) / 3f;
						}
					}
				}
				if (Main.myPlayer == p.owner)
				{
					if (Main.player[p.owner].channel)
					{
						Vector2 value2 = new Vector2((float)(Main.mouseX - Main.lastMouseX), (float)(Main.mouseY - Main.lastMouseY));
						if (p.velocity.X != 0f || p.velocity.Y != 0f)
						{
							if (flag)
							{
								value2 *= -1f;
							}
							if (flag3)
							{
								if (base.Center.X < Main.player[p.owner].Center.X && value2.X < 0f)
								{
									value2.X = 0f;
								}
								if (base.Center.X > Main.player[p.owner].Center.X && value2.X > 0f)
								{
									value2.X = 0f;
								}
								if (base.Center.Y < Main.player[p.owner].Center.Y && value2.Y < 0f)
								{
									value2.Y = 0f;
								}
								if (base.Center.Y > Main.player[p.owner].Center.Y && value2.Y > 0f)
								{
									value2.Y = 0f;
								}
							}
							p.velocity += value2 * scaleFactor;
							p.netUpdate = true;
						}
					}
					else
					{
						p.ai[0] = 10f;
						p.netUpdate = true;
					}
				}
				if (flag || p.type == 562 || p.type == 547 || p.type == 555 || p.type == 564 || p.type == 552 || p.type == 563 || p.type == 549 || p.type == 550 || p.type == 554 || p.type == 553 || p.type == 603)
				{
					float num6 = 800f;
					Vector2 vector6 = default(Vector2);
					bool flag4 = false;
					if (p.type == 549)
					{
						num6 = 200f;
					}
					if (p.type == 554)
					{
						num6 = 400f;
					}
					if (p.type == 553)
					{
						num6 = 250f;
					}
					if (p.type == 603)
					{
						num6 = 320f;
					}
					for (int j = 0; j < 200; j++)
					{
						if (Main.npc[j].CanBeChasedBy(p, false))
						{
							float num7 = Main.npc[j].position.X + (float)(Main.npc[j].width / 2);
							float num8 = Main.npc[j].position.Y + (float)(Main.npc[j].height / 2);
							float num9 = Math.Abs(p.position.X + (float)(p.width / 2) - num7) + Math.Abs(p.position.Y + (float)(p.height / 2) - num8);
							if (num9 < num6 && (p.type != 563 || num9 >= 200f) && Collision.CanHit(p.position, p.width, p.height, Main.npc[j].position, Main.npc[j].width, Main.npc[j].height) && (double)(Main.npc[j].Center - Main.player[p.owner].Center).Length() < (double)num * 0.9)
							{
								num6 = num9;
								vector6.X = num7;
								vector6.Y = num8;
								flag4 = true;
							}
						}
					}
					if (flag4)
					{
						vector6 -= base.Center;
						vector6.Normalize();
						if (p.type == 563)
						{
							vector6 *= 4f;
							p.velocity = (p.velocity * 14f + vector6) / 15f;
						}
						else if (p.type == 553)
						{
							vector6 *= 5f;
							p.velocity = (p.velocity * 12f + vector6) / 13f;
						}
						else if (p.type == 603)
						{
							vector6 *= 16f;
							p.velocity = (p.velocity * 9f + vector6) / 10f;
						}
						else if (p.type == 554)
						{
							vector6 *= 8f;
							p.velocity = (p.velocity * 6f + vector6) / 7f;
						}
						else
						{
							vector6 *= 6f;
							p.velocity = (p.velocity * 7f + vector6) / 8f;
						}
					}
				}
				if (p.velocity.Length() > num2)
				{
					p.velocity.Normalize();
					p.velocity *= num2;
				}
				if (p.velocity.Length() < num3)
				{
					p.velocity.Normalize();
					p.velocity *= num3;
					return;
				}
			}
			else
			{
				p.tileCollide = false;
				Vector2 vector7 = Main.player[p.owner].Center - base.Center;
				if (vector7.Length() < 40f || vector7.HasNaNs())
				{
					p.Kill();
					return;
				}
				float num10 = num2 * 1.5f;
				if (p.type == 546)
				{
					num10 *= 1.5f;
				}
				if (p.type == 554)
				{
					num10 *= 1.25f;
				}
				if (p.type == 555)
				{
					num10 *= 1.35f;
				}
				if (p.type == 562)
				{
					num10 *= 1.25f;
				}
				float num11 = 12f;
				vector7.Normalize();
				vector7 *= num10;
				p.velocity = (p.velocity * (num11 - 1f) + vector7) / num11;
			}	
		}
	
		*/

        public static void TileCollideYoyo(Projectile p, ref Vector2 velocity, Vector2 newVelocity)
        {
            bool normalizeVelocity = false;
            if (velocity.X != newVelocity.X)
            {
                normalizeVelocity = true;
                velocity.X = newVelocity.X * -1f;
            }
            if (velocity.Y != newVelocity.Y)
            {
                normalizeVelocity = true;
                velocity.Y = newVelocity.Y * -1f;
            }
            if (normalizeVelocity)
            {
                Vector2 centerDist = Main.player[p.owner].Center - p.Center;
                centerDist.Normalize();
                centerDist *= velocity.Length();
                centerDist *= 0.25f;
                velocity *= 0.75f;
                velocity += centerDist;
                if (velocity.Length() > 6f)
                {
                    velocity *= 0.5f;
                }
            }
        }

        public static void EntityCollideYoyo(Projectile p, ref float[] ai, ref float[] localAI, Entity owner, Entity target, bool spawnCounterweight = true, float velMult = 1f)
        {
            if (owner is Player player && spawnCounterweight) { player.Counterweight(target.Center, p.damage, p.knockBack); }
            if (target.Center.X < owner.Center.X) { p.direction = -1; } else { p.direction = 1; }
            if (ai[0] >= 0f)
            {
                Vector2 value2 = p.Center - target.Center;
                value2.Normalize();
                float scaleFactor = 16f;
                p.velocity *= -0.5f;
                p.velocity += value2 * scaleFactor;
                p.velocity *= velMult;
                p.netUpdate = true;
                localAI[0] += 20f;
                if (!Collision.CanHit(p.position, p.width, p.height, owner.position, owner.width, owner.height))
                {
                    localAI[0] += 40f;
                    //num8 = (int)((double)num8 * 0.75);
                }
            }
        }

        /*
		 * A cleaned up (and edited) copy of Star AI. (Starfury stars, etc.) (AIStyle 5)
		 * 
		 * landingHorizon: The Y value at which the star should begin tile colliding. (-1 == always tile collide)
		 * fadein: If true, projectile fades in when first spawned.
		 * delayedCollide: If true, star does not collide with tiles until it's past the horizon of it's target
		 */
        public static void AIStar(Projectile p, ref float[] ai, float landingHorizon = -1, bool fadein = true)
        {
            if (landingHorizon != -1)
            {
                if (p.position.Y > landingHorizon) p.tileCollide = true;
            }
            else
            {
                if (ai[0] == 0f && !Collision.SolidCollision(p.position, p.width, p.height))
                {
                    ai[0] = 1f;
                    p.netUpdate = true;
                }
                if (ai[0] != 0f) p.tileCollide = true;
            }
            if (fadein) p.alpha = Math.Max(0, p.alpha - 25);
        }

        /*
		 * A cleaned up (and edited) copy of Explosives AI. (Grenades, Bombs, etc.)
		 * 
		 * rocket : changes behavior to act like rockets.
		 * rotate : wether to rotate based on velocity or not.
		 * beginGravity : what tick to begin applying gravity. (not applied if rocket == true)
		 * slowdownX : How fast to slow down per tick. (not applied if rocket == true)
		 * gravity: The gravity amount. (not applied if rocket == true)
		 */
        public static void AIExplosive(Projectile p, ref float[] ai, bool rocket = false, bool rotate = true, int beginGravity = 10, float slowdownX = 0.97f, float gravity = 0.2f)
        {
            if (rocket)
            {
                if (Math.Abs(p.velocity.X) < 15f && Math.Abs(p.velocity.Y) < 15f) { p.velocity *= 1.1f; }
            }
            ai[0] += 1f;
            if (rocket)
            {
                if (p.velocity.X < 0f)
                {
                    p.spriteDirection = -1;
                    p.rotation = (float)Math.Atan2(-p.velocity.Y, -p.velocity.X) - 1.57f;
                }
                else
                {
                    p.spriteDirection = 1;
                    p.rotation = (float)Math.Atan2(p.velocity.Y, p.velocity.X) + 1.57f;
                }
            }
            else
            if (ai[0] > beginGravity)
            {
                ai[0] = beginGravity;
                if (p.velocity.Y == 0f && p.velocity.X != 0f)
                {
                    p.velocity.X *= slowdownX;
                    if (p.velocity.X is > -0.01f and < 0.01f)
                    {
                        p.velocity.X = 0f;
                        p.netUpdate = true;
                    }
                }
                p.velocity.Y += gravity;
            }
            if (rotate) { p.rotation += p.velocity.X * 0.1f; }
        }

        /*
		 * A cleaned up (and edited) copy of tile collison for Explosives.
		 * bomb: Set to true if you want bomblike collision.
		 */
        public static void TileCollideExplosive(Projectile p, ref Vector2 velocity, bool bomb = false)
        {
            if (p.velocity.X != velocity.X) { p.velocity.X = velocity.X * -0.4f; }
            if (p.velocity.Y != velocity.Y && velocity.Y > 0.7f && !bomb) { p.velocity.Y = velocity.Y * -0.4f; }
        }

        /*
		 * A cleaned up (and edited) copy of Arrow AI. (Arrows, etc.) (AIStyle 1) (Can be used with NPCs or Projectiles)
		 * 
		 * gravApplyInterval: The rate at which to apply gravity. Higher values == less gravity, greater values == more gravity.
		 * gravity: the amount to induce gravity upon the codable.
		 * maxSpeedY: The maximum speed the projectile can be doing down.
		 */
        public static void AIArrow(Entity codable, ref float[] ai, int gravApplyInterval = 50, float gravity = 0.1f, float maxSpeedY = 16f)
        {
            ai[0]++;
            if (ai[0] >= gravApplyInterval) { codable.velocity.Y += gravity; }
            if (codable.velocity.Y > maxSpeedY) { codable.velocity.Y = maxSpeedY; }
        }

        /*
		 * A cleaned up (and edited) copy of Demon Scythe AI. (Demon Scythe, etc.) (AIStyle 18) (Can be used with NPCs or Projectiles)
		 * 
		 * startSpeedupInterval : The value to begin velocity speedup.
		 * stopSpeedupInterval : The value to stop velocity speedup.
		 * rotateScalar : The scalar for rotational increase.
		 * speedupScalar : The scalar for the speedup interval.
		 * maxSpeed : The speed to cap the projectile/npc at.
		 */
        public static void AIDemonScythe(Entity codable, ref float[] ai, int startSpeedupInterval = 30, int stopSpeedupInterval = 100, float rotateScalar = 0.8f, float speedupScalar = 1.06f, float maxSpeed = 8f)
        {
            if (codable is Projectile projectile) { projectile.rotation += codable.direction * rotateScalar; }
            if (codable is NPC nPC) { nPC.rotation += codable.direction * rotateScalar; }
            ai[0] += 1f;
            if (ai[0] >= startSpeedupInterval)
            {
                if (ai[0] < stopSpeedupInterval) { codable.velocity *= speedupScalar; } else { ai[0] = stopSpeedupInterval; }
            }
            if ((Math.Abs(codable.velocity.X) + Math.Abs(codable.velocity.Y)) * 0.5f > maxSpeed)
            {
                codable.velocity.Normalize(); codable.velocity *= maxSpeed;
            }
        }

        /*
         * A cleaned up (and edited) copy of Vilethorn AI. (Vilethorn, etc.)
         * 
         * alphaInterval : The amount of alpha to add each tick. (higher values == faster spawning)
         * alphaReduction : The amount of alpha to reduce after spawning the next piece. (higher values == faster despawning)
         * length : How many segments to spawn.
         */
        public static void AIVilethorn(Projectile p, int alphaInterval = 50, int alphaReduction = 4, int length = 8)
        {
            if (p.ai[0] == 0f)
            {
                p.rotation = (float)Math.Atan2(p.velocity.Y, p.velocity.X) + 1.57f;
                p.alpha -= alphaInterval;
                if (p.alpha <= 0)
                {
                    p.alpha = 0;
                    p.ai[0] = 1f;
                    if (p.ai[1] == 0f) { p.ai[1] += 1f; p.position += p.velocity; }
                    if (p.ai[1] < length && Main.myPlayer == p.owner)
                    {
                        Vector2 rotVec = p.velocity;
                        int id = Projectile.NewProjectile(p.GetSource_FromAI(), p.Center.X + p.velocity.X, p.Center.Y + p.velocity.Y, rotVec.X, rotVec.Y, p.type, p.damage, p.knockBack, p.owner);
                        Main.projectile[id].damage = p.damage;
                        Main.projectile[id].ai[1] = p.ai[1] + 1f;
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, NetworkText.FromLiteral(""), id);
                        p.position -= p.velocity;
                        return;
                    }
                }
            }
            else
            {
                p.alpha += alphaReduction;
                if (p.alpha >= 255) { p.Kill(); return; }
            }
            p.position -= p.velocity;
        }

        /*
         * A cleaned up (and edited) copy of Stream AI. (Aqua Scepter, Golden Shower, etc.)
         * 
         * scaleReduce: the amount to reduce the scale of the codable each tick.
         * gravity : the amount of gravity to apply each tick.
		 * goldenShower : If true, acts more like golden shower then aqua specter.
		 * start : The value at which the AI begins running (creates a delay).
		 * SpawnDust: If not null, controlls the dust spawning.
         */
        public static void AIStream(Projectile p, float scaleReduce = 0.04f, float gravity = 0.075f, bool goldenShower = false, int start = 3, Func<Projectile, Vector2, int, int, int> spawnDust = null)
        {
            if (goldenShower)
            {
                p.scale -= scaleReduce;
                if (p.scale <= 0f) { p.Kill(); }
                if (p.ai[0] <= start) { p.ai[0] += 1f; return; }
                p.velocity.Y += gravity;
                if (Main.netMode != NetmodeID.Server && spawnDust != null)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        float dustX = p.velocity.X / 3f * m;
                        float dustY = p.velocity.Y / 3f * m;
                        int offset = 1;
                        Vector2 pos = new(p.position.X - offset, p.position.Y - offset);
                        int width = p.width + offset * 2; int height = p.height + offset * 2;
                        int dustID = spawnDust(p, pos, width, height);
                        if (dustID != -1)
                        {
                            Main.dust[dustID].noGravity = true;
                            Main.dust[dustID].velocity *= 0.1f;
                            Main.dust[dustID].velocity += p.velocity * 0.5f;
                            Main.dust[dustID].position.X -= dustX;
                            Main.dust[dustID].position.Y -= dustY;
                        }
                    }
                    if (Main.rand.NextBool(8))
                    {
                        int offset = 1;
                        Vector2 pos = new(p.position.X - offset, p.position.Y - offset);
                        int width = p.width + offset * 2; int height = p.height + offset * 2;
                        int dustID = spawnDust(p, pos, width, height);
                        if (dustID != -1)
                        {
                            Main.dust[dustID].velocity *= 0.25f;
                            Main.dust[dustID].velocity += p.velocity * 0.5f;
                        }
                    }
                }
            }
            else
            {
                p.scale -= scaleReduce;
                if (p.scale <= 0f) { p.Kill(); }
                p.velocity.Y += gravity;
                if (Main.netMode != NetmodeID.Server && spawnDust != null) spawnDust(p, p.position, p.width, p.height);
            }
        }

        /*
         * A cleaned up (and edited) copy of Thrown Weapon AI. (throwing knife, shuriken, etc.)
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * spin : wether to continously spin with velocity or point in the direction of velocity until slowdown.
         * timeUntilDrop : How many ticks to move until slowing down.
         * xScalar : the scalar to slow down on the X axis.
         * yIncrement : the amount to speed up by on the Y axis.
         * maxSpeedY : the max speed of the projectile on the Y axis.
         */
        public static void AIThrownWeapon(Projectile p, ref float[] ai, bool spin = false, int timeUntilDrop = 10, float xScalar = 0.99f, float yIncrement = 0.25f, float maxSpeedY = 16f)
        {
            p.rotation += (Math.Abs(p.velocity.X) + Math.Abs(p.velocity.Y)) * 0.03f * p.direction;
            ai[0] += 1f;
            if (ai[0] >= timeUntilDrop)
            {
                p.velocity.Y += yIncrement;
                p.velocity.X *= xScalar;
            }
            else
            if (!spin) { p.rotation = BaseUtility.RotationTo(p.Center, p.Center + p.velocity) + 1.57f; }
            if (p.velocity.Y > maxSpeedY) { p.velocity.Y = maxSpeedY; }
        }

        public static void AISpear(Projectile p, ref float[] ai, float initialSpeed = 3f, float moveOutward = 1.4f, float moveInward = 1.6f, bool overrideKill = false)
        {
            Player plr = Main.player[p.owner];
            Item item = plr.inventory[plr.selectedItem];
            if (Main.myPlayer == p.owner && item is { autoReuse: true } && plr.itemAnimation == 1) { p.Kill(); return; } //prevents a bug with autoReuse and spears
            Main.player[p.owner].heldProj = p.whoAmI;
            Main.player[p.owner].itemTime = Main.player[p.owner].itemAnimation;
            Vector2 gfxOffset = new(0, plr.gfxOffY);
            AISpear(p, ref ai, plr.Center + gfxOffset, plr.direction, plr.itemAnimation, plr.itemAnimationMax, initialSpeed, moveOutward, moveInward, overrideKill, plr.frozen);
        }

        /*
         * A cleaned up (and edited) copy of Spear AI.
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * center : the center of what is holding the spear.
         * ownerDirection : the direction of the owner of the spear.
         * itemAnimation : the current item animation tick of the spear.
         * itemAnimationMax : the max length of the item animation of the spear.
         * initialSpeed : how fast velocity initially is.
         * moveOutward : how fast to move outward.
         * moveInward : how fast to move inward.
         * overrideKill : If true, prevents the spear's death.
		 * frozen : If the holder of the spear is frozen or not (used to freeze the animation)
         */
        public static void AISpear(Projectile p, ref float[] ai, Vector2 center, int ownerDirection, int itemAnimation, int itemAnimationMax, float initialSpeed = 3f, float moveOutward = 1.4f, float moveInward = 1.6f, bool overrideKill = false, bool frozen = false)
        {
            p.direction = ownerDirection;
            p.position.X = center.X - p.width * 0.5f;
            p.position.Y = center.Y - p.height * 0.5f;
            if (ai[0] == 0f) { ai[0] = initialSpeed; p.netUpdate = true; }
            if (!frozen)
            {
                if (itemAnimation < itemAnimationMax * 0.33f) { ai[0] -= moveInward; } else { ai[0] += moveOutward; }
            }
            p.position += p.velocity * ai[0];
            if (!overrideKill && Main.player[p.owner].itemAnimation == 0) { p.Kill(); }
            p.rotation = (float)Math.Atan2(p.velocity.Y, p.velocity.X) + 2.355f;
            if (p.direction == -1) { p.rotation -= 0f; }
            else
            if (p.direction == 1) { p.rotation -= 1.57f; }
        }

        /*
         * A cleaned up (and edited) copy of Boomerang AI.
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * center : the center of where the boomerang should return to.
         * playSound : If true, plays the air sound boomerangs make while in the air.
         * maxDistance : the maximum 'distance' for the projectile to go before it rebounds.
         * returnDelay : the amount of time in ticks until the projectile returns to it's source.
         * speedInterval : the amount to move the projectile by each tick.
         * rotationInterval : the amount for the projectile to rotate by each tick.
         * direct : If true, when returning simply reverses the boomerang velocity.
         */
        public static void AIBoomerang(Projectile p, ref float[] ai, Vector2 position = default, int width = -1, int height = -1, bool playSound = true, float maxDistance = 9f, int returnDelay = 35, float speedInterval = 0.4f, float rotationInterval = 0.4f, bool direct = false)
        {
            if (position == default) { position = Main.player[p.owner].position; }
            if (width == -1) { width = Main.player[p.owner].width; }
            if (height == -1) { height = Main.player[p.owner].height; }
            Vector2 center = position + new Vector2(width * 0.5f, height * 0.5f);
            if (playSound && p.soundDelay == 0)
            {
                p.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, p.position);
            }
            if (ai[0] == 0f)
            {
                ai[1] += 1f;
                if (ai[1] >= returnDelay)
                {
                    ai[0] = 1f;
                    ai[1] = 0f;
                    p.netUpdate = true;
                }
            }
            else
            {
                p.tileCollide = false;
                float distPlayerX = center.X - p.Center.X;
                float distPlayerY = center.Y - p.Center.Y;
                float distPlayer = (float)Math.Sqrt(distPlayerX * distPlayerX + distPlayerY * distPlayerY);
                if (distPlayer > 3000f)
                {
                    p.Kill();
                }
                if (direct)
                {
                    p.velocity = BaseUtility.RotateVector(default, new Vector2(speedInterval, 0f), BaseUtility.RotationTo(p.Center, center));
                }
                else
                {
                    distPlayer = maxDistance / distPlayer;
                    distPlayerX *= distPlayer;
                    distPlayerY *= distPlayer;
                    if (p.velocity.X < distPlayerX)
                    {
                        p.velocity.X += speedInterval;
                        if (p.velocity.X < 0f && distPlayerX > 0f) { p.velocity.X += speedInterval; }
                    }
                    else
                    if (p.velocity.X > distPlayerX)
                    {
                        p.velocity.X -= speedInterval;
                        if (p.velocity.X > 0f && distPlayerX < 0f) { p.velocity.X -= speedInterval; }
                    }
                    if (p.velocity.Y < distPlayerY)
                    {
                        p.velocity.Y += speedInterval;
                        if (p.velocity.Y < 0f && distPlayerY > 0f) { p.velocity.Y += speedInterval; }
                    }
                    else
                    if (p.velocity.Y > distPlayerY)
                    {
                        p.velocity.Y -= speedInterval;
                        if (p.velocity.Y > 0f && distPlayerY < 0f) { p.velocity.Y -= speedInterval; }
                    }
                }
                if (Main.myPlayer == p.owner)
                {
                    Rectangle rectangle = p.Hitbox;
                    Rectangle value = new((int)position.X, (int)position.Y, width, height);
                    if (rectangle.Intersects(value)) { p.Kill(); }
                }
            }
            p.rotation += rotationInterval * p.direction;
        }

        /*
         * A cleaned up (and edited) copy of tile collison for Boomerangs.
         * bounce : Set to true if your projectile acts like Light Discs or the Thorn Chakram.
         */
        public static void TileCollideBoomerang(Projectile p, ref Vector2 velocity, bool bounce = false)
        {
            if (bounce)
            {
                if (p.velocity.X != velocity.X) { p.velocity.X = -velocity.X; }
                if (p.velocity.Y != velocity.Y) { p.velocity.Y = -velocity.Y; }
            }
            else
            {
                p.ai[0] = 1f;
                p.velocity.X = -velocity.X;
                p.velocity.Y = -velocity.Y;
            }
            p.netUpdate = true;
        }

        public static void AIFlail(Projectile p, ref float[] ai, bool noKill = false, float chainDistance = 160f)
        {
            if (Main.player[p.owner] != null)
            {
                if (Main.player[p.owner].dead) { p.Kill(); return; }
                Main.player[p.owner].itemAnimation = 10;
                Main.player[p.owner].itemTime = 10;
            }
            AIFlail(p, ref ai, Main.player[p.owner].Center, Main.player[p.owner].velocity, Main.player[p.owner].GetAttackSpeed(DamageClass.Melee), Main.player[p.owner].channel, noKill, chainDistance);
            Main.player[p.owner].direction = p.direction;
        }

        /*
         * A cleaned up (and edited) copy of Flail AI.
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * connectedPoint : The point for the flail to be 'attached' to, and rebound to, etc.
         * connectedPointVelocity : The velocity of the connected point, if it is moving.
         * GetAttackSpeed(DamageClass.Melee) : the GetAttackSpeed(DamageClass.Melee) of whatever is using the flail.
         * channel : Wether or not the source is 'channeling' (holding down the fire button) projectile flail.
         * noKill : If true, do not kill the projectile when it returns to the connected point.
         * chainDistance : How far for the flail to actually go.
         */
        public static void AIFlail(Projectile p, ref float[] ai, Vector2 connectedPoint, Vector2 connectedPointVelocity, float meleeSpeed, bool channel, bool noKill = false, float chainDistance = 160f)
        {
            p.direction = p.Center.X > connectedPoint.X ? 1 : -1;
            float pointX = connectedPoint.X - p.Center.X;
            float pointY = connectedPoint.Y - p.Center.Y;
            float pointDist = (float)Math.Sqrt(pointX * pointX + pointY * pointY);
            if (ai[0] == 0f)
            {
                p.tileCollide = true;
                if (pointDist > chainDistance)
                {
                    ai[0] = 1f;
                    p.netUpdate = true;
                }
                else
                {
                    if (!channel)
                    {
                        if (p.velocity.Y < 0f) { p.velocity.Y *= 0.9f; }
                        p.velocity.Y += 1f;
                        p.velocity.X *= 0.9f;
                    }
                }
            }
            else
            if (ai[0] == 1f)
            {
                float meleeSpeed1 = 14f / meleeSpeed;
                float meleeSpeed2 = 0.9f / meleeSpeed;
                float maxBallDistance = chainDistance + 140f;
                Math.Abs(pointX);
                Math.Abs(pointY);
                if (ai[1] == 1f) { p.tileCollide = false; }
                if (!channel || pointDist > maxBallDistance || !p.tileCollide)
                {
                    ai[1] = 1f;
                    if (p.tileCollide) { p.netUpdate = true; }
                    p.tileCollide = false;
                    if (!noKill && pointDist < 20f)
                    {
                        p.Kill();
                    }
                }
                if (!p.tileCollide) { meleeSpeed2 *= 2f; }
                if (pointDist > 60f || !p.tileCollide)
                {
                    pointDist = meleeSpeed1 / pointDist;
                    pointX *= pointDist;
                    pointY *= pointDist;
                    float pointX2 = pointX - p.velocity.X;
                    float pointY2 = pointY - p.velocity.Y;
                    float pointDist2 = (float)Math.Sqrt(pointX2 * pointX2 + pointY2 * pointY2);
                    pointDist2 = meleeSpeed2 / pointDist2;
                    pointX2 *= pointDist2;
                    pointY2 *= pointDist2;
                    p.velocity.X *= 0.98f;
                    p.velocity.Y *= 0.98f;
                    p.velocity.X += pointX2;
                    p.velocity.Y += pointY2;
                }
                else
                {
                    if (Math.Abs(p.velocity.X) + Math.Abs(p.velocity.Y) < 6f)
                    {
                        p.velocity.X *= 0.96f;
                        p.velocity.Y += 0.2f;
                    }
                    if (connectedPointVelocity.X == 0f) { p.velocity.X *= 0.96f; }
                }
            }
            p.rotation = (float)Math.Atan2(pointY, pointX) - p.velocity.X * 0.1f;
        }

        /*
         * A cleaned up (and edited) copy of tile collison for Flails.
         */
        public static void TileCollideFlail(Projectile p, ref Vector2 velocity, bool playSound = true)
        {
            if (velocity != p.velocity)
            {
                bool updateAndCollide = false;
                if (velocity.X != p.velocity.X)
                {
                    if (Math.Abs(velocity.X) > 4f) { updateAndCollide = true; }
                    p.position.X += p.velocity.X;
                    p.velocity.X = -velocity.X * 0.2f;
                }
                if (velocity.Y != p.velocity.Y)
                {
                    if (Math.Abs(velocity.Y) > 4f) { updateAndCollide = true; }
                    p.position.Y += p.velocity.Y;
                    p.velocity.Y = -velocity.Y * 0.2f;
                }
                p.ai[0] = 1f;
                if (updateAndCollide)
                {
                    p.netUpdate = true;
                    Collision.HitTiles(p.position, p.velocity, p.width, p.height);
                    if (playSound) { SoundEngine.PlaySound(SoundID.Dig, p.position); }
                }
            }
        }

        #endregion

        #region Vanilla Projectile AI Code Excerpts

        /*
		 * (Edited) Sticky code used by projectiles to stick to tiles. Returns true if it is 'sticking' to a tile.
		 * 
		 * beginGravity : the time (in ticks) when to begin applying gravity
		 * 
		 * 
		 */
        public static bool StickToTiles(Vector2 position, ref Vector2 velocity, int width, int height, Func<int, int, bool> canStick = null)
        {
            int tileLeftX = (int)(position.X / 16f) - 1;
            int tileRightX = (int)((position.X + width) / 16f) + 2;
            int tileLeftY = (int)(position.Y / 16f) - 1;
            int tileRightY = (int)((position.Y + height) / 16f) + 2;
            if (tileLeftX < 0) { tileLeftX = 0; }
            if (tileRightX > Main.maxTilesX) { tileRightX = Main.maxTilesX; }
            if (tileLeftY < 0) { tileLeftY = 0; }
            if (tileRightY > Main.maxTilesY) { tileRightY = Main.maxTilesY; }
            bool stick = false;
            for (int x = tileLeftX; x < tileRightX; x++)
            {
                for (int y = tileLeftY; y < tileRightY; y++)
                {
                    if (Main.tile[x, y] != null && Main.tile[x, y].HasUnactuatedTile && (canStick != null ? canStick(x, y) : Main.tileSolid[Main.tile[x, y].TileType] || Main.tileSolidTop[Main.tile[x, y].TileType] && Main.tile[x, y].TileFrameY == 0))
                    {
                        Vector2 pos = new(x * 16, y * 16);
                        if (position.X + width - 4f > pos.X && position.X + 4f < pos.X + 16f && position.Y + height - 4f > pos.Y && position.Y + 4f < pos.Y + 16f)
                        {
                            stick = true; velocity *= 0f; break;
                        }
                    }
                }
                if (stick) break;
            }
            return stick;
        }


        #endregion

        #region Vanilla NPC AI Copy Methods

        /*
		 * A cleaned up (and edited) copy of Shadowflame Ghost AI. (aiStyle 86) (Shadowflame Apparition, etc.)
		 * 
		 * speedupOverTime: Wether or not to speed up when aligned to the player over time.
		 * distanceBeforeTakeoff: The distance from the target player before the NPC turns around.
		 */
        public static void AIShadowflameGhost(NPC npc, ref float[] ai, bool speedupOverTime = false, float distanceBeforeTakeoff = 660f, float velIntervalX = 0.3f, float velMaxX = 7f, float velIntervalY = 0.2f, float velMaxY = 4f, float velScalarY = 4f, float velScalarYMax = 15f, float velIntervalXTurn = 0.4f, float velIntervalYTurn = 0.4f, float velIntervalScalar = 0.95f, float velIntervalMaxTurn = 5f)
        {
            int npcAvoidCollision;
            for (int m = 0; m < 200; m = npcAvoidCollision + 1)
            {
                if (m != npc.whoAmI && Main.npc[m].active && Main.npc[m].type == npc.type)
                {
                    Vector2 dist = Main.npc[m].Center - npc.Center;
                    if (dist.Length() < 50f)
                    {
                        dist.Normalize();
                        if (dist.X == 0f && dist.Y == 0f)
                        {
                            if (m > npc.whoAmI)
                                dist.X = 1f;
                            else
                                dist.X = -1f;
                        }
                        dist *= 0.4f;
                        npc.velocity -= dist;
                        Main.npc[m].velocity += dist;
                    }
                }
                npcAvoidCollision = m;
            }
            if (speedupOverTime)
            {
                float timerMax = 120f;
                if (npc.localAI[0] < timerMax)
                {
                    if (npc.localAI[0] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                        npc.TargetClosest();
                        if (npc.direction > 0)
                        {
                            npc.velocity.X += 2f;
                        }
                        else
                        {
                            npc.velocity.X -= 2f;
                        }
                        for (int m = 0; m < 20; m = npcAvoidCollision + 1)
                        {
                            npcAvoidCollision = m;
                        }
                    }
                    npc.localAI[0] += 1f;
                    float timerPartial = 1f - npc.localAI[0] / timerMax;
                    float timerPartialTimes20 = timerPartial * 20f;
                    int nextNPC = 0;
                    while (nextNPC < timerPartialTimes20)
                    {
                        npcAvoidCollision = nextNPC;
                        nextNPC = npcAvoidCollision + 1;
                    }
                }
            }
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest();
                npc.ai[0] = 1f;
                npc.ai[1] = npc.direction;
            }
            else if (npc.ai[0] == 1f)
            {
                npc.TargetClosest();
                npc.velocity.X += npc.ai[1] * velIntervalX;

                if (npc.velocity.X > velMaxX)
                    npc.velocity.X = velMaxX;
                else if (npc.velocity.X < -velMaxX)
                    npc.velocity.X = -velMaxX;

                float playerDistY = Main.player[npc.target].Center.Y - npc.Center.Y;
                if (Math.Abs(playerDistY) > velMaxY)
                    velScalarY = velScalarYMax;

                if (playerDistY > velMaxY)
                    playerDistY = velMaxY;
                else if (playerDistY < -velMaxY)
                    playerDistY = -velMaxY;

                npc.velocity.Y = (npc.velocity.Y * (velScalarY - 1f) + playerDistY) / velScalarY;
                if (npc.ai[1] > 0f && Main.player[npc.target].Center.X - npc.Center.X < -distanceBeforeTakeoff || npc.ai[1] < 0f && Main.player[npc.target].Center.X - npc.Center.X > distanceBeforeTakeoff)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    if (npc.Center.Y + 20f > Main.player[npc.target].Center.Y)
                        npc.ai[1] = -1f;
                    else
                        npc.ai[1] = 1f;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.velocity.Y += npc.ai[1] * velIntervalYTurn;

                if (npc.velocity.Length() > velIntervalMaxTurn)
                    npc.velocity *= velIntervalScalar;

                if (npc.velocity.X is > -1f and < 1f)
                {
                    npc.TargetClosest();
                    npc.ai[0] = 3f;
                    npc.ai[1] = npc.direction;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.velocity.X += npc.ai[1] * velIntervalXTurn;

                if (npc.Center.Y > Main.player[npc.target].Center.Y)
                    npc.velocity.Y -= velIntervalY;
                else
                    npc.velocity.Y += velIntervalY;

                if (npc.velocity.Length() > velIntervalMaxTurn)
                    npc.velocity *= velIntervalScalar;

                if (npc.velocity.Y is > -1f and < 1f)
                {
                    npc.TargetClosest();
                    npc.ai[0] = 0f;
                    npc.ai[1] = npc.direction;
                }
            }
        }


        public static void AISpaceOctopus(NPC npc, ref float[] ai, float moveSpeed = 0.15f, float velMax = 5f, float hoverDistance = 250f, float shootProjInterval = 70f, Action<NPC, Vector2> fireProj = null)
        {
            npc.TargetClosest();
            AISpaceOctopus(npc, ref ai, Main.player[npc.target].Center, moveSpeed, velMax, hoverDistance, shootProjInterval, fireProj);
        }

        public static void AISpaceOctopus(NPC npc, ref float[] ai, Vector2 targetCenter = default, float moveSpeed = 0.15f, float velMax = 5f, float hoverDistance = 250f, float shootProjInterval = 70f, Action<NPC, Vector2> fireProj = null)
        {
            Vector2 wantedVelocity = targetCenter - npc.Center + new Vector2(0f, -hoverDistance);
            float dist = wantedVelocity.Length();
            if (dist < 20f)
            {
                wantedVelocity = npc.velocity;
            }
            else if (dist < 40f)
            {
                wantedVelocity.Normalize();
                wantedVelocity *= velMax * 0.35f;
            }
            else if (dist < 80f)
            {
                wantedVelocity.Normalize();
                wantedVelocity *= velMax * 0.65f;
            }
            else
            {
                wantedVelocity.Normalize();
                wantedVelocity *= velMax;
            }
            npc.SimpleFlyMovement(wantedVelocity, moveSpeed);
            npc.rotation = npc.velocity.X * 0.1f;
            if (fireProj != null && shootProjInterval > -1 && (ai[0] += 1f) >= shootProjInterval)
            {
                ai[0] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 projVelocity = Vector2.Zero;
                    while (Math.Abs(projVelocity.X) < 1.5f)
                    {
                        projVelocity = Vector2.UnitY.RotatedByRandom(1.5707963705062866) * new Vector2(5f, 3f);
                    }
                    fireProj(npc, projVelocity);
                }
            }
        }

        public static void AIElemental(NPC npc, ref float[] ai, bool? noDamageMode = null, int noDamageTimeMax = 120, bool gravityChange = true, bool tileCollideChange = true, float startPhaseDist = 800f, float stopPhaseDist = 600f, int idleTooLong = 180, float velSpeed = 2f)
        {
            int timerDummy = (int)npc.localAI[0];
            AIElemental(npc, ref ai, ref timerDummy, noDamageMode, noDamageTimeMax, gravityChange, tileCollideChange, startPhaseDist, stopPhaseDist, idleTooLong, velSpeed);
            npc.localAI[0] = timerDummy;
        }

        /*
		 * A cleaned up (and edited) copy of Elemental AI. (aiStyle 91) (Granite Elemental, etc.)
		 * 
		 * idleTimer : A localized value, which is randomly ticked up to 5.
		 * noDamageMode : A bool?. Set to true to force on no damage mode, set to false to force it off, return null to have it only on in expert.
		 * noDamageTimeMax : The maximum amount of ticks before no damage mode returns to normal. (default 120)
		 * gravityChange : if true, npc.noGravity is changed during immortality states. If false, nothing is changed.
		 * tileCollideChange : if true, npc.noTileCollide is changed between phasing through tiles and not. If false, nothing is changed.
		 * startPhaseDist : the distance at which the npc begins phasing through tiles to get near the player. (default 800)
		 * stopPhaseDist : The distance at which the npc stops phasing through tiles to get near the player. (default 600)
		 * idleTooLong : The maximum amount of ticks the npc can be 'idle' before it attempts to change movement modes. (default 180)
		 * velSpeed : The speed of the entity when moving to the player. This value is used for all states; changing it speeds or slows the npc in all of them.
		 */
        public static void AIElemental(NPC npc, ref float[] ai, ref int idleTimer, bool? noDamageMode = null, int noDamageTimeMax = 120, bool gravityChange = true, bool tileCollideChange = true, float startPhaseDist = 800f, float stopPhaseDist = 600f, int idleTooLong = 180, float velSpeed = 2f)
        {
            bool noDmg = noDamageMode == null ? Main.expertMode : (bool)noDamageMode;
            if (gravityChange) npc.noGravity = true;
            if (tileCollideChange) npc.noTileCollide = false;
            if (noDmg) npc.dontTakeDamage = false;
            Player targetPlayer = npc.target < 0 ? null : Main.player[npc.target];
            Vector2 playerCenter = targetPlayer == null ? npc.Center + new Vector2(0, 5f) : targetPlayer.Center;

            if (npc.justHit && Main.netMode != NetmodeID.MultiplayerClient && noDmg && Main.rand.NextBool(6))
            {
                npc.netUpdate = true;
                ai[0] = -1f;
                ai[1] = 0f;
            }
            if (ai[0] == -1f) //immortal
            {
                if (noDmg) npc.dontTakeDamage = true;
                if (gravityChange) npc.noGravity = false;
                npc.velocity.X *= 0.98f;
                ai[1] += 1f;
                if (ai[1] >= noDamageTimeMax)
                {
                    ai[0] = ai[1] = ai[2] = ai[3] = 0f;
                }
            }
            else if (ai[0] == 0f) //targeting mode (chosing how to act)
            {
                npc.TargetClosest();
                targetPlayer = Main.player[npc.target];
                playerCenter = targetPlayer.Center;
                if (Collision.CanHit(npc.Center, 1, 1, playerCenter, 1, 1))
                {
                    ai[0] = 1f;
                    return;
                }
                Vector2 centerDiff = playerCenter - npc.Center;
                centerDiff.Y -= targetPlayer.height / 4;
                float centerDist = centerDiff.Length();
                if (centerDist > startPhaseDist)
                {
                    ai[0] = 2f;
                    return;
                }
                Vector2 npcCenter = npc.Center;
                npcCenter.X = playerCenter.X;
                Vector2 npcCentDiff = npcCenter - npc.Center;
                if (npcCentDiff.Length() > 8f && Collision.CanHit(npc.Center, 1, 1, npcCenter, 1, 1))
                {
                    ai[0] = 3f;
                    ai[1] = npcCenter.X;
                    ai[2] = npcCenter.Y;
                    Vector2 npcCenter2 = npc.Center;
                    npcCenter2.Y = playerCenter.Y;
                    if (npcCentDiff.Length() > 8f && Collision.CanHit(npc.Center, 1, 1, npcCenter2, 1, 1) && Collision.CanHit(npcCenter2, 1, 1, targetPlayer.position, 1, 1))
                    {
                        ai[0] = 3f;
                        ai[1] = npcCenter2.X;
                        ai[2] = npcCenter2.Y;
                    }
                }
                else
                {
                    npcCenter = npc.Center;
                    npcCenter.Y = playerCenter.Y;
                    if ((npcCenter - npc.Center).Length() > 8f && Collision.CanHit(npc.Center, 1, 1, npcCenter, 1, 1))
                    {
                        ai[0] = 3f;
                        ai[1] = npcCenter.X;
                        ai[2] = npcCenter.Y;
                    }
                }
                if (ai[0] == 0f)
                {
                    npc.localAI[0] = 0f;
                    centerDiff.Normalize();
                    centerDiff *= 0.5f;
                    npc.velocity += centerDiff;
                    ai[0] = 4f;
                    ai[1] = 0f;
                }
            }
            else if (ai[0] == 1f) //move to player
            {
                Vector2 distDiff = playerCenter - npc.Center;
                float distLength = distDiff.Length();
                float velSpeed2 = velSpeed; velSpeed2 += distLength / 200f;
                float speedAdjuster = 50f;
                distDiff.Normalize();
                distDiff *= velSpeed2;
                npc.velocity = (npc.velocity * (speedAdjuster - 1) + distDiff) / speedAdjuster;
                if (!Collision.CanHit(npc.Center, 1, 1, playerCenter, 1, 1))
                {
                    ai[0] = 0f;
                    ai[1] = 0f;
                }
            }
            else if (ai[0] == 2f) //phase slowly through tiles to player
            {
                npc.noTileCollide = true;
                Vector2 distDiff = playerCenter - npc.Center;
                float distLength = distDiff.Length();
                float velSpeedPhase = velSpeed;
                float speedAdjusterPhase = 4f;
                distDiff.Normalize();
                distDiff *= velSpeedPhase;
                npc.velocity = (npc.velocity * (speedAdjusterPhase - 1) + distDiff) / speedAdjusterPhase;
                if (distLength < stopPhaseDist && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    ai[0] = 0f;
                }
            }
            else if (ai[0] == 3f) // horizontal floating to player
            {
                Vector2 targetLoc = new(ai[1], ai[2]);
                Vector2 targetDiff = targetLoc - npc.Center;
                float targetLength = targetDiff.Length();
                float velSpeedHorizontal = velSpeed < 1f ? velSpeed * 0.5f : Math.Max(0.1f, velSpeed - 1f);
                float speedAdjusterHorizontal = 3f;
                targetDiff.Normalize();
                targetDiff *= velSpeedHorizontal;
                npc.velocity = (npc.velocity * (speedAdjusterHorizontal - 1f) + targetDiff) / speedAdjusterHorizontal;
                if (npc.collideX || npc.collideY)
                {
                    ai[0] = 4f;
                    ai[1] = 0f;
                }
                if (targetLength < velSpeedHorizontal || targetLength > startPhaseDist || Collision.CanHit(npc.Center, 1, 1, playerCenter, 1, 1))
                {
                    ai[0] = 0f;
                }
            }
            else if (ai[0] == 4f) //idle floating
            {
                if (npc.collideX) npc.velocity.X *= -0.8f;
                if (npc.collideY) npc.velocity.Y *= -0.8f;
                Vector2 velVec;
                if (npc.velocity.X == 0f && npc.velocity.Y == 0f)
                {
                    velVec = playerCenter - npc.Center;
                    velVec.Y -= targetPlayer.height / 4;
                    velVec.Normalize();
                    npc.velocity = velVec * 0.1f;
                }
                float velSpeedIdle = velSpeed < 1f ? velSpeed * 0.75f : Math.Max(0.1f, velSpeed - 0.5f);
                float speedAdjusterIdle = 20f;
                velVec = npc.velocity;
                velVec.Normalize();
                velVec *= velSpeedIdle;
                npc.velocity = (npc.velocity * (speedAdjusterIdle - 1f) + velVec) / speedAdjusterIdle;
                ai[1] += 1f;
                if (ai[1] > idleTooLong)
                {
                    ai[0] = 0f;
                    ai[1] = 0f;
                }
                if (Collision.CanHit(npc.Center, 1, 1, playerCenter, 1, 1))
                {
                    ai[0] = 0f;
                }
                idleTimer += 1;
                if (idleTimer >= 5 && !Collision.SolidCollision(npc.position - new Vector2(10f, 10f), npc.width + 20, npc.height + 20))
                {
                    idleTimer = 0;
                    Vector2 npcCenter = npc.Center;
                    npcCenter.X = playerCenter.X;
                    if (Collision.CanHit(npc.Center, 1, 1, npcCenter, 1, 1) && Collision.CanHit(npc.Center, 1, 1, npcCenter, 1, 1) && Collision.CanHit(playerCenter, 1, 1, npcCenter, 1, 1))
                    {
                        ai[0] = 3f;
                        ai[1] = npcCenter.X;
                        ai[2] = npcCenter.Y;
                        return;
                    }
                    npcCenter = npc.Center;
                    npcCenter.Y = playerCenter.Y;
                    if (Collision.CanHit(npc.Center, 1, 1, npcCenter, 1, 1) && Collision.CanHit(playerCenter, 1, 1, npcCenter, 1, 1))
                    {
                        ai[0] = 3f;
                        ai[1] = npcCenter.X;
                        ai[2] = npcCenter.Y;
                    }
                }
            }
        }


        public static void AIWeapon(NPC npc, ref float[] ai, int rotTime = 120, int moveTime = 100, float maxSpeed = 6f, float movementScalar = 1f, float rotScalar = 1f)
        {
            if (npc.target is < 0 or 255 || Main.player[npc.target].dead) npc.TargetClosest();
            AIWeapon(npc, ref ai, ref npc.rotation, Main.player[npc.target].Center, npc.justHit, rotTime, moveTime, maxSpeed, movementScalar, rotScalar);
        }

        /*
		 * A cleaned up (and edited) copy of Possessed Weapon AI. (aiStyle 23) (Enchanted Sword, Demon Hammer, etc.)
		 * 
		 * targetPos : The center of the target of projectile codable.
		 * justHit : Set true to reset the AI (ie return it to a rotating state), false otherwise
		 * rotTime : The time (in ticks) for the codable to rotate.
		 * moveTime : The time (in ticks) for the codable to move.
		 * movementScalar : A scalar for how much to move per tick.
		 * 
		 */
        public static void AIWeapon(Entity codable, ref float[] ai, ref float rotation, Vector2 targetPos, bool justHit = false, int rotTime = 120, int moveTime = 100, float maxSpeed = 6f, float movementScalar = 1f, float rotScalar = 1f)
        {
            if (ai[0] == 0f)
            {
                Vector2 vector2 = codable.Center;
                float distX = targetPos.X - vector2.X;
                float distY = targetPos.Y - vector2.Y;
                float dist = (float)Math.Sqrt(distX * distX + distY * distY);
                float distMult = 9f / dist;
                codable.velocity.X = distX * distMult * movementScalar;
                codable.velocity.Y = distY * distMult * movementScalar;
                if (codable.velocity.X > maxSpeed) { codable.velocity.X = maxSpeed; }
                if (codable.velocity.X < -maxSpeed) { codable.velocity.X = -maxSpeed; }
                if (codable.velocity.Y > maxSpeed) { codable.velocity.Y = maxSpeed; }
                if (codable.velocity.Y < -maxSpeed) { codable.velocity.Y = -maxSpeed; }
                rotation = (float)Math.Atan2(codable.velocity.Y, codable.velocity.X);
                ai[0] = 1f;
                ai[1] = 0.0f;
            }
            else if (ai[0] == 1f)
            {
                if (justHit)
                {
                    ai[0] = 2f;
                    ai[1] = 0.0f;
                }
                codable.velocity *= 0.99f;
                ++ai[1];
                if (ai[1] < moveTime) return;
                ai[0] = 2f;
                ai[1] = 0.0f;
                codable.velocity.X = 0.0f;
                codable.velocity.Y = 0.0f;
            }
            else
            {
                if (justHit)
                {
                    ai[0] = 2f;
                    ai[1] = 0.0f;
                }
                codable.velocity *= 0.96f;
                ++ai[1];
                rotation += (float)(0.1 + (double)(ai[1] / rotTime) * 0.4f) * codable.direction * rotScalar;
                if (ai[1] < rotTime) return;
                if (codable is NPC nPC) { nPC.netUpdate = true; } else if (codable is Projectile projectile) { projectile.netUpdate = true; }
                ai[0] = 0.0f;
                ai[1] = 0.0f;
            }
        }


        /*
         * A cleaned up (and edited) copy of Snail AI. (Snail, etc.) (AIStyle 67)
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * snailStatus: An int value which is set to the 'gravity' status of the AI. (0 == down (ground), 1 == left wall, 2 == right wall, 3 == up (ceiling))
		 * moveInterval : The amount to move by per tick.
		 * rotAmt : The amount to rotate by when reaching corners.
         */
        public static void AISnail(NPC npc, ref float[] ai, ref int snailStatus, float moveInterval = 0.3f, float rotAmt = 0.1f)
        {
            if (ai[0] == 0f)
            {
                npc.TargetClosest(); npc.directionY = 1; ai[0] = 1f;
                if (npc.direction > 0) { npc.spriteDirection = 1; }
            }
            bool collisonOnX = false;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (ai[2] == 0f && Main.rand.NextBool(7200)) { ai[2] = 2f; npc.netUpdate = true; }
                if (!npc.collideX && !npc.collideY)
                {
                    npc.localAI[3] += 1f;
                    if (npc.localAI[3] > 5f) { ai[2] = 2f; npc.netUpdate = true; }
                }
                else { npc.localAI[3] = 0f; }
            }
            if (ai[2] > 0f)
            {
                ai[1] = 0f; ai[0] = 1f; npc.directionY = 1;
                if (npc.velocity.Y > moveInterval) { npc.rotation += npc.direction * 0.1f; } else { npc.rotation = 0f; }
                npc.spriteDirection = npc.direction;
                npc.velocity.X = moveInterval * npc.direction;
                npc.noGravity = false;
                snailStatus = 0;
                //int tileX = (int)(npc.Center.X + (float)(npc.width / 2 * -npc.direction)) / 16;
                //int tileY = (int)(npc.position.Y + (float)npc.height + 8f) / 16;
                //if (Main.tile[tileX, tileY] != null && Main.tile[tileX, tileY].Slope != 1 && npc.collideY){ ai[2] -= 1f; }
                //tileX = (int)(npc.Center.X + (float)(npc.width / 2 * npc.direction)) / 16;
                //tileY = (int)(npc.position.Y + (float)npc.height - 4f) / 16;
                //if (Main.tile[num1058, scaleChange] != null && Main.tile[num1058, scaleChange].bottomSlope){ npc.direction *= -1; }
                if (npc.collideX && npc.velocity.Y == 0f) { collisonOnX = true; ai[2] = 0f; ai[1] = 1f; npc.directionY = -1; }
                if (npc.velocity.Y == 0f)
                {
                    if (npc.localAI[1] == npc.position.X)
                    {
                        npc.localAI[2] += 1f;
                        if (npc.localAI[2] > 10f)
                        {
                            npc.direction = 1;
                            npc.velocity.X = npc.direction * moveInterval;
                            npc.localAI[2] = 0f;
                        }
                    }
                    else { npc.localAI[2] = 0f; npc.localAI[1] = npc.position.X; }
                }
            }
            if (ai[2] == 0f)
            {
                npc.noGravity = true;
                if (ai[1] == 0f)
                {
                    if (npc.collideY) { ai[0] = 2f; }
                    if (!npc.collideY && ai[0] == 2f)
                    {
                        npc.direction = -npc.direction;
                        ai[1] = 1f;
                        ai[0] = 1f;
                    }
                    if (npc.collideX) { npc.directionY = -npc.directionY; ai[1] = 1f; }
                }
                else
                {
                    if (npc.collideX) { ai[0] = 2f; }
                    if (!npc.collideX && ai[0] == 2f)
                    {
                        npc.directionY = -npc.directionY;
                        ai[1] = 0f;
                        ai[0] = 1f;
                    }
                    if (npc.collideY) { npc.direction = -npc.direction; ai[1] = 0f; }
                }
                if (!collisonOnX)
                {
                    float prevRot = npc.rotation;
                    if (npc.directionY < 0)
                    {
                        if (npc.direction < 0)
                        {
                            if (npc.collideX)
                            {
                                npc.rotation = 1.57f;
                                npc.spriteDirection = -1;
                            }
                            else if (npc.collideY)
                            {
                                npc.rotation = 3.14f;
                                npc.spriteDirection = 1;
                            }
                        }
                        else if (npc.collideY)
                        {
                            npc.rotation = 3.14f;
                            npc.spriteDirection = -1;
                        }
                        else if (npc.collideX)
                        {
                            npc.rotation = 4.71f;
                            npc.spriteDirection = 1;
                        }
                    }
                    else
                    if (npc.direction < 0)
                    {
                        if (npc.collideY)
                        {
                            npc.rotation = 0f;
                            npc.spriteDirection = -1;
                        }
                        else if (npc.collideX)
                        {
                            npc.rotation = 1.57f;
                            npc.spriteDirection = 1;
                        }
                    }
                    else if (npc.collideX) { npc.rotation = 4.71f; npc.spriteDirection = -1; }
                    else
                   if (npc.collideY) { npc.rotation = 0f; npc.spriteDirection = 1; }
                    float prevRot2 = npc.rotation;
                    npc.rotation = prevRot;
                    if (npc.rotation > 6.28) { npc.rotation -= 6.28f; }
                    if (npc.rotation < 0f) { npc.rotation += 6.28f; }
                    float rotDiffAbs = Math.Abs(npc.rotation - prevRot2);
                    if (npc.rotation > prevRot2)
                    {
                        if (rotDiffAbs > 3.14) { npc.rotation += rotAmt; }
                        else
                        {
                            npc.rotation -= rotAmt;
                            if (npc.rotation < prevRot2) { npc.rotation = prevRot2; }
                        }
                    }
                    if (npc.rotation < prevRot2)
                    {
                        if (rotDiffAbs > 3.14) { npc.rotation -= rotAmt; }
                        else
                        {
                            npc.rotation += rotAmt;
                            if (npc.rotation > prevRot2) { npc.rotation = prevRot2; }
                        }
                    }
                }

                if (npc.directionY == -1 && !npc.collideX) { snailStatus = 1; }
                else
                if (npc.directionY == 1 && !npc.collideX) { snailStatus = 0; }
                else
                if (npc.direction == 1 && !npc.collideY) { snailStatus = 3; }
                else
                { snailStatus = 2; }
                npc.velocity.X = moveInterval * npc.direction;
                npc.velocity.Y = moveInterval * npc.directionY;
            }
        }

        public static void CollisionTest(NPC npc, ref bool left, ref bool right, ref bool up, ref bool down)
        {
            up = down = left = right = false;
            int lengthX = npc.width / 16 + (npc.width % 16 > 0 ? 1 : 0);
            int lengthY = npc.height / 16 + (npc.height % 16 > 0 ? 1 : 0);
            int xLeft = Math.Max(0, Math.Min(Main.maxTilesX - 1, (int)(npc.position.X / 16f) - 1)), xRight = Math.Max(0, Math.Min(Main.maxTilesX - 1, xLeft + lengthX + 1));
            int yUp = Math.Max(0, Math.Min(Main.maxTilesY - 1, (int)(npc.position.Y / 16f) - 1)), yDown = Math.Max(0, Math.Min(Main.maxTilesY - 1, yUp + lengthY + 1));
            //TOP/DOWN
            for (int x2 = xLeft; x2 < xRight; x2++)
            {
                Tile tileUp = Main.tile[x2, yUp], tileDown = Main.tile[x2, yDown];
                if (tileUp is { HasUnactuatedTile: true } && Main.tileSolid[tileUp.TileType] && !Main.tileSolidTop[tileUp.TileType]) up = true;
                if (tileDown is { HasUnactuatedTile: true } && Main.tileSolid[tileDown.TileType]) down = true;
                if (up && down) break;
            }
            //LEFT/RIGHT
            for (int y2 = yUp; y2 < yDown; y2++)
            {
                Tile tileLeft = Main.tile[xLeft, y2], tileRight = Main.tile[xRight, y2];
                if (tileLeft is { HasUnactuatedTile: true } && Main.tileSolid[tileLeft.TileType] && !Main.tileSolidTop[tileLeft.TileType]) left = true;
                if (tileRight is { HasUnactuatedTile: true } && Main.tileSolid[tileRight.TileType] && !Main.tileSolidTop[tileRight.TileType]) right = true;
                if (left && right) break;
            }
        }

        public static void AISpore(NPC npc, ref float[] ai, float moveIntervalX = 0.1f, float moveIntervalY = 0.02f, float maxSpeedX = 5f, float maxSpeedY = 1f)
        {
            npc.TargetClosest();
            AISpore(npc, ref ai, Main.player[npc.target].Center, Main.player[npc.target].width, moveIntervalX, moveIntervalY, maxSpeedX, maxSpeedY);
        }

        /*
         * A cleaned up (and edited) copy of Spore AI. (Fungi Spore, Plantera Spore, etc.) (AIStyle 50)
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * target : The center of the target.
		 * targetWidth : The width of the target.
		 * moveIntervalX : The amount to move by on the X axis each tick.
		 * moveIntervalY : The amount to move by on the Y axis each tick.
		 * maxSpeedX : The maximum speed of the codable on the X axis.
		 * maxSpeedY : The maximum speed of the codable on the Y axis.
         */
        public static void AISpore(Entity codable, ref float[] ai, Vector2 target, int targetWidth = 16, float moveIntervalX = 0.1f, float moveIntervalY = 0.02f, float maxSpeedX = 5f, float maxSpeedY = 1f)
        {
            codable.velocity.Y += moveIntervalY;
            if (codable.velocity.Y < 0f) codable.velocity.Y *= 0.99f;
            if (codable.velocity.Y > 1f) codable.velocity.Y = 1f;
            int widthHalf = targetWidth / 2;
            if (codable.position.X + codable.width < target.X - widthHalf)
            {
                if (codable.velocity.X < 0) codable.velocity.X *= 0.98f;
                codable.velocity.X += moveIntervalX;
            }
            else if (codable.position.X > target.X + widthHalf)
            {
                if (codable.velocity.X > 0) codable.velocity.X *= 0.98f;
                codable.velocity.X -= moveIntervalX;
            }
            if (codable.velocity.X > maxSpeedX || codable.velocity.X < -maxSpeedX) codable.velocity.X *= 0.97f;
        }

        /*
		 * *UNTESTED, MAY NOT WORK PROPERLY*
		 * 
         * A cleaned up (and edited) copy of Charger AI. (Unicorns, wolves, etc.) (AIStyle 26)
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * moveInterval : How much to move each tick.
		 * maxSpeed : The maxium speed the npc can move per tick.
         * allowBoredom : If false, npc will not get 'bored' trying to harass a target and wander off.
         * ticksUntilBoredom : the amount of ticks until the npc gets 'bored' following a target.
         */
        public static void AICharger(NPC npc, ref float[] ai, float moveInterval = 0.07f, float maxSpeed = 6f, bool allowBoredom = true, int ticksUntilBoredom = 30)
        {
            bool isMoving = false;
            if (npc.velocity.Y == 0f && (npc.velocity.X > 0f && npc.direction < 0 || npc.velocity.X < 0f && npc.direction > 0))
            {
                isMoving = true;
                ++ai[3];
            }
            if (npc.position.X == npc.oldPosition.X || ai[3] >= ticksUntilBoredom || isMoving) ++ai[3];
            else if (ai[3] > 0f) --ai[3];
            if (ai[3] > ticksUntilBoredom * 10) ai[3] = 0f;
            if (npc.justHit) ai[3] = 0f;
            if (ai[3] == ticksUntilBoredom) npc.netUpdate = true;
            Vector2 npcCenter = npc.Center;
            float distX = Main.player[npc.target].Center.X - npcCenter.X;
            float distY = Main.player[npc.target].Center.Y - npcCenter.Y;
            float dist = (float)Math.Sqrt(distX * distX + distY * distY);
            if (dist < 200f) ai[3] = 0f;
            if (!allowBoredom || ai[3] < ticksUntilBoredom)
            {
                npc.TargetClosest();
            }
            else
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        ++ai[0];
                        if (ai[0] >= 2.0) { npc.direction *= -1; npc.spriteDirection = npc.direction; ai[0] = 0f; }
                    }
                }
                else ai[0] = 0f;
                npc.directionY = -1;
                if (npc.direction == 0) npc.direction = 1;
            }
            if (npc.velocity.Y == 0f || npc.wet || npc.velocity.X <= 0f && npc.direction < 0 || npc.velocity.X >= 0f && npc.direction > 0)
            {
                if (npc.velocity.X < -maxSpeed || npc.velocity.X > maxSpeed)
                {
                    if (npc.velocity.Y == 0f) npc.velocity *= 0.8f;
                }
                else if (npc.velocity.X < maxSpeed && npc.direction == 1)
                {
                    npc.velocity.X += moveInterval;
                    if (npc.velocity.X > maxSpeed) npc.velocity.X = maxSpeed;
                }
                else if (npc.velocity.X > -maxSpeed && npc.direction == -1)
                {
                    npc.velocity.X -= moveInterval;
                    if (npc.velocity.X < -maxSpeed) npc.velocity.X = -maxSpeed;
                }
            }
        }

        /*
         * A cleaned up (and edited) copy of Friendly AI. (Town NPCs, Bunnies, Penguins, etc.) (AIStyle 7)
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * critter : If true, targets players when idle and follows them.
         * moveInterval : how much to move each tick.
		 * seekHome: If null, use normal behavior. If true or false, overrides normal behavior. Wether or not to seek a home.
		 * canTeleportHome : If true, npc will teleport to it's home tile if noone is within it's view range.
		 * canFindHouse : If true, the npc can search for a home. If false, it will only ever attempt to teleport to it's preset home.
		 * canOpenDoors : If true, the npc can open and close doors.
         */
        public static void AIFriendly(NPC npc, ref float[] ai, float moveInterval = 0.07f, float maxSpeed = 1f, bool critter = false, bool? seekHome = null, bool canTeleportHome = true, bool canFindHouse = true, bool canOpenDoors = true)
        {
            bool seekHouse = Main.raining;
            if (!Main.dayTime || Main.eclipse) seekHouse = true;
            if (seekHome != null) seekHouse = (bool)seekHome;
            int npcTileX = (int)(npc.position.X + (double)(npc.width / 2)) / 16;
            int npcTileY = (int)(npc.position.Y + (double)npc.height + 1.0) / 16;
            if (critter && npc.target == 255)
            {
                npc.TargetClosest();
                npc.direction = npc.Center.X < Main.player[npc.target].Center.X ? 1 : -1;
                npc.spriteDirection = npc.direction;
                if (npc.homeTileX == -1) npc.homeTileX = (int)(npc.Center.X / 16f);
            }
            bool isTalking = false;
            npc.directionY = -1;
            if (npc.direction == 0) npc.direction = 1;
            //Sets AI if the npc is speaking to a player
            for (int m1 = 0; m1 < 255; ++m1)
            {
                if (Main.player[m1].active && Main.player[m1].talkNPC == npc.whoAmI)
                {
                    isTalking = true;
                    if (ai[0] != 0f) npc.netUpdate = true;
                    ai[0] = 0f; ai[1] = 300f; ai[2] = 100f;
                    npc.direction = Main.player[m1].Center.X >= npc.Center.X ? 1 : -1;
                }
            }
            if (ai[3] > 0f)
            {
                npc.life = -1;
                npc.HitEffect();
                npc.active = false;
                if (npc.type == NPCID.OldMan)
                    SoundEngine.PlaySound(SoundID.Roar, npc.position);
            }
            //prevent a -1, -1 saving scenario
            if (npc.homeTileX == -1 && npc.homeTileY == -1 || npc.homeTileX == ushort.MaxValue && npc.homeTileY == ushort.MaxValue)
            {
                npc.homeTileX = (int)npc.Center.X / 16;
                npc.homeTileY = (int)npc.Center.Y / 16;
            }
            int homeTileY = npc.homeTileY;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.homeTileY > 0)
            {
                while (!WorldGen.SolidTile(npc.homeTileX, homeTileY) && homeTileY < Main.maxTilesY - 20)
                    ++homeTileY;
            }
            //handle teleporting to the home tile
            if (Main.netMode != NetmodeID.MultiplayerClient && canTeleportHome && seekHouse && (npcTileX != npc.homeTileX || npcTileY != homeTileY) && !npc.homeless)
            {
                bool moveToHome = true;
                for (int m2 = 0; m2 < 2; ++m2)
                {
                    Rectangle checkRect = new((int)(npc.Center.X - NPC.sWidth / 2 - NPC.safeRangeX), (int)(npc.Center.Y - NPC.sHeight / 2 - NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                    if (m2 == 1)
                        checkRect = new Rectangle(npc.homeTileX * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, homeTileY * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                    for (int m3 = 0; m3 < 255; m3++)
                    {
                        if (Main.player[m3].active && Main.player[m3].Hitbox.Intersects(checkRect)) { moveToHome = false; break; }
                        if (!moveToHome) break;
                    }
                }
                if (moveToHome)
                {
                    if (!Collision.SolidTiles(npc.homeTileX - 1, npc.homeTileX + 1, homeTileY - 3, homeTileY - 1)) //either move to preestablished home..
                    {
                        npc.velocity.X = 0.0f;
                        npc.velocity.Y = 0.0f;
                        npc.position.X = npc.homeTileX * 16 + 8 - npc.width / 2;
                        npc.position.Y = homeTileY * 16 - npc.height - 0.1f;
                        npc.netUpdate = true;
                    }
                    else //or find a new one
                    if (canFindHouse)
                    {
                        npc.homeless = true;
                        WorldGen.QuickFindHome(npc.whoAmI);
                    }
                }
            }
            //slow down
            if (ai[0] == 0f)
            {
                if (ai[2] > 0f) --ai[2];
                if (seekHouse && !isTalking && !critter)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //stop at the home tile
                        if (npcTileX == npc.homeTileX && npcTileY == homeTileY)
                        {
                            if (npc.velocity.X != 0f) npc.netUpdate = true;
                            if (npc.velocity.X > 0.1f) npc.velocity.X -= 0.1f;
                            else if (npc.velocity.X < -0.1f) npc.velocity.X += 0.1f;
                            else npc.velocity.X = 0f;
                        }
                        else
                        {
                            npc.direction = npcTileX <= npc.homeTileX ? 1 : -1;
                            ai[0] = 1f; ai[1] = 200 + Main.rand.Next(200); ai[2] = 0f;
                            npc.netUpdate = true;
                        }
                    }
                }
                else
                {
                    //just stop in general
                    if (npc.velocity.X > 0.1f) npc.velocity.X -= 0.1f;
                    else if (npc.velocity.X < -0.1f) npc.velocity.X += 0.1f;
                    else npc.velocity.X = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (ai[1] > 0) --ai[1];
                        if (ai[1] <= 0)
                        {
                            ai[0] = 1f;
                            ai[1] = 200 + Main.rand.Next(200);
                            if (critter) ai[1] += Main.rand.Next(200, 400);
                            ai[2] = 0f;
                            npc.netUpdate = true;
                        }
                    }
                }
                if (Main.netMode == NetmodeID.MultiplayerClient || seekHouse && (npcTileX != npc.homeTileX || npcTileY != homeTileY)) return;
                //move towards the home point
                if (npcTileX < npc.homeTileX - 25 || npcTileX > npc.homeTileX + 25)
                {
                    if (ai[2] != 0f) return;
                    if (npcTileX < npc.homeTileX - 50 && npc.direction == -1) { npc.direction = 1; npc.netUpdate = true; }
                    else
                    { if (npcTileX <= npc.homeTileX + 50 || npc.direction != 1) return; npc.direction = -1; npc.netUpdate = true; }
                }
                else
                {
                    if (!Main.rand.NextBool(80) || (double)ai[2] != 0) return;
                    ai[2] = 200f;
                    npc.direction *= -1;
                    npc.netUpdate = true;
                }
            }
            else
            if (ai[0] != 1)
            {
            }
            else
            //move around within the home
            if (Main.netMode != NetmodeID.MultiplayerClient && !critter && seekHouse && npcTileX == npc.homeTileX && npcTileY == npc.homeTileY)
            {
                ai[0] = 0f; ai[1] = 200 + Main.rand.Next(200); ai[2] = 60f;
                npc.netUpdate = true;
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && !npc.homeless && !Main.tileDungeon[Main.tile[npcTileX, npcTileY].TileType] && (npcTileX < npc.homeTileX - 35 || npcTileX > npc.homeTileX + 35))
                {
                    if (npc.Center.X < npc.homeTileX * 16 && npc.direction == -1)
                        ai[1] -= 5f;
                    else if (npc.Center.X > npc.homeTileX * 16 && npc.direction == 1)
                        ai[1] -= 5f;
                }
                --ai[1];
                if (ai[1] <= 0f)
                {
                    ai[0] = 0f; ai[1] = 300 + Main.rand.Next(300);
                    if (critter) ai[1] -= Main.rand.Next(100);
                    ai[2] = 60f; npc.netUpdate = true;
                }
                //close doors the npc has opened
                if (npc.closeDoor && (npc.Center.X / 16f > npc.doorX + 2 || npc.Center.X / 16f < npc.doorX - 2))
                {
                    if (WorldGen.CloseDoor(npc.doorX, npc.doorY))
                    {
                        npc.closeDoor = false;
                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, NetworkText.FromLiteral(""), 1, npc.doorX, npc.doorY, npc.direction);
                    }
                    if (npc.Center.X / 16f > npc.doorX + 4 || npc.Center.X / 16f < npc.doorX - 4 || npc.Center.Y / 16f > npc.doorY + 4 || npc.Center.Y / 16f < npc.doorY - 4)
                        npc.closeDoor = false;
                }
                if (npc.Center.X < -maxSpeed || npc.velocity.X > maxSpeed)
                {
                    if (npc.velocity.Y == 0) npc.velocity *= 0.8f;
                }
                else if (npc.velocity.X < maxSpeed && npc.direction == 1)
                {
                    npc.velocity.X += moveInterval;
                    if (npc.velocity.X > maxSpeed) npc.velocity.X = maxSpeed;
                }
                else if (npc.velocity.X > -maxSpeed && npc.direction == -1)
                {
                    npc.velocity.X -= moveInterval;
                    if (npc.velocity.X > maxSpeed) npc.velocity.X = maxSpeed;
                }
                WalkupHalfBricks(npc);
                if (npc.velocity.Y != 0f) return;
                if (npc.position.X == ai[2]) npc.direction *= -1;
                ai[2] = -1f;
                int tileX2 = (int)((npc.Center.X + 15 * npc.direction) / 16f);
                int tileY2 = (int)((npc.position.Y + npc.height - 16f) / 16f);
                //Main.tile[tileX2 - npc.direction, tileY2 + 1].halfBrick();
                if (canOpenDoors && Main.tile[tileX2, tileY2 - 2].HasUnactuatedTile && Main.tile[tileX2, tileY2 - 2].TileType == 10 && (Main.rand.NextBool(10) || seekHouse))
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        return;
                    //attempt to open the door...
                    if (WorldGen.OpenDoor(tileX2, tileY2 - 2, npc.direction))
                    {
                        npc.closeDoor = true;
                        npc.doorX = tileX2;
                        npc.doorY = tileY2 - 2;
                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, NetworkText.FromLiteral(""), 0, tileX2, tileY2 - 2, npc.direction);
                        npc.netUpdate = true;
                        ai[1] += 80f;
                        //if that fails, attempt to open it the other way...
                    }
                    else if (WorldGen.OpenDoor(tileX2, tileY2 - 2, -npc.direction))
                    {
                        npc.closeDoor = true;
                        npc.doorX = tileX2;
                        npc.doorY = tileY2 - 2;
                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, NetworkText.FromLiteral(""), 0, tileX2, tileY2 - 2, -npc.direction);
                        npc.netUpdate = true;
                        ai[1] += 80f;
                        //if it still fails, you can't open the door, so turn around
                    }
                    else
                    {
                        npc.direction *= -1;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    if (npc.velocity.X < 0f && npc.spriteDirection == -1 || npc.velocity.X > 0f && npc.spriteDirection == 1)
                    {
                        if (Main.tile[tileX2, tileY2 - 2].HasUnactuatedTile && Main.tileSolid[Main.tile[tileX2, tileY2 - 2].TileType] && !Main.tileSolidTop[Main.tile[tileX2, tileY2 - 2].TileType])
                        {
                            if (npc.direction == 1 && !Collision.SolidTiles(tileX2 - 2, tileX2 - 1, tileY2 - 5, tileY2 - 1) || npc.direction == -1 && !Collision.SolidTiles(tileX2 + 1, tileX2 + 2, tileY2 - 5, tileY2 - 1))
                            {
                                if (!Collision.SolidTiles(tileX2, tileX2, tileY2 - 5, tileY2 - 3))
                                {
                                    npc.velocity.Y = -6f; npc.netUpdate = true;
                                }
                                else { npc.direction *= -1; npc.netUpdate = true; }
                            }
                            else { npc.direction *= -1; npc.netUpdate = true; }
                        }
                        else if (Main.tile[tileX2, tileY2 - 1].HasUnactuatedTile && Main.tileSolid[Main.tile[tileX2, tileY2 - 1].TileType] && !Main.tileSolidTop[Main.tile[tileX2, tileY2 - 1].TileType])
                        {
                            if (npc.direction == 1 && !Collision.SolidTiles(tileX2 - 2, tileX2 - 1, tileY2 - 4, tileY2 - 1) || npc.direction == -1 && !Collision.SolidTiles(tileX2 + 1, tileX2 + 2, tileY2 - 4, tileY2 - 1))
                            {
                                if (!Collision.SolidTiles(tileX2, tileX2, tileY2 - 4, tileY2 - 2))
                                {
                                    npc.velocity.Y = -5f; npc.netUpdate = true;
                                }
                                else { npc.direction *= -1; npc.netUpdate = true; }
                            }
                            else { npc.direction *= -1; npc.netUpdate = true; }
                        }
                        else if (npc.position.Y + npc.height - tileY2 * 16f > 20f)
                        {
                            if (Main.tile[tileX2, tileY2].HasUnactuatedTile && Main.tileSolid[Main.tile[tileX2, tileY2].TileType] && Main.tile[tileX2, tileY2].Slope == 0)
                            {
                                if (npc.direction == 1 && !Collision.SolidTiles(tileX2 - 2, tileX2, tileY2 - 3, tileY2 - 1) || npc.direction == -1 && !Collision.SolidTiles(tileX2, tileX2 + 2, tileY2 - 3, tileY2 - 1))
                                {
                                    npc.velocity.Y = -4.4f;
                                    npc.netUpdate = true;
                                }
                                else { npc.direction *= -1; npc.netUpdate = true; }
                            }
                        }
                        try
                        {
                            if (!critter && npcTileX >= npc.homeTileX - 35 && npcTileX <= npc.homeTileX + 35 && (!Main.tile[tileX2, tileY2 + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX2, tileY2 + 1].TileType]) && (!Main.tile[tileX2 - npc.direction, tileY2 + 1].HasTile || !Main.tileSolid[Main.tile[tileX2 - npc.direction, tileY2 + 1].TileType]) && (!Main.tile[tileX2, tileY2 + 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX2, tileY2 + 2].TileType]) && (!Main.tile[tileX2 - npc.direction, tileY2 + 2].HasTile || !Main.tileSolid[Main.tile[tileX2 - npc.direction, tileY2 + 2].TileType]) && (!Main.tile[tileX2, tileY2 + 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX2, tileY2 + 3].TileType]) && (!Main.tile[tileX2 - npc.direction, tileY2 + 3].HasTile || !Main.tileSolid[Main.tile[tileX2 - npc.direction, tileY2 + 3].TileType]) && (!Main.tile[tileX2, tileY2 + 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX2, tileY2 + 4].TileType]) && (!Main.tile[tileX2 - npc.direction, tileY2 + 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX2 - npc.direction, tileY2 + 4].TileType]))
                            {
                                npc.direction *= -1;
                                npc.velocity.X *= -1f;
                                npc.netUpdate = true;
                            }
                        }
                        catch { }
                        if ((double)npc.velocity.Y < 0f) ai[2] = npc.position.X;
                    }
                    if (npc.velocity.Y < 0f && npc.wet) npc.velocity.Y *= 1.2f;
                    npc.velocity.Y *= 1.2f;
                }
            }
        }


        /*
		 * A cleaned up (and edited) copy of Eater of Souls AI. (EoS, Corruptor, etc.) (AIStyle 5)
		 * 
		 * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * moveInterval : how much to move each tick.
		 * distanceDivider : The amount that is divided by the distance; determines velocity.
		 * bounceScalar : scalar for how big a 'bounce' is if the npc hits a tile.
		 * fleeAtDay : If true, npc will flee if it becomes day.
		 * ignoreWet : If true, npc will ignore being wet.
		 */
        public static void AIEater(NPC npc, ref float[] ai, float moveInterval = 0.022f, float distanceDivider = 4.2f, float bounceScalar = 0.7f, bool fleeAtDay = false, bool ignoreWet = false)
        {
            if (npc.target is < 0 or byte.MaxValue || Main.player[npc.target].dead) { npc.TargetClosest(); }
            float distX = Main.player[npc.target].Center.X;
            float distY = Main.player[npc.target].Center.Y;
            Vector2 npcCenter = npc.Center;
            float distDx = (int)(distX / 8f) * 8f;
            float distDy = (int)(distY / 8f) * 8f;
            npcCenter.X = (int)(npcCenter.X / 8f) * 8f;
            npcCenter.Y = (int)(npcCenter.Y / 8f) * 8f;
            float distX2 = distDx - npcCenter.X;
            float distY2 = distDy - npcCenter.Y;
            float dist = (float)Math.Sqrt(distX2 * distX2 + distY2 * distY2);
            float speedX1;
            float speedY1;
            if (dist == 0f)
            {
                speedX1 = npc.velocity.X;
                speedY1 = npc.velocity.Y;
            }
            else
            {
                float distScalar = distanceDivider / dist;
                speedX1 = distX2 * distScalar;
                speedY1 = distY2 * distScalar;
            }
            ++ai[0];
            if (ai[0] > 0f) { npc.velocity.Y += 23f / 1000f; } else { npc.velocity.Y -= 23f / 1000f; }
            if (ai[0] < -100f || (double)ai[0] > 100f) { npc.velocity.X += 23f / 1000f; } else { npc.velocity.X -= 23f / 1000f; }
            if (ai[0] > 200f) { ai[0] = -200f; }
            if (dist < 150f) { npc.velocity.X += speedX1 * 0.007f; npc.velocity.Y += speedY1 * 0.007f; }
            if (Main.player[npc.target].dead)
            {
                speedX1 = npc.direction * distanceDivider / 2f;
                speedY1 = -distanceDivider / 2f;
            }
            if (npc.velocity.X < speedX1) { npc.velocity.X += moveInterval; }
            else
            if (npc.velocity.X > speedX1) { npc.velocity.X -= moveInterval; }
            if (npc.velocity.Y < speedY1) { npc.velocity.Y += moveInterval; }
            else
            if (npc.velocity.Y > speedY1) { npc.velocity.Y -= moveInterval; }
            npc.rotation = (float)Math.Atan2(speedY1, speedX1) - 1.57f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -bounceScalar;
                if (npc.direction == -1 && npc.velocity.X is > 0f and < 2f) { npc.velocity.X = 2f; }
                if (npc.direction == 1 && npc.velocity.X is < 0f and > -2f) { npc.velocity.X = -2f; }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -bounceScalar;
                if (npc.velocity.Y is > 0f and < 1.5f) { npc.velocity.Y = 2f; }
                if (npc.velocity.Y is < 0f and > -1.5f) { npc.velocity.Y = -2f; }
            }
            if (!ignoreWet && npc.wet)
            {
                if (npc.velocity.Y > 0f) { npc.velocity.Y *= 0.95f; }
                npc.velocity.Y -= 0.3f;
                if (npc.velocity.Y < -2f) { npc.velocity.Y = -2f; }
            }
            if (fleeAtDay && Main.dayTime || Main.player[npc.target].dead)
            {
                npc.velocity.Y -= moveInterval * 2f;
                if (npc.timeLeft > 10) { npc.timeLeft = 10; }
            }
            if ((npc.velocity.X <= 0f || npc.oldVelocity.X >= 0f) && (npc.velocity.X >= 0f || npc.oldVelocity.X <= 0f) && (npc.velocity.Y <= 0f || npc.oldVelocity.Y >= 0f) && (npc.velocity.Y >= 0.0 || npc.oldVelocity.Y <= 0f) || npc.justHit)
                return;
            npc.netUpdate = true;
        }

        /*
		 * A cleaned up (and edited) copy of Wheel AI. (Blazing Wheel) (AIStyle 21)
		 * 
		 * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * moveInterval : How much to move each tick (NOTE: very high numbers can result in the AI breaking!)
		 * rotate : If true, rotates the npc like a Blazing Wheel.
		 */
        public static void AIWheel(NPC npc, ref float[] ai, float moveInterval = 6f, bool rotate = false)
        {
            if (ai[0] == 0f)
            {
                npc.TargetClosest();
                npc.directionY = 1;
                ai[0] = 1f;
            }
            if (ai[1] == 0f)
            {
                if (rotate) { npc.rotation += npc.direction * npc.directionY * 0.13f; }
                if (npc.collideY) ai[0] = 2f;
                if (!npc.collideY && ai[0] == 2f)
                {
                    npc.direction = -npc.direction;
                    ai[1] = 1f;
                    ai[0] = 1f;
                }
                if (npc.collideX) { npc.directionY = -npc.directionY; ai[1] = 1f; }
            }
            else
            {
                if (rotate) { npc.rotation -= npc.direction * npc.directionY * 0.13f; }
                if (npc.collideX) ai[0] = 2f;
                if (!npc.collideX && ai[0] == 2f)
                {
                    npc.directionY = -npc.directionY;
                    ai[1] = 0f;
                    ai[0] = 1f;
                }
                if (npc.collideY) { npc.direction = -npc.direction; ai[1] = 0.0f; }
            }
            npc.velocity.X = moveInterval * npc.direction;
            npc.velocity.Y = moveInterval * npc.directionY;
        }

        /*
		 * A cleaned up (and edited) copy of Spider Wallwalking AI. (Blood Crawler, etc.) (AIStyle 40)
		 * 
		 * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * ignoreSight : If true, ignores if the npc can see the player or not.
		 * moveInterval : how much to move each tick.
		 * slowSpeed : how fast the npc much go before you begin to slow it down.
		 * maxSpeed : the max speed the npc can go.
		 * distanceDivider : the amount that is divided by the distance; determins velocity.
		 * bounceScalar : scalar for how big a 'bounce' is if the npc hits a tile.
		 * transformType : if not -1, will check if there's a wall behind the npc and if there is not, will change projectile npc to the type provided.
		 */
        public static void AISpider(NPC npc, ref float[] ai, bool ignoreSight = false, float moveInterval = 0.08f, float slowSpeed = 1.5f, float maxSpeed = 3f, float distanceDivider = 2f, float bounceScalar = 0.5f, int transformType = -1)
        {
            if (npc.target is < 0 or 255 || Main.player[npc.target].dead)
                npc.TargetClosest();
            Vector2 npcCenter = npc.Center;
            float distX = Main.player[npc.target].Center.X;
            float distY = Main.player[npc.target].Center.Y;
            float distDx = (int)(distX / 8f) * 8f;
            float distDy = (int)(distY / 8f) * 8f;
            npcCenter.X = (int)(npcCenter.X / 8f) * 8f;
            npcCenter.Y = (int)(npcCenter.Y / 8f) * 8f;
            float distX2 = distDx - npcCenter.X;
            float distY2 = distDy - npcCenter.Y;
            float dist = (float)Math.Sqrt(distX2 * distX2 + distY2 * distY2);
            float velX;
            float velY;
            if (dist == 0f)
            {
                velX = npc.velocity.X;
                velY = npc.velocity.Y;
            }
            else
            {
                float speedMult = distanceDivider / dist;
                velX = distX2 * speedMult;
                velY = distY2 * speedMult;
            }
            if (Main.player[npc.target].dead)
            {
                velX = npc.direction * distanceDivider / 2f;
                velY = -distanceDivider / 2f;
            }
            npc.spriteDirection = -1;
            if (!ignoreSight && !Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
            {
                ++ai[0];
                if (ai[0] > 0f) { npc.velocity.Y += 23f / 1000f; } else { npc.velocity.Y -= 23f / 1000f; }
                if (ai[0] < -100f || (double)ai[0] > 100f) { npc.velocity.X += 23f / 1000f; } else { npc.velocity.X -= 23f / 1000f; }
                if (ai[0] > 200f) { ai[0] = -200f; }
                npc.velocity.X += velX * 0.007f;
                npc.velocity.Y += velY * 0.007f;
                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);
                if (npc.velocity.X > slowSpeed || npc.velocity.X < -slowSpeed) { npc.velocity.X *= 0.9f; }
                if (npc.velocity.Y > slowSpeed || npc.velocity.Y < -slowSpeed) { npc.velocity.Y *= 0.9f; }
                if (npc.velocity.X > maxSpeed) { npc.velocity.X = maxSpeed; }
                else
                if (npc.velocity.X < -maxSpeed) { npc.velocity.X = -maxSpeed; }
                if (npc.velocity.Y > maxSpeed) { npc.velocity.Y = maxSpeed; }
                else
                if (npc.velocity.Y < -maxSpeed) { npc.velocity.Y = -maxSpeed; }
            }
            else
            {
                if (npc.velocity.X < (double)velX)
                {
                    npc.velocity.X += moveInterval;
                    if ((double)npc.velocity.X < 0 && (double)velX > 0)
                        npc.velocity.X += moveInterval;
                }
                else if (npc.velocity.X > (double)velX)
                {
                    npc.velocity.X -= moveInterval;
                    if ((double)npc.velocity.X > 0 && (double)velX < 0)
                        npc.velocity.X -= moveInterval;
                }
                if (npc.velocity.Y < (double)velY)
                {
                    npc.velocity.Y += moveInterval;
                    if ((double)npc.velocity.Y < 0 && (double)velY > 0)
                        npc.velocity.Y += moveInterval;
                }
                else if (npc.velocity.Y > (double)velY)
                {
                    npc.velocity.Y -= moveInterval;
                    if ((double)npc.velocity.Y > 0 && (double)velY < 0)
                        npc.velocity.Y -= moveInterval;
                }
                npc.rotation = (float)Math.Atan2(velY, velX);
            }
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -bounceScalar;
                if (npc.direction == -1 && npc.velocity.X is > 0f and < 2f) { npc.velocity.X = 2f; }
                if (npc.direction == 1 && npc.velocity.X is < 0f and > -2f) { npc.velocity.X = -2f; }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -bounceScalar;
                if (npc.velocity.Y is > 0f and < 1.5f) { npc.velocity.Y = 2f; }
                if (npc.velocity.Y is < 0f and > -1.5f) { npc.velocity.Y = -2f; }
            }
            if ((npc.velocity.X > 0f && npc.oldVelocity.X < 0f || npc.velocity.X < 0f && npc.oldVelocity.X > 0f || npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f || npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f) && !npc.justHit)
                npc.netUpdate = true;
            if (Main.netMode != NetmodeID.MultiplayerClient && transformType != -1)
            {
                int centerTileX = (int)npc.Center.X / 16;
                int centerTileY = (int)npc.Center.Y / 16;
                bool wallExists = false;
                for (int x = centerTileX - 1; x <= centerTileX + 1; ++x)
                {
                    for (int y = centerTileY - 1; y <= centerTileY + 1; ++y)
                    {
                        if (Main.tile[x, y].WallType > 0) { wallExists = true; break; }
                    }
                }
                if (!wallExists) npc.Transform(transformType);
            }
        }

        /*
		 * A cleaned up (and edited) copy of Skull AI. (Cursed Skull) (AIStyle 10)
		 * 
		 * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * tacklePlayer : If true, the npc will occasionally charge at the player.
		 * maxDistanceAmt : The maxmimum amount of 'distance' the npc is allowed to wander to from the player.
		 * maxDistance : The maximum amount of distance from the player until the npc begins to speed up.
		 * increment : the amount to move per tick.
		 * closeIncrement : the amount to move per tick when close to the player.
		 */
        public static void AISkull(NPC npc, ref float[] ai, bool tacklePlayer = true, float maxDistanceAmt = 4f, float maxDistance = 350f, float increment = 0.011f, float closeIncrement = 0.019f)
        {
            float distanceAmt = 1f;
            npc.TargetClosest();
            float distX = Main.player[npc.target].Center.X - npc.Center.X;
            float distY = Main.player[npc.target].Center.Y - npc.Center.Y;
            float dist = (float)Math.Sqrt(distX * distX + distY * distY);
            ai[1] += 1f;
            if (ai[1] > 600f)
            {
                if (tacklePlayer)
                {
                    increment *= 8f;
                    distanceAmt = 4f;
                    if (ai[1] > 650f) { ai[1] = 0f; }
                }
                else { ai[1] = 0f; }
            }
            else
            if (dist < 250f)
            {
                ai[0] += 0.9f;
                if (ai[0] > 0f) { npc.velocity.Y += closeIncrement; } else { npc.velocity.Y -= closeIncrement; }
                if (ai[0] < -100f || ai[0] > 100f) { npc.velocity.X += closeIncrement; } else { npc.velocity.X -= closeIncrement; }
                if (ai[0] > 200f) { ai[0] = -200f; }
            }
            if (dist > maxDistance)
            {
                distanceAmt = maxDistanceAmt + maxDistanceAmt / 4f;
                increment = 0.3f;
            }
            else
            if (dist > maxDistance - maxDistance / 7f)
            {
                distanceAmt = maxDistanceAmt - maxDistanceAmt / 4f;
                increment = 0.2f;
            }
            else
            if (dist > maxDistance - 2 * (maxDistance / 7f))
            {
                distanceAmt = maxDistanceAmt / 2.66f;
                increment = 0.1f;
            }
            dist = distanceAmt / dist;
            distX *= dist; distY *= dist;
            if (Main.player[npc.target].dead)
            {
                distX = npc.direction * distanceAmt / 2f;
                distY = -distanceAmt / 2f;
            }
            if (npc.velocity.X < distX) { npc.velocity.X += increment; }
            else
            if (npc.velocity.X > distX) { npc.velocity.X -= increment; }
            if (npc.velocity.Y < distY) { npc.velocity.Y += increment; }
            else
            if (npc.velocity.Y > distY) { npc.velocity.Y -= increment; }
        }

        /*
		 * A cleaned up (and edited) copy of Floater AI. (Pixie, Gastropod, etc.) (AIStyle 22)
		 * 
		 * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * ignoreWet : If true, does not slow down in liquids.
		 * moveInterval : how much to move each tick.
		 * maxSpeedX/maxSpeedY : the max speed of the npc on the X and Y axis, respectively.
		 * hoverInterval : how much to hover by each tick.
		 * hoverMaxSpeed : the maximum speed to hover by.
		 * hoverHeight : the amount of tiles below an npc before it needs ground to hover over.
		 */
        public static void AIFloater(NPC npc, ref float[] ai, bool ignoreWet = false, float moveInterval = 0.2f, float maxSpeedX = 2f, float maxSpeedY = 1.5f, float hoverInterval = 0.04f, float hoverMaxSpeed = 1.5f, int hoverHeight = 3)
        {
            bool flyUpward = false;
            if (npc.justHit) { ai[2] = 0f; }
            if (ai[2] >= 0f)
            {
                int tileDist = 16;
                bool inRangeX = false;
                bool inRangeY = false;
                if (npc.position.X > ai[0] - tileDist && npc.position.X < ai[0] + tileDist) { inRangeX = true; }
                else
                    if (npc.velocity.X < 0f && npc.direction > 0 || npc.velocity.X > 0f && npc.direction < 0) { inRangeX = true; }
                tileDist += 24;
                if (npc.position.Y > ai[1] - tileDist && npc.position.Y < ai[1] + tileDist)
                {
                    inRangeY = true;
                }
                if (inRangeX && inRangeY)
                {
                    ai[2] += 1f;
                    //i'm pretty sure projectile is never called, but it's in the original so...
                    if (ai[2] >= 30f && tileDist == 16)
                    {
                        flyUpward = true;
                    }
                    if (ai[2] >= 60f)
                    {
                        ai[2] = -200f;
                        npc.direction *= -1;
                        npc.velocity.X *= -1f;
                        npc.collideX = false;
                    }
                }
                else
                {
                    ai[0] = npc.position.X;
                    ai[1] = npc.position.Y;
                    ai[2] = 0f;
                }
                npc.TargetClosest();
            }
            else
            {
                ai[2] += 1f;
                if (Main.player[npc.target].position.X + Main.player[npc.target].width / 2 > npc.position.X + npc.width / 2)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
            }
            int tileX = (int)(npc.Center.X / 16f) + npc.direction * 2;
            int tileY = (int)((npc.position.Y + npc.height) / 16f);
            bool tileBelowEmpty = true;
            for (int tY = tileY; tY < tileY + hoverHeight; tY++)
            {
                if (Main.tile[tileX, tY].HasUnactuatedTile && Main.tileSolid[Main.tile[tileX, tY].TileType] || Main.tile[tileX, tY].LiquidAmount > 0)
                {
                    tileBelowEmpty = false;
                    break;
                }
            }
            if (flyUpward)
            {
                tileBelowEmpty = true;
            }
            if (tileBelowEmpty)
            {
                npc.velocity.Y += moveInterval;
                if (npc.velocity.Y > maxSpeedY) { npc.velocity.Y = maxSpeedY; }
            }
            else
            {
                if (npc.directionY < 0 && npc.velocity.Y > 0f) { npc.velocity.Y -= moveInterval; }
                if (npc.velocity.Y < -maxSpeedY) { npc.velocity.Y = -maxSpeedY; }
            }
            if (!ignoreWet && npc.wet)
            {
                npc.velocity.Y -= moveInterval;
                if (npc.velocity.Y < -maxSpeedY * 0.75f) { npc.velocity.Y = -maxSpeedY * 0.75f; }
            }
            if (npc.collideX)
            {
                npc.velocity.X = npc.oldVelocity.X * -0.4f;
                if (npc.direction == -1 && npc.velocity.X is > 0f and < 1f) { npc.velocity.X = 1f; }
                if (npc.direction == 1 && npc.velocity.X is < 0f and > -1f) { npc.velocity.X = -1f; }
            }
            if (npc.collideY)
            {
                npc.velocity.Y = npc.oldVelocity.Y * -0.25f;
                if (npc.velocity.Y is > 0f and < 1f) { npc.velocity.Y = 1f; }
                if (npc.velocity.Y is < 0f and > -1f) { npc.velocity.Y = -1f; }
            }
            if (npc.direction == -1 && npc.velocity.X > -maxSpeedX)
            {
                npc.velocity.X -= moveInterval * 0.5f;
                if (npc.velocity.X > maxSpeedX) { npc.velocity.X -= 0.1f; }
                else
                    if (npc.velocity.X > 0f) { npc.velocity.X += 0.05f; }
                if (npc.velocity.X < -maxSpeedX) { npc.velocity.X = -maxSpeedX; }
            }
            else
                if (npc.direction == 1 && npc.velocity.X < maxSpeedX)
            {
                npc.velocity.X += moveInterval * 0.5f;
                if (npc.velocity.X < -maxSpeedX) { npc.velocity.X += 0.1f; }
                else
                    if (npc.velocity.X < 0f) { npc.velocity.X -= 0.05f; }
                if (npc.velocity.X > maxSpeedX) { npc.velocity.X = maxSpeedX; }
            }
            if (npc.directionY == -1 && (double)npc.velocity.Y > -hoverMaxSpeed)
            {
                npc.velocity.Y -= hoverInterval;
                if ((double)npc.velocity.Y > hoverMaxSpeed) { npc.velocity.Y -= 0.05f; }
                else
                    if (npc.velocity.Y > 0f) { npc.velocity.Y += hoverInterval - 0.01f; }
                if ((double)npc.velocity.Y < -hoverMaxSpeed) { npc.velocity.Y = -hoverMaxSpeed; }
            }
            else
                if (npc.directionY == 1 && (double)npc.velocity.Y < hoverMaxSpeed)
            {
                npc.velocity.Y += hoverInterval;
                if ((double)npc.velocity.Y < -hoverMaxSpeed) { npc.velocity.Y += 0.05f; }
                else
                    if (npc.velocity.Y < 0f) { npc.velocity.Y -= hoverInterval - 0.01f; }
                if ((double)npc.velocity.Y > hoverMaxSpeed) { npc.velocity.Y = hoverMaxSpeed; }
            }
        }

        /*
		 * A cleaned up (and edited) copy of Flier AI. (Bat, Demon, etc.) (AIStyle 14)
		 * 
		 * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * sporadic : If true, npc will overshoot targets.
		 * maxSpeedX/maxSpeedY : the max speed of the npc on the X and Y axis, respectively.
		 * slowdownIncrementX/slowdownIncrementY : the slowdown increment on the X and Y axis, respectively.
		 */
        public static void AIFlier(NPC npc, ref float[] ai, bool sporadic = true, float moveIntervalX = 0.1f, float moveIntervalY = 0.04f, float maxSpeedX = 4f, float maxSpeedY = 1.5f, bool canBeBored = true, int timeUntilBoredom = 300)
        {
            if (npc.collideX)
            {
                npc.velocity.X = npc.oldVelocity.X * -0.5f;
                float max = maxSpeedX * 0.5f;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < max) { npc.velocity.X = max; }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -max) { npc.velocity.X = -max; }
            }
            if (npc.collideY)
            {
                npc.velocity.Y = npc.oldVelocity.Y * -0.5f;
                float max = maxSpeedY * 0.66f;
                if (npc.velocity.Y > 0f && npc.velocity.Y < max) { npc.velocity.Y = max; }
                if (npc.velocity.Y < 0f && npc.velocity.Y > -max) { npc.velocity.Y = -max; }
            }
            npc.TargetClosest();
            void move()
            {
                if (npc.direction == -1 && npc.velocity.X > -maxSpeedX)
                {
                    npc.velocity.X -= moveIntervalX;
                    if (npc.velocity.X > maxSpeedX) { npc.velocity.X -= moveIntervalX; }
                    else
                    if (npc.velocity.X > 0f) { npc.velocity.X += moveIntervalX * 0.5f; }
                    if (npc.velocity.X < -maxSpeedX) { npc.velocity.X = -maxSpeedX; }
                }
                else
                if (npc.direction == 1 && npc.velocity.X < maxSpeedX)
                {
                    npc.velocity.X += moveIntervalX;
                    if (npc.velocity.X < -maxSpeedX) { npc.velocity.X += moveIntervalX; }
                    else
                    if (npc.velocity.X < 0f) { npc.velocity.X -= moveIntervalX * 0.5f; }
                    if (npc.velocity.X > maxSpeedX) { npc.velocity.X = maxSpeedX; }
                }
                if (npc.directionY == -1 && (double)npc.velocity.Y > -maxSpeedY)
                {
                    npc.velocity.Y -= moveIntervalY;
                    if ((double)npc.velocity.Y > maxSpeedY) { npc.velocity.Y -= moveIntervalY; }
                    else
                    if (npc.velocity.Y > 0f) { npc.velocity.Y += moveIntervalY * 0.5f; }
                    if ((double)npc.velocity.Y < -maxSpeedY) { npc.velocity.Y = -maxSpeedY; }
                }
                else
                if (npc.directionY == 1 && (double)npc.velocity.Y < maxSpeedY)
                {
                    npc.velocity.Y += moveIntervalY;
                    if ((double)npc.velocity.Y < -maxSpeedY) { npc.velocity.Y += moveIntervalY; }
                    else
                    if (npc.velocity.Y < 0f) { npc.velocity.Y -= moveIntervalY * 0.5f; }
                    if ((double)npc.velocity.Y > maxSpeedY) { npc.velocity.Y = maxSpeedY; }
                }
            }
            if (canBeBored) { ai[0] += 1f; }
            if (canBeBored && ai[0] > timeUntilBoredom)
            {
                if (!Main.player[npc.target].wet && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    ai[0] = 0f;
                }
                if (ai[0] > timeUntilBoredom * 2) { ai[0] = 0f; }
                npc.direction = Main.player[npc.target].Center.X < npc.Center.X ? 1 : -1;
                npc.directionY = Main.player[npc.target].Center.Y < npc.Center.Y ? 1 : -1;
                move();
            }
            else
            {
                move();
                if (sporadic)
                {
                    if (npc.wet)
                    {
                        if (npc.velocity.Y > 0f) { npc.velocity.Y *= 0.95f; }
                        npc.velocity.Y -= 0.5f;
                        if (npc.velocity.Y < -maxSpeedX) { npc.velocity.Y = -maxSpeedX; }
                        npc.TargetClosest();
                    }
                    move();
                }
            }
        }

        /*
		 * A cleaned up (and edited) copy of Plant AI. (Man Eater, etc.) (AIStyle 13)
		 * 
		 * ai : A float array that stores AI data. (Note projectile array should be synced!)
		 * checkTilePoint : If true, check if the tile the npc is connected to is there. If it isn't, kill the npc.
		 * endPoint : the end point of the npc. 
		 * isTilePos : true if the endPoint is an tile coordinates, false if it's an npc position.
		 * vineLength : the distance from the endPoint the npc can go up to.
		 * vineTimeExtend : the time until the vine's distance is extended.
		 * vineTimeMax : the time until the vine's distance is retracted (must be greater than vineTimeExtend).
		 * moveInterval : the increment to move by.
		 * speedMax : the max speed of the npc.
		 * targetOffset : A vector2 representing an 'offset' from the target, allows for some variation and misaccuracy.
		 */
        public static void AIPlant(NPC npc, ref float[] ai, bool checkTilePoint = true, Vector2 endPoint = default, bool isTilePos = true, float vineLength = 150f, float vineLengthLong = 200f, int vineTimeExtend = 300, int vineTimeMax = 450, float moveInterval = 0.035f, float speedMax = 2f, Vector2 targetOffset = default)
        {
            if (endPoint != default)
            {
                ai[0] = endPoint.X;
                ai[1] = endPoint.Y;
            }
            if (checkTilePoint)
            {
                Vector2 tilePos = isTilePos ? new Vector2(ai[0], ai[1]) : new Vector2(ai[0] / 16f, ai[1] / 16f);
                int tx = (int)tilePos.X; int ty = (int)tilePos.Y;
                if (!Main.tile[tx, ty].HasUnactuatedTile || !Main.tileSolid[Main.tile[tx, ty].TileType] || Main.tileSolid[Main.tile[tx, ty].TileType] && Main.tileSolidTop[Main.tile[tx, ty].TileType])
                {
                    if (npc.DeathSound != null) SoundEngine.PlaySound((SoundStyle)npc.DeathSound, npc.Center);
                    npc.life = -1;
                    npc.HitEffect();
                    npc.active = false;
                    return;
                }
            }
            npc.TargetClosest();
            ai[2] += 1f;
            if (ai[2] > vineTimeExtend)
            {
                vineLength = vineLengthLong;
                if (ai[2] > vineTimeMax) { ai[2] = 0f; }
            }
            Vector2 endPointCenter = isTilePos ? new Vector2(ai[0] * 16f + 8f, ai[1] * 16f + 8f) : new Vector2(ai[0], ai[1]);
            Vector2 playerCenter = Main.player[npc.target].Center + targetOffset;
            float distPlayerX = playerCenter.X - npc.width / 2 - endPointCenter.X;
            float distPlayerY = playerCenter.Y - npc.height / 2 - endPointCenter.Y;
            float distPlayer = (float)Math.Sqrt(distPlayerX * distPlayerX + distPlayerY * distPlayerY);
            if (distPlayer > vineLength)
            {
                distPlayer = vineLength / distPlayer;
                distPlayerX *= distPlayer;
                distPlayerY *= distPlayer;
            }
            if (npc.position.X < endPointCenter.X + distPlayerX)
            {
                npc.velocity.X += moveInterval;
                if (npc.velocity.X < 0f && distPlayerX > 0f) { npc.velocity.X += moveInterval * 1.5f; }
            }
            else
            if (npc.position.X > endPointCenter.X + distPlayerX)
            {
                npc.velocity.X -= moveInterval;
                if (npc.velocity.X > 0f && distPlayerX < 0f) { npc.velocity.X -= moveInterval * 1.5f; }
            }
            if (npc.position.Y < endPointCenter.Y + distPlayerY)
            {
                npc.velocity.Y += moveInterval;
                if (npc.velocity.Y < 0f && distPlayerY > 0f) { npc.velocity.Y += moveInterval * 1.5f; }
            }
            else
            if (npc.position.Y > endPointCenter.Y + distPlayerY)
            {
                npc.velocity.Y -= moveInterval;
                if (npc.velocity.Y > 0f && distPlayerY < 0f) { npc.velocity.Y -= moveInterval * 1.5f; }
            }
            npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -speedMax, speedMax);
            npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -speedMax, speedMax);
            if (distPlayerX > 0f) { npc.spriteDirection = 1; npc.rotation = (float)Math.Atan2(distPlayerY, distPlayerX); }
            else
            if (distPlayerX < 0f) { npc.spriteDirection = -1; npc.rotation = (float)Math.Atan2(distPlayerY, distPlayerX) + 3.14f; }
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -0.7f;
                npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -speedMax, speedMax);
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -0.7f;
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -speedMax, speedMax);
            }
        }

        public static void AIWorm(NPC npc, int[] wormTypes, int wormLength = 3, float partDistanceAddon = 0f, float maxSpeed = 8f, float gravityResist = 0.07f, bool fly = false, bool split = false, bool ignoreTiles = false, bool spawnTileDust = true, bool soundEffects = true, bool rotateAverage = false, Action<NPC, int, bool> onChangeType = null)
        {
            bool diggingDummy = false;
            AIWorm(npc, ref diggingDummy, wormTypes, wormLength, partDistanceAddon, maxSpeed, gravityResist, fly, split, ignoreTiles, spawnTileDust, soundEffects, rotateAverage, onChangeType);
        }

        public static void AIWorm(NPC npc, ref bool isDigging, int[] wormTypes, int wormLength = 3, float partDistanceAddon = 0f, float maxSpeed = 8f, float gravityResist = 0.07f, bool fly = false, bool split = false, bool ignoreTiles = false, bool spawnTileDust = true, bool soundEffects = true, bool rotateAverage = false, Action<NPC, int, bool> onChangeType = null)
        {
            int[] wtypes = new int[wormTypes.Length == 1 ? 1 : wormLength];
            wtypes[0] = wormTypes[0];
            if (wormTypes.Length > 1)
            {
                wtypes[^1] = wormTypes[2];
                for (int m = 1; m < wtypes.Length - 1; m++)
                {
                    wtypes[m] = wormTypes[1];
                }
            }
            AIWorm(npc, ref isDigging, wtypes, partDistanceAddon, maxSpeed, gravityResist, fly, split, ignoreTiles, spawnTileDust, soundEffects, rotateAverage, onChangeType);
        }

        public static void AIWorm(NPC npc, int[] wormTypes, float partDistanceAddon = 0f, float maxSpeed = 8f, float gravityResist = 0.07f, bool fly = false, bool split = false, bool ignoreTiles = false, bool spawnTileDust = true, bool soundEffects = true, bool rotateAverage = false, Action<NPC, int, bool> onChangeType = null)
        {
            bool diggingDummy = false;
            AIWorm(npc, ref diggingDummy, wormTypes, partDistanceAddon, maxSpeed, gravityResist, fly, split, ignoreTiles, spawnTileDust, soundEffects, rotateAverage, onChangeType);
        }

        /*
		 * A cleaned up (and edited) copy of Worm AI. (Giant Worm, EoW, Wyvern, etc.) (AIStyle 6)
		 * 
		 * wormTypes: An array of the worm npc types. The first type is for the head, the last type is for the tail, and the rest are body types in the order they appear on the worm.
		 *            If wormTypes has a length of 1, then it will spawn the head but not any other parts. This lets you make single npcs that dig like worms.
		 * partDistanceAddon : an addon to the distance between parts of the worm.
		 * maxSpeed : the fastest the worm can accellerate to.
		 * gravityResist : how much resistance on the X axis the worm has when it is out of tiles.
		 * fly : If true, acts like a Wvyern.
		 * split : If true, worm will split when parts of it die.
		 * ignoreTiles : If true, Allows the worm to move outside of tiles as if it were in them. (ignored if fly is true)
		 * spawnTileDust : If true, worm will spawn tile dust when it digs through tiles.
		 * soundEffects : If true, will produce a digging sound when nearing the player.
		 * rotateAverage : If true, takes the rotations of the the piece before and after this npc and averages them and adds to it to get a rotation. More accurate for some weirder shaped worms.
		 * onChangeType : an Action<NPC, int, bool> which is called when the worm splits and the npc changes to a head or tail. (NPC npc, int oldType, bool isHead)
		 */
        public static void AIWorm(NPC npc, ref bool isDigging, int[] wormTypes, float partDistanceAddon = 0f, float maxSpeed = 8f, float gravityResist = 0.07f, bool fly = false, bool split = false, bool ignoreTiles = false, bool spawnTileDust = true, bool soundEffects = true, bool rotateAverage = false, Action<NPC, int, bool> onChangeType = null)
        {
            bool singlePiece = wormTypes.Length == 1;
            bool isHead = npc.type == wormTypes[0];
            bool isTail = npc.type == wormTypes[^1];
            bool isBody = !isHead && !isTail;
            int wormLength = wormTypes.Length;

            if (split)
            {
                npc.realLife = -1;
            }
            else
            if (npc.ai[3] > 0f)
                npc.realLife = (int)npc.ai[3];

            if (npc.ai[0] == -1f)
                npc.ai[0] = npc.whoAmI;
            if (npc.target is < 0 or 255 || Main.player[npc.target].dead)
                npc.TargetClosest();
            if (isHead)
            {
                if ((npc.target is < 0 or 255 || Main.player[npc.target].dead) && npc.timeLeft > 300)
                    npc.timeLeft = 300;
            }
            else
            {
                npc.timeLeft = 50;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!singlePiece)
                {
                    //spawn pieces (flying)
                    if (fly && isHead && npc.ai[0] == 0f)
                    {
                        npc.ai[3] = npc.whoAmI;
                        npc.realLife = npc.whoAmI;
                        int npcID = npc.whoAmI;
                        for (int m = 1; m < wormLength - 1; m++)
                        {
                            int npcType = wormTypes[m];

                            float ai0 = 0;
                            float ai1 = npcID;
                            float ai2 = 0;
                            float ai3 = npc.ai[3];

                            int newnpcID = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, npcType, npc.whoAmI, ai0, ai1, ai2, ai3);
                            Main.npc[npcID].ai[0] = newnpcID;
                            Main.npc[npcID].netUpdate = true;
                            //Main.npc[newnpcID].ai[3] = (float)npc.whoAmI;
                            //Main.npc[newnpcID].realLife = npc.whoAmI;
                            //Main.npc[newnpcID].ai[1] = (float)npcID;
                            //Main.npc[npcID].ai[0] = (float)newnpcID;
                            //Main.npc[newnpcID].netUpdate = true;							
                            //NetMessage.SendData(23, -1, -1, NetworkText.FromLiteral(""), newnpcID, 0f, 0f, 0f, 0);
                            npcID = newnpcID;
                        }
                        npc.netUpdate = true;
                    }
                    else //spawn pieces
                    if ((isHead || isBody) && npc.ai[0] == 0f)
                    {
                        if (isHead)
                        {
                            if (!split)
                            {
                                npc.ai[3] = npc.whoAmI;
                                npc.realLife = npc.whoAmI;
                            }
                            npc.ai[2] = wormLength - 1;
                        }
                        float ai0 = 0;
                        float ai1 = npc.whoAmI;
                        float ai2 = npc.ai[2] - 1f;
                        float ai3 = npc.ai[3];
                        if (split)
                            npc.ai[3] = 0f;

                        if (isHead)
                        {
                            npc.ai[0] = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, wormTypes[1], npc.whoAmI, ai0, ai1, ai2, ai3);
                        }
                        else
                        if (isBody && npc.ai[2] > 0f)
                        {
                            npc.ai[0] = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, wormTypes[wormLength - (int)npc.ai[2]], npc.whoAmI, ai0, ai1, ai2, ai3);
                        }
                        else
                        {
                            npc.ai[0] = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, wormTypes[^1], npc.whoAmI, ai0, ai1, ai2, ai3);
                        }
                        /*if (!split)
						{
							Main.npc[(int)npc.ai[0]].ai[3] = npc.ai[3];
							Main.npc[(int)npc.ai[0]].realLife = npc.realLife;
						}
						Main.npc[(int)npc.ai[0]].ai[1] = (float)npc.whoAmI;
						Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;*/
                        Main.npc[(int)npc.ai[0]].netUpdate = true;
                        npc.netUpdate = true;
                    }
                }
                //if npc can split, check if pieces are dead and if so split.
                if (!singlePiece && split)
                {
                    if (!Main.npc[(int)npc.ai[1]].active && !Main.npc[(int)npc.ai[0]].active) //if this is in the middle and both parts are inactive, kill self
                    {
                        npc.life = 0;
                        npc.HitEffect();
                        npc.active = false;
                    }
                    if (npc.type == wormTypes[0] && !Main.npc[(int)npc.ai[0]].active) //if this is the head and the bottom is inactive, kill self
                    {
                        npc.life = 0;
                        npc.HitEffect();
                        npc.active = false;
                    }
                    if (npc.type == wormTypes[^1] && !Main.npc[(int)npc.ai[1]].active) //if this is the tail and the top is inactive, kill self
                    {
                        npc.life = 0;
                        npc.HitEffect();
                        npc.active = false;
                    }
                    if (isBody && !Main.npc[(int)npc.ai[1]].active) //if the body was just split, turn it into a head
                    {
                        int oldType = npc.type;
                        npc.type = wormTypes[0];
                        int npcID = npc.whoAmI;
                        float lifePercent = npc.life / (float)npc.lifeMax;
                        float lastPiece = npc.ai[0];
                        npc.SetDefaults(npc.type);
                        npc.life = (int)(npc.lifeMax * lifePercent);
                        npc.ai[0] = lastPiece;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                        npc.whoAmI = npcID;
                        onChangeType?.Invoke(npc, oldType, true);
                    }
                    else
                    if (isBody && !Main.npc[(int)npc.ai[0]].active) //if the body was just split, turn it into a tail
                    {
                        int oldType = npc.type;
                        npc.type = wormTypes[^1];
                        int npcID = npc.whoAmI;
                        float lifePercent = npc.life / (float)npc.lifeMax;
                        float lastPiece = npc.ai[1];
                        npc.SetDefaults(npc.type);
                        npc.life = (int)(npc.lifeMax * lifePercent);
                        npc.ai[1] = lastPiece;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                        npc.whoAmI = npcID;
                        onChangeType?.Invoke(npc, oldType, false);
                    }
                }
                else
                if (!singlePiece)
                {
                    if (npc.type != wormTypes[0] && (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != npc.aiStyle))
                    {
                        npc.life = 0;
                        npc.HitEffect();
                        npc.active = false;
                    }
                    if (npc.type != wormTypes[^1] && (!Main.npc[(int)npc.ai[0]].active || Main.npc[(int)npc.ai[0]].aiStyle != npc.aiStyle))
                    {
                        npc.life = 0;
                        npc.HitEffect();
                        npc.active = false;
                    }
                }
                if (!npc.active && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, NetworkText.FromLiteral(""), npc.whoAmI, 1, 0f, 0f, -1);
            }
            int tileX = (int)(npc.position.X / 16f) - 1;
            int tileCenterX = (int)(npc.Center.X / 16f) + 2;
            int tileY = (int)(npc.position.Y / 16f) - 1;
            int tileCenterY = (int)(npc.Center.Y / 16f) + 2;
            if (tileX < 0) { tileX = 0; }
            if (tileCenterX > Main.maxTilesX) { tileCenterX = Main.maxTilesX; }
            if (tileY < 0) { tileY = 0; }
            if (tileCenterY > Main.maxTilesY) { tileCenterY = Main.maxTilesY; }
            bool canMove = false;
            if (fly || ignoreTiles) { canMove = true; }
            if (!canMove || spawnTileDust)
            {
                for (int tX = tileX; tX < tileCenterX; tX++)
                {
                    for (int tY = tileY; tY < tileCenterY; tY++)
                    {
                        Tile checkTile = BaseWorldGen.GetTileSafely(tX, tY);
                        if (checkTile != null && (checkTile.HasUnactuatedTile && (Main.tileSolid[checkTile.TileType] || Main.tileSolidTop[checkTile.TileType] && checkTile.TileFrameY == 0) || checkTile.LiquidAmount > 64))
                        {
                            Vector2 tPos;
                            tPos.X = tX * 16;
                            tPos.Y = tY * 16;
                            if (npc.position.X + npc.width > tPos.X && npc.position.X < tPos.X + 16f && npc.position.Y + npc.height > tPos.Y && npc.position.Y < tPos.Y + 16f)
                            {
                                canMove = true;
                                if (spawnTileDust && Main.rand.NextBool(100) && checkTile.HasUnactuatedTile)
                                {
                                    WorldGen.KillTile(tX, tY, true, true);
                                }
                            }
                        }
                    }
                }
            }
            if (!canMove && npc.type == wormTypes[0])
            {
                Rectangle rectangle = new((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int playerCheckDistance = 1000;
                bool canMove2 = true;
                for (int m3 = 0; m3 < 255; m3++)
                {
                    if (Main.player[m3].active)
                    {
                        Rectangle rectangle2 = new((int)Main.player[m3].position.X - playerCheckDistance, (int)Main.player[m3].position.Y - playerCheckDistance, playerCheckDistance * 2, playerCheckDistance * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            canMove2 = false;
                            break;
                        }
                    }
                }
                if (canMove2) { canMove = true; }
            }
            if (fly)
            {
                if (npc.velocity.X < 0f) { npc.spriteDirection = 1; } else if (npc.velocity.X > 0f) { npc.spriteDirection = -1; }
            }
            Vector2 npcCenter = npc.Center;
            float playerCenterX = Main.player[npc.target].Center.X;
            float playerCenterY = Main.player[npc.target].Center.Y;
            playerCenterX = (int)(playerCenterX / 16f) * 16; playerCenterY = (int)(playerCenterY / 16f) * 16;
            npcCenter.X = (int)(npcCenter.X / 16f) * 16; npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
            playerCenterX -= npcCenter.X; playerCenterY -= npcCenter.Y;
            float dist = (float)Math.Sqrt(playerCenterX * playerCenterX + playerCenterY * playerCenterY);
            isDigging = canMove;
            if (npc.ai[1] > 0f && npc.ai[1] < Main.maxNPCs)
            {
                try
                {
                    npcCenter = npc.Center;
                    playerCenterX = Main.npc[(int)npc.ai[1]].Center.X - npcCenter.X;
                    playerCenterY = Main.npc[(int)npc.ai[1]].Center.Y - npcCenter.Y;
                }
                catch
                {
                }
                if (!rotateAverage || npc.type == wormTypes[0])
                {
                    npc.rotation = (float)Math.Atan2(playerCenterY, playerCenterX) + 1.57f;
                }
                else
                {
                    NPC frontNPC = Main.npc[(int)npc.ai[1]];
                    Vector2 rotVec = BaseUtility.RotateVector(frontNPC.Center, frontNPC.Center + new Vector2(0f, 30f), frontNPC.rotation);
                    npc.rotation = BaseUtility.RotationTo(npc.Center, rotVec) + 1.57f;
                }
                dist = (float)Math.Sqrt(playerCenterX * playerCenterX + playerCenterY * playerCenterY);
                dist = (dist - npc.width - partDistanceAddon) / dist;
                playerCenterX *= dist;
                playerCenterY *= dist;
                npc.velocity = default;
                npc.position.X += playerCenterX;
                npc.position.Y += playerCenterY;
                if (fly)
                {
                    if (playerCenterX < 0f)
                    {
                        npc.spriteDirection = 1;
                    }
                    else
                    if (playerCenterX > 0f)
                    {
                        npc.spriteDirection = -1;
                    }
                }
            }
            else
            {
                if (!canMove)
                {
                    npc.TargetClosest();
                    npc.velocity.Y += 0.11f;
                    if (npc.velocity.Y > maxSpeed) { npc.velocity.Y = maxSpeed; }
                    if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.4)
                    {
                        if (npc.velocity.X < 0f) { npc.velocity.X -= gravityResist * 1.1f; } else { npc.velocity.X += gravityResist * 1.1f; }
                    }
                    else
                        if (npc.velocity.Y == maxSpeed)
                    {
                        if (npc.velocity.X < playerCenterX) { npc.velocity.X += gravityResist; }
                        else
                            if (npc.velocity.X > playerCenterX) { npc.velocity.X -= gravityResist; }
                    }
                    else
                            if (npc.velocity.Y > 4f)
                    {
                        if (npc.velocity.X < 0f) { npc.velocity.X += gravityResist * 0.9f; } else { npc.velocity.X -= gravityResist * 0.9f; }
                    }
                }
                else
                {
                    if (soundEffects && npc.soundDelay == 0)
                    {
                        float distSoundDelay = dist / 40f;
                        if (distSoundDelay < 10f) { distSoundDelay = 10f; }
                        if (distSoundDelay > 20f) { distSoundDelay = 20f; }
                        npc.soundDelay = (int)distSoundDelay;
                        SoundEngine.PlaySound(SoundID.Roar, npc.position);
                    }
                    dist = (float)Math.Sqrt(playerCenterX * playerCenterX + playerCenterY * playerCenterY);
                    float absPlayerCenterX = Math.Abs(playerCenterX);
                    float absPlayerCenterY = Math.Abs(playerCenterY);
                    float newSpeed = maxSpeed / dist;
                    playerCenterX *= newSpeed;
                    playerCenterY *= newSpeed;
                    bool dontFall = false;
                    if (fly)
                    {
                        if ((npc.velocity.X > 0f && playerCenterX < 0f || npc.velocity.X < 0f && playerCenterX > 0f || npc.velocity.Y > 0f && playerCenterY < 0f || npc.velocity.Y < 0f && playerCenterY > 0f) && Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) > gravityResist / 2f && dist < 300f)
                        {
                            dontFall = true;
                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed) { npc.velocity *= 1.1f; }
                        }
                        if (npc.position.Y > Main.player[npc.target].position.Y || Main.player[npc.target].position.Y / 16f > Main.worldSurface || Main.player[npc.target].dead)
                        {
                            dontFall = true;
                            if (Math.Abs(npc.velocity.X) < maxSpeed / 2f)
                            {
                                if (npc.velocity.X == 0f) { npc.velocity.X -= npc.direction; }
                                npc.velocity.X *= 1.1f;
                            }
                            else
                                if (npc.velocity.Y > -maxSpeed) { npc.velocity.Y -= gravityResist; }
                        }
                    }
                    if (!dontFall)
                    {
                        if (npc.velocity.X > 0f && playerCenterX > 0f || npc.velocity.X < 0f && playerCenterX < 0f || npc.velocity.Y > 0f && playerCenterY > 0f || npc.velocity.Y < 0f && playerCenterY < 0f)
                        {
                            if (npc.velocity.X < playerCenterX) { npc.velocity.X += gravityResist; }
                            else
                                if (npc.velocity.X > playerCenterX) { npc.velocity.X -= gravityResist; }
                            if (npc.velocity.Y < playerCenterY) { npc.velocity.Y += gravityResist; }
                            else
                                if (npc.velocity.Y > playerCenterY) { npc.velocity.Y -= gravityResist; }
                            if (Math.Abs(playerCenterY) < maxSpeed * 0.2 && (npc.velocity.X > 0f && playerCenterX < 0f || npc.velocity.X < 0f && playerCenterX > 0f))
                            {
                                if (npc.velocity.Y > 0f) { npc.velocity.Y += gravityResist * 2f; } else { npc.velocity.Y -= gravityResist * 2f; }
                            }
                            if (Math.Abs(playerCenterX) < maxSpeed * 0.2 && (npc.velocity.Y > 0f && playerCenterY < 0f || npc.velocity.Y < 0f && playerCenterY > 0f))
                            {
                                if (npc.velocity.X > 0f) { npc.velocity.X += gravityResist * 2f; } else { npc.velocity.X -= gravityResist * 2f; }
                            }
                        }
                        else
                            if (absPlayerCenterX > absPlayerCenterY)
                        {
                            if (npc.velocity.X < playerCenterX) { npc.velocity.X += gravityResist * 1.1f; }
                            else
                                if (npc.velocity.X > playerCenterX) { npc.velocity.X -= gravityResist * 1.1f; }

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5)
                            {
                                if (npc.velocity.Y > 0f) { npc.velocity.Y += gravityResist; } else { npc.velocity.Y -= gravityResist; }
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < playerCenterY) { npc.velocity.Y += gravityResist * 1.1f; }
                            else
                                if (npc.velocity.Y > playerCenterY) { npc.velocity.Y -= gravityResist * 1.1f; }
                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5)
                            {
                                if (npc.velocity.X > 0f) { npc.velocity.X += gravityResist; } else { npc.velocity.X -= gravityResist; }
                            }
                        }
                    }
                }
                if (!rotateAverage || npc.type == wormTypes[0])
                {
                    npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;
                }
                else
                {
                    NPC frontNPC = Main.npc[(int)npc.ai[1]];
                    Vector2 rotVec = BaseUtility.RotateVector(frontNPC.Center, frontNPC.Center + new Vector2(0f, 30f), frontNPC.rotation);
                    npc.rotation = BaseUtility.RotationTo(npc.Center, rotVec) + 1.57f;
                }
                if (npc.type == wormTypes[0])
                {
                    if (canMove)
                    {
                        if (npc.localAI[0] != 1f) { npc.netUpdate = true; }
                        npc.localAI[0] = 1f;
                    }
                    else
                    {
                        if (npc.localAI[0] != 0f) { npc.netUpdate = true; }
                        npc.localAI[0] = 0f;
                    }
                    if ((npc.velocity.X > 0f && npc.oldVelocity.X < 0f || npc.velocity.X < 0f && npc.oldVelocity.X > 0f || npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f || npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f) && !npc.justHit)
                    {
                        npc.netUpdate = true;
                    }
                }
            }
        }

        /*
		 * A cleaned up (and edited) copy of Teleporter AI. (Fire Imps, Tim, Chaos Elementals, etc.) (AIStyle 8)
		 * 
		 * ai : A float array that stores AI data.
		 * checkGround: If true, npc checks for ground underneath where it teleports to.
		 * immobile : If true, npc's x velocity is continuously shrunken until it stops moving.
		 * distFromPlayer: The amount (in tiles) from the player to have a chance to teleport to.
		 * teleportInterval : The time until the npc attempts to teleport again.
		 * attackInterval : The time until the npc attempts to attack again. If -1, never attack.
		 * delayOnHit : If true, will delay the npc's teleporting and attacking.
		 * TeleportEffects<bool> : Action that can be used to add custom teleport effects. (if the bool is true, it's pre-teleport. If false, post-teleport.
		 * CanTeleportTo<int, int> : Action that can be used to check if the npc can teleport to a specific place.
		 * Attack : Action that can be used to have the npc periodically attack.
		 */
        public static void AITeleporter(NPC npc, ref float[] ai, bool checkGround = true, bool immobile = true, int distFromPlayer = 20, int teleportInterval = 650, int attackInterval = 100, int stopAttackInterval = 500, bool delayOnHit = true, Action<bool> teleportEffects = null, Func<int, int, bool> canTeleportTo = null, Action attack = null)
        {
            npc.TargetClosest();
            if (immobile)
            {
                npc.velocity.X *= 0.93f;
                if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1) { npc.velocity.X = 0f; }
            }
            if (ai[0] == 0f) { ai[0] = Math.Max(0, Math.Max(teleportInterval, teleportInterval - 150)); }
            if (ai[2] != 0f && ai[3] != 0f)
            {
                teleportEffects?.Invoke(true); npc.position.X = ai[2] * 16f - npc.width / 2 + 8f;
                npc.position.Y = ai[3] * 16f - npc.height;
                npc.velocity.X = 0f; npc.velocity.Y = 0f;
                ai[2] = 0f; ai[3] = 0f;
                teleportEffects?.Invoke(false);
            }
            if (npc.justHit) { ai[0] = 0; }
            ai[0]++;
            if (attackInterval != -1 && ai[0] < stopAttackInterval && ai[0] % attackInterval == 0)
            {
                ai[1] = 30f;
                npc.netUpdate = true;
            }
            else
            if (ai[0] >= teleportInterval && Main.netMode != NetmodeID.MultiplayerClient)
            {
                ai[0] = 1f;
                int playerTileX = (int)Main.player[npc.target].position.X / 16;
                int playerTileY = (int)Main.player[npc.target].position.Y / 16;
                int tileX = (int)npc.position.X / 16;
                int tileY = (int)npc.position.Y / 16;
                int teleportCheckCount = 0;
                bool hasTeleportPoint = false;
                //player is too far away, don't teleport.
                if (Vector2.Distance(npc.Center, Main.player[npc.target].Center) > 2000f)
                {
                    teleportCheckCount = 100;
                    hasTeleportPoint = true;
                }
                while (!hasTeleportPoint && teleportCheckCount < 100)
                {
                    teleportCheckCount++;
                    int tpTileX = Main.rand.Next(playerTileX - distFromPlayer, playerTileX + distFromPlayer);
                    int tpTileY = Main.rand.Next(playerTileY - distFromPlayer, playerTileY + distFromPlayer);
                    for (int tpY = tpTileY; tpY < playerTileY + distFromPlayer; tpY++)
                    {
                        if ((tpY < playerTileY - 4 || tpY > playerTileY + 4 || tpTileX < playerTileX - 4 || tpTileX > playerTileX + 4) && (tpY < tileY - 1 || tpY > tileY + 1 || tpTileX < tileX - 1 || tpTileX > tileX + 1) && (!checkGround || Main.tile[tpTileX, tpY].HasUnactuatedTile))
                        {
                            if (canTeleportTo != null && canTeleportTo(tpTileX, tpY) || Main.tile[tpTileX, tpY - 1].LiquidType != 2 && (!checkGround || Main.tileSolid[Main.tile[tpTileX, tpY].TileType]) && !Collision.SolidTiles(tpTileX - 1, tpTileX + 1, tpY - 4, tpY - 1))
                            {
                                if (attackInterval != -1) { ai[1] = 20f; }
                                ai[2] = tpTileX;
                                ai[3] = tpY;
                                hasTeleportPoint = true;
                                break;
                            }
                        }
                    }
                }
                npc.netUpdate = true;
            }
            if (attack != null && attackInterval != -1 && ai[1] > 0f)
            {
                ai[1] -= 1f;
                if (ai[1] == 25f) { attack(); }
            }
        }

        /*
         * A cleaned up (and edited) copy of Fish AI. (Goldfish, Angler Fish, etc.)
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * hostile : If true, will target players.
         * ignoreNonWetPlayer : If false, npc will target players even if they are out of water.
         * ignoreWater : If true, npc will not be bound to water. (ie npc flies)
         * velMaxX/velMaxY : the max velocities on the X and Y axis, respectively.
         */
        public static void AIFish(NPC npc, ref float[] ai, bool hostile = false, bool ignoreNonWetPlayer = true, bool ignoreWater = false, float velMaxX = 3f, float velMaxY = 2f)
        {
            //if the npc is hostile and it has no direction, target the closest player.
            if (hostile && npc.direction == 0) { npc.TargetClosest(); }
            if (ignoreWater || npc.wet)//if wet or ignore water is true...
            {
                bool hasTarget = false;
                //if hostile, get a target and check that the player found is wet.
                if (hostile)
                {
                    npc.TargetClosest(false);
                    if ((!ignoreNonWetPlayer || Main.player[npc.target].wet) && !Main.player[npc.target].dead) { hasTarget = true; }
                }
                //if the target is wet or there is no target...
                if (!hasTarget)
                {
                    if (npc.collideX)
                    {
                        npc.velocity.X *= -1f;
                        npc.direction *= -1;
                        npc.netUpdate = true;
                    }
                    if (npc.collideY)
                    {
                        npc.netUpdate = true;
                        int mult = npc.velocity.Y > 0f ? -1 : 1;
                        npc.velocity.Y = Math.Abs(npc.velocity.Y) * mult;
                        npc.directionY = 1 * mult;
                        ai[0] = 1f * mult;
                    }
                }
                //if the npc has a target that fits the requirements, attempt to move toward that target.
                if (hasTarget)
                {
                    npc.TargetClosest();
                    npc.velocity.X += npc.direction * 0.1f;
                    npc.velocity.Y += npc.directionY * 0.1f;
                    if (npc.velocity.X > velMaxX) { npc.velocity.X = velMaxX; }
                    if (npc.velocity.X < -velMaxX) { npc.velocity.X = -velMaxX; }
                    if (npc.velocity.Y > velMaxY) { npc.velocity.Y = velMaxY; }
                    if (npc.velocity.Y < -velMaxY) { npc.velocity.Y = -velMaxY; }
                }
                else//otherwise, move horizontally, slowly bobbing up and down as well.
                {
                    npc.velocity.X += npc.direction * 0.1f;
                    if (npc.velocity.X is < -1f or > 1f) { npc.velocity.X *= 0.95f; }
                    if (ai[0] == -1f)
                    {
                        npc.velocity.Y -= 0.01f;
                        if (npc.velocity.Y < -0.3)
                        {
                            ai[0] = 1f;
                        }
                    }
                    else
                    {
                        npc.velocity.Y += 0.01f;
                        if (npc.velocity.Y > 0.3)
                        {
                            ai[0] = -1f;
                        }
                    }
                    int tileX = (int)(npc.Center.X / 16);
                    int tileY = (int)(npc.Center.Y / 16);
                    if (Main.tile[tileX, tileY - 1].LiquidAmount > 128)
                    {
                        if (Main.tile[tileX, tileY + 1].HasUnactuatedTile || Main.tile[tileX, tileY + 2].HasUnactuatedTile) { ai[0] = -1f; }
                    }
                    //if npc's y speed goes above max velocity, slow the npc down.
                    if (npc.velocity.Y > velMaxY || npc.velocity.Y < -velMaxY) { npc.velocity.Y *= 0.95f; }
                }
            }
            else
            {
                //if y velocity is 0, set the npc's velocity to a random number to get it started.
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y == 0f)
                {
                    npc.velocity.Y = Main.rand.Next(-50, -20) * 0.1f;
                    npc.velocity.X = Main.rand.Next(-20, 20) * 0.1f;
                    npc.netUpdate = true;
                }
                npc.velocity.Y += 0.3f;
                if (npc.velocity.Y > 10f) { npc.velocity.Y = 10f; }
                ai[0] = 1f;
            }
            npc.rotation = npc.velocity.Y * npc.direction * 0.1f;
            if (npc.rotation < -0.2) { npc.rotation = -0.2f; }
            if (npc.rotation > 0.2) { npc.rotation = 0.2f; }
        }

        /*
         * A cleaned up (and edited) copy of Zombie AI. (Stripped Fighter AI)
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * fleeWhenDay : If true, flees when it is daytime.
         * allowBoredom : If false, npc will not get 'bored' trying to harass a target and wander off.
         * openDoors : -1 == do not interact with doors, 0 == go up to door but do not break it, 1 == attempt to break down doors, 2 == attempt to open doors.
         * velMaxX : the maximum velocity on the X axis.
         * maxJumpTilesX/maxJumpTilesY : The max tiles it can jump across and over, respectively. 
         * ticksUntilBoredom : the amount of ticks until the npc gets 'bored' following a target.
         * targetPlayers : If false, will not target players actively.
         * doorBeatCounterMax : how many beat ticks until the door is opened/broken.
         * doorCounterMax : how many ticks to iterate doorBeatCounter.
         * jumpUpPlatforms : If true, the npc will jump up if a platform is above it and it is within jumping range.
         */
        public static void AIZombie(NPC npc, ref float[] ai, bool fleeWhenDay = true, bool allowBoredom = true, int openDoors = 1, float moveInterval = 0.07f, float velMax = 1f, int maxJumpTilesX = 3, int maxJumpTilesY = 4, int ticksUntilBoredom = 60, bool targetPlayers = true, int doorBeatCounterMax = 10, int doorCounterMax = 60, bool jumpUpPlatforms = false, Action<bool, bool, Vector2, Vector2> onTileCollide = null, bool ignoreJumpTiles = false)
        {
            bool xVelocityChanged = false;
            //This block of code checks for major X velocity/directional changes as well as periodically updates the npc.
            if (npc.velocity.Y == 0f && (npc.velocity.X > 0f && npc.direction < 0 || npc.velocity.X < 0f && npc.direction > 0))
            {
                xVelocityChanged = true;
            }
            if (npc.position.X == npc.oldPosition.X || ai[3] >= ticksUntilBoredom || xVelocityChanged)
            {
                ai[3] += 1f;
            }
            else
            if (Math.Abs(npc.velocity.X) > 0.9 && ai[3] > 0f) { ai[3] -= 1f; }
            if (ai[3] > ticksUntilBoredom * 10) { ai[3] = 0f; }
            if (npc.justHit) { ai[3] = 0f; }
            if (ai[3] == ticksUntilBoredom) { npc.netUpdate = true; }

            bool notBored = ai[3] < ticksUntilBoredom;
            //if npc does not flee when it's day, if is night, or npc is not on the surface and it hasn't updated projectile pass, update target.
            if (targetPlayers && (!fleeWhenDay || !Main.dayTime || npc.position.Y > Main.worldSurface * 16.0) && (fleeWhenDay && Main.dayTime ? notBored : !allowBoredom || notBored))
            {
                npc.TargetClosest();
            }
            else
            if (ai[2] <= 0f)//if 'bored'
            {
                if (fleeWhenDay && Main.dayTime && npc.position.Y / 16f < Main.worldSurface && npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        ai[0] += 1f;
                        if (ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = npc.direction;
                            ai[0] = 0f;
                        }
                    }
                }
                else { ai[0] = 0f; }
                if (npc.direction == 0) { npc.direction = 1; }
            }
            //if velocity is less than -1 or greater than 1...
            if (npc.velocity.X < -velMax || npc.velocity.X > velMax)
            {
                //...and npc is not falling or jumping, slow down x velocity.
                if (npc.velocity.Y == 0f) { npc.velocity *= 0.8f; }
            }
            else
            if (npc.velocity.X < velMax && npc.direction == 1) //handles movement to the right. Clamps at velMaxX.
            {
                npc.velocity.X += moveInterval;
                if (npc.velocity.X > velMax) { npc.velocity.X = velMax; }
            }
            else
            if (npc.velocity.X > -velMax && npc.direction == -1) //handles movement to the left. Clamps at -velMaxX.
            {
                npc.velocity.X -= moveInterval;
                if (npc.velocity.X < -velMax) { npc.velocity.X = -velMax; }
            }
            WalkupHalfBricks(npc);
            //if allowed to open doors and is currently doing so, reduce npc velocity on the X axis to 0. (so it stops moving)
            if (openDoors != -1 && AttemptOpenDoor(npc, ref ai[1], ref ai[2], ref ai[3], ticksUntilBoredom, doorBeatCounterMax, doorCounterMax, openDoors))
            {
                npc.velocity.X = 0;
            }
            else //if no door to open, reset ai.
            if (openDoors != -1) { ai[1] = 0f; ai[2] = 0f; }
            //if there's a solid floor under us...
            if (HitTileOnSide(npc, 3))
            {
                //if the npc's velocity is going in the same direction as the npc's direction...
                if (npc.velocity.X < 0f && npc.direction == -1 || npc.velocity.X > 0f && npc.direction == 1)
                {
                    //...attempt to jump if needed.
                    Vector2 newVec = AttemptJump(npc.position, npc.velocity, npc.width, npc.height, npc.direction, npc.directionY, maxJumpTilesX, maxJumpTilesY, velMax, jumpUpPlatforms, jumpUpPlatforms && notBored ? Main.player[npc.target] : null, ignoreJumpTiles);
                    if (!npc.noTileCollide)
                    {
                        newVec = Collision.TileCollision(npc.position, newVec, npc.width, npc.height);
                        Vector4 slopeVec = Collision.SlopeCollision(npc.position, newVec, npc.width, npc.height);
                        Vector2 slopeVel = new(slopeVec.Z, slopeVec.W);
                        if (onTileCollide != null && npc.velocity != slopeVel) onTileCollide(npc.velocity.X != slopeVel.X, npc.velocity.Y != slopeVel.Y, npc.velocity, slopeVel);
                        npc.position = new Vector2(slopeVec.X, slopeVec.Y);
                        npc.velocity = slopeVel;
                    }
                    if (npc.velocity != newVec) { npc.velocity = newVec; npc.netUpdate = true; }
                }
            }
        }

        /*
         * A cleaned up copy of Demon Eye AI. (Flier AI)
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * fleeWhenDay : If true, npc will lose interest in players and fly away.
         * ignoreWet : If true, ignores code for forcing the npc out of water.
         * velMaxX, velMaxY : the maximum velocity on the X and Y axis, respectively.
         * bounceScalarX, bounceScalarY : scalars to increase the amount of velocity from bouncing on the X and Y axis, respectively.
         */
        public static void AIEye(NPC npc, ref float[] ai, bool fleeWhenDay = true, bool ignoreWet = false, float moveIntervalX = 0.1f, float moveIntervalY = 0.04f, float velMaxX = 4f, float velMaxY = 1.5f, float bounceScalarX = 1f, float bounceScalarY = 1f)
        {
            //controls the npc's bouncing when it hits a wall.
            if (npc.collideX)
            {
                npc.velocity.X = npc.oldVelocity.X * -0.5f;
                if (npc.direction == -1 && npc.velocity.X is > 0f and < 2f) { npc.velocity.X = 2f; }
                if (npc.direction == 1 && npc.velocity.X is < 0f and > -2f) { npc.velocity.X = -2f; }
                npc.velocity.X *= bounceScalarX;
            }
            //controls the npc's bouncing when it hits a floor or ceiling.
            if (npc.collideY)
            {
                npc.velocity.Y = npc.oldVelocity.Y * -0.5f;
                if (npc.velocity.Y is > 0f and < 1f) { npc.velocity.Y = 1f; }
                if (npc.velocity.Y is < 0f and > -1f) { npc.velocity.Y = -1f; }
                npc.velocity.Y *= bounceScalarY;
            }
            //if it should flee when it's day, and it is day, the npc's position is at or above the surface, it will flee.
            if (fleeWhenDay && Main.dayTime && npc.position.Y <= Main.worldSurface * 16.0)
            {
                if (npc.timeLeft > 10) { npc.timeLeft = 10; }
                npc.directionY = -1;
                if (npc.velocity.Y > 0f) { npc.direction = 1; }
                npc.direction = -1;
                if (npc.velocity.X > 0f) { npc.direction = 1; }
            }
            else
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead)
                {
                    if (npc.timeLeft > 10) { npc.timeLeft = 10; }
                    npc.directionY = -1;
                    if (npc.velocity.Y > 0f) { npc.direction = 1; }
                    npc.direction = -1;
                    if (npc.velocity.X > 0f) { npc.direction = 1; }
                }
            }
            //controls momentum when going left, and clamps velocity at -velMaxX.
            if (npc.direction == -1 && npc.velocity.X > -velMaxX)
            {
                npc.velocity.X -= moveIntervalX;
                if (npc.velocity.X > 4f) { npc.velocity.X -= 0.1f; }
                else
                    if (npc.velocity.X > 0f) { npc.velocity.X += 0.05f; }
                if (npc.velocity.X < -4f) { npc.velocity.X = -velMaxX; }
            }
            else //controls momentum when going right on the x axis and clamps velocity at velMaxX.
                if (npc.direction == 1 && npc.velocity.X < velMaxX)
            {
                npc.velocity.X += moveIntervalX;
                if (npc.velocity.X < -velMaxX) { npc.velocity.X += 0.1f; }
                else
                    if (npc.velocity.X < 0f) { npc.velocity.X -= 0.05f; }

                if (npc.velocity.X > velMaxX) { npc.velocity.X = velMaxX; }
            }
            //controls momentum when going up on the Y axis and clamps velocity at -velMaxY.
            if (npc.directionY == -1 && (double)npc.velocity.Y > -velMaxY)
            {
                npc.velocity.Y -= moveIntervalY;
                if ((double)npc.velocity.Y > velMaxY) { npc.velocity.Y -= 0.05f; }
                else
                    if (npc.velocity.Y > 0f) { npc.velocity.Y += 0.03f; }

                if ((double)npc.velocity.Y < -velMaxY) { npc.velocity.Y = -velMaxY; }
            }
            else //controls momentum when going down on the Y axis and clamps velocity at velMaxY.
                if (npc.directionY == 1 && (double)npc.velocity.Y < velMaxY)
            {
                npc.velocity.Y += moveIntervalY;
                if ((double)npc.velocity.Y < -velMaxY) { npc.velocity.Y += 0.05f; }
                else
                    if (npc.velocity.Y < 0f) { npc.velocity.Y -= 0.03f; }

                if ((double)npc.velocity.Y > velMaxY) { npc.velocity.Y = velMaxY; }
            }
            if (!ignoreWet && npc.wet) //if don't ignore being wet and is wet, accelerate upwards to get out.
            {
                if (npc.velocity.Y > 0f) { npc.velocity.Y *= 0.95f; }
                npc.velocity.Y -= 0.5f;
                if (npc.velocity.Y < -velMaxY * 1.5f) { npc.velocity.Y = -velMaxY * 1.5f; }
                npc.TargetClosest();
            }
        }

        /*
         * A cleaned up copy of Slime AI.
         * 
         * ai : A float array that stores AI data. (Note projectile array should be synced!)
         * jumpTime : the amount of cooldown after the slime has jumped.
         */
        public static void AISlime(NPC npc, ref float[] ai, bool fleeWhenDay = false, int jumpTime = 200, float jumpVelX = 2f, float jumpVelY = -6f, float jumpVelHighX = 3f, float jumpVelHighY = -8f)
        {
            //ai[0] is a timer that iterates after the npc has jumped. If it is >= 0, the npc will attempt to jump again.
            //ai[1] is used to determine what jump type to do. (if 2, large jump, else smaller jump.)
            //ai[2] is used for target updating. 
            //ai[3] is used to house the landing position of the npc for bigger jumps. This is used to make it turn around when it hits
            //      an impassible wall.

            //if (jumpTime < 100) { jumpTime = 100; }
            bool getNewTarget = false; //getNewTarget is used to iterate the 'boredom' scale. If it's night, the npc is hurt, or it's
            //below a certain depth, it will attempt to find the nearest target to it.
            if (fleeWhenDay && !Main.dayTime || npc.life != npc.lifeMax || npc.position.Y > Main.worldSurface * 16.0)
            {
                getNewTarget = true;
            }
            if (ai[2] > 1f) { ai[2] -= 1f; }
            if (npc.wet)//if the npc is wet...
            {
                //handles the npc's 'bobbing' in water.
                if (npc.collideY) { npc.velocity.Y = -2f; }
                if (npc.velocity.Y < 0f && ai[3] == npc.position.X) { npc.direction *= -1; ai[2] = 200f; }
                if (npc.velocity.Y > 0f) { ai[3] = npc.position.X; }
                if (npc.velocity.Y > 2f) { npc.velocity.Y *= 0.9f; }
                npc.velocity.Y -= 0.5f;
                if (npc.velocity.Y < -4f) { npc.velocity.Y = -4f; }
                //if ai[2] is 1f, and we should get a target, target nearby players.
                if (ai[2] == 1f && getNewTarget) { npc.TargetClosest(); }
            }
            npc.aiAction = 0;
            //if ai[2] is 0f (just spawned)
            if (ai[2] == 0f)
            {
                ai[0] = -100f;
                ai[2] = 1f;
                npc.TargetClosest();
            }
            //if npc is not jumping or falling
            if (npc.velocity.Y == 0f)
            {
                if (ai[3] == npc.position.X) { npc.direction *= -1; ai[2] = 200f; }
                ai[3] = 0f;
                npc.velocity.X *= 0.8f;
                if (npc.velocity.X is > -0.1f and < 0.1f) { npc.velocity.X = 0f; }
                if (getNewTarget) { ai[0] += 1f; }
                ai[0] += 1f;
                if (ai[0] >= 0f)
                {
                    npc.netUpdate = true;
                    if (ai[2] == 1f && getNewTarget) { npc.TargetClosest(); }
                    if (ai[1] == 2f) //larger jump
                    {
                        npc.velocity.Y = jumpVelHighY;
                        npc.velocity.X += jumpVelHighX * npc.direction;
                        ai[0] = -jumpTime;
                        ai[1] = 0f;
                        ai[3] = npc.position.X;
                    }
                    else //smaller jump
                    {
                        npc.velocity.Y = jumpVelY;
                        npc.velocity.X += jumpVelX * npc.direction;
                        ai[0] = -jumpTime - 80f;
                        ai[1] += 1f;
                    }
                }
                else
                if (ai[0] >= -30f)
                {
                    npc.aiAction = 1;
                }
            }
            else //handle moving the npc while in air.
            if (npc.target < 255 && (npc.direction == 1 && npc.velocity.X < 3f || npc.direction == -1 && npc.velocity.X > -3f))
            {
                if (npc.direction == -1 && npc.velocity.X < 0.1 || npc.direction == 1 && npc.velocity.X > -0.1)
                {
                    npc.velocity.X += 0.2f * npc.direction;
                    return;
                }
                npc.velocity.X *= 0.93f;
            }
        }

        #endregion

        #region Vanilla NPC AI Code Excerpts
        //Code Excerpts are pieces of code from vanilla AI that were converted into standalone methods.

        public static void WalkupHalfBricks(NPC npc)
        {
            WalkupHalfBricks(npc, ref npc.gfxOffY, ref npc.stepSpeed);
        }

        /*
		 *  Code based on vanilla halfbrick walkup code, checks for and attempts to walk over half tiles.
		 */
        public static void WalkupHalfBricks(Entity codable, ref float gfxOffY, ref float stepSpeed)
        {
            if (codable == null)
                return;
            if (codable.velocity.Y >= 0f)
            {
                int offset = 0;
                if (codable.velocity.X < 0f) offset = -1;
                if (codable.velocity.X > 0f) offset = 1;
                Vector2 pos = codable.position;
                pos.X += codable.velocity.X;
                int tileX = (int)((pos.X + (double)(codable.width / 2) + (codable.width / 2 + 1) * offset) / 16.0);
                int tileY = (int)((pos.Y + (double)codable.height - 1.0) / 16.0);

                if (tileX * 16 < pos.X + (double)codable.width && tileX * 16 + 16 > (double)pos.X && (Main.tile[tileX, tileY].HasUnactuatedTile && Main.tile[tileX, tileY].Slope == 0 && Main.tile[tileX, tileY - 1].Slope == 0 && Main.tileSolid[Main.tile[tileX, tileY].TileType] && !Main.tileSolidTop[Main.tile[tileX, tileY].TileType] || Main.tile[tileX, tileY - 1].IsHalfBlock && Main.tile[tileX, tileY - 1].HasUnactuatedTile) && (!Main.tile[tileX, tileY - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 1].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 1].TileType] || Main.tile[tileX, tileY - 1].IsHalfBlock && (!Main.tile[tileX, tileY - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 4].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 4].TileType])) && (!Main.tile[tileX, tileY - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 2].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 2].TileType]) && (!Main.tile[tileX, tileY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 3].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 3].TileType]) && (!Main.tile[tileX - offset, tileY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX - offset, tileY - 3].TileType]))
                {
                    float tileWorldY = tileY * 16;
                    if (Main.tile[tileX, tileY].IsHalfBlock)
                        tileWorldY += 8f;
                    if (Main.tile[tileX, tileY - 1].IsHalfBlock)
                        tileWorldY -= 8f;
                    if (tileWorldY < pos.Y + (double)codable.height)
                    {
                        float tileWorldYHeight = pos.Y + codable.height - tileWorldY;
                        float heightNeeded = 16.1f;
                        if (tileWorldYHeight <= (double)heightNeeded)
                        {
                            gfxOffY += codable.position.Y + codable.height - tileWorldY;
                            codable.position.Y = tileWorldY - codable.height;
                            stepSpeed = tileWorldYHeight >= 9.0 ? 2f : 1f;
                        }
                    }
                    else
                    {
                        gfxOffY = Math.Max(0f, gfxOffY - stepSpeed);
                    }
                }
                else
                {
                    gfxOffY = Math.Max(0f, gfxOffY - stepSpeed);
                }
            }
            else
            {
                gfxOffY = Math.Max(0f, gfxOffY - stepSpeed);
            }
        }

        /*
         *  Code based on vanilla jumping code, checks for and attempts to jump over tiles and gaps when needed.
         *  
         *  direction/directionY : the direction and directionY of the object jumping (usually an NPC)
         *  tileDistX/tileDistY : the tile amounts the object can jump across and over, respectively.
         *  float maxSpeedX : The maximum speed of the npc.
         */
        public static Vector2 AttemptJump(Vector2 position, Vector2 velocity, int width, int height, int direction, float directionY = 0, int tileDistX = 3, int tileDistY = 4, float maxSpeedX = 1f, bool jumpUpPlatforms = false, Entity target = null, bool ignoreTiles = false)
        {
            try
            {
                tileDistX -= 2;
                Vector2 newVelocity = velocity;
                int tileX = Math.Max(10, Math.Min(Main.maxTilesX - 10, (int)((position.X + width * 0.5f + (width * 0.5f + 8f) * direction) / 16f)));
                int tileY = Math.Max(10, Math.Min(Main.maxTilesY - 10, (int)((position.Y + height - 15f) / 16f)));
                int tileItX = Math.Max(10, Math.Min(Main.maxTilesX - 10, tileX + direction * tileDistX));
                int tileItY = Math.Max(10, Math.Min(Main.maxTilesY - 10, tileY - tileDistY));
                int lastY = tileY;
                int tileHeight = (int)(height / 16f);
                if (height > tileHeight * 16) { tileHeight += 1; }

                Rectangle hitbox = new((int)position.X, (int)position.Y, width, height);
                //attempt to jump over walls if possible.

                if (ignoreTiles && target != null && Math.Abs(position.X + width * 0.5f - target.Center.X) < width + 120)
                {
                    float dist = (int)Math.Abs(position.Y + height * 0.5f - target.Center.Y) / 16;
                    if (dist < tileDistY + 2) { newVelocity.Y = -8f + dist * -0.5f; } // dist +=2; newVelocity.Y = -(5f + dist * (dist > 3 ? 1f - ((dist - 2f) * 0.0525f) : 1f)); }
                }
                if (newVelocity.Y == velocity.Y)
                {
                    for (int y = tileY; y >= tileItY; y--)
                    {
                        Tile tile = Framing.GetTileSafely(tileX, y);
                        Tile tileNear = Main.tile[Math.Min(Main.maxTilesX, tileX - direction), y];
                        if (tile.HasUnactuatedTile && (y != tileY || tile.Slope == 0) && Main.tileSolid[tile.TileType] && (jumpUpPlatforms || !Main.tileSolidTop[tile.TileType]))
                        {
                            if (!Main.tileSolidTop[tile.TileType])
                            {
                                Rectangle tileHitbox = new(tileX * 16, y * 16, 16, 16)
                                {
                                    Y = hitbox.Y
                                };
                                if (tileHitbox.Intersects(hitbox)) { newVelocity = velocity; break; }
                            }
                            if (tileNear.HasUnactuatedTile && Main.tileSolid[tileNear.TileType] && !Main.tileSolidTop[tileNear.TileType])
                            {
                                newVelocity = velocity;
                                break;
                            }
                            if (target != null && y * 16 < target.Center.Y)
                                continue;
                            lastY = y;
                            newVelocity.Y = -(5f + (tileY - y) * (tileY - y > 3 ? 1f - (tileY - y - 2) * 0.0525f : 1f));
                        }
                        //else
                        //if (lastY - y >= tileHeight) { break; }
                    }
                }
                // if the npc isn't jumping already...
                if (newVelocity.Y == velocity.Y)
                {
                    //...and there's a gap in front of the npc, attempt to jump across it.
                    if (directionY < 0 && (!Main.tile[tileX, tileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY + 1].TileType]) && (!Main.tile[tileX + direction, tileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX + direction, tileY + 1].TileType]))
                    {
                        if (!Main.tile[tileX + direction, tileY + 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY + 2].TileType] || target == null || target.Center.Y + target.height * 0.25f < tileY * 16f)
                        {
                            newVelocity.Y = -8f;
                            newVelocity.X *= 1.5f * (1f / maxSpeedX);
                            if (tileX <= tileItX)
                            {
                                for (int x = tileX; x < tileItX; x++)
                                {
                                    Tile tile = Framing.GetTileSafely(x, tileY + 1);
                                    if (x != tileX && !tile.HasUnactuatedTile)
                                    {
                                        newVelocity.Y -= 0.0325f;
                                        newVelocity.X += direction * 0.255f;
                                    }
                                }
                            }
                            else
                            if (tileX > tileItX)
                            {
                                for (int x = tileItX; x < tileX; x++)
                                {
                                    Tile tile = Framing.GetTileSafely(x, tileY + 1);
                                    if (x != tileItX && !tile.HasUnactuatedTile)
                                    {
                                        newVelocity.Y -= 0.0325f;
                                        newVelocity.X += direction * 0.255f;
                                    }
                                }
                            }
                        }
                    }
                }
                return newVelocity;
            }
            catch (Exception e)
            {
                BaseUtility.LogFancy("Redemption~ ATTEMPT JUMP ERROR:", e);
                return velocity;
            }
        }

        /*
         * Attempts to interact with a door.
         * 
         * Returns : true if it found and is trying to open a door, false otherwise.
         * doorBeatCounter : counter that goes from 0-10. When it hits 10 or more the door is opened.
         * doorCounter : counter that goes from 0-60. When it hits 60 it increments doorBeatCounter by one.
         * tickUpdater : counter that goes from 0-60+. See AIZombie() on what projectile is.
         * ticksUntilBoredom : See AIZombie() on what projectile is.
         * interactDoorStyle : 0 == hit door but don't break it, 1 == smash down door, 2 == open door.
         */
        public static bool AttemptOpenDoor(NPC npc, ref float doorBeatCounter, ref float doorCounter, ref float tickUpdater, float ticksUntilBoredom, int doorBeatCounterMax = 10, int doorCounterMax = 60, int interactDoorStyle = 0)
        {
            bool hitTile = HitTileOnSide(npc, 3);
            if (hitTile)
            {
                int tileX = (int)((npc.Center.X + (npc.width / 2 + 8f) * npc.spriteDirection) / 16f);
                int tileY = (int)((npc.position.Y + npc.height - 15f) / 16f);
                if (Framing.GetTileSafely(tileX, tileY - 1).HasUnactuatedTile && (Framing.GetTileSafely(tileX, tileY - 1).TileType == TileID.ClosedDoor || TileLists.ModdedDoors.Contains(Framing.GetTileSafely(tileX, tileY - 1).TileType)))
                {
                    doorCounter += 1f;
                    tickUpdater = 0f;
                    if (doorCounter >= doorCounterMax)
                    {
                        npc.velocity.X = 0.5f * -npc.spriteDirection;
                        doorBeatCounter += 1f;
                        doorCounter = 0f;
                        bool attemptOpenDoor = false;
                        if (doorBeatCounter >= doorBeatCounterMax)
                        {
                            attemptOpenDoor = true;
                            doorBeatCounter = 10f;
                        }
                        WorldGen.KillTile(tileX, tileY - 1, true);
                        if (attemptOpenDoor && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bool openedDoor = false;
                            if (interactDoorStyle != 0)
                            {
                                if (interactDoorStyle == 1)
                                {
                                    WorldGen.KillTile(tileX, tileY);
                                    openedDoor = !Main.tile[tileX, tileY].HasUnactuatedTile;
                                }
                                else
                                {
                                    openedDoor = WorldGen.OpenDoor(tileX, tileY, npc.spriteDirection);
                                }
                            }
                            if (!openedDoor)
                            {
                                tickUpdater = ticksUntilBoredom;
                                npc.netUpdate = true;
                            }
                            if (Main.netMode == NetmodeID.Server && openedDoor)
                                NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, NetworkText.FromLiteral(""), 0, tileX, tileY, npc.spriteDirection);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        #endregion


        /*
         * Checks if a space is completely devoid of solid tiles.
         */
        public static bool EmptyTiles(Rectangle rect)
        {
            int topX = rect.X / 16, topY = rect.Y / 16;
            for (int x = topX; x < topX + rect.Width; x++)
            {
                for (int y = topY; x < topY + rect.Height; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /*
         * Checks if a Entity object (Player, NPC, Item or Projectile) has hit a tile on it's sides.
         * 
         * noYMovement : If true, will not calculate unless the Entity is not moving on the Y axis.
         */
        public static bool HitTileOnSide(Entity codable, int dir, bool noYMovement = true)
        {
            if (!noYMovement || codable.velocity.Y == 0f)
            {
                Vector2 dummyVec = default;
                return HitTileOnSide(codable.position, codable.width, codable.height, dir, ref dummyVec);
            }
            return false;
        }

        /*
         * Checks if a bounding box has hit a tile on it's sides.
         * 
         * position : the position of the bounding box.
         * width : the width of the bounding box.
         * height : the height of the bounding box.
         * dir : The direction to check. 0 == left, 1 == right, 2 == up, 3 == down.
         * hitTilePos : A Vector2 that is set to the hit tile position, if it hit one.
         */
        public static bool HitTileOnSide(Vector2 position, int width, int height, int dir, ref Vector2 hitTilePos)
        {
            switch (dir)
            {
                case 0:
                    if (Collision.SolidCollision(position - new Vector2(1, 0), 8, height))
                        return true;
                    break;
                case 1:
                    if (Collision.SolidCollision(position + new Vector2(width - 8, 0), 9, height))
                        return true;
                    break;
                case 2:
                    if (Collision.SolidCollision(position - new Vector2(0, 1), width, 8))
                        return true;
                    break;
                case 3:
                    if (Collision.SolidCollision(position + new Vector2(0, height - 8), width, 9, true))
                        return true;
                    break;
            }
            int tilePosX = 0;
            int tilePosY = 0;
            int tilePosWidth = 0;
            int tilePosHeight = 0;
            if (dir == 0) //left
            {
                tilePosX = (int)(position.X - 8f) / 16;
                tilePosY = (int)position.Y / 16;
                tilePosWidth = tilePosX + 1;
                tilePosHeight = (int)(position.Y + height) / 16;
            }
            else
            if (dir == 1) //right
            {
                tilePosX = (int)(position.X + width + 8f) / 16;
                tilePosY = (int)position.Y / 16;
                tilePosWidth = tilePosX + 1;
                tilePosHeight = (int)(position.Y + height) / 16;
            }
            else
            if (dir == 2) //up, ie ceiling
            {
                tilePosX = (int)position.X / 16;
                tilePosY = (int)(position.Y - 8f) / 16;
                tilePosWidth = (int)(position.X + width) / 16;
                tilePosHeight = tilePosY + 1;
            }
            else
            if (dir == 3) //down, ie floor
            {
                tilePosX = (int)position.X / 16;
                tilePosY = (int)(position.Y + height + 8f) / 16;
                tilePosWidth = (int)(position.X + width) / 16;
                tilePosHeight = tilePosY + 1;
            }
            for (int x2 = tilePosX; x2 < tilePosWidth; x2++)
            {
                for (int y2 = tilePosY; y2 < tilePosHeight; y2++)
                {
                    if (Framing.GetTileSafely(x2, y2) == null)
                        return false;
                    bool solidTop = dir == 3 && Main.tileSolidTop[Framing.GetTileSafely(x2, y2).TileType];
                    if (Framing.GetTileSafely(x2, y2).HasUnactuatedTile && (Main.tileSolid[Framing.GetTileSafely(x2, y2).TileType] || solidTop))
                    {
                        hitTilePos = new Vector2(x2, y2);
                        return true;
                    }
                }
            }
            return false;
        }

        /* THESE TWO ARE WIP - BROKEN IN TMODLOADER */
        /*public static LootRule AddLoot(object npcTypes, int type, int amtMin, int amtMax, int chance)
		{
			return AddLoot(npcTypes, type, amtMin, amtMax, (float)chance / 100f);
		}
		public static LootRule AddLoot(object npcTypes, int type, int amtMin, int amtMax, float chance)
		{
			LootRule rule = new LootRule().Chance(chance).Item(type).Stack(amtMin, amtMax);
			if (npcTypes is int) LootRule.AddFor((int)npcTypes, rule);
			else if (npcTypes is int[]) LootRule.AddFor((int[])npcTypes, rule);
			else if (npcTypes is string) LootRule.AddFor((string)npcTypes, rule);
			else if (npcTypes is string[]) LootRule.AddFor((string[])npcTypes, rule);
			else if (npcTypes is Tuple<int, int>) LootRule.AddFor((Tuple<int, int>)npcTypes, rule);
			return rule;
		}*/



        public static void DamagePlayer(Player player, int dmgAmt, float knockback, Entity damager, bool dmgVariation = true, bool hitThroughDefense = false)
        {
            int hitDirection = damager == null ? 0 : damager.direction;
            DamagePlayer(player, dmgAmt, knockback, hitDirection, damager, dmgVariation, hitThroughDefense);
        }

        /*
         *  Damages the player by the given amount.
         * 
         *  dmgAmt : The amount of damage to inflict.
         *  knockback : The amount of knockback to inflict.
         *  hitDirection : The direction of the damage.
         *  damager : The thing actually doing damage (Player, Projectile, NPC or null)
         *  dmgVariation : If true, the damage will vary based on Main.DamageVar().
         *  hitThroughDefense : If true, boosts damage to get around player defense.
         */
        public static void DamagePlayer(Player player, int dmgAmt, float knockback, int hitDirection, Entity damager, bool dmgVariation = true, bool hitThroughDefense = false)
        {
            for (int i = 0; i < player.hurtCooldowns.Length; i++)
            {
                if (player.hurtCooldowns[i] > 0)
                    return;
            }
            if (player.immune)
                return;

            int parsedDamage = dmgAmt; if (dmgVariation) { parsedDamage = Main.DamageVar(dmgAmt); }
            Player.HurtModifiers stat = new();
            if (hitThroughDefense)
                stat.ScalingArmorPenetration += 1f;
            Player.HurtInfo hurtInfo = stat.ToHurtInfo(parsedDamage, player.statDefense, player.DefenseEffectiveness.Value, knockback, player.noKnockback);
            if (damager == null)
                player.Hurt(PlayerDeathReason.ByOther(-1), parsedDamage, hitDirection);
            else if (damager is Projectile p)
            {
                if (p.friendly)
                {
                    p.playerImmune[player.whoAmI] = 40;

                    ProjectileLoader.ModifyHitPlayer(p, player, ref stat);
                    ProjectileLoader.OnHitPlayer(p, player, hurtInfo);
                    PlayerLoader.ModifyHitByProjectile(player, p, ref stat);
                    PlayerLoader.OnHitByProjectile(player, p, hurtInfo);
                    player.Hurt(PlayerDeathReason.ByProjectile(p.owner, p.whoAmI), parsedDamage, hitDirection);
                }
                else if (p.hostile)
                {
                    ProjectileLoader.ModifyHitPlayer(p, player, ref stat);
                    ProjectileLoader.OnHitPlayer(p, player, hurtInfo);
                    PlayerLoader.ModifyHitByProjectile(player, p, ref stat);
                    PlayerLoader.OnHitByProjectile(player, p, hurtInfo);
                    player.Hurt(PlayerDeathReason.ByProjectile(-1, p.whoAmI), parsedDamage, hitDirection);
                }
                else
                {
                    ProjectileLoader.ModifyHitPlayer(p, player, ref stat);
                    ProjectileLoader.OnHitPlayer(p, player, hurtInfo);
                    PlayerLoader.ModifyHitByProjectile(player, p, ref stat);
                    PlayerLoader.OnHitByProjectile(player, p, hurtInfo);
                    player.Hurt(PlayerDeathReason.ByProjectile(-1, p.whoAmI), parsedDamage, hitDirection);
                }
            }
            else if (damager is NPC npc)
            {
                PlayerDeathReason death = PlayerDeathReason.ByNPC(npc.whoAmI);
                if (!PlayerLoader.ImmuneTo(player, death, -1, true))
                {
                    NPCLoader.ModifyHitPlayer(npc, player, ref stat);
                    NPCLoader.OnHitPlayer(npc, player, hurtInfo);
                    PlayerLoader.ModifyHitByNPC(player, npc, ref stat);
                    PlayerLoader.OnHitByNPC(player, npc, hurtInfo);
                    player.Hurt(death, parsedDamage, hitDirection);
                }
            }
        }

        /*
         *  Damages the given NPC by the given amount.
         */
        public static void DamageNPC(NPC npc, int dmgAmt, float knockback, Entity damager, bool dmgVariation = true, bool hitThroughDefense = false, bool crit = false, Item item = null)
        {
            int hitDirection = damager == null ? 0 : damager.direction;
            DamageNPC(npc, dmgAmt, knockback, hitDirection, damager, dmgVariation, hitThroughDefense, crit, item);
        }

        /*
         *  Damages the NPC by the given amount.
         *  
         *  dmgAmt : The amount of damage to inflict.
         *  knockback : The amount of knockback to inflict.
         *  hitDirection : The direction of the damage.
         *  damager : the thing actually doing damage (Player, Projectile or null)
         *  dmgVariation : If true, the damage will vary based on Main.DamageVar().
         *  hitThroughDefense : If true, boosts damage to get around npc defense.
         */
        public static void DamageNPC(NPC npc, int dmgAmt, float knockback, int hitDirection, Entity damager, bool dmgVariation = true, bool hitThroughDefense = false, bool crit = false, Item item = null)
        {
            item ??= new Item(ItemID.WoodenSword);
            if (npc.dontTakeDamage || (npc.immortal && npc.type != NPCID.TargetDummy))
                return;

            NPC.HitModifiers stat = new();
            if (hitThroughDefense)
                stat.ScalingArmorPenetration += 1f;
            if (damager == null || damager is NPC)
            {
                if (damager is NPC)
                {
                    NPCLoader.ModifyHitNPC(damager as NPC, npc, ref stat);
                    NPCLoader.OnHitNPC(damager as NPC, npc, default);
                    npc.Redemption().attacker = damager;
                }

                npc.SimpleStrikeNPC(dmgAmt, hitDirection, crit, knockback, null, dmgVariation);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, NetworkText.FromLiteral(""), npc.whoAmI, 1, knockback, hitDirection, dmgAmt);
            }
            else if (damager is Projectile p)
            {
                if (p.owner == Main.myPlayer && NPCLoader.CanBeHitByProjectile(npc, p) != false)
                {
                    NPCLoader.ModifyHitByProjectile(npc, p, ref stat);
                    NPCLoader.OnHitByProjectile(npc, p, default, dmgAmt);
                    PlayerLoader.ModifyHitNPCWithProj(Main.player[p.owner], p, npc, ref stat);
                    PlayerLoader.OnHitNPCWithProj(Main.player[p.owner], p, npc, default, dmgAmt);
                    ProjectileLoader.ModifyHitNPC(p, npc, ref stat);
                    ProjectileLoader.OnHitNPC(p, npc, default, dmgAmt);
                    if (p.HasElement(ElementID.Psychic))
                        npc.RedemptionGuard().IgnoreArmour = true;

                    if (!npc.immortal && npc.canGhostHeal && p.DamageType == DamageClass.Magic && Main.player[p.owner].setNebula && Main.player[p.owner].nebulaCD == 0 && Main.rand.NextBool(3))
                    {
                        Main.player[p.owner].nebulaCD = 30;
                        int num35 = Utils.SelectRandom(Main.rand, 3453, 3454, 3455);
                        int num36 = Item.NewItem(p.GetSource_OnHit(npc), (int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, num35);
                        Main.item[num36].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
                        Main.item[num36].velocity.X = Main.rand.Next(10, 31) * 0.2f * hitDirection;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num36);
                        }
                    }

                    npc.SimpleStrikeNPC(dmgAmt, hitDirection, crit, knockback, null, dmgVariation);
                    if (Main.player[p.owner].accDreamCatcher)
                        Main.player[p.owner].addDPS(dmgAmt);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, NetworkText.FromLiteral(""), npc.whoAmI, 1, knockback, hitDirection, dmgAmt);

                    if (p.penetrate != 1) { npc.immune[p.owner] = 10; }
                }
            }
            else if (damager is Player player)
            {
                if (player.whoAmI == Main.myPlayer && NPCLoader.CanBeHitByItem(npc, player, item) != false)
                {
                    NPCLoader.ModifyHitByItem(npc, player, item, ref stat);
                    NPCLoader.OnHitByItem(npc, player, item, default, dmgAmt);
                    PlayerLoader.ModifyHitNPC(player, npc, ref stat);
                    PlayerLoader.OnHitNPC(player, npc, default, dmgAmt);

                    npc.SimpleStrikeNPC(dmgAmt, hitDirection, crit, knockback, null, dmgVariation);
                    if (player.accDreamCatcher)
                        player.addDPS(dmgAmt);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, NetworkText.FromLiteral(""), npc.whoAmI, 1, knockback, hitDirection, dmgAmt);
                    npc.immune[player.whoAmI] = player.itemAnimation;
                }
            }
        }

        /*
         * Convenience method that handles killing an NPC and having it drop loot.
         * If you want the NPC to just dissapear, use KillNPC().
         */
        public static void KillNPCWithLoot(NPC npc)
        {
            DamageNPC(npc, npc.lifeMax + npc.defense + 1, 0f, 0, null, false, true);
        }

        /*
         * Convenience method that handles killing an NPC without loot.
         */
        public static void KillNPC(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            npc.active = false;
            int npcID = npc.whoAmI;
            Main.npc[npcID] = new NPC();
            if (Main.netMode == NetmodeID.Server) NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcID);
        }

        public static int GetProjectile(Vector2 center, int projType = -1, int owner = -1, float distance = -1, Func<Projectile, bool> canAdd = null)
        {
            return GetProjectile(center, projType, owner, default, distance, canAdd);
        }
        /*
		 * Gets the closest Projectile with the given type within the given distance from the center. If distance is -1, it gets the closest Projectile.
		 * 
		 * projType : If -1, check for ANY projectiles in the area. If not, check for the projectiles who match the type given.
		 * projsToExclude : An array of projectile whoAmIs to exclude from the search.
		 * distance : The distance to check.
		 */
        public static int GetProjectile(Vector2 center, int projType = -1, int owner = -1, int[] projsToExclude = default, float distance = -1, Func<Projectile, bool> canAdd = null)
        {
            int currentProj = -1;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj is { active: true } && (projType == -1 || proj.type == projType) && (owner == -1f || proj.owner == owner) && (distance == -1f || proj.Distance(center) < distance))
                {
                    bool add = true;
                    if (projsToExclude != default(int[]))
                    {
                        foreach (int m in projsToExclude)
                        {
                            if (m == proj.whoAmI) { add = false; break; }
                        }
                    }
                    if (add && canAdd != null && !canAdd(proj)) { continue; }
                    if (add)
                    {
                        distance = proj.Distance(center);
                        currentProj = i;
                    }
                }
            }
            return currentProj;
        }

        public static int[] GetProjectiles(Vector2 center, int projType = -1, int owner = -1, float distance = 500f, Func<Projectile, bool> canAdd = null)
        {
            return GetProjectiles(center, projType, owner, default, distance, canAdd);
        }
        /*
		 * Gets the all Projectiles with the given type within the given distance from the center.
		 * 
         * projType : If -1, check for ANY projectiles in the area. If not, check for the projectiles who match the type given.
         * projsToExclude : An array of projectile whoAmIs to exclude from the search.
         * distance : The distance to check.
		 */
        public static int[] GetProjectiles(Vector2 center, int projType = -1, int owner = -1, int[] projsToExclude = default, float distance = 500f, Func<Projectile, bool> canAdd = null)
        {
            List<int> allProjs = new();
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj is { active: true } && (projType == -1 || proj.type == projType) && (owner == -1 || proj.owner == owner) && (distance == -1 || proj.Distance(center) < distance))
                {
                    bool add = true;
                    if (projsToExclude != default(int[]))
                    {
                        foreach (int m in projsToExclude)
                        {
                            if (m == proj.whoAmI) { add = false; break; }
                        }
                    }
                    if (add && canAdd != null && !canAdd(proj)) { continue; }
                    if (add) { allProjs.Add(i); }
                }
            }
            return allProjs.ToArray();
        }


        public static int[] GetProjectiles(Vector2 center, int[] projTypes, int owner = -1, float distance = 500f, Func<Projectile, bool> canAdd = null)
        {
            return GetProjectiles(center, projTypes, owner, default, distance, canAdd);
        }

        /*
		 * Gets the all Projectiles with the given type within the given distance from the center.
		 * 
         * projType : If -1, check for ANY projectiles in the area. If not, check for the projectiles who match the type given.
         * projsToExclude : An array of projectile whoAmIs to exclude from the search.
         * distance : The distance to check.
		 */
        public static int[] GetProjectiles(Vector2 center, int[] projTypes, int owner = -1, int[] projsToExclude = default, float distance = 500f, Func<Projectile, bool> canAdd = null)
        {
            List<int> allProjs = new();
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj is { active: true } && (owner == -1 || proj.owner == owner) && (distance == -1 || proj.Distance(center) < distance))
                {
                    bool isType = false;
                    foreach (int type in projTypes) { if (proj.type == type) { isType = true; break; } }
                    if (!isType) { continue; }
                    bool add = true;
                    if (projsToExclude != default(int[]))
                    {
                        foreach (int m in projsToExclude)
                        {
                            if (m == proj.whoAmI) { add = false; break; }
                        }
                    }
                    if (add && canAdd != null && !canAdd(proj)) { continue; }
                    if (add) { allProjs.Add(i); }
                }
            }
            return allProjs.ToArray();
        }

        /*
		 * Gets all NPCs of the given type within the given rectangle.
		 * 
		 * rect : The box to check.
		 * npcType : If -1, check for ANY npcs in the area. If not, check for the npcs who match the type given.
		 * npcsToExclude : An array of npc whoAmIs to exclude from the search.
		 */
        public static int[] GetNPCsInBox(Rectangle rect, int npcType = -1, int[] npcsToExclude = default, Func<NPC, bool> canAdd = null)
        {
            List<int> allNPCs = new();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc is { active: true, life: > 0 } && (npcType == -1 || npc.type == npcType) && npc.type != NPCID.TargetDummy)
                {
                    if (!rect.Intersects(npc.Hitbox)) continue;
                    bool add = true;
                    if (npcsToExclude != default(int[]))
                    {
                        foreach (int m in npcsToExclude)
                        {
                            if (m == npc.whoAmI) { add = false; break; }
                        }
                    }
                    if (add && canAdd != null && !canAdd(npc)) continue;
                    if (add) { allNPCs.Add(i); }
                }
            }
            return allNPCs.ToArray();
        }

        public static int GetNPC(Vector2 center, int npcType = -1, float distance = -1, Func<NPC, bool> canAdd = null)
        {
            return GetNPC(center, npcType, default, distance, canAdd);
        }
        /*
         * Gets the closest NPC with the given type within the given distance from the center. If distance is -1, it gets the closest NPC.
         * 
         * npcType : If -1, check for ANY npcs in the area. If not, check for the npcs who match the type given.
         * npcsToExclude : An array of npc whoAmIs to exclude from the search.
         * distance : The distance to check.
         */
        public static int GetNPC(Vector2 center, int npcType = -1, int[] npcsToExclude = default, float distance = -1, Func<NPC, bool> canAdd = null)
        {
            int currentNPC = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc is { active: true, life: > 0 } && (npcType == -1 || npc.type == npcType) && npc.type != NPCID.TargetDummy && (distance == -1f || npc.Distance(center) < distance))
                {
                    bool add = true;
                    if (npcsToExclude != default(int[]))
                    {
                        foreach (int m in npcsToExclude)
                        {
                            if (m == npc.whoAmI) { add = false; break; }
                        }
                    }
                    if (add && canAdd != null && !canAdd(npc)) { continue; }
                    if (add)
                    {
                        distance = npc.Distance(center);
                        currentNPC = i;
                    }
                }
            }
            return currentNPC;
        }

        public static int[] GetNPCs(Vector2 center, int npcType = -1, float distance = 500F, Func<NPC, bool> canAdd = null)
        {
            return GetNPCs(center, npcType, Array.Empty<int>(), distance, canAdd);
        }
        /*
         * Gets all NPCs of the given type within a given distance from the center.
         * 
         * npcType : If -1, check for ANY npcs in the area. If not, check for the npcs who match the type given.
         * npcsToExclude : an array of npc whoAmIs to exclude from the search.
         * distance : the distance to check.
         */
        public static int[] GetNPCs(Vector2 center, int npcType = -1, int[] npcsToExclude = default, float distance = 500F, Func<NPC, bool> canAdd = null)
        {
            List<int> allNPCs = new();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc is { active: true, life: > 0 } && (npcType == -1 || npc.type == npcType) && npc.type != NPCID.TargetDummy && (distance == -1 || npc.Distance(center) < distance))
                {
                    bool add = true;
                    if (npcsToExclude != default(int[]))
                    {
                        foreach (int m in npcsToExclude)
                        {
                            if (m == npc.whoAmI) { add = false; break; }
                        }
                    }
                    if (add && canAdd != null && !canAdd(npc)) { continue; }
                    if (add) { allNPCs.Add(i); }
                }
            }
            return allNPCs.ToArray();
        }

        /*
         * Gets all players colliding with the given rectangle.
         * 
         * rect : The rectangle to search.
         * playersToExclude : An array of player whoAmis that will be excluded from the search.
         */
        public static int[] GetPlayersInBox(Rectangle rect, int[] playersToExclude = default, Func<Player, bool> canAdd = null)
        {
            List<int> allPlayers = new();
            for (int i = 0; i < Main.player.Length; i++)
            {
                Player plr = Main.player[i];
                if (plr is { active: true, dead: false })
                {
                    if (!rect.Intersects(plr.Hitbox)) { continue; }
                    bool add = true;
                    if (playersToExclude != default(int[]))
                    {
                        foreach (int m in playersToExclude)
                        {
                            if (m == plr.whoAmI) { add = false; break; }
                        }
                    }
                    if (add && canAdd != null && !canAdd(plr)) { continue; }
                    if (add) { allPlayers.Add(i); }
                }
            }
            return allPlayers.ToArray();
        }

        /*
         * Gets the player whoAmI connected to the player with the given name, or -1 if they aren't found.
         * 
         * aliveOnly : If true, it only returns the player whoAmI if the player is not dead.
         */
        public static int GetPlayerByName(string name, bool aliveOnly = true)
        {
            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];
                if (player is { active: true } && (!aliveOnly || !player.dead))
                {
                    if (player.name == name) { return i; }
                }
            }
            return -1;
        }

        public static int GetPlayer(Vector2 center, float distance = -1, Func<Player, bool> canAdd = null)
        {
            return GetPlayer(center, default, true, distance, canAdd);
        }
        /*
         * Gets the closest player within the given distance from the center. If distance is -1, it gets the closest player.
         * 
         * playersToExclude : An array of player whoAmis that will be excluded from the search.
         * aliveOnly : If true, it only returns the player whoAmI if the player is not dead.
         * distance : The distance to search.
         */
        public static int GetPlayer(Vector2 center, int[] playersToExclude = default, bool activeOnly = true, float distance = -1, Func<Player, bool> canAdd = null)
        {
            int currentPlayer = -1;
            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];
                if (player != null && (!activeOnly || player.active && !player.dead) && (distance == -1f || player.Distance(center) < distance))
                {
                    bool add = true;
                    if (playersToExclude != default(int[]))
                    {
                        foreach (int m in playersToExclude)
                        {
                            if (m == player.whoAmI) { add = false; break; }
                        }
                    }
                    if (add && canAdd != null && !canAdd(player)) { continue; }
                    if (add)
                    {
                        distance = player.Distance(center);
                        currentPlayer = i;
                    }
                }
            }
            return currentPlayer;
        }

        public static int[] GetPlayers(Vector2 center, float distance = 500F, Func<Player, bool> canAdd = null)
        {
            return GetPlayers(center, default, true, distance, canAdd);
        }
        /*
         * Gets all players within a given distance from the center.
         * 
         * playersToExclude is an array of player ids you do not want included in the array.
         * aliveOnly : If true, it only returns the player whoAmI if the player is not dead.
         */
        public static int[] GetPlayers(Vector2 center, int[] playersToExclude = default, bool aliveOnly = true, float distance = 500F, Func<Player, bool> canAdd = null)
        {
            List<int> allPlayers = new();
            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];
                if (player is { active: true } && (!aliveOnly || !player.dead) && player.Distance(center) < distance)
                {
                    bool add = true;
                    if (playersToExclude != default(int[]))
                    {
                        foreach (int m in playersToExclude)
                        {
                            if (m == player.whoAmI) { add = false; break; }
                        }
                    }
                    if (add && canAdd != null && !canAdd(player)) { continue; }
                    if (add) { allPlayers.Add(i); }
                }
            }
            return allPlayers.ToArray();
        }

        /*
         * Returns true if the player can target the given codable.
         */
        public static bool CanTarget(Player player, Entity codable)
        {
            if (codable is NPC npc)
            {
                return npc.life > 0 && (!npc.friendly || npc.type == NPCID.Guide && player.killGuide) && !npc.dontTakeDamage;
            }

            if (codable is Player player2)
            {
                return player2.statLife > 0 && !player2.immune && player2.hostile && (player.team == 0 || player2.team == 0 || player.team != player2.team);
            }
            return false;
        }

        /*
         * Sets the npc's target to the given target and adjusts the according variables.
         */
        public static void SetTarget(NPC npc, int target)
        {
            npc.target = target;
            if (npc.target is < 0 or >= 255) { npc.target = 0; }
            npc.targetRect = Main.player[npc.target].Hitbox;
            if (npc.target != npc.oldTarget && !npc.collideX && !npc.collideY)
            {
                npc.netUpdate = true;
            }
        }

        public static void Look(Projectile p, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.1f, bool flipSpriteDir = false)
        {
            Look(p, ref p.rotation, ref p.spriteDirection, lookType, rotAddon, rotAmount, flipSpriteDir);
        }
        public static void Look(NPC npc, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.1f, bool flipSpriteDir = false)
        {
            Look(npc, ref npc.rotation, ref npc.spriteDirection, lookType, rotAddon, rotAmount, flipSpriteDir);
        }
        /*
         * Makes the rotation value and sprite direction 'look' based on factors from the Entity.
         * lookType : the type of look code to run.
         *        0 -> Rotates the entity and changes spriteDirection based on velocity.
         *        1 -> changes spriteDirection based on velocity.
         *        2 -> Rotates the entity based on velocity.
         *        3 -> Smoothly rotate and change sprite direction based on velocity.
         *        4 -> Smoothly rotate based on velocity. 
         * rotAddon : the amount to add to the rotation. (only used by lookType 3/4)
         * rotAmount: the amount to rotate by. (only used by lookType 3/4)
         */
        public static void Look(Entity c, ref float rotation, ref int spriteDirection, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.1f, bool flipSpriteDir = false)
        {
            LookAt(c.position + c.velocity, c.position, ref rotation, ref spriteDirection, lookType, rotAddon, rotAmount, flipSpriteDir);
        }

        public static void LookAt(Vector2 lookTarget, Entity c, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.1f, bool flipSpriteDir = false)
        {
            int spriteDirection = c is NPC nPc1 ? nPc1.spriteDirection : c is Projectile projectile1 ? projectile1.spriteDirection : 0;
            float rotation = c is NPC nPc ? nPc.rotation : c is Projectile projectile ? projectile.rotation : 0f;
            LookAt(lookTarget, c.Center, ref rotation, ref spriteDirection, lookType, rotAddon, rotAmount, flipSpriteDir);
            if (c is NPC nPc2)
            {
                nPc2.spriteDirection = spriteDirection;
                nPc2.rotation = rotation;
            }
            else
            if (c is Projectile projectile2)
            {
                projectile2.spriteDirection = spriteDirection;
                projectile2.rotation = rotation;
            }
        }

        /*
         * Makes the rotation value and sprite direction 'look' at the given target.
         * lookType : the type of look code to run.
         *        0 -> Rotate the entity and change sprite direction based on the look target.
         *        1 -> change spriteDirection based on the look target.
         *        2 -> Rotate the entity based on the look target.
         *        3 -> Smoothly rotate and change sprite direction based on the look target.
         *        4 -> Smoothly rotate based on the look target.       
         * rotAddon : the amount to add to the rotation. (only used by lookType 3/4)
         * rotAmount: the amount to rotate by. (only used by lookType 3/4)
         */
        public static void LookAt(Vector2 lookTarget, Vector2 center, ref float rotation, ref int spriteDirection, int lookType = 0, float rotAddon = 0f, float rotAmount = 0.075f, bool flipSpriteDir = false)
        {
            if (lookType == 0)
            {
                if (lookTarget.X > center.X) { spriteDirection = -1; } else { spriteDirection = 1; }
                if (flipSpriteDir) { spriteDirection *= -1; }
                float rotX = lookTarget.X - center.X;
                float rotY = lookTarget.Y - center.Y;
                rotation = -((float)Math.Atan2(rotX, rotY) - 1.57f + rotAddon);
                if (spriteDirection == 1) { rotation -= (float)Math.PI; }
            }
            else
            if (lookType == 1)
            {
                if (lookTarget.X > center.X) { spriteDirection = -1; } else { spriteDirection = 1; }
                if (flipSpriteDir) { spriteDirection *= -1; }
            }
            else
            if (lookType == 2)
            {
                float rotX = lookTarget.X - center.X;
                float rotY = lookTarget.Y - center.Y;
                rotation = -((float)Math.Atan2(rotX, rotY) - 1.57f + rotAddon);
            }
            else
            if (lookType is 3 or 4)
            {
                int oldDirection = spriteDirection;
                if (lookType == 3 && lookTarget.X > center.X) { spriteDirection = -1; } else { spriteDirection = 1; }
                if (lookType == 3 && flipSpriteDir) { spriteDirection *= -1; }
                if (oldDirection != spriteDirection)
                {
                    rotation += (float)Math.PI * spriteDirection;
                }
                float pi2 = (float)Math.PI * 2f;
                float rotX = lookTarget.X - center.X;
                float rotY = lookTarget.Y - center.Y;
                float rot = (float)Math.Atan2(rotY, rotX) + rotAddon;
                if (spriteDirection == 1) { rot += (float)Math.PI; }
                if (rot > pi2) { rot -= pi2; } else if (rot < 0) { rot += pi2; }
                if (rotation > pi2) { rotation -= pi2; } else if (rotation < 0) { rotation += pi2; }
                if (rotation < rot)
                {
                    if ((double)(rot - rotation) > (float)Math.PI) { rotation -= rotAmount; } else { rotation += rotAmount; }
                }
                else
                if (rotation > rot)
                {
                    if ((double)(rotation - rot) > (float)Math.PI) { rotation += rotAmount; } else { rotation -= rotAmount; }
                }
                if (rotation > rot - rotAmount && rotation < rot + rotAmount) { rotation = rot; }
            }
        }

        public static void RotateTo(ref float rotation, float rotDestination, float rotAmount = 0.075f)
        {
            float pi2 = (float)Math.PI * 2f;
            float rot = rotDestination;
            if (rot > pi2) { rot -= pi2; } else if (rot < 0) { rot += pi2; }
            if (rotation > pi2) { rotation -= pi2; } else if (rotation < 0) { rotation += pi2; }
            if (rotation < rot)
            {
                if ((double)(rot - rotation) > (float)Math.PI) { rotation -= rotAmount; } else { rotation += rotAmount; }
            }
            else
            if (rotation > rot)
            {
                if ((double)(rotation - rot) > (float)Math.PI) { rotation += rotAmount; } else { rotation -= rotAmount; }
            }
            if (rotation > rot - rotAmount && rotation < rot + rotAmount) { rotation = rot; }
        }

        public static Vector2 TraceTile(Vector2 start, float distance, float rotation, Vector2 ignoreTile, bool npcCheck = true, bool tileCheck = true, bool playerCheck = true, bool ignorePlatforms = true)
        {
            Vector2 end = BaseUtility.RotateVector(start, start + new Vector2(distance, 0f), rotation);
            return Trace(start, end, ignoreTile, 1, npcCheck, tileCheck, playerCheck, 1F, ignorePlatforms);
        }

        public static Vector2 TracePlayer(Vector2 start, float distance, float rotation, int ignorePlayer, bool npcCheck = true, bool tileCheck = true, bool playerCheck = true, bool ignorePlatforms = true)
        {
            Vector2 end = BaseUtility.RotateVector(start, start + new Vector2(distance, 0f), rotation);
            return Trace(start, end, ignorePlayer, 0, npcCheck, tileCheck, playerCheck, 1F, ignorePlatforms);
        }

        public static Vector2 TraceNPC(Vector2 start, float distance, float rotation, int ignoreNPC, bool npcCheck = true, bool tileCheck = true, bool playerCheck = true, bool ignorePlatforms = true)
        {
            Vector2 end = BaseUtility.RotateVector(start, start + new Vector2(distance, 0f), rotation);
            return Trace(start, end, ignoreNPC, 2, npcCheck, tileCheck, playerCheck, 1F, ignorePlatforms);
        }

        public static Vector2 Trace(Vector2 start, Vector2 end, object ignore, int ignoreType, bool npcCheck = true, bool tileCheck = true, bool playerCheck = true, float jump = 1F, bool ignorePlatforms = true)
        {
            return Trace(start, end, ignore, ignoreType, null, npcCheck, tileCheck, playerCheck, false, jump, ignorePlatforms);
        }

        public static Vector2 Trace(Vector2 start, Vector2 end, object ignore, int ignoreType, object dim, bool npcCheck = true, bool tileCheck = true, bool playerCheck = true, bool returnCenter = false, float jump = 1F, bool ignorePlatforms = true)
        {
            return Trace(start, end, ignore, ignoreType, dim, npcCheck, tileCheck, playerCheck, returnCenter, ignorePlatforms ? new[] { 19 } : default, jump); //ignores wooden platforms
        }

        /* **Code edited from Yoraiz0r's 'Holowires' Mod!**
         * 
         * From the start point, it iterates to the end point. If it hits anything on the way, it will return the collision point. If not it returns the end point.
         * 
         * dim : a Rectangle instance of the collision's dimensions. Can be null.
         * npcCheck : If true, Check for npc collision while iterating.
         * tileCheck : If true, check for tile collision while iterating.
         * playerCheck : If true, check for player collision while iterating.
         * returnCenter : If true, if it hits anything it returns it's center instead of where it hit.
         * tileTypesToIgnore : An array of tile types that it will assume it can trace through.
         * Jump: The amount to iterate by.
         */
        public static Vector2 Trace(Vector2 start, Vector2 end, object ignore, int ignoreType, object dim, bool npcCheck = true, bool tileCheck = true, bool playerCheck = true, bool returnCenter = false, int[] tileTypesToIgnore = default, float jump = 1F)
        {
            try
            {
                if (ignore == null) { return start; }
                if (dim == null) { dim = new Rectangle(0, 0, 1, 1); }
                if (start.X < 0) { start.X = 0; }
                if (start.X > Main.maxTilesX * 16) { start.X = Main.maxTilesX * 16; }
                if (start.Y < 0) { start.Y = 0; }
                if (start.Y > Main.maxTilesY * 16) { start.Y = Main.maxTilesY * 16; }
                if (end.X < 0) { end.X = 0; }
                if (end.X > Main.maxTilesX * 16) { end.X = Main.maxTilesX * 16; }
                if (end.Y < 0) { end.Y = 0; }
                if (end.Y > Main.maxTilesY * 16) { end.Y = Main.maxTilesY * 16; }
                Vector2 tc = new(1, 1);
                Vector2 pstart = start;
                Vector2 pend = end;
                Vector2 dir = pend - pstart;
                dir.Normalize();
                float length = Vector2.Distance(pstart, pend);
                float way = 0f;
                while (way < length)
                {
                    Vector2 v = pstart + dir * way + tc;
                    Rectangle dimensions = (Rectangle)dim;
                    Rectangle posRect = new((int)v.X - (dimensions.Width == 1 ? 0 : dimensions.Width / 2), (int)v.Y - (dimensions.Height == 1 ? 0 : dimensions.Height / 2), dimensions.Width, dimensions.Height);
                    if (tileCheck)
                    {
                        int vecX = (int)v.X / 16;
                        int vecY = (int)v.Y / 16;
                        Rectangle rect = new((int)v.X, (int)v.Y, 16, 16);
                        if (posRect.Intersects(rect))
                        {
                            Vector2 vec = ignoreType == 1 ? (Vector2)ignore : new Vector2(-1, -1);
                            if ((int)vec.X != vecX && (int)vec.Y != vecY)
                            {
                                Tile tile = Framing.GetTileSafely(vecX, vecY);
                                if (tile is { HasUnactuatedTile: true })
                                {
                                    bool ignoreTile = tileTypesToIgnore is { Length: > 0 } && BaseUtility.InArray(tileTypesToIgnore, tile.TileType);
                                    if (!ignoreTile && Main.tileSolid[tile.TileType])
                                    {
                                        return returnCenter ? new Vector2(vecX * 16 + 8, vecY * 16 + 8) : v;
                                    }
                                }
                            }
                        }
                    }
                    if (npcCheck)
                    {
                        int[] npcs = GetNPCs(v, -1, 5F);
                        for (int i = 0; i < npcs.Length; i++)
                        {
                            NPC npc = Main.npc[npcs[i]];
                            if (!npc.active || npc.life <= 0) { continue; }
                            if (ignoreType == 2 && npc.whoAmI == (int)ignore) { continue; }
                            Rectangle npcRect = new((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                            if (posRect.Intersects(npcRect)) { return returnCenter ? npc.Center : v; }
                        }
                    }
                    if (playerCheck)
                    {
                        int[] players = GetPlayers(v, 5F);
                        for (int i = 0; i < players.Length; i++)
                        {
                            Player player = Main.player[players[i]];
                            if (player.dead || !player.active) { continue; }
                            if (ignoreType == 0 && player.whoAmI == (int)ignore) { continue; }
                            Rectangle playerRect = new((int)player.position.X, (int)player.position.Y, player.width, player.height);
                            if (posRect.Intersects(playerRect)) { return returnCenter ? player.Center : v; }
                        }
                    }
                    way += jump;
                }
            }
            catch (Exception e)
            {
                BaseUtility.LogFancy("Redemption~ TRACE ERROR:", e);
            }
            return end;
        }

        /*
         * 
         * Returns an array of points in a straight line from start to end.
         * jump : the interval between points.
         */
        public static Vector2[] GetLinePoints(Vector2 start, Vector2 end, float jump = 1f)
        {
            Vector2 dir = end - start;
            dir.Normalize();
            float length = Vector2.Distance(start, end); float way = 0f;
            List<Vector2> vList = new();
            while (way < length)
            {
                vList.Add(start + dir * way);
                way += jump;
            }
            return vList.ToArray();
        }
    }
}