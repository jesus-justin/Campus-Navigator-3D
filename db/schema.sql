-- Campus Navigator 3D schema
-- MySQL 8+ recommended

CREATE DATABASE IF NOT EXISTS campus_navigator CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE campus_navigator;

CREATE TABLE IF NOT EXISTS users (
  id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  external_id VARCHAR(64) NULL,
  display_name VARCHAR(100) NOT NULL,
  role ENUM('player','admin') NOT NULL DEFAULT 'player',
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY ux_users_external_id (external_id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS sessions (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  user_id INT UNSIGNED NOT NULL,
  token_hash CHAR(64) NOT NULL,
  device_label VARCHAR(100) NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  expires_at TIMESTAMP NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY ux_sessions_token_hash (token_hash),
  KEY ix_sessions_user_id (user_id),
  CONSTRAINT fk_sessions_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS locations (
  id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  name VARCHAR(120) NOT NULL,
  type ENUM('building','room','landmark') NOT NULL,
  parent_id INT UNSIGNED NULL,
  floor TINYINT NULL,
  pos_x DECIMAL(9,3) NOT NULL,
  pos_y DECIMAL(9,3) NOT NULL,
  pos_z DECIMAL(9,3) NOT NULL,
  radius_m DECIMAL(6,2) NOT NULL DEFAULT 2.00,
  PRIMARY KEY (id),
  KEY ix_locations_type_parent (type, parent_id),
  CONSTRAINT fk_locations_parent_id FOREIGN KEY (parent_id) REFERENCES locations(id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS quests (
  id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  title VARCHAR(120) NOT NULL,
  description TEXT NULL,
  type ENUM('find','deliver','submit','escort','timed') NOT NULL,
  reward_points INT NOT NULL DEFAULT 0,
  time_limit_sec INT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS quest_steps (
  id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  quest_id INT UNSIGNED NOT NULL,
  step_order INT NOT NULL,
  target_location_id INT UNSIGNED NOT NULL,
  action_type ENUM('reach','interact','submit') NOT NULL,
  action_payload JSON NULL,
  PRIMARY KEY (id),
  KEY ix_quest_steps_quest (quest_id, step_order),
  CONSTRAINT fk_quest_steps_quest_id FOREIGN KEY (quest_id) REFERENCES quests(id) ON DELETE CASCADE,
  CONSTRAINT fk_quest_steps_location_id FOREIGN KEY (target_location_id) REFERENCES locations(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS quest_runs (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  quest_id INT UNSIGNED NOT NULL,
  user_id INT UNSIGNED NOT NULL,
  status ENUM('active','success','failed') NOT NULL DEFAULT 'active',
  started_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  ended_at TIMESTAMP NULL,
  time_used_sec INT NULL,
  PRIMARY KEY (id),
  KEY ix_quest_runs_user_started (user_id, started_at),
  CONSTRAINT fk_quest_runs_quest_id FOREIGN KEY (quest_id) REFERENCES quests(id) ON DELETE CASCADE,
  CONSTRAINT fk_quest_runs_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS telemetry_events (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  user_id INT UNSIGNED NOT NULL,
  session_id BIGINT UNSIGNED NOT NULL,
  event_type ENUM('move','enter','quest_start','quest_end','quest_step','interact') NOT NULL,
  location_id INT UNSIGNED NULL,
  pos_x DECIMAL(9,3) NULL,
  pos_y DECIMAL(9,3) NULL,
  pos_z DECIMAL(9,3) NULL,
  payload JSON NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY ix_telemetry_type_created (event_type, created_at),
  KEY ix_telemetry_location_created (location_id, created_at),
  KEY ix_telemetry_session_created (session_id, created_at),
  CONSTRAINT fk_telemetry_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  CONSTRAINT fk_telemetry_session_id FOREIGN KEY (session_id) REFERENCES sessions(id) ON DELETE CASCADE,
  CONSTRAINT fk_telemetry_location_id FOREIGN KEY (location_id) REFERENCES locations(id) ON DELETE SET NULL
) ENGINE=InnoDB;
