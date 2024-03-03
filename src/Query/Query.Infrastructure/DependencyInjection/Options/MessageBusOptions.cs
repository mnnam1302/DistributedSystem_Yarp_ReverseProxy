﻿using System.ComponentModel.DataAnnotations;

namespace Query.Infrastructure.DependencyInjection.Options
{
    public class MesssageBusOptions
    {
        public int RetryLimit { get; init; }

        [Required, Timestamp]
        public TimeSpan InitialInterval { get; init; }

        [Required, Timestamp]
        public TimeSpan IntervalIncrement { get; init; }
    }
}