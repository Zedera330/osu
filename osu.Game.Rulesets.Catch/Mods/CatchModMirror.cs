// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Catch.Beatmaps;
using osu.Game.Rulesets.Catch.Objects;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Catch.UI;
using osu.Game.Rulesets.Objects;
using osuTK;

namespace osu.Game.Rulesets.Catch.Mods
{
    public class CatchModMirror : ModMirror, IApplicableToBeatmap
    {
        public override string Description => "Fruits are flipped horizontally.";

        /// <remarks>
        /// <see cref="IApplicableToBeatmap"/> is used instead of <see cref="IApplicableToHitObject"/>,
        /// as <see cref="CatchBeatmapProcessor"/> applies offsets in <see cref="CatchBeatmapProcessor.PostProcess"/>.
        /// <see cref="IApplicableToBeatmap"/> runs after post-processing, while <see cref="IApplicableToHitObject"/> runs before it.
        /// </remarks>
        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            foreach (var hitObject in beatmap.HitObjects)
                applyToHitObject(hitObject);
        }

        private void applyToHitObject(HitObject hitObject)
        {
            if (hitObject is BananaShower)
                return;

            var catchObject = (CatchHitObject)hitObject;

            catchObject.OriginalX = CatchPlayfield.WIDTH - catchObject.OriginalX;
            catchObject.XOffset = -catchObject.XOffset;

            foreach (var nested in catchObject.NestedHitObjects.Cast<CatchHitObject>())
            {
                nested.OriginalX = CatchPlayfield.WIDTH - nested.OriginalX;
                nested.XOffset = -nested.XOffset;
            }

            if (catchObject is JuiceStream juiceStream)
            {
                var controlPoints = juiceStream.Path.ControlPoints.Select(p => new PathControlPoint(p.Position.Value, p.Type.Value)).ToArray();
                foreach (var point in controlPoints)
                    point.Position.Value = new Vector2(-point.Position.Value.X, point.Position.Value.Y);

                juiceStream.Path = new SliderPath(controlPoints, juiceStream.Path.ExpectedDistance.Value);
            }
        }
    }
}
