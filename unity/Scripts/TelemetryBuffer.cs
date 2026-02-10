using System.Collections.Generic;
using UnityEngine;

namespace CampusNavigator
{
    public class TelemetryBuffer : MonoBehaviour
    {
        public int maxBufferSize = 200;
        public float flushIntervalSec = 10f;

        private readonly List<TelemetryEvent> buffer = new List<TelemetryEvent>();
        private float nextFlushAt;

        private void Update()
        {
            if (Time.time >= nextFlushAt)
            {
                Flush();
                nextFlushAt = Time.time + flushIntervalSec;
            }
        }

        public void Enqueue(TelemetryEvent evt)
        {
            if (evt == null)
            {
                return;
            }
            buffer.Add(evt);
            if (buffer.Count >= maxBufferSize)
            {
                Flush();
            }
        }

        public void Flush()
        {
            if (buffer.Count == 0 || ApiClient.Instance == null)
            {
                return;
            }

            var copy = buffer.ToArray();
            buffer.Clear();

            var req = new TelemetryBatchRequest { events = copy };
            StartCoroutine(ApiClient.Instance.SendTelemetry(req, _ => { }, _ => { }));
        }
    }
}
