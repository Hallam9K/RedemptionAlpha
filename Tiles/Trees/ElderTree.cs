using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Tiles.Trees
{
    public class ElderTree : ModTree
	{
		private static Mod Mod
		{
			get
			{
				return ModLoader.GetMod("Redemption");
			}
		}

		public override int CreateDust()
		{
            return DustID.t_BorealWood;
		}

		public override int GrowthFXGore()
		{
			return ModContent.Find<ModGore>("Redemption/ElderTreeFX").Type;
		}

		public override int DropWood()
		{
			return ModContent.ItemType<ElderWood>();
		}

		public override Texture2D GetTexture()
		{
			return Mod.Assets.Request<Texture2D>("Tiles/Trees/ElderTree").Value;
		}

		public override Texture2D GetTopTextures(int i, int j, ref int frame, ref int frameWidth, ref int frameHeight, ref int xOffsetLeft, ref int yOffset)
		{
			return Mod.Assets.Request<Texture2D>("Tiles/Trees/ElderTree_Tops").Value;
		}

		public override Texture2D GetBranchTextures(int i, int j, int trunkOffset, ref int frame)
		{
			return Mod.Assets.Request<Texture2D>("Tiles/Trees/ElderTree_Branches").Value;
		}
	}
}