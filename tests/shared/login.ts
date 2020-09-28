export function loginAsAdmin() {
    doLogin('admin', '@dm1n1stR4tOr');

    cy.url().then(($url) => {
        if ($url.match(Cypress.config().baseUrl + '/login')) {
            // Change password
            cy.get('#change-password').click().type('@dm1n1stR4tOr');

            // Accept new password
            cy.get('#change-button').click();
        }
    });
}

export function doLogin(username: string, password: string) {
    // Navigate to login page (baseUrl from cypress.json is used as base)
    cy.visit('/login');

    // Click input with id 'login-user'
    cy.get('#login-user').click();

    // Type 'admin' into username field
    cy.get('#login-user').click().type(username);

    // Type password into password field
    cy.get('#login-password').click().type(password);

    // Click login button
    cy.get('#login-button').click();

    // Wait for cookie storage
    cy.wait(100);
}
