import { loginAsAdmin } from '../../shared/login';

describe('Home Tests',()=>{
  
    it('should appear disclaimer beacause no data protection officer are in system',()=>{
        loginAsAdmin();
        
        cy.url().should('equal', Cypress.config().baseUrl + '/?from=login');
        cy.get('#dataProtectionOfficeDisclaimer').should('have.text', 'Warnmeldung');
        cy.get('#resolveButton').should('exist');
        cy.get('#ignoreButton').should('exist');        
    })

    it('should not appear disclaimer beacause not from Login',()=>{
        loginAsAdmin();
        
        cy.visit('/')
        
        cy.url().should('equal', Cypress.config().baseUrl + '/');
        cy.get('#dataProtectionOfficeDisclaimer').should('not.exist');
      
    })

    
    it('should not appear disclaimer beacause data protection officer is in system',()=>{
        
        loginAsAdmin();

        // make admin to data protection officer                
        updateAdminToDataProtectionOfficer();
        
        // wait till api save the user update
        cy.wait(2000);
        
        // after reloade the disclaimer should not be visible
        cy.reload();

        cy.url().should('equal', Cypress.config().baseUrl + '/?from=login');
        cy.get('#dataProtectionOfficeDisclaimer').should('not.exist');

        // take role of admin
        resetAdmin();
        
        // wait till api save the user update 
        cy.wait(2000);
              
    })


})

function updateAdminToDataProtectionOfficer() {
    cy.request({
         method: 'PUT',
         url: '/api/role/1',
         body: ["Admin","Datenschutzbeauftragter"],
         auth: getAccessToken()
     })
 }
 
function resetAdmin() {
    cy.request({
         method: 'PUT',
         url: '/api/role/1',
         body: ["Admin"],
         auth: getAccessToken()
     })
 }

function getAccessToken() {
    return {
        bearer: localStorage.getItem('access_token')
    };
}