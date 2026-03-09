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

-- Additional sample locations for more comprehensive testing
INSERT INTO locations (name, type, parent_id, floor, pos_x, pos_y, pos_z, radius_m)
VALUES
  ('Science Building', 'building', NULL, NULL, 200.0, 0.0, -100.0, 30.0),
  ('Chemistry Lab', 'room', 6, 2, 205.0, 8.0, -95.0, 8.0),
  ('Physics Lab', 'room', 6, 2, 210.0, 8.0, -105.0, 8.0),
  ('Cafeteria', 'building', NULL, NULL, 50.0, 0.0, 150.0, 22.0),
  ('Sports Complex', 'building', NULL, NULL, -150.0, 0.0, -80.0, 35.0),
  ('Computer Lab A', 'room', 4, 2, -75.0, 8.0, 62.0, 10.0),
  ('Reading Room', 'room', 4, 1, -82.0, 4.0, 58.0, 12.0),
  ('Parking Lot A', 'landmark', NULL, NULL, -80.0, 0.0, -150.0, 15.0),
  ('Campus Garden', 'landmark', NULL, NULL, 100.0, 0.0, 120.0, 25.0);

-- Additional sample users for testing
INSERT INTO users (external_id, display_name, role)
VALUES 
  ('student-001', 'Alice Johnson', 'player'),
  ('student-002', 'Bob Smith', 'player'),
  ('student-003', 'Carol Davis', 'player')
ON DUPLICATE KEY UPDATE display_name = VALUES(display_name);

-- Additional sample quests
INSERT INTO quests (title, description, type, reward_points, time_limit_sec, is_active)
VALUES
  ('Science Building Tour', 'Explore the Science Building and visit the labs.', 'find', 100, 900, 1),
  ('Campus Orientation', 'Visit all major landmarks on campus.', 'find', 150, 1800, 1),
  ('Lunch Break Quest', 'Find the Cafeteria within time limit.', 'find', 25, 300, 1);

-- Quest steps for new quests
INSERT INTO quest_steps (quest_id, step_order, target_location_id, action_type, action_payload)
VALUES
  (4, 1, 6, 'reach', JSON_OBJECT('hint', 'Head to the Science Building')),
  (4, 2, 7, 'reach', JSON_OBJECT('hint', 'Visit the Chemistry Lab on floor 2')),
  (4, 3, 8, 'reach', JSON_OBJECT('hint', 'Check out the Physics Lab')),
  (5, 1, 1, 'reach', JSON_OBJECT('hint', 'Start at the Main Gate')),
  (5, 2, 4, 'reach', JSON_OBJECT('hint', 'Visit the Library')),
  (5, 3, 5, 'reach', JSON_OBJECT('hint', 'Find the Student Hub')),
  (5, 4, 9, 'reach', JSON_OBJECT('hint', 'Stop by the Cafeteria')),
  (6, 1, 9, 'reach', JSON_OBJECT('hint', 'The Cafeteria is near the Garden'));
