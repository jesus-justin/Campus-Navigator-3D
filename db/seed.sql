USE campus_navigator;

INSERT INTO users (external_id, display_name, role)
VALUES ('admin-001', 'Admin User', 'admin')
ON DUPLICATE KEY UPDATE display_name = VALUES(display_name), role = VALUES(role);

INSERT INTO locations (name, type, parent_id, floor, pos_x, pos_y, pos_z, radius_m)
VALUES
  ('Main Gate', 'landmark', NULL, NULL, 0.0, 0.0, 0.0, 5.0),
  ('Admin Building', 'building', NULL, NULL, 120.0, 0.0, -40.0, 20.0),
  ('Registrar Office', 'room', 2, 1, 122.0, 4.0, -38.0, 5.0),
  ('Library', 'building', NULL, NULL, -80.0, 0.0, 60.0, 25.0),
  ('Student Hub', 'building', NULL, NULL, -10.0, 0.0, 90.0, 18.0);

INSERT INTO quests (title, description, type, reward_points, time_limit_sec, is_active)
VALUES
  ('Find the Registrar', 'Locate the Registrar Office inside the Admin Building.', 'find', 50, 600, 1),
  ('Submit Enrollment Form', 'Deliver the form to the Registrar Office.', 'submit', 80, 900, 1),
  ('Library Tour', 'Reach the Library main entrance.', 'find', 30, 300, 1);

INSERT INTO quest_steps (quest_id, step_order, target_location_id, action_type, action_payload)
VALUES
  (1, 1, 2, 'reach', JSON_OBJECT('hint', 'Enter the Admin Building')),
  (1, 2, 3, 'reach', JSON_OBJECT('hint', 'Registrar is on the 1st floor')),
  (2, 1, 3, 'submit', JSON_OBJECT('item', 'Enrollment Form')),
  (3, 1, 4, 'reach', JSON_OBJECT('hint', 'Look for the Library entrance'));
