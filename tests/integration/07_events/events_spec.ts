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
        cy.wait(3000);
        cy.get('#ignoreButton').click();
        // go to contacts page
        cy.visit('/events');
        cy.wait(10000);
        cy.get('#addDummyEventButton').click();
        cy.wait(4000);
        cy.get('.editButton').click();
        cy.wait(8000);
        cy.get('#eventName').should('have.value', 'Veranstaltung0');
    }); 
});


