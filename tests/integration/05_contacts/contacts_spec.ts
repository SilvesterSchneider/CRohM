import { cookieClear } from '../../shared/cookie_clear';
import { doLogin } from '../../shared/login';

describe('Contacts Tests', () => {
    beforeEach(() => {
        cookieClear();
    });

    it('should correctly create a new user', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');
        // go to contacts page
        cy.visit('/contacts');
        //click on add button
        cy.get('#addButton').click();
        //type in all values
        cy.get('#name').click().type('testName');
        cy.get('#preName').click().type('testVorname');
        cy.get('#contactPartner').click().type('razvan');
        cy.get('#gender')
            .click()
            .get('mat-option')
            //get the female gender
            .contains('Weiblich')
            .click()
        cy.get('#street').click().type('testStrasse');
        cy.get('#streetNumber').click().type('33');
        cy.get('#zipcode').click().type('90478');
        cy.get('#city').click().type('testStadt');
        cy.get('#country')
            .click()
            .get('mat-option')
            //get the germany as country
            .contains('Deutschland')
            .click()
        cy.get('#mail').click().type('info@test.de');
        cy.get('#phoneNumber').click().type('014-234234');
        cy.get('#fax').click().type('0123-123');
        cy.get('#save').click({ force: true });
        cy.get('#contactsTable').should("contain.text", 'testName');
    });

    it('should correctly edit the fields gender and contactPerson of an existing user', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');
        // go to contacts page
        cy.visit('/contacts');
        //click on edit button
        cy.get('.editButton').click();
        cy.get('#gender')
            .click()
            .get('mat-option')
            //get the male gender
            .contains('Männlich')
            .click()
        cy.get('#contactPartner').click().type('matis');
        cy.get('#saveEdit').click();
        cy.get('.infoButton').click();
        cy.get('#gender').should("have.value", 'Männlich');
        cy.get('#contactPartner').should("have.value", 'razvanmatis');
    });
});