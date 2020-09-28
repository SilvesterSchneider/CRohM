import { loginAsAdmin } from '../../shared/login';

describe('Home Tests', () => {
    beforeEach(() => {
        loginAsAdmin();
    });

    it('should appear disclaimer beacause no data protection officer are in system', () => {
        cy.url().should('equal', Cypress.config().baseUrl + '/?from=login');
        cy.get('#dataProtectionOfficeDisclaimer').should('have.text', 'Warnmeldung');
        cy.get('#resolveButton').should('exist');
        cy.get('#ignoreButton').should('exist');
    });

    it('should not appear disclaimer beacause not from Login', () => {
        cy.visit('/')

        cy.url().should('equal', Cypress.config().baseUrl + '/');
        cy.get('#dataProtectionOfficeDisclaimer').should('not.exist');
    });
});
