import { loginAsAdmin } from '../../shared/login';

describe('Home Tests', () => {

    it('should appear disclaimer beacause no data protection officer are in system', () => {
        loginAsAdmin();

        cy.url().should('equal', Cypress.config().baseUrl + '/?from=login');
        cy.get('#dataProtectionOfficeDisclaimer').should('have.text', 'Warnung');
        cy.get('#resolveButton').should('exist');
        cy.get('#ignoreButton').should('exist');
    });

    it('should not appear disclaimer beacause not from Login', () => {
        loginAsAdmin();

        cy.visit('/');

        cy.url().should('equal', Cypress.config().baseUrl + '/');
        cy.get('#dataProtectionOfficeDisclaimer').should('not.exist');

    });

    it('should switch language to english', () => {
        loginAsAdmin();

        cy.visit('/');
        cy.get('.userMenu #language-select .mat-icon').click();
        cy.get('button').contains('English').click();

        cy.contains('Contacts').should('exist');
        cy.contains('Organizations').should('exist');
    });

    it('should switch language to german', () => {
        loginAsAdmin();

        cy.visit('/');
        cy.get('.userMenu #language-select .mat-icon').click();
        cy.get('button').contains('Deutsch').click();

        cy.contains('Kontakte').should('exist');
        cy.contains('Organisationen').should('exist');
    });
});
