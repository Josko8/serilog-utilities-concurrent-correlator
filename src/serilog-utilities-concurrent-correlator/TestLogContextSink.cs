﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    class TestLogContextSink : ILogEventSink
    {
        readonly ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>> testLogContextGuidBags = new ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>>();

        public void Emit(LogEvent logEvent)
        {
            foreach (var guid in testLogContextGuidBags.Keys)
            {
                if (LogicalCallContext.Contains(guid))
                {
                    testLogContextGuidBags[guid].Add(logEvent);
                }
            }
        }

        public TestLogContext CreateTestLogContext()
        {
            var testLogContext = new TestLogContext();

            testLogContextGuidBags.GetOrAdd(testLogContext.Guid, new ConcurrentBag<LogEvent>());

            return testLogContext;
        }

        public IEnumerable<LogEvent> GetLogEventsFromTestLogContext(Guid testLogContextGuid)
        {
            return testLogContextGuidBags[testLogContextGuid];
        }
    }
}