# Unity Client Stubs

Drop these scripts into a Unity project under Assets/Scripts.

Setup:
- Create an empty GameObject named ApiConfig and add ApiConfig.
- Set Base URL to your XAMPP API, e.g. http://localhost/Campus-Navigator-3D/api
- Create an empty GameObject named ApiClient and add ApiClient.
- Create an empty GameObject named TelemetryBuffer and add TelemetryBuffer.

Usage example:
- Start login: StartCoroutine(ApiClient.Instance.Login("student-001", "Student", OnLogin, OnError))
- Fetch locations: StartCoroutine(ApiClient.Instance.GetLocations(OnLocations, OnError))
- Enqueue telemetry: TelemetryBuffer.Enqueue(new TelemetryEvent { eventType = "enter", locationId = 1 })
