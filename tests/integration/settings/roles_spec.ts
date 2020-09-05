import { doLogin } from '../../shared/login';
/*
describe('Login Tests', () => {
    beforeEach(() => {
        // Delete all cookies
        cy.clearCookies();

        // Reload page
        cy.reload();

        // Validate that no JSON Webtoken has been issued
        cy.getCookie('.AspNetCore.Identity.Application').should('not.exist');
    });

    it('should correctly create a new role', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');

        cy.visit('/settings');

        cy.get('#tabRoles').click();

        cy.get('#createRoleButton').click();

        cy.url().then(x => {
            cy.get('#roleNameInputField').type('testRole');
            cy.get('#permissionSelection').select('Anlegen einer Organisation');
            cy.get('#saveRoleButton').click();
        });
    });


});*/


