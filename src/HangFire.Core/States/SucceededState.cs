﻿// This file is part of HangFire.
// Copyright © 2013-2014 Sergey Odinokov.
// 
// HangFire is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// HangFire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with HangFire. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Globalization;
using HangFire.Common;
using HangFire.Storage;

namespace HangFire.States
{
    public class SucceededState : IState
    {
        public static readonly string StateName = "Succeeded";

        public SucceededState(long latency, long performanceDuration)
        {
            Latency = latency;
            PerformanceDuration = performanceDuration;
            SucceededAt = DateTime.UtcNow;
        }

        public DateTime SucceededAt { get; private set; }
        public long Latency { get; private set; }
        public long PerformanceDuration { get; private set; }

        public string Name { get { return StateName; } }
        public string Reason { get; set; }

        public bool IsFinal { get { return true; } }
        public bool IgnoreJobLoadException { get { return false; } }

        public Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "SucceededAt",  JobHelper.ToStringTimestamp(SucceededAt) },
                { "PerformanceDuration", PerformanceDuration.ToString(CultureInfo.InvariantCulture) },
                { "Latency", Latency.ToString(CultureInfo.InvariantCulture) }
            };
        }

        internal class Handler : IStateHandler
        {
            public void Apply(ApplyStateContext context, IWriteOnlyTransaction transaction)
            {
                transaction.IncrementCounter("stats:succeeded");
            }

            public void Unapply(ApplyStateContext context, IWriteOnlyTransaction transaction)
            {
                transaction.DecrementCounter("stats:succeeded");
            }

            public string StateName
            {
                get { return SucceededState.StateName; }
            }
        }
    }
}
