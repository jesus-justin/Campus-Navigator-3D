# WebGL Beta Playability

This project can be tested in the browser using Unity WebGL.

## Build Settings

1) File > Build Settings > WebGL.
2) Player Settings:
   - Resolution: 1280x720 or higher.
   - Publishing: Gzip or Brotli (requires correct server headers).
3) Build to a folder under XAMPP, e.g. C:\xampp\htdocs\Campus-Navigator-3D\build\webgl

## API Base URL

For WebGL, the API must be served from the same origin or CORS enabled.

- Recommended: use relative path via WebGLApiOverride script.
- Default relative path: /Campus-Navigator-3D/api

## Controls

- Click the game to lock the cursor.
- WASD to move, mouse to look.
- E to interact, I for inventory, Q for quest list.
- ESC to unlock cursor.

## Common Issues

- 404 on API calls: check ApiConfig baseUrl and WebGLApiOverride.
- CORS errors: host API and WebGL on same domain.
- No input: click the canvas to lock the cursor.
