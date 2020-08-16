describe('Login', () => {
    it('successfully loads', () => {
      cy.visit('/login');
      cy.get('#mat-input-0').click();
      cy.get('#mat-input-0').type('admin');
      cy.get('#mat-input-1').type('@dm1n1stR4tOr');
      cy.get('.mat-button-wrapper').click();
    })
  })