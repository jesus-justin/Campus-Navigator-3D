# Analytics Dashboard

Place this folder under your XAMPP web root so it can call the PHP API without CORS issues.

Suggested URL:
- http://localhost/Campus-Navigator-3D/dashboard/

Setup:
1) Ensure the API is working and you have an admin token from /auth/login.
2) Paste the token into the Admin Token field.
3) Set dates and apply filters.

Notes:
- The heatmap uses /admin/heatmap with type building, room, or landmark.
- Paths uses /admin/paths and expects MySQL 8 window functions.
- Quest stats uses /admin/quest-stats.
