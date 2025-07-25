using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.NPCs.Bosses.Obliterator;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Redemption.Globals.RedeNet;

namespace Redemption.Globals
{
    public enum ArenaBoss
    {
        None,
        OO,
        OC,
        ADD
    }
    public class ArenaSystem : ModSystem
    {
        private static Asset<Texture2D> _omegaBarrierAsset;
        private static Asset<Texture2D> _cloudBarrierAsset;
        private static Asset<Texture2D> _cloudBarrierFogAsset;

        /// <summary>
        /// If true, an arena is active somewhere in the world.
        /// </summary>
        public static bool ArenaActive { get; internal set; }

        /// <summary>
        /// The arena boss currently alive, or <see cref="ArenaBoss.None"/> if no arena boss is alive.
        /// </summary>
        public static ArenaBoss ArenaBoss { get; internal set; }

        /// <summary>
        /// The bounds of the current arena in tile coordinates.
        /// </summary>
        public static Rectangle ArenaBoundsTile { get; internal set; }

        /// <summary>
        /// The index of the player in <see cref="Main.player"/> who is fighting in this arena solo, or <c>-1</c> if no solo fight is taking place.
        /// </summary>
        public static int SoloPlayer { get; internal set; }

        /// <summary>
        /// Indexes for players that should be kept inside the arena
        /// When a player dies, they are removed from this array and cannot enter the arena
        /// Once empty, the boss disappears
        /// </summary>
        public static HashSet<int> ArenaPlayerIDs { get; } = [];

        private static IReadOnlyDictionary<ArenaBoss, Point> ArenaSizes { get => arenaSizes; set => arenaSizes = value; }
        private static IReadOnlyDictionary<ArenaBoss, Point> arenaSizes;

        /// <summary>
        /// The bounds of the current arena in world coordinates.
        /// </summary>
        public static Rectangle ArenaBoundsWorld => new(ArenaBoundsTile.X * 16, ArenaBoundsTile.Y * 16, (ArenaBoundsTile.Width * 16) + 16, ArenaBoundsTile.Height * 16);

        /// <summary>
        /// The bottom-middle of the arena in world coordinates.
        /// </summary>
        public static Vector2 ArenaOriginWorld => new Vector2(ArenaBoundsWorld.X + (ArenaBoundsWorld.Width / 2), ArenaBoundsWorld.Bottom);

        private static int _despawnCounter;

        private enum FailType
        {
            OutOfWorld,
            Obstruction,
            Gap
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawPlayers_AfterProjectiles += DrawBarrierOverPlayers;

            _omegaBarrierAsset = Request<Texture2D>("Redemption/Textures/OOBarrier");
            _cloudBarrierAsset = Request<Texture2D>("Redemption/Textures/UkkoBarrier");
            _cloudBarrierFogAsset = Request<Texture2D>("Redemption/Textures/UkkoBarrier_Fog");
        }

        public override void PostSetupContent()
        {
            ArenaSizes = new Dictionary<ArenaBoss, Point>
            {
                { ArenaBoss.None, Point.Zero },
                { ArenaBoss.OO, new Point(240, 240) },
                { ArenaBoss.OC, new Point(240, 598) },
                { ArenaBoss.ADD, new Point(240, 240) }
            };
        }

        public override void Unload()
        {
            ArenaSizes = null;
            if (Main.dedServ)
                return;

            On_Main.DrawPlayers_AfterProjectiles -= DrawBarrierOverPlayers;
        }

        public override void ClearWorld()
        {
            DeactivateArena();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(ArenaActive);
            writer.Write((byte)ArenaBoss);
            writer.Write((short)ArenaBoundsTile.X);
            writer.Write((short)ArenaBoundsTile.Y);
            writer.Write((short)ArenaBoundsTile.Width);
            writer.Write((short)ArenaBoundsTile.Height);
            writer.Write((short)SoloPlayer);
            writer.Write7BitEncodedInt(ArenaPlayerIDs.Count);
            foreach (int playerID in ArenaPlayerIDs)
            {
                writer.Write((byte)playerID);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            ArenaActive = reader.ReadBoolean();
            ArenaBoss = (ArenaBoss)reader.ReadByte();
            ArenaBoundsTile = new Rectangle(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
            SoloPlayer = reader.ReadInt16();
            int count = reader.Read7BitEncodedInt();
            ArenaPlayerIDs.Clear();
            for (int i = 0; i < count; ++i)
            {
                ArenaPlayerIDs.Add(reader.ReadByte());
            }
        }

        public override void PostUpdateNPCs()
        {
            if (!ArenaActive || ArenaBoss == ArenaBoss.None || Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            if (!ArenaBossAlive(ArenaBoss))
            {
                _despawnCounter++;
                if (_despawnCounter >= 10)
                {
                    DeactivateArena();
                }
            }
            else
            {
                _despawnCounter = 0;
            }
        }

        public override void PostUpdatePlayers()
        {
            if (SoloPlayer > -1 && !Main.player[SoloPlayer].active && Main.netMode != NetmodeID.MultiplayerClient)
            {
                _despawnCounter++;
                if (_despawnCounter >= 10)
                {
                    DeactivateArena();
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && ArenaPlayerIDs.Count != 0)
            {
                int removedCount = ArenaPlayerIDs.RemoveWhere(id => !Main.player[id].active || Main.player[id].DeadOrGhost);
                if (removedCount > 0 && Main.netMode == NetmodeID.Server)
                {
                    SendSyncArena();
                }
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Terraria.Player player = Main.LocalPlayer;
                int direction = player.Center.X > ArenaBoundsWorld.Center.X ? 1 : -1;

                float distanceX = Math.Abs(player.Center.X - ArenaBoundsWorld.Center.X);
                float distanceY = Math.Abs(player.Center.Y - ArenaBoundsWorld.Center.Y);

                Visibility = 0f;

                if (distanceX >= ArenaBoundsWorld.Width / 2 - 500)
                {
                    distanceX -= ArenaBoundsWorld.Width / 2 - 500;

                    if (distanceX > 500f)
                    {
                        distanceX = 500f;
                    }

                    Visibility += 0.5f * distanceX / 500f;
                }
                if (distanceY >= ArenaBoundsWorld.Height / 2 - 500)
                {
                    distanceY -= ArenaBoundsWorld.Height / 2 - 500;

                    if (distanceY > 500f)
                    {
                        distanceY = 500f;
                    }

                    Visibility += 0.5f * distanceY / 500f;
                }
                Visibility = MathHelper.Min(Visibility, 1f);
            }
        }

        private float Visibility = 0f;
        private float BarrierAnimation = 0f;

        public override void PostDrawTiles()
        {
            switch (ArenaBoss)
            {
                case ArenaBoss.OO or ArenaBoss.OC:

                    SpriteBatch spriteBatch = Main.spriteBatch;
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                    Texture2D texture = _omegaBarrierAsset.Value;

                    int drawHeight = texture.Height;

                    Vector2 drawPositionLR = ArenaOriginWorld - Main.screenPosition;
                    Vector2 drawPositionUD = drawPositionLR;
                    drawPositionLR.Y += drawHeight * 200f;
                    drawPositionUD.X += drawHeight * 200f;
                    Color drawColor = RedeColor.RedPulse * Visibility;

                    for (int i = 0; i < 400; i++)
                    {
                        for (int j = -1; j <= 1; j += 2)
                        {
                            Vector2 origin = new(texture.Width, texture.Height);
                            SpriteEffects effects = j == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                            if (j == 1)
                            {
                                origin.X = 0;
                            }

                            Vector2 leftRightOffset = new(ArenaBoundsWorld.Width / 2 * j, -ArenaBoundsWorld.Height / 2);

                            spriteBatch.Draw(texture, drawPositionLR + leftRightOffset, null, drawColor, 0f, origin, 1f, effects, 0);
                        }
                        for (int j = -1; j <= 1; j += 2)
                        {
                            Vector2 origin = new(texture.Width, texture.Height);
                            SpriteEffects effects = j == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                            if (j == 1)
                            {
                                origin.X = 0;
                            }

                            Vector2 upDownOffset = new(0f, -ArenaBoundsWorld.Height / 2 + (ArenaBoundsWorld.Height / 2 * j));

                            spriteBatch.Draw(texture, drawPositionUD + upDownOffset, null, drawColor, MathHelper.PiOver2, origin, 1f, effects, 0);
                        }

                        drawPositionLR.Y -= drawHeight;
                        drawPositionUD.X -= drawHeight;
                    }

                    spriteBatch.End();
                    break;
            }
        }
        private void DrawBarrierOverPlayers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);

            if (ArenaBoss is ArenaBoss.ADD)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                Texture2D texture = _cloudBarrierAsset.Value;
                Texture2D textureFog = _cloudBarrierFogAsset.Value;

                int drawHeight = texture.Height;

                Vector2 drawPositionLR = ArenaOriginWorld - Main.screenPosition;
                Vector2 drawPositionUD = drawPositionLR;
                drawPositionLR.Y += drawHeight * 10f;
                drawPositionUD.X += drawHeight * 10f;
                Color drawColor = Color.Gray;

                BarrierAnimation += 25.6f;
                if (BarrierAnimation >= 1024)
                    BarrierAnimation = 0;

                for (int i = 0; i < 20; i++)
                {
                    for (int j = -1; j <= 1; j += 2)
                    {
                        Vector2 origin = new(texture.Width - 65, texture.Height - 512);
                        SpriteEffects effects = j == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                        if (j == 1)
                        {
                            origin.X = 65;
                        }

                        Vector2 leftRightOffset = new(ArenaBoundsWorld.Width / 2 * j, -ArenaBoundsWorld.Height / 2);

                        spriteBatch.Draw(textureFog, drawPositionLR + new Vector2(j == 1 ? 130 : -1500, 0) + leftRightOffset, new Rectangle(0, 0, 1500, drawHeight), drawColor, 0f, origin, 1f, effects, 0);

                        Rectangle scrollRect = new(0, (int)BarrierAnimation, texture.Width, texture.Height);

                        spriteBatch.Draw(texture, drawPositionLR + leftRightOffset, scrollRect, drawColor, 0f, origin, 1f, effects, 0);

                        scrollRect.Y = (int)-BarrierAnimation;
                        spriteBatch.Draw(texture, drawPositionLR + new Vector2(-50 * j, 0) + leftRightOffset, scrollRect, drawColor * 0.6f, 0f, origin, 1f, effects, 0);
                    }
                    for (int j = -1; j <= 1; j += 2)
                    {
                        Vector2 origin = new(texture.Width - 65, texture.Height - 512);
                        SpriteEffects effects = j == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                        if (j == 1)
                        {
                            origin.X = 65;
                        }

                        Vector2 upDownOffset = new(0f, -ArenaBoundsWorld.Height / 2 + (ArenaBoundsWorld.Height / 2 * j));

                        spriteBatch.Draw(textureFog, drawPositionUD + new Vector2(0, j == -1 ? -1500 : 130) + upDownOffset, new Rectangle(0, 0, 1500, drawHeight), drawColor, MathHelper.PiOver2, origin, 1f, effects, 0);

                        Rectangle scrollRect = new(0, (int)BarrierAnimation, texture.Width, texture.Height);

                        spriteBatch.Draw(texture, drawPositionUD + upDownOffset, scrollRect, drawColor, MathHelper.PiOver2, origin, 1f, effects, 0);

                        scrollRect.Y = (int)-BarrierAnimation;
                        spriteBatch.Draw(texture, drawPositionUD + new Vector2(0, -50 * j) + upDownOffset, scrollRect, drawColor * 0.6f, MathHelper.PiOver2, origin, 1f, effects, 0);
                    }

                    drawPositionLR.Y -= drawHeight;
                    drawPositionUD.X -= drawHeight;
                }

                spriteBatch.End();
            }
        }

        /// <summary>
        /// Check if any part of a specific arena boss is alive.
        /// </summary>
        /// <param name="boss">The boss to check for.</param>
        public static bool ArenaBossAlive(ArenaBoss boss)
        {
            switch (boss)
            {
                case ArenaBoss.None:
                    return false;

                case ArenaBoss.OO:
                    if (Terraria.NPC.AnyNPCs(NPCType<OO>()))
                        return true;
                    break;
                case ArenaBoss.OC:
                    if (Terraria.NPC.AnyNPCs(NPCType<OmegaCleaver>()))
                        return true;
                    break;
                case ArenaBoss.ADD:
                    if (Terraria.NPC.AnyNPCs(NPCType<Ukko>()) || Terraria.NPC.AnyNPCs(NPCType<Akka>()))
                        return true;
                    break;
            }

            return false;
        }

        /// <summary>
        /// Activates the arena and spawns the specified boss.
        /// </summary>
        /// <param name="player">The player spawning this boss.</param>
        /// <param name="boss">The boss to activate the arena for.</param>
        /// <param name="origin">The bottom-middle of the arena.</param>
        /// <param name="sizeChange">The initial size change of the arena. Useful if the arena should expand over time.</param>
        /// <param name="createBarriers">Whether or not barriers should be created at the edges of the arena.</param>
        /// <param name="soloPlayer">The index/whoAmI of the player soloing this boss, if applicable.</param>
        public static void ActivateArena(ArenaBoss boss, Point origin, Point? sizeChange = null, bool createBarriers = true, int soloPlayer = -1)
        {
            if (ArenaActive)
            {
                return;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SendRequestArena(boss, origin, sizeChange, createBarriers, soloPlayer);
                return;
            }

            ArenaBoss = boss;
            Point arenaSize = ArenaSizes[boss];
            if (sizeChange != null)
            {
                arenaSize += sizeChange.Value;
            }

            ArenaBoundsTile = new Rectangle(origin.X - (arenaSize.X / 2), origin.Y - arenaSize.Y, arenaSize.X, arenaSize.Y + 1);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Terraria.Player perhapsPlayer = Main.player[i];

                if (perhapsPlayer.active && !perhapsPlayer.dead && perhapsPlayer.Hitbox.Intersects(ArenaBoundsWorld))
                {
                    ArenaPlayerIDs.Add(i);
                }
            }

            ArenaActive = true;

            if (soloPlayer >= 0 && soloPlayer <= Main.maxPlayers)
            {
                SoloPlayer = soloPlayer;
            }
        }

        /// <summary>
        /// Deactivate the arena.
        /// </summary>
        public static void DeactivateArena()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            ArenaActive = false;
            ArenaPlayerIDs.Clear();
            ArenaBoss = ArenaBoss.None;
            ArenaBoundsTile = Rectangle.Empty;
            SoloPlayer = -1;

            if (Main.netMode == NetmodeID.Server)
            {
                SendSyncArena();
            }
        }

        /// <summary>
        /// Resize the current arena. Expands in every direction but downwards, so the origin is in the same place.
        /// </summary>
        /// <param name="sizeChange">The change in size, in tiles.</param>
        public static void ResizeArena(Point sizeChange)
        {
            if (!ArenaActive)
            {
                return;
            }

            ArenaBoundsTile = new Rectangle(ArenaBoundsTile.X - (sizeChange.X / 2), ArenaBoundsTile.Y - sizeChange.Y, ArenaBoundsTile.Width + sizeChange.X, ArenaBoundsTile.Height + sizeChange.Y);

            if (Main.netMode == NetmodeID.Server)
            {
                SendSyncArena();
            }
        }
        #region Netcode
        internal static void SendRequestArena(ArenaBoss boss, Point origin, Point? sizeChange = null, bool createBarriers = true, int soloPlayer = -1)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                return;
            }

            ModPacket packet = Redemption.Instance.GetPacket(ModMessageType.RequestArena, 12);
            packet.Write((byte)boss);
            packet.Write((short)origin.X);
            packet.Write((short)origin.Y);
            packet.Write((short)(sizeChange?.X ?? 0));
            packet.Write((short)(sizeChange?.Y ?? 0));
            packet.Write(createBarriers);
            packet.Write((short)soloPlayer);
            packet.Send();
        }

        public static void HandleRequestArena(BinaryReader reader)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                return;
            }

            ArenaBoss boss = (ArenaBoss)reader.ReadByte();
            Point origin = new(reader.ReadInt16(), reader.ReadInt16());
            Point sizeChange = new(reader.ReadInt16(), reader.ReadInt16());
            bool createBarriers = reader.ReadBoolean();
            int soloPlayer = reader.ReadInt16();

            // TODO: Player arg (?)
            ActivateArena(boss, origin, sizeChange, createBarriers, soloPlayer);

            SendSyncArena();
        }

        internal static void SendSyncArena(int toWho = -1)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                return;
            }

            ModPacket packet = Redemption.Instance.GetPacket(ModMessageType.SyncArena, 299);
            packet.Write(ArenaActive);
            packet.Write((byte)ArenaBoss);
            packet.Write((short)ArenaBoundsTile.X);
            packet.Write((short)ArenaBoundsTile.Y);
            packet.Write((short)ArenaBoundsTile.Width);
            packet.Write((short)ArenaBoundsTile.Height);
            packet.Write((short)SoloPlayer);
            packet.Write7BitEncodedInt(ArenaPlayerIDs.Count);
            foreach (int playerID in ArenaPlayerIDs)
            {
                packet.Write((byte)playerID);
            }
            packet.Send(toClient: toWho);
        }

        public static void HandleSyncArena(BinaryReader reader)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                return;
            }

            ArenaActive = reader.ReadBoolean();
            ArenaBoss = (ArenaBoss)reader.ReadByte();
            ArenaBoundsTile = new(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
            SoloPlayer = reader.ReadInt16();
            int count = reader.Read7BitEncodedInt();
            ArenaPlayerIDs.Clear();
            for (int i = 0; i < count; ++i)
            {
                ArenaPlayerIDs.Add(reader.ReadByte());
            }
        }

        internal static void SendRequestSyncArena()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                return;
            }

            ModPacket packet = Redemption.Instance.GetPacket();
            packet.Write((byte)ModMessageType.RequestSyncArena);
            packet.Send();
        }

        public static void HandleRequestSyncArena(int fromWho)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                return;
            }

            SendSyncArena(fromWho);
        }
        #endregion
    }
    public class ArenaPlayer : ModPlayer
    {
        public bool ShouldBeInBounds => (ArenaSystem.SoloPlayer == -1 || ArenaSystem.SoloPlayer == Player.whoAmI) && ArenaSystem.ArenaPlayerIDs.Contains(Player.whoAmI);

        public override void PlayerConnect()
        {
            ArenaSystem.SendRequestSyncArena();
        }

        public override void UpdateDead()
        {
            //if(ArenaPlayerIDs != null && ArenaPlayerIDs.Contains(Player.whoAmI))
            //{
            //	ArenaPlayerIDs.Remove(Player.whoAmI);
            //	if (Main.netMode == NetmodeID.MultiplayerClient)
            //	{
            //		SoANet.SendArenaPlayerIDsSync();
            //	}
            //}
        }

        public override void PostUpdate()
        {
            if (ArenaSystem.ArenaActive && ArenaSystem.ArenaBoundsTile != Rectangle.Empty)
            {
                if (ShouldBeInBounds)
                {
                    if (Player.Left.X < ArenaSystem.ArenaBoundsWorld.Left)
                    {
                        Player.Left = new Vector2(ArenaSystem.ArenaBoundsWorld.Left, Player.Left.Y);
                        Player.velocity.X = 0f;
                    }
                    else if (Player.Right.X > ArenaSystem.ArenaBoundsWorld.Right)
                    {
                        Player.Right = new Vector2(ArenaSystem.ArenaBoundsWorld.Right, Player.Right.Y);
                        Player.velocity.X = 0f;
                    }

                    if (Player.Top.Y < ArenaSystem.ArenaBoundsWorld.Top)
                    {
                        Player.Top = new Vector2(Player.Top.X, ArenaSystem.ArenaBoundsWorld.Top);
                        Player.velocity.Y = 0f;
                    }
                    else if (Player.Bottom.Y > ArenaSystem.ArenaBoundsWorld.Bottom)
                    {
                        Player.Bottom = new Vector2(Player.Bottom.X, ArenaSystem.ArenaBoundsWorld.Bottom);
                        Player.velocity.Y = 0f;
                    }
                }
                else if (!ShouldBeInBounds)
                {
                    if (Player.Right.X > ArenaSystem.ArenaBoundsWorld.Left && Player.Right.X < ArenaSystem.ArenaOriginWorld.X)
                    {
                        Player.Right = new Vector2(ArenaSystem.ArenaBoundsWorld.Left, Player.Right.Y);
                        Player.velocity.X = 0f;
                    }
                    else if (Player.Left.X < ArenaSystem.ArenaBoundsWorld.Right && Player.Left.X > ArenaSystem.ArenaOriginWorld.X)
                    {
                        Player.Left = new Vector2(ArenaSystem.ArenaBoundsWorld.Right, Player.Left.Y);
                        Player.velocity.X = 0f;
                    }
                }
            }
        }
    }
}
