import { OrganizationDto } from '../shared/api-generated/api-generated';

export const ORGANIZATIONS: OrganizationDto[] = [
    {
        id: 0,
        name: 'testCompany1',
        description: 'testDesc1',
        address: {
            street: 'teststreet1',
            city: 'nuremberg',
            streetNumber: '10',
            zipcode: '90449',
            country: 'germanay'
        },
        contact: {
            mail: 'inog@foff.de',
            phoneNumber: '0232-2323',
            fax: '43434-34344'
        },
        employees: [
        {
            firstName: 'raz',
            id: 0,
            lastName: 'matis',
            twoFactorEnabled: true
        },
        {
            firstName: 'domi',
            id: 1,
            lastName: 'müller',
            twoFactorEnabled: true
        }
    ]
    },
    {
        id: 1,
        name: 'testCompany2',
        description: 'testDesc2',
        address: {
            street: 'teststreet2',
            city: 'nuremberg2',
            streetNumber: '10',
            zipcode: '90443',
            country: 'germanay2'
        },
        contact: {
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
            street: 'teststreet3',
            city: 'nuremberg',
            streetNumber: '10',
            zipcode: '90444',
            country: 'germanay'
        },
        contact: {
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
            street: 'teststreet4',
            city: 'nuremberg',
            zipcode: '90445',
            streetNumber: '10',
            country: 'germanay'
        },
        contact: {
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
            street: 'teststreet5',
            city: 'nuremberg',
            zipcode: '90446',
            streetNumber: '10',
            country: 'germanay'
        },
        contact: {
            mail: 'inog@foff.de',
            phoneNumber: '0232-2323',
            fax: '43434-34344'
        },
        employees: [
            {
                firstName: 'paul',
                id: 2,
                lastName: 'matis',
                twoFactorEnabled: true
            },
            {
                firstName: 'heidi',
                id: 3,
                lastName: 'müller',
                twoFactorEnabled: true
            }
        ]
    }
];
