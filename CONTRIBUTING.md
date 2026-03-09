# Contributing to Campus Navigator 3D

Thank you for your interest in contributing to Campus Navigator 3D! This document provides guidelines for contributing to the project.

## Getting Started

1. Fork the repository
2. Clone your fork locally
3. Set up the development environment (see README.md)
4. Create a new branch for your feature or bugfix

## Development Setup

### Prerequisites
- XAMPP with PHP 7.4+ and MySQL
- Unity 2021.3 LTS or later
- Modern web browser for dashboard testing

### Installation
1. Clone the repository to your XAMPP htdocs folder
2. Import the database schema from `db/schema.sql`
3. Configure API settings in `api/config.php`
4. Open the Unity project from the `unity/` folder

## Coding Standards

### PHP
- Follow PSR-12 coding standards
- Use type hints where possible
- Document all public functions with PHPDoc
- Handle errors gracefully with appropriate HTTP status codes

### JavaScript
- Use ES6+ syntax
- Add JSDoc comments for functions
- Use meaningful variable and function names
- Keep functions focused and concise

### C# (Unity)
- Follow Unity C# coding conventions
- Use XML documentation comments
- Keep MonoBehaviour scripts focused on single responsibilities
- Use serialized fields for inspector-visible properties

## Commit Messages

Write clear, descriptive commit messages:
- Use present tense ("Add feature" not "Added feature")
- Keep the first line under 50 characters
- Add detailed description if needed after a blank line
- Reference issue numbers when applicable

## Pull Request Process

1. Update documentation for any changed functionality
2. Test your changes thoroughly
3. Ensure all existing tests pass
4. Submit a pull request with a clear description
5. Link any related issues

## Code Review

All submissions require review before merging. Please be patient and responsive to feedback.

## Questions?

Feel free to open an issue for any questions or concerns.
