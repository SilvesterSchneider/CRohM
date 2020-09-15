import { loginAsAdmin } from '../../shared/login';

describe('Delete inactive users', () => {

    beforeEach(() => {
        loginAsAdmin();
    });

    afterEach(() => {
        cy.clearCookies();
    });

    it('should delete inactive user', () => {
        // Create user
        cy.request({
            method: 'POST',
            url: '/api/users',
            body: {
                FirstName: 'Jane', Lastname: 'Doe', Email: 'test@test.de'
            },
            auth: getAuth()
        }).then(response => {
            // Update lastLoginDate to 3 years prior
            cy.request({
                method: 'POST',
                url: `/api/test/user/${response.body.id}/lastLoginDate/${Cypress.moment().add(-3, 'years').format('YYYY-MM-DD')}/`,
                auth: getAuth()
            }).then(() => {
                //Trigger deletion of inactive users
                cy.request({
                    method: 'DELETE',
                    url: '/api/test/inactiveuser',
                    auth: getAuth()
                }).then(() => {
                    //Validate that user was deleted
                    cy.request({
                        method: 'GET',
                        url: '/api/users',
                        auth: getAuth()
                    }).its('body').should('have.length', 1);
                });
            });
        });
    });
});

function getAuth() {
    return {
        bearer: localStorage.getItem('access_token')
    };
}