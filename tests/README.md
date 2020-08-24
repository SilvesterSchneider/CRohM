# General
Cypress is used as frontend testing tool for testing Crohm's functionality.

# Run Tests

1. Run `npm install` when running the tests for the first time
2. Tests are run against the baseurl `https://localhost`. If your local configuration is different, the url can be adjusted in the `cypress.json` file
3. Run `npx cypress run` or `npm run test` to run Cypress tests from the CLI without the GUI
or use `npx cypress open` or `npm run debug` to open Cypress in the interactive GUI and have the possibility to debug test cases

# Write tests

Test files are located in integration folder, and structured by functionality.
See `\integration\login\login_spec.ts` for a sample test.

## Important functions
`visit` Visit a remote URL. Takes a path as parameter, which is appended to the baseUrl<br/>
`get` Get one or more DOM elements by selector or alias.<br/>
`should` Create an assertion. Assertions are automatically retried until they pass or time out.<br/>

To get more help on Cypress use `ngx cypress --help` or go check out the [Cypress Documentation](https://docs.cypress.io/api/api/table-of-contents.html)

# Additional Information

## Additional Tools
Chrome Browser Plugin `Cypress Recorder` can be used to record actions and generate scripts automatically.

## Folder Structure

### Fixtures
Fixtures are used as external pieces of static data that can be used by your tests. Fixture files are located in /fixtures.
You would typically use them with the cy.fixture() command and most often when you’re stubbing Network Requests.

### Test files
Test files are located in integration, and are arranged into folders by functionality. 

### Plugin files
By default Cypress will automatically include the plugins file plugins/index.js before every single spec file it runs. 
Cypress does this purely as a convenience mechanism so you don’t have to import this file in every single one of your spec files.

### Support file
By default Cypress will automatically include the support file support/index.js. This file runs before every single spec file. 
Cypress does this purely as a convenience mechanism so you don’t have to import this file in every single one of your spec files.


