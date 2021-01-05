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

    it('should correctly create a new user', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');
        cy.wait(1000);
        // go to contacts page
        cy.visit('/contacts');
        cy.wait(1000);
        //click on add button
        cy.get('#addButton').click();
        cy.wait(2000);
        //type in all values
        cy.get('#name').type('testName');
        cy.get('#preName').type('testVorname');
        cy.get('#contactPartner').type('razvan');
        cy.get('#gender')
            .click()
            .get('mat-option')
            //get the female gender
            .contains('Weiblich')
            .click()
        cy.get('#street').type('testStrasse');
        cy.get('#streetNumber').type('33');
        cy.get('#zipcode').type('90478');
        cy.get('#city').type('testStadt');
        cy.get('#country')
            .click()
            .get('mat-option')
            //get the germany as country
            .contains('Deutschland')
            .click()
        cy.get('#mail').type('info@test.de');
        cy.get('#fax').type('0123-123');
        cy.get('#phoneNumber').type('014-234234');
        cy.get('#save').click({ force: true });
        cy.wait(10000);
        cy.get('#contactsTable').should("contain.text", 'testName');
    });
    it('should correctly edit the fields gender and contactPerson of an existing user', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');
        cy.wait(1000);
        // go to contacts page
        cy.visit('/contacts');
        cy.wait(1000);
        //click on edit button
        cy.get('[data-cy=submit_btn]')
            .should('be.enabled')
            .click();
        cy.wait(1000);
        cy.get('[data-cy=submit]')
            .should('be.visible')
            .click();
        cy.wait(4000);
        cy.get('#gender')
            .click()
            .get('mat-option')
            //get the male gender
            .contains('Männlich')
            .click()
        cy.get('#contactPartner').type('matis');
        cy.get('#saveEdit').click();
        cy.wait(3000);
        cy.get('[data-cy=submit_btn]')
            .should('be.enabled')
            .click();
        cy.wait(1000);
        cy.get('[data-cy=submit]')
            .should('be.visible')
            .click();
        cy.wait(4000);
        cy.get('#contactPartner').should("have.value", 'razvanmatis');
    });
});
