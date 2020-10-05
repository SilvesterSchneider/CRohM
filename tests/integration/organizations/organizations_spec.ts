import { doLogin } from '../../shared/login';

describe('Organizations Tests', () => {
    beforeEach(() => {
        // Delete all cookies
        cy.clearCookies();

        // Reload page
        cy.reload();

        // Validate that no JSON Webtoken has been issued
        cy.getCookie('.AspNetCore.Identity.Application').should('not.exist');
    });

    it('should correctly filter by tags', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');
        cy.wait(5000);
        // go to organizations page
        cy.visit('/organizations');
        cy.wait(2000);
        //click on add dummy organization
        cy.get('#addDummy').click();
        cy.wait(4000);
        //type in the tag
        cy.get('#tagInput').type('hallo').type('{enter}');
        cy.wait(1000);
        //verify no element is available
        cy.get('#tableOrganization').should("not.contain", 'Organisation');
        //remove the tag
        cy.get('#tagInput').type('{backspace}');
        cy.wait(1000);
        //click on edit button of organization
        cy.get('.editOrganization').click();
        cy.wait(5000);
        //type in the tag
        cy.get('#tagInputEdit').type('hallo').type('{enter}');
        cy.wait(1000);
        //save the changes
        cy.get('#saveButton').click({ force: true });
        cy.wait(2000);
        //type again the tag within the main page
        cy.get('#tagInput').type('hallo').type('{enter}');
        cy.wait(5000);
        //verify that one element is available
        cy.get('#tableOrganization').should("have.length", '1');
    });
});


