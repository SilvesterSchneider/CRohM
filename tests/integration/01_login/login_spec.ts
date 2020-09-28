import { cookieClear } from '../../shared/cookie_clear';
import { doLogin, loginAsAdmin } from '../../shared/login';
import { doLogout } from '../../shared/logout';

describe('Login Tests', () => {
    beforeEach(() => {
        cookieClear();
    });

    it('should login with or without an initial password change', () => {
        loginAsAdmin();

        // Validate that url equals baseUrl
        cy.url().should('equal', Cypress.config().baseUrl + '/?from=login');

        // Validate that the correct page is displayed by checking the title
        cy.contains('CRohM - Customer Relationship Management System').should('exist');
    });

    it('should not appear disclaimer beacause data protection officer is in system', () => {
        loginAsAdmin();

        // make admin to data protection officer
        cy.request({
            method: 'PUT',
            url: '/api/role/1',
            body: ["Admin", "Datenschutzbeauftragter"],
            auth: getAccessToken()
        }).then(() => {
            // after reloade the disclaimer should not be visible
            cy.reload();

            cy.url().should('equal', Cypress.config().baseUrl + '/?from=login');
            cy.get('#dataProtectionOfficeDisclaimer').should('not.exist');

            // take role of admin
            cy.request({
                method: 'PUT',
                url: '/api/role/1',
                body: ["Admin"],
                auth: getAccessToken()
            }).then(() => { });
        })

        doLogout();
    })

    it('should not accept a wrong password', () => {
        // Login with credentials admin/wrongpassword
        doLogin('admin', 'wrongpassword');

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

function getAccessToken() {
    return {
        bearer: localStorage.getItem('access_token')
    };
}

function visitAndCheck(url: string) {
    // Navigate to url (baseUrl from cypress.json is used as base)
    cy.visit(url);

    // Validate that url equals baseUrl/login
    cy.url().should('equal', Cypress.config().baseUrl + '/login');
}
