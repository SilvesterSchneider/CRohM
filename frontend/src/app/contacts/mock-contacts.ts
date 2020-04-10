import { ContactDto } from '../shared/api-generated/api-generated'

export const CONTACTS: ContactDto[] = [
    {
        id: 0,
        preName: 'silvester',
        name: 'kracher',
        address: {
            country: 'Deutschland',
            street: 'Teststrasse 1',
            city: 'Nürnberg',
            zipcode: '12345',
            streetNumber: '10'
        },
        contactPossibilities: {
            mail: 'maxmustermann@getMaxListeners.com',
            phoneNumber: '0157 0011223344',
            fax: '0157-00231223344'
        }
    },
    {
        id: 1,
        preName: 'michelle',
        name: 'martin',
        address: {
            country: 'Deutschland',
            street: 'Teststrasse 1',
            city: 'Nürnberg',
            zipcode: '12345',
            streetNumber: '10'
        },
        contactPossibilities: {
            mail: 'maxmustermann@getMaxListeners.com',
            phoneNumber: '0157 0011223344',
            fax: '0157-00231223344'
        }
    },
    {
        id: 2,
        preName: 'raz',
        name: 'matis',
        address: {
            country: 'Deutschland',
            street: 'Teststrasse 1',
            city: 'Nürnberg',
            zipcode: '12345',
            streetNumber: '10'
        },
        contactPossibilities: {
            mail: 'maxmustermann@getMaxListeners.com',
            phoneNumber: '0157 0011223344',
            fax: '0157-00231223344'
        }
    },
    {
        id: 3,
        preName: 'markus',
        name: 'dietl',
        address: {
            country: 'Deutschland',
            street: 'Teststrasse 1',
            city: 'Nürnberg',
            zipcode: '12345',
            streetNumber: '10'
        },
        contactPossibilities: {
            mail: 'maxmustermann@getMaxListeners.com',
            phoneNumber: '0157 0011223344',
            fax: '0157-00231223344'
        }
    }
];
