#region Using directives

using System.IO;
using System.Linq;
using System.Reflection;

using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;

using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
#endregion

namespace Redemption
{
	public sealed class ShaderLoader : ILoadable
	{
		public float Priority => 1f;

		public bool LoadOnDedServer => false;

		public void Load()
		{
			MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
			var file = (TmodFile)info.Invoke(Redemption.Instance, null);

			var shaders = file.Where(x => x.Name.Contains("Effects/") && x.Name.EndsWith(".xnb"));

			foreach (var entry in shaders)
			{
				var shaderPath = entry.Name.Replace(".xnb", string.Empty);
				var shaderName = Path.GetFileName(shaderPath);

				LoadShader(shaderName, shaderPath);
			}
		}

		// TODO: Eldrazi - Should not have anything to unload AFAIK.
		public void Unload() { }

		internal static void LoadShader(string shaderName, string shaderPath)
		{
			var shaderRef = new Ref<Effect>(Redemption.Instance.Assets.Request<Effect>(shaderPath, AssetRequestMode.ImmediateLoad).Value);

			(Filters.Scene[Redemption.Abbreviation + ":" + shaderName] = new Filter(new ScreenShaderData(shaderRef, shaderName), EffectPriority.High))
				.Load();
		}
	}
}
