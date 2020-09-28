export function doLogout() {
    cy.get('.userMenu').click();

    // Proof of concept
    cy.get('.mat-menu-content > :nth-child(4)').click();

    cy.url().should('equal', Cypress.config().baseUrl + '/login');
    cy.getCookie('.AspNetCore.Identity.Application').should('not.exist');
}
