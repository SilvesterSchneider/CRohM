import { cookieClear } from '../../shared/cookie_clear';
import { loginAsAdmin } from '../../shared/login';

describe('Contacts Tests', () => {
    beforeEach(() => {
        cookieClear();
        loginAsAdmin();
    });

    it('should correctly preselect invitations after a invitation was send', () => {
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
        cy.get('#textInput').click().type('a');
        cy.get('#contactInput').find('input').click({ force: true });
        cy.get('#invitationButton').click({ force: true });
        cy.get('#buttonSend').click();
        cy.get('.checkBox').find('input').should("be.checked");
    });
});
