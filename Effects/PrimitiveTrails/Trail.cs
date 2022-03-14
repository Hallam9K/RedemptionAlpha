using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Redemption.PrimitiveTrails
{
	// Code by Boffin (From Spirit Mod)
	public class Trail
	{
		public bool Dead { get; private set; }
		public ITrailObject originalObject;
		public ITrailObject trailOject { get; private set; }
		private ITrailCap trailCap;
		private ITrailColor trailColor;
		private ITrailPosition trailPosition;
		private ITrailShader trailShader;
		private List<Vector2> points;

		public float widthStart;
		public float curLength;
		private float maxLength;
		public float totalLength;
		private bool dissolving;
		private float dissolveSpeed;
		private float originalMaxLength;
		private float originalWidth;

		public bool active;
		public Vector2 position;
		public Vector2 center;
		public Vector2 velocity;
		public float width;
		public float height;
		public float opacity;

		public Trail(ITrailObject trail, ITrailCap trailCap, ITrailColor trailColor, ITrailPosition trailPosition, ITrailShader trailShader, float widthStart, float maxLength)
		{
			originalObject = trail;
			trailOject = trail;
			Dead = false;

			this.trailCap = trailCap;
			this.trailColor = trailColor;
			this.trailPosition = trailPosition;
			this.trailShader = trailShader;
			this.maxLength = maxLength;
			this.widthStart = widthStart;

			points = new List<Vector2>();
		}

		public interface ITrailObject
		{
			void DrawTrail(TrailManager manager);
		}
		public void Update()
		{
			TrailManager.GetObjectValues(this, out (bool active, Vector2 position, Vector2 center, Vector2 velocity, float width, float height, float opacity) values);
			active = values.active;
			position = values.position;
			center = values.center;
			velocity = values.velocity;
			width = values.width;
			height = values.height;
			opacity = values.opacity;

			if (dissolving)
			{
				maxLength -= dissolveSpeed;
				widthStart = maxLength / originalMaxLength * originalWidth;
				if (maxLength <= 0f)
				{
					Dead = true;
					return;
				}

				TrimToLength(maxLength);
				return;
			}

			if (!active || trailOject != originalObject)
			{
				StartDissolve(maxLength / 10f);
				return;
			}

			Vector2 thisPoint = trailPosition.GetNextTrailPosition(trailOject);

			if (points.Count == 0)
			{
				points.Add(thisPoint);
				return;
			}

			float distance = Vector2.Distance(thisPoint, points[0]);
			points.Insert(0, thisPoint);

			//If adding the next point is too much
			if (curLength + distance > maxLength)
			{
				TrimToLength(maxLength);
				totalLength += distance;
			}
			else
			{
				curLength += distance;
				totalLength += distance;
			}
		}
		public void Draw(Effect effect, GraphicsDevice device)
		{
			if (Dead || points.Count <= 1) return;

			//calculate trail's length
			float trailLength = 0f;
			for (int i = 1; i < points.Count; i++)
			{
				trailLength += Vector2.Distance(points[i - 1], points[i]);
			}

			//Create vertice array, needs to be equal to the number of quads * 6 (each quad has two tris, which are 3 vertices)
			int currentIndex = 0;
			VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[(points.Count - 1) * 6 + trailCap.ExtraTris * 3];

			//method to make it look less horrible
			void AddVertex(Vector2 position, Color color, Vector2 uv)
			{
				vertices[currentIndex++] = new VertexPositionColorTexture(new Vector3(position - Main.screenPosition, 0f), color, uv);
			}

			float currentDistance = 0f;
			// In order to give the trail the desired width, we need a halfwidth to assist in making the tris.
			float halfWidth = widthStart * 0.5f;

			// This is the angle of the first point
			Vector2 startNormal = CurveNormal(points, 0);
			// Now we use the halfwidth to generate each half of the trail's tris, based on the list of points given
			Vector2 prevClockwise = points[0] + startNormal * halfWidth;
			Vector2 prevCClockwise = points[0] - startNormal * halfWidth;

			Color previousColor = trailColor.GetColourAt(0f, trailLength, points);

			trailCap.AddCap(vertices, ref currentIndex, previousColor, points[0], startNormal, widthStart);
			for (int i = 1; i < points.Count; i++)
			{
				currentDistance += Vector2.Distance(points[i - 1], points[i]);

				float quotient = (1f - i / (float)(points.Count - 1));
				float quotient2 = (int)(quotient * 100f) / 100f;
				float thisPointsWidth = halfWidth * quotient;

				Vector2 normal = CurveNormal(points, i);
				Vector2 clockwise = points[i] + normal * thisPointsWidth;
				Vector2 cclockwise = points[i] - normal * thisPointsWidth;
				Color color = trailColor.GetColourAt(currentDistance, trailLength, points);

				AddVertex(clockwise, color, Vector2.UnitX * i);
				AddVertex(prevClockwise, previousColor, Vector2.UnitX * (i - 1));
				AddVertex(prevCClockwise, previousColor, new Vector2(i - 1, 1f));

				AddVertex(clockwise, color, Vector2.UnitX * i);
				AddVertex(prevCClockwise, previousColor, new Vector2(i - 1, 1f));
				AddVertex(cclockwise, color, new Vector2(i, 1f));

				prevClockwise = clockwise;
				prevCClockwise = cclockwise;
				previousColor = color;
			}

			effect.Parameters["WorldViewProjection"].SetValue(Redemption.Trails.worldViewProjection);

			//apply this trail's shader pass and draw
			trailShader.ApplyShader(effect, this, points);
			device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, (points.Count - 1) * 2 + trailCap.ExtraTris);
		}
		public void StartDissolve(float speed)
		{
			dissolving = true;
			dissolveSpeed = speed;
			originalWidth = widthStart;
			originalMaxLength = maxLength;
		}
		public void TrimToLength(float length)
		{
			if (points.Count == 0) return;

			curLength = length;

			int firstPointOver = -1;
			float newLength = 0;

			for (int i = 1; i < points.Count; i++)
			{
				newLength += Vector2.Distance(points[i], points[i - 1]);
				if (newLength > length)
				{
					firstPointOver = i;
					break;
				}
			}

			if (firstPointOver == -1) return;

			//get new end point based on remaining distance
			float leftOverLength = newLength - length;
			Vector2 between = points[firstPointOver] - points[firstPointOver - 1];
			float newPointDistance = between.Length() - leftOverLength;
			between.Normalize();

			int toRemove = points.Count - firstPointOver;
			points.RemoveRange(firstPointOver, toRemove);

			points.Add(points.Last() + between * newPointDistance);
		}
		public Vector2 CurveNormal(List<Vector2> points, int index)
		{
			if (points.Count == 1) return points[0];

			if (index == 0)
			{
				return Vector2.Normalize(points[1] - points[0]).TurnRight();
			}
			if (index == points.Count - 1)
			{
				return Vector2.Normalize(points[index] - points[index - 1]).TurnRight();
			}
			return Vector2.Normalize(points[index] - points[index - 1]).TurnRight();
		}
	}
}
