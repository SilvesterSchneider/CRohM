describe('Login Tests', () => {

    function doLogin(username: string, password: string) {
        // Navigate to login page (baseUrl from cypress.json is used as base)
        cy.visit('/login');

        // Click input with id 'login-user'
        cy.get('#login-user').click();

        // Type 'admin' into username field
        cy.get('#login-user').type(username);

        // Type password into password field
        cy.get('#login-password').type(password);

        // Click login button
        cy.get('#login-button').click();

        // Wait for cookie storage
        cy.wait(1000);
    }

    function visitAndCheck(url: string) {
        // Navigate to url (baseUrl from cypress.json is used as base)
        cy.visit(url);

        // Validate that url equals baseUrl/login
        cy.url().should('equal', Cypress.config().baseUrl + '/login');
    }

    beforeEach(() => {
        // Delete all cookies
        cy.clearCookies();

        // Reload page
        cy.reload();

        // Validate that no JSON Webtoken has been issued
        cy.getCookie('.AspNetCore.Identity.Application').should('not.exist');
    });

    it('should login with or without an initial password change', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');

        cy.url().then(($url) => {
            if ($url.match(Cypress.config().baseUrl + '/login')) {
                // Change password
                cy.get('#change-password').type('@dm1n1stR4tOr')

                // Accept new password
                cy.get('#change-button').click();
            }
        });

        // Validate that JSON Webtoken has been saved
        cy.getCookie('.AspNetCore.Identity.Application').should('exist');

        // Validate that url equals baseUrl
        cy.url().should('equal', Cypress.config().baseUrl + '/');

        // Validate that the correct page is displayed by checking the title
        cy.contains('CRohM - Customer Relationship Management System').should('exist');
    });

    it('should not accept a wrong password', () => {
        // Login with credentials admin/wrongpassword
        doLogin('admin', 'wrongpassword');

        // Validate that no JSON Webtoken has been issued
        cy.getCookie('.AspNetCore.Identity.Application').should('not.exist');

        // Validate that url equals baseUrl/login
        cy.url().should('equal', Cypress.config().baseUrl + '/login');

        // Validate that error message is displayed
        cy.get('#login-error').should('have.text', 'Login fehlgeschlagen!');

    });

    it('should redirected to /login when the token is deleted', () => {
        // Validate that url equals baseUrl/login
        cy.url().should('equal', Cypress.config().baseUrl + '/login');

        visitAndCheck('/');
        visitAndCheck('/contacts');
        visitAndCheck('/events');
        visitAndCheck('/organizations');
        visitAndCheck('/settings');
    });
});
