# Bug Tracking System - Automation Tests

This project contains end-to-end automation tests for the Bug Tracking System using Playwright and NUnit.

## Prerequisites

1. .NET 8.0 SDK
2. Node.js (for Playwright browser installation)
3. Bug Tracking System application running locally

## Setup

1. **Install Playwright browsers:**
   ```bash
   pwsh bin/Debug/net8.0/playwright.ps1 install
   ```
   
   Or on Linux/Mac:
   ```bash
   ./bin/Debug/net8.0/playwright.sh install
   ```

2. **Update test configuration:**
   - Open `BaseTest.cs`
   - Update `BaseUrl` to match your application URL (currently set to https://localhost:44384)
   - Update test user credentials if different

3. **Ensure test data:**
   - Make sure your application has the test users configured:
     - Admin: admin@demo.com / Admin123!
     - User: qa@demo.com / Qa123!

## Running Tests

### Run all tests:
```bash
dotnet test
```

### Run specific test class:
```bash
dotnet test --filter "ClassName=AuthenticationTests"
```

### Run tests with specific browser:
```bash
dotnet test -- --browser=firefox
```

### Run tests in headless mode:
```bash
dotnet test -- --headless=true
```

## Test Structure

### Test Categories

1. **AuthenticationTests** - Login, logout, and access control tests
2. **UserProfileTests** - User profile management and password change tests
3. **BugReportTests** - Bug report creation, editing, and viewing tests
4. **CommentTests** - Comment functionality tests
5. **NavigationTests** - UI navigation and responsive design tests

### Test Data

Tests use the following test accounts:
- **Admin User**: admin@demo.com / Admin123!
- **Regular User**: qa@demo.com / Qa123!

### Test Reports

Test results are generated in multiple formats:
- HTML report: `test-results/index.html`
- JUnit XML: `test-results.xml`
- Screenshots and videos for failed tests

## Configuration

### Browser Configuration
- Default: Chromium (headless: false)
- Supported: Chromium, Firefox, WebKit
- Viewport: 1280x720
- Timeout: 30 seconds per test

### Parallel Execution
- Tests run in parallel for faster execution
- 4 workers by default
- 2 retries for flaky tests

## Best Practices

1. **Page Object Model**: Consider implementing page objects for complex pages
2. **Test Data**: Use unique identifiers (timestamps) for test data
3. **Cleanup**: Tests should clean up any data they create
4. **Assertions**: Use FluentAssertions for readable test assertions
5. **Waits**: Use explicit waits instead of Thread.Sleep

## Troubleshooting

### Common Issues

1. **Application not running**: Ensure the Bug Tracking System is running on the configured URL
2. **Browser not found**: Run `playwright install` to install browsers
3. **Test data issues**: Verify test users exist in the database
4. **Timeout errors**: Increase timeout values in configuration

### Debug Mode

To run tests in debug mode with visible browser:
```bash
dotnet test -- --headless=false --slowmo=1000
```

### Environment Variables

You can override configuration using environment variables:
- `TEST_BASE_URL`: Override base URL
- `TEST_ADMIN_EMAIL`: Override admin email
- `TEST_ADMIN_PASSWORD`: Override admin password
- `TEST_USER_EMAIL`: Override user email
- `TEST_USER_PASSWORD`: Override user password

## Contributing

When adding new tests:
1. Follow the existing naming conventions
2. Use the BaseTest class for common functionality
3. Add appropriate assertions using FluentAssertions
4. Include cleanup code for any test data created
5. Update this README if adding new test categories