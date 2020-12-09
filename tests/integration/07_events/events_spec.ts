import { doLogin } from '../../shared/login';

describe('Contacts Tests', () => {
    beforeEach(() => {
        // Delete all cookies
        cy.clearCookies();

        // Reload page
        cy.reload();

        // Validate that no JSON Webtoken has been issued
        cy.getCookie('.AspNetCore.Identity.Application').should('not.exist');
    });

    it('should correctly preselect invitations after a invitation was send', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');
        cy.get('#ignoreButton').click();
        // go to contacts page
        cy.visit('/events');
        cy.get('#addDummyEventButton').click();
        cy.get('.editButton').click();
        cy.get('#eventName').should('have.value', 'Veranstaltung0');
    });
});
