import { Contact } from './contacts.model';
//import { getMaxListeners } from 'cluster';

export const CONTACTS: Contact[] = [
    {
        id: 0,
        vorname: 'Max',
        nachname: 'Mustermann',
        adresse: {
            land: 'Deutschland',
            strasse: 'Teststrasse 1',
            ort: 'Nürnberg',
            plz: '12345'
        },
        mail: 'maxmustermann@getMaxListeners.com',
        phone: '0157 0011223344'
    },
    {
        id: 1,
        vorname: 'Judith',
        nachname: 'Androthe',
        adresse: {
            land: 'Deutschland',
            strasse: 'Teststrasse 2',
            ort: 'Nürnberg',
            plz: '12345'
        },
        mail: 'judithandrothe@getMaxListeners.com',
        phone: '0157 0011223355'
    },
    {
        id: 2,
        vorname: 'Julia',
        nachname: 'Stoh',
        adresse: {
            land: 'Deutschland',
            strasse: 'Teststrasse 3',
            ort: 'Nürnberg',
            plz: '12345'
        },
        mail: 'juliastoh@getMaxListeners.com',
        phone: '0157 0011223366'
    },
    {
        id: 3,
        vorname: 'Andreas',
        nachname: 'Meier',
        adresse: {
            land: 'Deutschland',
            strasse: 'Teststrasse 15',
            ort: 'Nürnberg',
            plz: '12345'
        },
        mail: 'andreasmeier@getMaxListeners.com',
        phone: '0157 0011223377'
    }
];
