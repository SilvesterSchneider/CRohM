export function cookieClear() {
    // Delete all cookies
    cy.clearCookies();

    // Reload page
    cy.reload();

    // Validate that no JSON Webtoken has been issued
    cy.getCookie('.AspNetCore.Identity.Application').should('not.exist');
}
