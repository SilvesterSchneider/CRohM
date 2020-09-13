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
        doLogin('admin', 'admin1!');
        cy.wait(1000);
        // go to setting page
        cy.visit('/contacts');
        cy.wait(1000);
        //change to roles tab
        cy.get('#addButton').click();
        cy.wait(2000);
        //click on create role button
        cy.get('#name').type('testName');
        cy.get('#preName').type('testVorname');
        cy.get('#contactPartner').type('razvan');
        cy.get('#gender')
                .click()
                .get('mat-option')
                //get the first permission
                .contains('Weiblich')
                .click()  
        cy.get('#street').type('testStrasse');
        cy.get('#streetNumber').type('33');
        cy.get('#zipcode').type('90478');
        cy.get('#city').type('testStadt');
        cy.get('#country')
                .click()
                .get('mat-option')
                //get the first permission
                .contains('Deutschland')
                .click()  
        cy.get('#mail').type('info@test.de');
        cy.get('#fax').type('0123-123');
        cy.get('#phoneNumber').type('014-234234');
        cy.get('#save').click({force: true});
        cy.wait(2000);
        cy.get('#contactsTable').should("contain.text", 'testName');
    }); 
    it('should correctly edit the fields gender and contactPerson of an existing user', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', 'admin1!');
        cy.wait(1000);
        // go to setting page
        cy.visit('/contacts');
        cy.wait(1000);
        cy.get('.editButton').click();
        cy.wait(4000);
        cy.get('#gender')
                .click()
                .get('mat-option')
                //get the first permission
                .contains('Männlich')
                .click()  
        cy.get('#contactPartner').type('matis');
        cy.get('#saveEdit').click();
        cy.wait(2000);
        cy.get('.infoButton').click();
        cy.wait(4000);
        cy.get('#gender').should("have.value", 'Männlich');
        cy.get('#contactPartner').should("have.value", 'razvanmatis');
    });
});


