using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.Base
{
    public class BaseUtility
    {
        //------------------------------------------------------//
        //------------------BASE UTILITY CLASS------------------//
        //------------------------------------------------------//
        // Contains utility methods a mod might want to use.    //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------// 

        public static string[] GetLoadedMods()
        {
            return ModLoader.Mods.Reverse().Select(m => m.Name).ToArray();
        }

        public static void LogBasic(string logText)
        {
            ILog logger = LogManager.GetLogger("Terraria");
            logger.Info(logText);
        }

        //public static void LogFancy(string logText)
        //{
        //	LogFancy("", logText, null);
        //}		

        public static void LogFancy(string prefix, Exception e)
        {
            LogFancy(prefix, null, e);
        }

        public static void LogFancy(string prefix, string logText, Exception e = null)
        {
            ILog logger = LogManager.GetLogger("Terraria");
            if (e != null)
            {
                logger.Info(">---------<");
                logger.Error(prefix + e.Message);
                logger.Error(e.StackTrace);
                logger.Info(">---------<");
                //ErrorLogger.Log(prefix + e.Message); ErrorLogger.Log(e.StackTrace);	ErrorLogger.Log(">---------<");	
            }
            else
            {
                logger.Info(">---------<");
                logger.Info(prefix + logText);
                logger.Info(">---------<");
                //ErrorLogger.Log(prefix + logText);
            }
        }

        public static void OpenChestUI(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            Tile tile = Framing.GetTileSafely(i, j);
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0) left--;
            if (tile.TileFrameY != 0) top--;
            if (player.sign >= 0)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, NetworkText.FromLiteral(""), left, top);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                int chest = Chest.FindChest(left, top);
                if (chest >= 0)
                {
                    Main.stackSplit = 600;
                    if (chest == player.chest)
                    {
                        player.chest = -1;
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        player.chest = chest;
                        Main.playerInventory = true;
                        Main.recBigList = false;
                        player.chestX = left;
                        player.chestY = top;
                        SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                    }
                    Recipe.FindRecipes();
                }
            }
        }

        public static void DisplayTime(double time = -1, Color? OverrideColor = null, bool sync = false)
        {
            string text = "AM";
            if (time <= -1) time = Main.time;

            if (!Main.dayTime) { time += 54000.0; }
            time = time / 86400.0 * 24.0;
            time = time - 7.5 - 12.0;

            if (time < 0.0) time += 24.0;
            if (time >= 12.0) text = "PM";

            int intTime = (int)time;
            double deltaTime = time - intTime;
            deltaTime = (int)(deltaTime * 60.0);
            string text2 = string.Concat(deltaTime);

            if (deltaTime < 10.0) text2 = "0" + text2;
            if (intTime > 12) intTime -= 12;
            if (intTime == 0) intTime = 12;
            string newText = string.Concat("Time: ", intTime, ":", text2, " ", text);
            Chat(newText, OverrideColor != null ? (Color)OverrideColor : new Color(255, 240, 20), sync);
        }

        public static int CalcValue(int plat, int gold, int silver, int copper, bool sellPrice = false)
        {
            int val = copper;
            val += silver * 100;
            val += gold * 10000;
            val += plat * 1000000;
            if (sellPrice) val *= 5;
            return val;
        }

        public static void AddTooltips(Item item, string[] tooltips)
        {
            AddTooltips(item.ModItem, tooltips);
        }

        public static void AddTooltips(ModItem item, string[] tooltips)
        {
            string supertip = "";
            for (int m = 0; m < tooltips.Length; m++)
            {
                supertip += tooltips[m] + (m == tooltips.Length - 1 ? "" : "\n");
            }
            // item.Tooltip.SetDefault(supertip);
        }

        public static bool CanHit(Rectangle rect, Rectangle rect2)
        {
            return Collision.CanHit(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, new Vector2(rect2.X, rect2.Y), rect2.Width, rect2.Height);
        }

        public static Vector2 TileToPos(Vector2 tile) { return tile * new Vector2(16, 16); }
        public static Vector2 PosToTile(Vector2 pos) { return pos / new Vector2(16, 16); }

        /**
         * These two methods convert from game ticks to seconds and vise versa.
         */
        public static int TicksToSeconds(int ticks) { return ticks / 60; }
        public static int SecondsToTicks(int seconds) { return seconds * 60; }

        public static int TicksToMinutes(int ticks) { return TicksToSeconds(ticks) / 60; }
        public static int MinutesToTicks(int minutes) { return SecondsToTicks(minutes) * 60; }

        /*
         * Adds a value to the given array at the specified index. If index is -1, it adds it to the end.
         */
        public static Color[] AddToArray(Color[] array, Color valueToAdd, int indexAt = -1)
        {
            Array.Resize(ref array, indexAt + 1 > array.Length + 1 ? indexAt + 1 : array.Length + 1);
            if (indexAt == -1)
            {
                array[^1] = valueToAdd;
            }
            else
            {
                List<Color> list = array.ToList();
                list.Insert(indexAt, valueToAdd);
                array = list.ToArray();
            }
            return array;
        }

        /*
         * Adds a value to the given array at the specified index. If index is -1, it adds it to the end.
         */
        public static string[] AddToArray(string[] array, string valueToAdd, int indexAt = -1)
        {
            Array.Resize(ref array, indexAt + 1 > array.Length + 1 ? indexAt + 1 : array.Length + 1);
            if (indexAt == -1)
            {
                array[^1] = valueToAdd;
            }
            else
            {
                List<string> list = array.ToList();
                list.Insert(indexAt, valueToAdd);
                array = list.ToArray();
            }
            return array;
        }

        /*
         * Adds a value to the given array at the specified index. If index is -1, it adds it to the end.
         */
        public static int[] AddToArray(int[] array, int valueToAdd, int indexAt = -1)
        {
            Array.Resize(ref array, indexAt + 1 > array.Length + 1 ? indexAt + 1 : array.Length + 1);
            if (indexAt == -1)
            {
                array[^1] = valueToAdd;
            }
            else
            {
                List<int> list = array.ToList();
                list.Insert(indexAt, valueToAdd);
                array = list.ToArray();
            }
            return array;
        }

        /*
         * Combines two int arrays.
         */
        public static int[] CombineArrays(int[] array1, int[] array2)
        {
            int[] newArray = new int[array1.Length + array2.Length];
            for (int m = 0; m < array1.Length; m++) { newArray[m] = array1[m]; }
            for (int m = 0; m < array2.Length; m++) { newArray[array1.Length + m] = array2[m]; }
            return newArray;
        }

        /*
         * Fills an int array entirely with the value.
         */
        public static int[] FillArray(int[] array, int value)
        {
            for (int m = 0; m < array.Length; m++) { array[m] = value; }
            return array;
        }

        /*
         * Returns true if value is in the given int array.
         */
        public static bool InArray(int[] array, int value)
        {
            for (int m = 0; m < array.Length; m++) { if (value == array[m]) { return true; } }
            return false;
        }

        /*
         * Returns true if value is in the given int array.
         * 
         * index : sets this to the index of the value in the array.
         */
        public static bool InArray(int[] array, int value, ref int index)
        {
            for (int m = 0; m < array.Length; m++) { if (value == array[m]) { index = m; return true; } }
            return false;
        }

        /*
         * Returns true if value is in the given float array.
         */
        public static bool InArray(float[] array, float value)
        {
            for (int m = 0; m < array.Length; m++) { if (value == array[m]) { return true; } }
            return false;
        }

        /*
         * Returns true if value is in the given float array.
         * 
         * index : sets this to the index of the value in the array.
         */
        public static bool InArray(float[] array, float value, ref int index)
        {
            for (int m = 0; m < array.Length; m++) { if (value == array[m]) { index = m; return true; } }
            return false;
        }


        /*
         * Returns a monochrome version of the given color.
         */
        public static Color ColorMonochrome(Color color)
        {
            int average = color.R + color.G + color.B;
            average /= 3;
            return new Color(average, average, average, color.A);
        }

        /*
         * Alters the coior by the amount of the alpha given.
         */
        public static Color ColorAlpha(Color color, int alpha)
        {
            return color * (1f - alpha / 255f);
        }

        /*
         * Alters the brightness of the color by the amount of the factor. If factor is negative, it darkens it. Else, it brightens it.
         */
        public static Color ColorBrightness(Color color, int factor)
        {
            int r = Math.Max(0, Math.Min(255, color.R + factor));
            int g = Math.Max(0, Math.Min(255, color.G + factor));
            int b = Math.Max(0, Math.Min(255, color.B + factor));
            return new Color(r, g, b, color.A);
        }

        /*
		 * Alters the brightness of the color by the multiplier.
		 */
        public static Color ColorMult(Color color, float mult)
        {
            int r = Math.Max(0, Math.Min(255, (int)(color.R * mult)));
            int g = Math.Max(0, Math.Min(255, (int)(color.G * mult)));
            int b = Math.Max(0, Math.Min(255, (int)(color.B * mult)));
            return new Color(r, g, b, color.A);
        }

        /*
         * Clamps the first color to be no lower then the values of the second color.
         */
        public static Color ColorClamp(Color color1, Color color2)
        {
            int r = color1.R;
            int g = color1.G;
            int b = color1.B;
            int a = color1.A;
            if (r < color2.R) { r = color2.R; }
            if (g < color2.G) { g = color2.G; }
            if (b < color2.B) { b = color2.B; }
            if (a < color2.A) { a = color2.A; }
            return new Color(r, g, b, a);
        }

        /*
		 * Clamps the first color to be no lower then the brightness of the second color.
		 */
        public static Color ColorBrightnessClamp(Color color1, Color color2)
        {
            float r = color1.R / 255f;
            float g = color1.G / 255f;
            float b = color1.B / 255f;
            float r2 = color2.R / 255f;
            float g2 = color2.G / 255f;
            float b2 = color2.B / 255f;
            float brightness = r2 > g2 ? r2 : g2 > b2 ? g2 : b2;
            r *= brightness; g *= brightness; b *= brightness;
            return new Color(r, g, b, color1.A / 255f);
        }

        /*
		 * Tints the light color according to the buff color given. (prevents 'darkness' occuring if more than one is applied)
		 */
        public static Color BuffColorize(Color buffColor, Color lightColor)
        {
            Color color2 = ColorBrightnessClamp(buffColor, lightColor);
            return ColorClamp(Colorize(buffColor, lightColor), color2);
        }

        /*
         * Tints the light color according to the tint color given.
         */
        public static Color Colorize(Color tint, Color lightColor)
        {
            float r = lightColor.R / 255f;
            float g = lightColor.G / 255f;
            float b = lightColor.B / 255f;
            float a = lightColor.A / 255f;
            Color newColor = tint;
            float nr = (byte)(newColor.R * r);
            float ng = (byte)(newColor.G * g);
            float nb = (byte)(newColor.B * b);
            float na = (byte)(newColor.A * a);
            newColor.R = (byte)nr;
            newColor.G = (byte)ng;
            newColor.B = (byte)nb;
            newColor.A = (byte)na;
            return newColor;
        }

        /* 
         * Returns a color of the rainbow. Percent goes from 0 to 1.
         */
        public Color Rainbow(float percent)
        {
            Color r = new(255, 50, 50);
            Color g = new(50, 255, 50);
            Color b = new(90, 90, 255);
            Color y = new(255, 255, 50);
            if (percent <= 0.25f)
            {
                return Color.Lerp(r, b, percent / 0.25f);
            }

            if (percent <= 0.5f)
            {
                return Color.Lerp(b, g, (percent - 0.25f) / 0.25f);
            }

            if (percent <= 0.75f)
            {
                return Color.Lerp(g, y, (percent - 0.5f) / 0.25f);
            }

            return Color.Lerp(y, r, (percent - 0.75f) / 0.25f);
        }

        /* 
         * Clamps the given position so it is not outside of the world dimensions.
         *  
         *  tilePos : If true, it assumes the position is a tile coordinate and not an NPC/Projectile position.
         */
        public static Vector2 ClampToWorld(Vector2 position, bool tilePos = false)
        {
            if (tilePos)
            {
                position.X = (int)MathHelper.Clamp(position.X, 0, Main.maxTilesX);
                position.Y = (int)MathHelper.Clamp(position.Y, 0, Main.maxTilesY);
            }
            else
            {
                position.X = (int)MathHelper.Clamp(position.X, 0, Main.maxTilesX * 16);
                position.Y = (int)MathHelper.Clamp(position.Y, 0, Main.maxTilesY * 16);
            }
            return position;
        }

        /*
         * Returns the total distance between points in the array, in order.
         */
        public static float GetTotalDistance(Vector2[] points)
        {
            float totalDistance = 0f;
            for (int m = 1; m < points.Length; m++)
            {
                totalDistance += Vector2.Distance(points[m - 1], points[m]);
            }
            return totalDistance;
        }

        /*
         * Scales a rectangle either up or down by the scale amount.
         */
        public static Rectangle ScaleRectangle(Rectangle rect, float scale)
        {
            float ratioWidth = (rect.Width * scale - rect.Width) / 2;
            float ratioHeight = (rect.Height * scale - rect.Height) / 2;
            int x = rect.X - (int)ratioWidth;
            int y = rect.Y - (int)ratioHeight;
            int width = rect.Width + (int)(ratioWidth * 2);
            int height = rect.Height + (int)(ratioHeight * 2);
            return new Rectangle(x, y, width, height);
        }

        /*
		 * Allows lerping between N float values.
		 */
        public static float MultiLerp(float percent, params float[] floats)
        {
            float per = 1f / ((float)floats.Length - 1);
            float total = per;
            int currentID = 0;
            while (percent / total > 1f && currentID < floats.Length - 2) { total += per; currentID++; }
            return MathHelper.Lerp(floats[currentID], floats[currentID + 1], (percent - per * currentID) / per);
        }

        /*
		 * Allows lerping between N vector values.
		 */
        public static Vector2 MultiLerpVector(float percent, params Vector2[] vectors)
        {
            float per = 1f / ((float)vectors.Length - 1);
            float total = per;
            int currentID = 0;
            while (percent / total > 1f && currentID < vectors.Length - 2) { total += per; currentID++; }
            return Vector2.Lerp(vectors[currentID], vectors[currentID + 1], (percent - per * currentID) / per);
        }

        /*
		 * Allows lerping between N color values.
		 */
        public static Color MultiLerpColor(float percent, params Color[] colors)
        {
            float per = 1f / ((float)colors.Length - 1);
            float total = per;
            int currentID = 0;
            while (percent / total > 1f && currentID < colors.Length - 2) { total += per; currentID++; }
            return Color.Lerp(colors[currentID], colors[currentID + 1], (percent - per * currentID) / per);
        }

        /*
         * Returns a rotation from startPos pointing to endPos.
         */
        public static float RotationTo(Vector2 startPos, Vector2 endPos)
        {
            return (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
        }

        /*
         * Flips a vector using another vector as the origin axis.
         * flipX/flipY : flip on the X and Y axis, respectively.
         */
        public static Vector2 FlipVector(Vector2 origin, Vector2 point, bool flipX = true, bool flipY = true)
        {
            float dX = point.X - origin.X; float dY = point.Y - origin.Y;
            if (flipX) { dX *= -1; }
            if (flipY) { dY *= -1; }
            return origin + new Vector2(dX, dY);
        }

        /*
         * Rotates a vector based on the origin and the given point to 'look' at.
         * The rotation vector is *NOT* relative to the origin.
         */
        public static Vector2 RotateVector(Vector2 origin, Vector2 vecToRot, float rot)
        {
            float newPosX = (float)(Math.Cos(rot) * (vecToRot.X - origin.X) - Math.Sin(rot) * (vecToRot.Y - origin.Y) + origin.X);
            float newPosY = (float)(Math.Sin(rot) * (vecToRot.X - origin.X) + Math.Cos(rot) * (vecToRot.Y - origin.Y) + origin.Y);
            return new Vector2(newPosX, newPosY);
        }

        public static Vector2 GetRandomPosNear(Vector2 pos, int minDistance, int maxDistance, bool circular = false)
        {
            return GetRandomPosNear(pos, Main.rand, minDistance, maxDistance, circular);
        }

        /*
         * Returns a random position near the position given.
         * 
         * rand : a Random to use to get the position.
         * minDistance : the minimum amount of distance from the position.
         * maxDistance : the maximum amount of distance from the position.
         * circular : If true, gets a random point around a circle instead of a square.
         */
        public static Vector2 GetRandomPosNear(Vector2 pos, UnifiedRandom rand, int minDistance, int maxDistance, bool circular = false)
        {
            int distance = maxDistance - minDistance;
            if (!circular)
            {
                float newPosX = pos.X + (Main.rand.NextBool(2)? -(minDistance + rand.Next(distance)) : minDistance + rand.Next(distance));
                float newPosY = pos.Y + (Main.rand.NextBool(2)? -(minDistance + rand.Next(distance)) : minDistance + rand.Next(distance));
                return new Vector2(newPosX, newPosY);
            }

            return RotateVector(pos, pos + new Vector2(minDistance + rand.Next(distance)), MathHelper.Lerp(0, (float)(Math.PI * 2f), (float)rand.NextDouble()));
        }

        /*
         * Sends the given string to chat, with the given color.
         */
        public static void Chat(string s, Color color, bool sync = true)
        {
            Chat(s, color.R, color.G, color.B, sync);
        }

        /*
         * Sends the given string to chat, with the given color values.
         */
        public static void Chat(string s, byte colorR = 255, byte colorG = 255, byte colorB = 255, bool sync = true)
        {
            if (Main.netMode == NetmodeID.SinglePlayer) { Main.NewText(s, colorR, colorG, colorB); }
            else
            if (Main.netMode == NetmodeID.MultiplayerClient) { Main.NewText(s, colorR, colorG, colorB); }
            else //if(sync){ NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(s), new Color(colorR, colorG, colorB), Main.myPlayer); } }else
            if (sync && Main.netMode == NetmodeID.Server) { ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(s), new Color(colorR, colorG, colorB)); }
        }

        public static Vector2[] ChainVector2(Vector2 start, Vector2 end, float jump = 0f)
        {
            List<Vector2> points = new();
            if (jump <= 0f) { jump = 16f; }
            Vector2 dir = end - start;
            dir.Normalize();
            float length = Vector2.Distance(start, end);
            float way = 0f;
            while (way < length)
            {
                points.Add(start + dir * way);
                way += jump;
            }
            return points.ToArray();
        }

        public static Point[] ChainPoint(Point start, Point end, float jump = 0f)
        {
            List<Point> points = new();
            if (jump <= 0f) { jump = 16f; }
            Vector2 dir = end.ToVector2() - start.ToVector2();
            dir.Normalize();
            float length = Vector2.Distance(start.ToVector2(), end.ToVector2());
            float way = 0f;
            while (way < length)
            {
                Vector2 vec = start.ToVector2() + dir * way;
                Point p = new((int)vec.X, (int)vec.Y);
                points.Add(p);
                way += jump;
            }
            return points.ToArray();
        }
    }
}