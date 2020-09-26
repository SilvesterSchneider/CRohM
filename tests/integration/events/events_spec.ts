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
        cy.visit('/contacts');
        //click on add button
        cy.get('#addDummyButton').click();
        cy.get('#addDummyButton').click();
        cy.get('#addDummyButton').click();
        cy.visit('/events');
        cy.get('#addDummyEventButton').click();
        cy.get('.editButton').click();
        cy.get('#textInput').type('a');
        cy.get('#contactInput').find('input').click({ force: true });
        cy.get('#invitationButton').click({ force: true });
        cy.get('#buttonSend').click();
        cy.get('.checkBox').find('input').should("be.checked");
    });
});
