import { OrganizationDto } from '../shared/api-generated/api-generated';

export const ORGANIZATIONS: OrganizationDto[] = [
    {
        id: 0,
        name: 'testCompany1',
        description: 'testDesc1',
        address: {
            id: 5,
            street: 'teststreet1',
            city: 'nuremberg',
            streetNumber: '10',
            zipcode: '90449',
            country: 'germanay'
        },
        contact: {
            id: 0,
            mail: 'inog@foff.de',
            phoneNumber: '0232-2323',
            fax: '43434-34344'
        }
    },
    {
        id: 1,
        name: 'testCompany2',
        description: 'testDesc2',
        address: {
            id: 4,
            street: 'teststreet2',
            city: 'nuremberg2',
            streetNumber: '10',
            zipcode: '90443',
            country: 'germanay2'
        },
        contact: {
            id: 1,
            mail: 'inog@foff.de',
            phoneNumber: '0232-2323',
            fax: '43434-34344'
        }
    },
    {
        id: 2,
        name: 'testCompany3',
        description: 'testDesc3',
        address: {
            id: 3,
            street: 'teststreet3',
            city: 'nuremberg',
            streetNumber: '10',
            zipcode: '90444',
            country: 'germanay'
        },
        contact: {
            id: 2,
            mail: 'inog@foff.de',
            phoneNumber: '0232-2323',
            fax: '43434-34344'
        }
    },
    {
        id: 3,
        name: 'testCompany4',
        description: 'testDesc4',
        address: {
            id: 2,
            street: 'teststreet4',
            city: 'nuremberg',
            zipcode: '90445',
            streetNumber: '10',
            country: 'germanay'
        },
        contact: {
            id: 3,
            mail: 'inog@foff.de',
            phoneNumber: '0232-2323',
            fax: '43434-34344'
        }
    },
    {
        id: 4,
        name: 'testCompany5',
        description: 'testDesc5',
        address: {
            id: 1,
            street: 'teststreet5',
            city: 'nuremberg',
            zipcode: '90446',
            streetNumber: '10',
            country: 'germanay'
        },
        contact: {
            id: 4,
            mail: 'inog@foff.de',
            phoneNumber: '0232-2323',
            fax: '43434-34344'
        }
    }
];
