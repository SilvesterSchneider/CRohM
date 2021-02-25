import { WatchDirectoryFlags } from 'typescript';
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
       // cy.intercept('Users').as('users');
      //  cy.intercept('Role').as('role');
       // cy.intercept('**/Users/1').as('adminUser');

        // Login with credentials admin/@dm1n1stR4tOr
        doLogin('admin', '@dm1n1stR4tOr');

        // go to setting page
        cy.visit('/settings');
        cy.wait('@role');

        //change to roles tab
        cy.get('#mat-tab-label-0-1').click();

        //click on create role button
        cy.get('#createRoleButton').click();

        //type in the role name
        cy.get('#roleNameInputField').type('Alles');

        //click on the selection field of permissions
        cy.get('#permissionSelection', {timeout: 5000})
            .click()
            .get('mat-option')
            //get the first permission
            .contains('Anlegen eines Kontakts')
            .click()
            .get('mat-option')
            //get the second permission
            .contains('Einsehen und Bearbeiten aller Kontakte')
            .click()
            .get('mat-option')
            .contains('Löschen eines Kontakts')
            .click()
            .get('mat-option')
            .contains('Anlegen einer Organisation')
            .click()
            .get('mat-option')
            .contains('Einsehen und Bearbeiten aller Organisationen')
            .click()
            .get('mat-option')
            .contains('Zuordnen eines Kontakts zu einer Organisation')
            .click()
            .get('mat-option')
            .contains('Löschen einer Organisation')
            .click()
            .get('mat-option')
            .contains('Hinzufügen eines Historieneintrags bei Kontakt oder Organisation')
            .click()
            .get('mat-option')
            .contains('Anlegen einer Veranstaltung')
            .click()
            .get('mat-option')
            .contains('Einsehen und Bearbeiten einer Veranstaltung')
            .click()
            .get('mat-option')
            .contains('Löschen einer Veranstaltung')
            .click()
            .get('mat-option')
            .contains('Auskunft gegenüber eines Kontakts zu dessen Daten')
            .click()
            .get('mat-option')
            .contains('Mitteilung an einen Kontakt nach Löschung oder Änderung')
            .click()
            .get('mat-option')
            .contains('Anlegen eines Benutzers')
            .click()
            .get('mat-option')
            .contains('Zuweisung einer neuen Rolle zu einem Benutzer')
            .click()
            .get('mat-option')
            .contains('Rücksetzen eines Passworts eines Benutzers')
            .click()
            .get('mat-option')
            .contains('Löschen / Deaktivieren eines Benutzers')
            .click()
            .get('mat-option')
            .contains('Einsehen und Überarbeiten des Rollenkonzepts')
            .click();

        //save the role by clicking on the save button
        cy.get('#saveRoleButton').click({ force: true });
        cy.wait('@role');

        //check whether the new role exists in the table
        cy.get('#rolesTable').should("contain.text", 'Alles');

        //click on the users tab
        cy.get('#mat-tab-label-0-0').click();

        //click on the edit user button of admin user
        cy.get('.editUserButton').click();
        cy.wait('@adminUser');
        cy.wait('@role');

        //click on the selection field for all roles
        cy.wait(500);
        cy.get('#permissionSelectionForUser').click().get('mat-option').contains('Alles').click(); //select the testRole

        //save the user by clicking the button
        cy.get('#saveUserChangesButton').click({ force: true });
        cy.wait('@role');

        //click on the edit user button of admin user
        cy.wait(500);
        cy.get('.editUserButton').click();
        cy.wait('@adminUser');
        cy.wait('@role');

        //check whether the field contains all roles
        cy.get('#permissionSelectionForUser').should("contain.text", 'Alles');
    });
});


