import { doLogin } from '../../shared/login';

describe('Contacts Tests', () => {
    /*
    beforeEach(() => {
        // Delete all cookies
        cy.clearCookies();

        // Reload page
        cy.reload();

        // Validate that no JSON Webtoken has been issued
        cy.getCookie('.AspNetCore.Identity.Application').should('not.exist');
    });

    it('should correctly create a new contact', () => {
       // cy.intercept('contact').as('contact');

        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');

        // go to contacts page
        cy.visit('/contacts');
        cy.wait('@contact');


        //click on add button
        cy.get('#addButton').click();
        //wait for dialog to open
        cy.wait(1000);

        //type in all values
        cy.get('#name', {timeout: 5000}).type('testName');
        cy.get('#preName').type('testVorname');
        cy.get('#contactPartner').type('razvan');
        cy.get('#gender').click().get('mat-option').contains('Weiblich').click();  //select female as gender
        cy.get('#street').type('testStrasse');
        cy.get('#streetNumber').type('33');
        cy.get('#zipcode').type('90478');
        cy.get('#city').type('testStadt');
        cy.get('#country').click().get('mat-option').contains('Deutschland').click();  //select germany as country
        cy.get('#mail').type('info@test.de');
        cy.get('#fax').type('0123-123');
        cy.get('#phoneNumber').type('014-234234');
        cy.get('#save').click({ force: true });

        // Wait for contact request
        cy.wait('@contact');
        cy.wait('@contact');


        // Check that contact shows up in table
        cy.get('#contactsTable').should("contain.text", 'testName');
    });
    it('should correctly edit the fields gender and contactPerson of an existing contact', () => {
       // cy.intercept('contact').as('contact');

        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');

        // go to contacts page
        cy.visit('/contacts');
        cy.wait('@contact');

        //click on edit button
        cy.get('[data-cy=submit_btn]').should('be.enabled').click();
        cy.get('[data-cy=submit]').should('be.visible').click();
        cy.get('#gender').click().get('mat-option').contains('Männlich').click();  //get the male gender
        cy.get('#contactPartner').type('matis');
        cy.get('#saveEdit').click();

        cy.wait('@contact');

        cy.get('[data-cy=submit_btn]').should('be.enabled').click();
        cy.get('[data-cy=submit]').should('be.visible').click({force: true});
        cy.get('#contactPartner').should("have.value", 'razvanmatis');
    });
    */
});
