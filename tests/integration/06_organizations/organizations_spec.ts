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
        // go to organizations page
        cy.visit('/organizations');
        //click on add dummy organization
        cy.get('#addDummy').click();
        //type in the tag
        cy.get('#tagInput').type('hallo').type('{enter}');
        //verify no element is available
        cy.get('#tableOrganization').should("not.contain", 'Organisation');
        //remove the tag
        cy.get('#tagInput').type('{backspace}');
        //click on edit button of organization
        cy.get('.editOrganization').click();
        //type in the tag
        cy.get('#tagInputEdit').type('hallo').type('{enter}');
        //save the changes
        cy.get('#saveButton').click({ force: true });
        //type again the tag within the main page
        cy.get('#tagInput').type('hallo').type('{enter}');
        //verify that one element is available
        cy.get('#tableOrganization').should("have.length", '1');
    });
});
