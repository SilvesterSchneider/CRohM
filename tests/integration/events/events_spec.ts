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
        cy.visit('/contacts');
        cy.wait(10000);
        //click on add button
        cy.get('#addDummyButton').click();
        cy.wait(10000);
        cy.get('#addDummyButton').click();
        cy.wait(10000);
        cy.get('#addDummyButton').click();
        cy.wait(10000);
        cy.visit('/events');
        cy.wait(10000);
        cy.get('#addDummyEventButton').click();
        cy.wait(18000);
        cy.get('.editButton').click();
        cy.wait(10000);
        cy.get('#textInput').type('a');
        cy.wait(200);
        cy.get('#contactInput').find('input').click({force:true});
        cy.wait(5000);
        cy.get('#invitationButton').click({force:true});
        cy.wait(2000);
        cy.get('#buttonSend').click();
        cy.wait(10000);
        cy.get('.checkBox').find('input').should("be.checked");
    }); 
});


