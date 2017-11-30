﻿namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Fade : Animation
    {
        public override IAnimation Merge(IAnimation other)
        {
            return other?.Merge(this) ?? base.Merge(other);
        }
    }

    public static class FadeExtensions
    {
        public static IHandler Fade(
            this IHandler handler, TimeSpan? duration = null)
        {
            return new RegionOptions
            {
                Animation = new Fade
                {
                    Duration = duration
                }
            }.Decorate(handler);
        }
    }
}
