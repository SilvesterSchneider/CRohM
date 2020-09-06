import { doLogin } from '../../shared/login';

describe('Login Tests', () => {
    beforeEach(() => {
        // Delete all cookies
        cy.clearCookies();

        // Reload page
        cy.reload();

        // Validate that no JSON Webtoken has been issued
        cy.getCookie('.AspNetCore.Identity.Application').should('not.exist');
    });

    it('should correctly create a new role and assign it to a user', () => {
        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');
        cy.wait(1000);
        // go to setting page
        cy.visit('/settings');
        cy.wait(1000);
        //change to roles tab
        cy.get('#mat-tab-label-0-1').click();
        cy.wait(2000);
        //click on create role button
        cy.get('#createRoleButton').click();
        cy.wait(1000);
        cy.url().then(x => {
            cy.wait(500);
            //type in the role name
            cy.get('#roleNameInputField').type('testRole');
            //click on the selection field of permissions
            cy.get('#permissionSelection')
                .click()
                .get('mat-option')
                //get the first permission
                .contains('Anlegen eines Kontakts')
                .click()  
                .get('mat-option')
                //get the second permission
                .contains('Anlegen einer Organisation')
                .click()  
        });
        //save the role by clicking on the save button
        cy.get('#saveRoleButton').click({ force: true });
        //check whether the new role exists in the table
        cy.get('#rolesTable').should("contain.text", 'testRole')
        //click on the users tab
        cy.get('#mat-tab-label-0-0').click();
        cy.wait(5000);
        //click on the edit user button of admin user
        cy.get('.editUserButton').click();
        cy.wait(3000);
        cy.url().then(x => {
            cy.wait(2000);
            //click on the selection field for all roles
            cy.get('#permissionSelectionForUser')
                .click()
                .get('mat-option')
                //select the testRole 
                .contains('testRole')
                .click()  // this is jquery click() not cypress click()
            cy.wait(500);
            //save the user by clicking the button
            cy.get('#saveUserChangesButton').click({ force: true });
        });
        cy.wait(2000);
        //click on the edit user button of admin user
        cy.get('.editUserButton').click();
        cy.wait(3000);
        //check whether the field contains all roles
        cy.get('#permissionSelectionForUser').should("contain.text", 'testRole');
    });
});


